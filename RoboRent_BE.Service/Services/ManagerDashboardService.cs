using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.DTOs.Dashboard;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ManagerDashboardService : IManagerDashboardService
{
    private readonly AppDbContext _context;

    public ManagerDashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardOverviewResponse> GetOverviewAsync()
    {
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var lastMonthStart = thisMonthStart.AddMonths(-1);

        // === KPIs ===

        // Revenue this month
        var revenueThisMonth = await _context.PaymentRecords
            .Where(p => p.Status == "Paid" && p.PaidAt >= thisMonthStart)
            .SumAsync(p => p.Amount);

        // Revenue last month
        var revenueLastMonth = await _context.PaymentRecords
            .Where(p => p.Status == "Paid" && p.PaidAt >= lastMonthStart && p.PaidAt < thisMonthStart)
            .SumAsync(p => p.Amount);

        var revenueTrend = revenueLastMonth > 0
            ? Math.Round((revenueThisMonth - revenueLastMonth) / revenueLastMonth * 100, 1)
            : 0;

        // Active orders
        var activeOrders = await _context.Rentals
            .Where(r => r.Status != "Completed" && r.Status != "Cancelled" && r.IsDeleted != true)
            .CountAsync();

        // Robot utilization
        var totalRobots = await _context.Robots.Where(r => r.IsDeleted != true).CountAsync();
        var rentingRobots = await _context.Robots
            .Where(r => r.IsDeleted != true && r.RobotStatus == "Renting")
            .CountAsync();
        var robotUtilization = totalRobots > 0 ? Math.Round((decimal)rentingRobots / totalRobots * 100, 1) : 0;

        // Repeat rate
        var customerRentalCounts = await _context.Rentals
            .Where(r => r.AccountId != null && r.IsDeleted != true)
            .GroupBy(r => r.AccountId)
            .Select(g => new { AccountId = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalCustomers = customerRentalCounts.Count;
        var repeatCustomers = customerRentalCounts.Count(c => c.Count > 1);
        var repeatRate = totalCustomers > 0
            ? Math.Round((decimal)repeatCustomers / totalCustomers * 100, 1)
            : 0;

        // === Alerts ===
        var pendingQuotes = await _context.PriceQuotes
            .Where(q => q.Status == "PendingManager" && q.IsDeleted != true)
            .CountAsync();

        var pendingContracts = await _context.ContractDrafts
            .Where(c => c.Status == "PendingManager")
            .CountAsync();

        var pendingReports = await _context.ContractReports
            .Where(r => r.Status == "Pending" || r.Status == "Open")
            .CountAsync();

        // === Revenue Chart (6 months) ===
        var sixMonthsAgo = thisMonthStart.AddMonths(-5);
        var revenueByMonth = await _context.PaymentRecords
            .Where(p => p.Status == "Paid" && p.PaidAt >= sixMonthsAgo)
            .GroupBy(p => new { p.PaidAt!.Value.Year, p.PaidAt.Value.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Revenue = g.Sum(p => p.Amount),
                Count = g.Count()
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        var revenueChart = revenueByMonth.Select(r => new RevenueChartItem
        {
            Month = $"T{r.Month}",
            Revenue = r.Revenue,
            Aov = r.Count > 0 ? Math.Round(r.Revenue / r.Count, 0) : 0
        }).ToList();

        // === Peak Time Analysis ===
        var dayOfWeekNames = new[] { "CN", "T2", "T3", "T4", "T5", "T6", "T7" };

        var rentalsWithPayments = await _context.Rentals
            .Where(r => r.EventDate != null && r.IsDeleted != true)
            .Join(_context.PaymentRecords.Where(p => p.Status == "Paid"),
                r => r.Id,
                p => p.RentalId,
                (r, p) => new { r.EventDate, p.Amount })
            .ToListAsync();

        var peakTimeAnalysis = rentalsWithPayments
            .GroupBy(x => (int)x.EventDate!.Value.DayOfWeek)
            .Select(g => new PeakTimeItem
            {
                DayOfWeek = dayOfWeekNames[g.Key],
                Revenue = g.Sum(x => x.Amount),
                OrderCount = g.Count()
            })
            .OrderBy(x => Array.IndexOf(dayOfWeekNames, x.DayOfWeek))
            .ToList();

        // === Package Distribution ===
        var packageRevenue = await _context.Rentals
            .Where(r => r.ActivityTypeId != null && r.IsDeleted != true)
            .Join(_context.PaymentRecords.Where(p => p.Status == "Paid"),
                r => r.Id,
                p => p.RentalId,
                (r, p) => new { r.ActivityTypeId, p.Amount })
            .Join(_context.ActivityTypes,
                rp => rp.ActivityTypeId,
                at => at.Id,
                (rp, at) => new { at.Code, rp.Amount })
            .GroupBy(x => x.Code)
            .Select(g => new { Package = g.Key, Revenue = g.Sum(x => x.Amount) })
            .ToListAsync();

        var totalPackageRevenue = packageRevenue.Sum(p => p.Revenue);
        var packageDistribution = packageRevenue.Select(p => new PackageDistributionItem
        {
            Package = p.Package ?? "Unknown",
            Revenue = p.Revenue,
            Percentage = totalPackageRevenue > 0
                ? Math.Round(p.Revenue / totalPackageRevenue * 100, 1)
                : 0
        }).ToList();

        // === Robot Type Revenue ===
        var robotTypeRevenue = await _context.RoboTypes
            .Where(rt => rt.IsDeleted != true)
            .Select(rt => new RobotTypeRevenueItem
            {
                RobotType = rt.TypeName ?? "Unknown",
                Revenue = 0, // Simplified - would need RentalDetails join
                Percentage = 0
            })
            .ToListAsync();

        // === System Notifications (recent 5) ===
        var notifications = await _context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .Select(n => new SystemNotificationItem
            {
                Id = n.Id,
                Time = n.CreatedAt.ToString("HH:mm"),
                Type = n.Type.ToString(),
                Message = n.Content ?? "",
                RentalId = n.RentalId
            })
            .ToListAsync();

        return new DashboardOverviewResponse
        {
            Kpis = new OverviewKpis
            {
                RevenueThisMonth = revenueThisMonth,
                RevenueTrend = revenueTrend,
                ActiveOrders = activeOrders,
                RobotUtilization = robotUtilization,
                RepeatRate = repeatRate
            },
            Alerts = new OverviewAlerts
            {
                PendingQuotes = pendingQuotes,
                PendingContracts = pendingContracts,
                PendingReports = pendingReports
            },
            RevenueChart = revenueChart,
            PeakTimeAnalysis = peakTimeAnalysis,
            PackageDistribution = packageDistribution,
            RobotTypeRevenue = robotTypeRevenue,
            Notifications = notifications
        };
    }

    public async Task<DashboardRobotsResponse> GetRobotsAsync(string? status = null, int? typeId = null)
    {
        var query = _context.Robots.Where(r => r.IsDeleted != true);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.RobotStatus == status);
        if (typeId.HasValue)
            query = query.Where(r => r.RoboTypeId == typeId.Value);

        // Overview
        var allRobots = await _context.Robots.Where(r => r.IsDeleted != true).ToListAsync();
        var overview = new RobotFleetOverview
        {
            Total = allRobots.Count,
            Available = allRobots.Count(r => r.RobotStatus == "Stored" || r.RobotStatus == "Available"),
            Renting = allRobots.Count(r => r.RobotStatus == "Renting"),
            Maintenance = allRobots.Count(r => r.RobotStatus == "Maintenance"),
            Utilization = allRobots.Count > 0
                ? Math.Round((decimal)allRobots.Count(r => r.RobotStatus == "Renting") / allRobots.Count * 100, 1)
                : 0
        };

        // By type
        var byType = await _context.Robots
            .Where(r => r.IsDeleted != true)
            .Include(r => r.RoboType)
            .GroupBy(r => new { r.RoboTypeId, r.RoboType.TypeName })
            .Select(g => new RobotByTypeItem
            {
                TypeId = g.Key.RoboTypeId ?? 0,
                TypeName = g.Key.TypeName ?? "Unknown",
                Total = g.Count(),
                Available = g.Count(r => r.RobotStatus == "Stored" || r.RobotStatus == "Available"),
                Renting = g.Count(r => r.RobotStatus == "Renting"),
                Maintenance = g.Count(r => r.RobotStatus == "Maintenance"),
                Utilization = g.Count() > 0
                    ? Math.Round((decimal)g.Count(r => r.RobotStatus == "Renting") / g.Count() * 100, 1)
                    : 0
            })
            .ToListAsync();

        // Robot list
        var robots = await query
            .Include(r => r.RoboType)
            .OrderBy(r => r.RoboType.TypeName)
            .ThenBy(r => r.RobotName)
            .Select(r => new RobotListItem
            {
                Id = r.Id,
                RobotName = r.RobotName ?? "",
                ModelName = r.ModelName ?? "",
                TypeName = r.RoboType.TypeName ?? "",
                Status = r.RobotStatus ?? "",
                Location = r.Location
            })
            .ToListAsync();

        return new DashboardRobotsResponse
        {
            Overview = overview,
            ByType = byType,
            Robots = robots
        };
    }

    public async Task<DashboardRentalsResponse> GetRentalsAsync(
        string? status = null,
        string? package = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _context.Rentals
            .Where(r => r.IsDeleted != true)
            .Include(r => r.Account)
            .Include(r => r.ActivityType)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);
        if (!string.IsNullOrEmpty(package))
            query = query.Where(r => r.ActivityType.Code == package);
        if (fromDate.HasValue)
            query = query.Where(r => r.EventDate >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(r => r.EventDate <= toDate.Value);

        // Alerts
        var pendingApprovals = await _context.ContractDrafts
            .Where(c => c.Status == "PendingManager" || c.Status == "PendingCustomer")
            .CountAsync();

        var unpaidRentals = await _context.Rentals
            .Where(r => r.IsDeleted != true)
            .GroupJoin(_context.PaymentRecords.Where(p => p.Status == "Paid"),
                r => r.Id,
                p => p.RentalId,
                (r, payments) => new { r, payments })
            .Where(x => !x.payments.Any())
            .CountAsync();

        var unpaidAmount = await _context.PriceQuotes
            .Where(q => q.IsDeleted != true)
            .Join(_context.Rentals.Where(r => r.IsDeleted != true),
                q => q.RentalId,
                r => r.Id,
                (q, r) => new { q, r })
            .GroupJoin(_context.PaymentRecords.Where(p => p.Status == "Paid"),
                qr => qr.q.RentalId,
                p => p.RentalId,
                (qr, payments) => new { qr.q, payments })
            .Where(x => !x.payments.Any())
            .SumAsync(x => x.q.RentalFee + x.q.StaffFee + (x.q.DeliveryFee ?? 0) + x.q.CustomizationFee + x.q.DamageDeposit);

        // Statistics
        var allRentals = await _context.Rentals.Where(r => r.IsDeleted != true).ToListAsync();
        var statistics = new RentalStatistics
        {
            Active = allRentals.Count(r => r.Status == "Active" || r.Status == "InProgress"),
            Confirmed = allRentals.Count(r => r.Status == "Confirmed"),
            Pending = allRentals.Count(r => r.Status == "Pending" || r.Status == "PendingQuote"),
            Completed = allRentals.Count(r => r.Status == "Completed")
        };

        // Payment stats
        var paidPayments = await _context.PaymentRecords
            .Where(p => p.Status == "Paid")
            .GroupBy(p => 1)
            .Select(g => new { Count = g.Count(), Amount = g.Sum(p => p.Amount) })
            .FirstOrDefaultAsync();

        var partialPayments = await _context.PaymentRecords
            .Where(p => p.Status == "Partial")
            .GroupBy(p => 1)
            .Select(g => new { Count = g.Count(), Amount = g.Sum(p => p.Amount) })
            .FirstOrDefaultAsync();

        var unpaidPayments = await _context.PaymentRecords
            .Where(p => p.Status == "Pending" || p.Status == "Unpaid")
            .GroupBy(p => 1)
            .Select(g => new { Count = g.Count(), Amount = g.Sum(p => p.Amount) })
            .FirstOrDefaultAsync();

        var totalPayments = (paidPayments?.Amount ?? 0) + (partialPayments?.Amount ?? 0) + (unpaidPayments?.Amount ?? 0);
        var paymentStats = new PaymentStats
        {
            Paid = new PaymentStatItem { Count = paidPayments?.Count ?? 0, Amount = paidPayments?.Amount ?? 0 },
            Partial = new PaymentStatItem { Count = partialPayments?.Count ?? 0, Amount = partialPayments?.Amount ?? 0 },
            Unpaid = new PaymentStatItem { Count = unpaidPayments?.Count ?? 0, Amount = unpaidPayments?.Amount ?? 0 },
            CollectionRate = totalPayments > 0
                ? Math.Round((paidPayments?.Amount ?? 0) / totalPayments * 100, 1)
                : 0
        };

        // Get rentals with price quotes for total value
        var totalCount = await query.CountAsync();

        var rentalIds = await query
            .OrderByDescending(r => r.EventDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => r.Id)
            .ToListAsync();

        var rentalsData = await query
            .Where(r => rentalIds.Contains(r.Id))
            .OrderByDescending(r => r.EventDate)
            .Select(r => new
            {
                r.Id,
                CustomerName = r.Account.FullName ?? "Unknown",
                r.EventName,
                r.EventDate,
                StartTime = r.StartTime.HasValue ? r.StartTime.Value.ToString("HH:mm") : null,
                EndTime = r.EndTime.HasValue ? r.EndTime.Value.ToString("HH:mm") : null,
                Package = r.ActivityType.Code ?? "Unknown",
                r.Status
            })
            .ToListAsync();

        // Get price quotes for these rentals
        var quotes = await _context.PriceQuotes
            .Where(q => rentalIds.Contains(q.RentalId) && q.IsDeleted != true)
            .GroupBy(q => q.RentalId)
            .Select(g => new
            {
                RentalId = g.Key,
                TotalValue = g.OrderByDescending(q => q.CreatedAt).First().RentalFee +
                             g.OrderByDescending(q => q.CreatedAt).First().StaffFee +
                             (g.OrderByDescending(q => q.CreatedAt).First().DeliveryFee ?? 0) +
                             g.OrderByDescending(q => q.CreatedAt).First().CustomizationFee +
                             g.OrderByDescending(q => q.CreatedAt).First().DamageDeposit
            })
            .ToDictionaryAsync(x => x.RentalId, x => x.TotalValue);

        // Get payment status for these rentals
        var payments = await _context.PaymentRecords
            .Where(p => rentalIds.Contains(p.RentalId ?? 0))
            .GroupBy(p => p.RentalId)
            .Select(g => new { RentalId = g.Key, HasPaid = g.Any(p => p.Status == "Paid") })
            .ToDictionaryAsync(x => x.RentalId ?? 0, x => x.HasPaid);

        var rentals = rentalsData.Select(r => new RentalListItem
        {
            Id = r.Id,
            RentalCode = $"RNT-{r.Id:D4}",
            CustomerName = r.CustomerName,
            EventName = r.EventName,
            EventDate = r.EventDate,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            Package = r.Package,
            TotalValue = quotes.GetValueOrDefault(r.Id, 0),
            Status = r.Status ?? "Unknown",
            PaymentStatus = payments.GetValueOrDefault(r.Id, false) ? "Paid" : "Unpaid"
        }).ToList();

        return new DashboardRentalsResponse
        {
            Alerts = new RentalAlerts
            {
                PendingApprovals = pendingApprovals,
                UnpaidOrders = unpaidRentals,
                UnpaidAmount = unpaidAmount
            },
            Statistics = statistics,
            PaymentStats = paymentStats,
            Rentals = rentals,
            TotalCount = totalCount
        };
    }

    public async Task<DashboardCustomersResponse> GetCustomersAsync(int topCount = 10)
    {
        // Customer rental counts
        var customerData = await _context.Rentals
            .Where(r => r.AccountId != null && r.IsDeleted != true)
            .GroupBy(r => r.AccountId)
            .Select(g => new { AccountId = g.Key, OrderCount = g.Count() })
            .ToListAsync();

        var totalCustomers = customerData.Count;
        var repeatCustomers = customerData.Count(c => c.OrderCount > 1);
        var repeatRate = totalCustomers > 0
            ? Math.Round((decimal)repeatCustomers / totalCustomers * 100, 1)
            : 0;

        // Total revenue
        var totalRevenue = await _context.PaymentRecords
            .Where(p => p.Status == "Paid")
            .SumAsync(p => p.Amount);
        var avgLTV = totalCustomers > 0
            ? Math.Round(totalRevenue / totalCustomers, 0)
            : 0;

        // Top customers by spending
        var customerSpending = await _context.PaymentRecords
            .Where(p => p.Status == "Paid" && p.RentalId != null)
            .Join(_context.Rentals,
                p => p.RentalId,
                r => r.Id,
                (p, r) => new { r.AccountId, p.Amount })
            .Where(x => x.AccountId != null)
            .GroupBy(x => x.AccountId)
            .Select(g => new { AccountId = g.Key, TotalSpent = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.TotalSpent)
            .Take(topCount)
            .ToListAsync();

        var topAccountIds = customerSpending.Select(c => c.AccountId).ToList();

        var accounts = await _context.Accounts
            .Where(a => topAccountIds.Contains(a.Id))
            .Include(a => a.ModifyIdentityUser)
            .ToDictionaryAsync(a => a.Id, a => a);

        var accountOrderCounts = await _context.Rentals
            .Where(r => topAccountIds.Contains(r.AccountId) && r.IsDeleted != true)
            .GroupBy(r => r.AccountId)
            .Select(g => new { AccountId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AccountId ?? 0, x => x.Count);

        var favoritePackages = await _context.Rentals
            .Where(r => topAccountIds.Contains(r.AccountId) && r.IsDeleted != true && r.ActivityTypeId != null)
            .Include(r => r.ActivityType)
            .GroupBy(r => new { r.AccountId, r.ActivityType.Code })
            .Select(g => new { AccountId = g.Key.AccountId, Package = g.Key.Code, Count = g.Count() })
            .ToListAsync();

        var topCustomers = customerSpending.Select(c =>
        {
            var account = accounts.GetValueOrDefault(c.AccountId ?? 0);
            var favPackage = favoritePackages
                .Where(f => f.AccountId == c.AccountId)
                .OrderByDescending(f => f.Count)
                .FirstOrDefault();

            return new TopCustomerItem
            {
                AccountId = c.AccountId ?? 0,
                FullName = account?.FullName ?? "Unknown",
                Email = account?.ModifyIdentityUser?.Email,
                TotalSpent = c.TotalSpent,
                OrderCount = accountOrderCounts.GetValueOrDefault(c.AccountId ?? 0, 0),
                FavoritePackage = favPackage?.Package ?? "N/A"
            };
        }).ToList();

        // Segmentation
        var segmentation = new CustomerSegmentation
        {
            Vip = customerData.Count(c => c.OrderCount >= 10),
            Regular = customerData.Count(c => c.OrderCount >= 3 && c.OrderCount < 10),
            Occasional = customerData.Count(c => c.OrderCount == 2),
            OneTime = customerData.Count(c => c.OrderCount == 1)
        };

        return new DashboardCustomersResponse
        {
            Overview = new CustomerOverview
            {
                TotalCustomers = totalCustomers,
                RepeatRate = repeatRate,
                AvgLTV = avgLTV
            },
            TopCustomers = topCustomers,
            Segmentation = segmentation
        };
    }
}

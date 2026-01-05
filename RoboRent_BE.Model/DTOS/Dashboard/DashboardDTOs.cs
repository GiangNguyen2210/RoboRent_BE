namespace RoboRent_BE.Model.DTOs.Dashboard;

// ============ TAB 1: OVERVIEW RESPONSE ============

public class OverviewKpis
{
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueTrend { get; set; } // % change vs last month
    public int ActiveOrders { get; set; }
    public decimal RobotUtilization { get; set; } // percentage
    public decimal RepeatRate { get; set; } // percentage
}

public class OverviewAlerts
{
    public int PendingQuotes { get; set; }
    public int PendingContracts { get; set; }
    public int PendingReports { get; set; }
}

public class RevenueChartItem
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Aov { get; set; } // Average Order Value
}

public class PeakTimeItem
{
    public string DayOfWeek { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

public class PackageDistributionItem
{
    public string Package { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
}

public class RobotTypeRevenueItem
{
    public string RobotType { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
}

public class SystemNotificationItem
{
    public int Id { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // warning, error, success, info
    public string Message { get; set; } = string.Empty;
    public int? RentalId { get; set; }
}

public class DashboardOverviewResponse
{
    public OverviewKpis Kpis { get; set; } = new();
    public OverviewAlerts Alerts { get; set; } = new();
    public List<RevenueChartItem> RevenueChart { get; set; } = new();
    public List<PeakTimeItem> PeakTimeAnalysis { get; set; } = new();
    public List<PackageDistributionItem> PackageDistribution { get; set; } = new();
    public List<RobotTypeRevenueItem> RobotTypeRevenue { get; set; } = new();
    public List<SystemNotificationItem> Notifications { get; set; } = new();
}

// ============ TAB 2: ROBOTS RESPONSE ============

public class RobotFleetOverview
{
    public int Total { get; set; }
    public int Available { get; set; }
    public int Renting { get; set; }
    public int Maintenance { get; set; }
    public decimal Utilization { get; set; }
}

public class RobotByTypeItem
{
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Available { get; set; }
    public int Renting { get; set; }
    public int Maintenance { get; set; }
    public decimal Utilization { get; set; }
}

public class RobotListItem
{
    public int Id { get; set; }
    public string RobotName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Location { get; set; }
}

public class DashboardRobotsResponse
{
    public RobotFleetOverview Overview { get; set; } = new();
    public List<RobotByTypeItem> ByType { get; set; } = new();
    public List<RobotListItem> Robots { get; set; } = new();
}

// ============ TAB 3: RENTALS RESPONSE ============

public class RentalAlerts
{
    public int PendingApprovals { get; set; }
    public int UnpaidOrders { get; set; }
    public decimal UnpaidAmount { get; set; }
}

public class RentalStatistics
{
    public int Active { get; set; }
    public int Confirmed { get; set; }
    public int Pending { get; set; }
    public int Completed { get; set; }
}

public class PaymentStatItem
{
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentStats
{
    public PaymentStatItem Paid { get; set; } = new();
    public PaymentStatItem Partial { get; set; } = new();
    public PaymentStatItem Unpaid { get; set; } = new();
    public decimal CollectionRate { get; set; }
}

public class RentalListItem
{
    public int Id { get; set; }
    public string RentalCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? EventName { get; set; }
    public DateTime? EventDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public string Package { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
}

public class DashboardRentalsResponse
{
    public RentalAlerts Alerts { get; set; } = new();
    public RentalStatistics Statistics { get; set; } = new();
    public PaymentStats PaymentStats { get; set; } = new();
    public List<RentalListItem> Rentals { get; set; } = new();
    public int TotalCount { get; set; }
}

// ============ TAB 4: CUSTOMERS RESPONSE ============

public class CustomerOverview
{
    public int TotalCustomers { get; set; }
    public decimal RepeatRate { get; set; }
    public decimal AvgLTV { get; set; }
}

public class TopCustomerItem
{
    public int AccountId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public decimal TotalSpent { get; set; }
    public int OrderCount { get; set; }
    public string FavoritePackage { get; set; } = string.Empty;
}

public class CustomerSegmentation
{
    public int Vip { get; set; }       // 10+ orders
    public int Regular { get; set; }   // 3-9 orders
    public int Occasional { get; set; } // 2 orders
    public int OneTime { get; set; }   // 1 order
}

public class DashboardCustomersResponse
{
    public CustomerOverview Overview { get; set; } = new();
    public List<TopCustomerItem> TopCustomers { get; set; } = new();
    public CustomerSegmentation Segmentation { get; set; } = new();
}

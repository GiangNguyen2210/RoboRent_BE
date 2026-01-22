namespace RoboRent_BE.Service.Configuration;

public static class DeliveryFeeConfig
{
    // Base config
    private const decimal HCM_FLAT_FEE = 7000m;
    private const decimal RATE_PER_KM_ROUND_TRIP = 20m; // 15k × 2

    // Dictionary: Key = tên tỉnh/thành (y hệt API), Value = khoảng cách (km)
    private static readonly Dictionary<string, int> DistanceFromHCM = new()
    {
        { "Thành phố Hồ Chí Minh", 0 },
        { "Cao Bằng", 1900 },
        { "Tuyên Quang", 1750 },
        { "Điện Biên", 1950 },
        { "Lai Châu", 2000 },
        { "Sơn La", 1800 },
        { "Lào Cai", 1900 },
        { "Thái Nguyên", 1680 },
        { "Lạng Sơn", 1800 },
        { "Quảng Ninh", 1750 },
        { "Bắc Ninh", 1700 },
        { "Phú Thọ", 1700 },
        { "Thành phố Hà Nội", 1730 },
        { "Thành phố Hải Phòng", 1700 },
        { "Hưng Yên", 1710 },
        { "Ninh Bình", 1600 },
        { "Thanh Hóa", 1450 },
        { "Nghệ An", 1300 },
        { "Hà Tĩnh", 1200 },
        { "Quảng Trị", 1100 },
        { "Thành phố Huế", 1050 },
        { "Thành phố Đà Nẵng", 950 },
        { "Quảng Ngãi", 850 },
        { "Gia Lai", 700 },
        { "Khánh Hòa", 450 },
        { "Đắk Lắk", 350 },
        { "Lâm Đồng", 300 },
        { "Đồng Nai", 50 },
        { "Tây Ninh", 100 },
        { "Đồng Tháp", 150 },
        { "Vĩnh Long", 140 },
        { "An Giang", 200 },
        { "Thành phố Cần Thơ", 170 },
        { "Cà Mau", 350 }
    };

    public static (decimal Fee, int? Distance) CalculateFee(string city)
    {
        // Normalize city name (trim spaces)
        city = city?.Trim() ?? string.Empty;

        // Check if city exists in dictionary
        if (!DistanceFromHCM.TryGetValue(city, out int distance))
        {
            // Fallback: return 0 fee, null distance (không crash)
            return (0m, null);
        }

        // HCM: flat fee
        if (distance == 0)
        {
            return (HCM_FLAT_FEE, null);
        }

        // Tỉnh khác: distance × 30,000
        decimal fee = distance * RATE_PER_KM_ROUND_TRIP;
        return (fee, distance);
    }

    /// <summary>
    /// Tính thời gian vận chuyển (giờ) dựa trên khoảng cách
    /// Logic: Staff phải đi trước setup time để kịp đến
    /// </summary>
    public static int GetTravelTimeHours(int? distanceKm)
    {
        if (distanceKm == null) return 2; // Default fallback
        
        return distanceKm switch
        {
            0 => 2,              // HCM: 2h (chuẩn bị + di chuyển nội thành)
            <= 100 => 3,         // Lân cận: 3h
            <= 200 => 4,         // ĐBSCL gần: 4h
            <= 400 => 6,         // ĐBSCL xa + Tây Nguyên gần: 6h
            <= 700 => 10,        // Tây Nguyên xa + Miền Trung gần: 10h
            <= 1200 => 16,       // Miền Trung: 16h (qua đêm)
            _ => 24              // Miền Bắc: 24h (1 ngày)
        };
    }
}
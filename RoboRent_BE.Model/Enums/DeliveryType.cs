namespace RoboRent_BE.Model.Enums;

/// <summary>
/// Phân loại delivery trong ngày của Technical Staff
/// </summary>
public enum DeliveryType
{
    /// <summary>
    /// Đơn đầu ngày - Rời kho, có status "LeftWarehouse"
    /// </summary>
    FirstOfDay = 1,

    /// <summary>
    /// Đơn giữa ngày - Không rời/về kho
    /// </summary>
    MidDay = 2,

    /// <summary>
    /// Đơn cuối ngày - Về kho, có status "ReturnedWarehouse"
    /// </summary>
    LastOfDay = 3
}

namespace RoboRent_BE.Model.Enums;

/// <summary>
/// Phân loại delivery trong ngày của Technical Staff
/// </summary>
public enum DeliveryType
{
    /// <summary>
    /// Đơn đầu ngày - Rời kho, có status "LeftWarehouse"
    /// </summary>
    FirstOfDay = 0,

    /// <summary>
    /// Đơn giữa ngày - Không rời/về kho
    /// </summary>
    MidDay = 1,

    /// <summary>
    /// Đơn cuối ngày - Về kho, có status "ReturnedWarehouse"
    /// </summary>
    LastOfDay = 2,

    /// <summary>
    /// Đơn độc lập - Không liên quan đến ngày
    /// </summary>
    SoleDelivery = 3
}

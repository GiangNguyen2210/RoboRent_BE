namespace RoboRent_BE.Model.Enums;

/// <summary>
/// Types of notifications based on the rental flow phases
/// </summary>
public enum NotificationType
{
    // Phase 1: Request
    NewRequest,           // Customer tạo request → Staff
    RequestReceived,      // Staff nhận request → Customer
    RequestUpdate,        // Staff request update → Customer
    RequestCancelled,     // Customer cancel → Staff

    // Phase 2: Quote
    QuotePendingApproval, // Staff tạo quote → Manager
    QuoteApproved,        // Manager approve → Customer
    QuoteRejected,        // Manager reject → Staff
    QuoteAccepted,        // Customer accept → Staff
    QuoteRejectedByCustomer, // Customer reject → Staff

    // Phase 3: Schedule
    ScheduleCreated,      // Staff tạo schedule → Customer
    ScheduleUpdated,      // Staff update schedule → Customer
    ScheduleCancelled,    // Staff cancel schedule → Customer

    // Phase 4: Demo
    DemoCreated,          // Staff gửi demo → Customer
    DemoAccepted,         // Customer accept → Staff
    DemoRejected,         // Customer reject → Staff

    // Phase 5: Contract
    ContractPendingApproval,    // Staff tạo contract → Manager
    ContractManagerSigned,      // Manager ký → Customer
    ContractManagerRejected,    // Manager reject → Staff
    ContractCustomerSigned,     // Customer ký → Staff
    ContractChangeRequested,    // Customer yêu cầu sửa → Staff

    // Phase 6: Delivery
    DeliveryCreated,      // Delivery tạo → Manager
    DeliveryAssigned,     // Staff tech assigned → Staff tech
    DeliveryStatusUpdate, // Status update → Customer

    // Phase 7: Contract Report
    ReportCreated,        // Report tạo → Manager
    ReportResolved,       // Manager resolve → Customer
    ReportRejected,       // Manager reject → Reporter

    // Phase 8: Payment
    PaymentLinkCreated,   // Payment link → Customer
    PaymentSuccess,       // Payment success → Staff
    PaymentFailed,        // Payment failed → Customer

    // General
    SystemNotification    // System messages
}

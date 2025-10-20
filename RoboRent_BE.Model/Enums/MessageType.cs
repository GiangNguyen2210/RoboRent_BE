namespace RoboRent_BE.Model.Enums;

public enum MessageType
{
    Text,                       // Chat message bình thường
    Demo,                       // Staff gửi demo video
    PriceQuoteNotification,     // Notification: "Staff created quote #X"
    ContractNotification,       // Notification: "Staff sent contract"
    SystemNotification          // System messages: "Demo accepted", "Quote rejected"
}
namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class PendingDeliveriesGroupedResponse
{
    public List<GroupedDeliveryInfo> Groups { get; set; } = new();
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
}

public class GroupedDeliveryInfo
{
    public int ActivityTypeGroupId { get; set; }
    public string ActivityTypeGroupName { get; set; } = string.Empty;
    public string EventDate { get; set; } = string.Empty;
    public List<int> DeliveryIds { get; set; } = new();
    public List<int> GroupScheduleIds { get; set; } = new();
    public int ScheduleCount { get; set; }
    public ScheduleInfoSummary ScheduleInfo { get; set; } = new();
    public RentalInfoSummary RentalInfo { get; set; } = new();
}

public class ScheduleInfoSummary
{
    public List<string> EventLocations { get; set; } = new();
    public List<string> EventCities { get; set; } = new();
    public string EarliestSetupTime { get; set; } = string.Empty;
    public string LatestFinishTime { get; set; } = string.Empty;
}

public class RentalInfoSummary
{
    public List<string> EventNames { get; set; } = new();
    public List<string> CustomerNames { get; set; } = new();
}

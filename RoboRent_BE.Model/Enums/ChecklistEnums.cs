namespace RoboRent_BE.Model.Entities;

public enum ChecklistDeliveryType
{
    PreDispatch = 1,   // trước khi xuất kho/vận chuyển
    Handover = 2,      // bàn giao tại nơi
    ReturnIntake = 3   // nhận trả
}

public enum ChecklistDeliveryStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4
}

public enum ChecklistItemResult
{
    Unknown = 0,
    Pass = 1,
    Fail = 2,
    NotApplicable = 3
}

public enum ChecklistSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum EvidenceType
{
    Photo = 1,
    Video = 2,
    File = 3
}

public enum EvidenceScope
{
    Overall = 1,
    Item = 2
}
namespace RaceDay.Mvc.Models;

public record ApiLoginResult(
    string Token,
    int UserID,
    string FirstName,
    string LastName,
    string Email,
    string Role
);

public record ApiUser(
    int UserID,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Role,
    string? ProfilePictureUrl
);

public record ApiEvent(
    int EventID,
    string EventName,
    string EventType,
    string? Description,
    DateTime EventDate,
    TimeSpan StartTime,
    string Location,
    int OrganiserID,
    string? BannerUrl,
    decimal? DistanceKm
);

public record ApiCategory(
    int CategoryID,
    int EventID,
    string CategoryName,
    decimal? DistanceKm,
    int? MaximumParticipants,
    decimal EntryFee
);

public record ApiResult(
    int ResultID,
    int EnrolmentID,
    string? EventName,
    int? FinishPosition,
    TimeSpan? FinishTime,
    string ResultStatus
);

public record ApiEnrolment(
    int EnrolmentID,
    int ParticipantID,
    string ParticipantName,
    int CategoryID,
    string CategoryName,
    DateTime EnrolmentDate,
    string Status
);

public record ApiEventResult(
    int ResultID,
    int EnrolmentID,
    string ParticipantName,
    int? FinishPosition,
    TimeSpan? FinishTime,
    string ResultStatus
);

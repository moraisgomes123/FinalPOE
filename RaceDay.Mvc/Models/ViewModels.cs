using System.ComponentModel.DataAnnotations;
namespace RaceDay.Mvc.Models;
public class LoginViewModel{[Required,EmailAddress]public string Email{get;set;}="";[Required,DataType(DataType.Password)]public string Password{get;set;}="";}
public class RegisterViewModel{[Required]public string FirstName{get;set;}="";[Required]public string LastName{get;set;}="";[Required,EmailAddress]public string Email{get;set;}="";[Required,MinLength(8),DataType(DataType.Password)]public string Password{get;set;}="";[Phone]public string? Phone{get;set;}}
public class EventViewModel{public int EventID{get;set;}[Required]public string EventName{get;set;}="";[Required]public string EventType{get;set;}="Running";public string? Description{get;set;}[Required]public DateTime EventDate{get;set;}=DateTime.Today.AddDays(30);public TimeSpan StartTime{get;set;}=new(8,0,0);[Required]public string Location{get;set;}="";public decimal DistanceKm{get;set;}=5;public string? BannerUrl{get;set;}public IFormFile? BannerImage{get;set;}}
public class CategoryViewModel{[Required]public string CategoryName{get;set;}="";public decimal? DistanceKm{get;set;}public int? MaximumParticipants{get;set;}public decimal EntryFee{get;set;}}
public class ProfileViewModel{[Required]public string FirstName{get;set;}="";[Required]public string LastName{get;set;}="";[Required,EmailAddress]public string Email{get;set;}="";[Phone]public string? Phone{get;set;}public string? ProfilePictureUrl{get;set;}public IFormFile? ProfilePicture{get;set;}}

public class OrganiserEnrolmentsViewModel
{
    public int EventID { get; set; }

    public string EventName { get; set; } = "";

    public List<ApiEnrolment> Enrolments { get; set; } = new();
}

public class OrganiserResultsViewModel
{
    public int EventID { get; set; }

    public string EventName { get; set; } = "";

    public List<ApiEnrolment> Enrolments { get; set; } = new();

    public List<ApiEventResult> Results { get; set; } = new();
}

public class ResultEntryViewModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int EventID { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int EnrolmentID { get; set; }

    [Range(1, int.MaxValue)]
    public int? FinishPosition { get; set; }

    public string? FinishTime { get; set; }

    [Required]
    public string ResultStatus { get; set; } = "Finished";
}
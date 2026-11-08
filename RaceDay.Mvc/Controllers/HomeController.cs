using Microsoft.AspNetCore.Mvc;using RaceDay.Mvc.Services;using RaceDay.Mvc.Models;
namespace RaceDay.Mvc.Controllers;
public class HomeController(RaceDayApiClient api):Controller
{
 public async Task<IActionResult> Index(){var events=await api.GetAsync<List<ApiEvent>>("api/events")??new();ViewBag.Events=events.Where(x=>x.EventDate.Date>=DateTime.Today).OrderBy(x=>x.EventDate).ToList();return View();}
 public IActionResult AccessDenied()=>View();
 public IActionResult Error()=>View();
}

using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class HomeController(RaceDayApiClient api) : Controller
{
    public async Task<IActionResult> Index()
    {
        var events = await api.GetAsync<List<ApiEvent>>("api/events") ?? [];
        
        ViewBag.Events = events
            .Where(x => x.EventDate.Date >= DateTime.Today)
            .OrderBy(x => x.EventDate)
            .ToList();
            
        return View();
    }

    public IActionResult AccessDenied() => View();

    public IActionResult Error() => View();
}

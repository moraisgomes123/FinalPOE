using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class ParticipantController(RaceDayApiClient api) : Controller
{
    public async Task<IActionResult> Index()
    {
        var all = await api.GetAsync<List<ApiEvent>>("api/events") ?? [];
        
        var upcomingEvents = all
            .Where(e => e.EventDate.Date >= DateTime.Today)
            .OrderBy(e => e.EventDate)
            .ToList();
            
        return View(upcomingEvents);
    }

    public async Task<IActionResult> Event(int id)
    {
        ViewBag.Categories = await api.GetAsync<List<ApiCategory>>($"api/events/{id}/categories") ?? [];
        
        var e = await api.GetAsync<ApiEvent>($"api/events/{id}");
        
        return e == null ? NotFound() : View(e);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(int id, int categoryId)
    {
        var r = await api.SendAsync<object>(HttpMethod.Post, $"api/events/{id}/enrolments", new { CategoryID = categoryId });
        
        TempData[r.Ok ? "Success" : "Error"] = r.Ok ? "Your enrolment is confirmed." : r.Message;
        
        return RedirectToAction(nameof(Event), new { id });
    }

    public async Task<IActionResult> MyEvents()
    {
        ViewBag.Items = await api.GetAsync<List<System.Text.Json.JsonElement>>("api/enrolments/me") ?? [];
        return View();
    }

    public async Task<IActionResult> MyResults()
    {
        ViewBag.Items = await api.GetAsync<List<ApiResult>>("api/results/me") ?? [];
        return View();
    }

    public async Task<IActionResult> Profile()
    {
        var p = await api.GetAsync<ProfileViewModel>("api/users/me");
        return View(p ?? new());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel m)
    {
        if (!ModelState.IsValid)
        {
            return View(m);
        }

        var r = await api.SendAsync<object>(HttpMethod.Put, "api/users/me", new { m.FirstName, m.LastName, m.Email, m.Phone });
        
        if (!r.Ok)
        {
            ModelState.AddModelError("", r.Message);
            return View(m);
        }

        if (m.ProfilePicture != null)
        {
            var up = await api.UploadAsync("api/users/me/profile-picture", m.ProfilePicture);
            if (!up.Ok)
            {
                TempData["Error"] = up.Message;
            }
        }

        TempData["Success"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }
}

using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class OrganiserController(RaceDayApiClient api) : Controller
{
    // ============================================
    // DASHBOARD
    // ============================================

    public async Task<IActionResult> Index()
    {
        var all = await api.GetAsync<List<ApiEvent>>("api/events") ?? new();

        var mine = all
            .Where(e => e.OrganiserID.ToString() == HttpContext.Session.GetString("UserID"))
            .OrderBy(e => e.EventDate)
            .ToList();

        ViewBag.Total = mine.Count;
        ViewBag.Upcoming = mine.Count(e => e.EventDate >= DateTime.Today);

        return View(mine);
    }

    // ============================================
    // EVENTS
    // ============================================

    public async Task<IActionResult> Events()
    {
        var all = await api.GetAsync<List<ApiEvent>>("api/events") ?? new();

        return View(all
            .Where(e => e.OrganiserID.ToString() == HttpContext.Session.GetString("UserID"))
            .ToList());
    }

    [HttpGet]
    public IActionResult CreateEvent() => View("EventForm", new EventViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(EventViewModel m)
    {
        if (!ModelState.IsValid)
            return View("EventForm", m);

        var dto = new
        {
            m.EventName,
            m.EventType,
            m.Description,
            m.EventDate,
            m.StartTime,
            m.Location
        };

        var r = await api.SendAsync<System.Text.Json.JsonElement>(HttpMethod.Post, "api/events", dto);
        if (!r.Ok)
        {
            ModelState.AddModelError("", r.Message);
            return View("EventForm", m);
        }

        // Upload banner (optional)
        if (m.BannerImage != null
            && r.Data.ValueKind == System.Text.Json.JsonValueKind.Object
            && r.Data.TryGetProperty("eventID", out var id))
        {
            var up = await api.UploadAsync($"api/events/{id.GetInt32()}/banner", m.BannerImage);
            if (!up.Ok)
                TempData["Error"] = up.Message;
        }

        TempData["Success"] = "Event created.";
        return RedirectToAction(nameof(Events));
    }

    // ============================================
    // CATEGORIES
    // ============================================

    public async Task<IActionResult> Categories(int id)
    {
        ViewBag.EventId = id;
        ViewBag.Categories = await api.GetAsync<List<ApiCategory>>($"api/events/{id}/categories") ?? new();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(int id, CategoryViewModel m)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter a valid category.";
            return RedirectToAction(nameof(Categories), new { id });
        }

        var r = await api.SendAsync<object>(HttpMethod.Post, $"api/events/{id}/categories", m);
        TempData[r.Ok ? "Success" : "Error"] = r.Ok ? "Category added." : r.Message;

        return RedirectToAction(nameof(Categories), new { id });
    }

    // ============================================
    // ENROLMENTS
    // ============================================

    public async Task<IActionResult> Enrolments(int id)
    {
        var eventItem = await api.GetAsync<ApiEvent>($"api/events/{id}");

        if (eventItem == null)
            return NotFound();

        var enrolments = await api.GetAsync<List<ApiEnrolment>>(
            $"api/events/{id}/enrolments");

        var model = new OrganiserEnrolmentsViewModel
        {
            EventID = id,
            EventName = eventItem.EventName,
            Enrolments = enrolments ?? new List<ApiEnrolment>()
        };

        return View(model);
    }

    // ============================================
    // RESULTS
    // ============================================

    public async Task<IActionResult> Results(int id)
    {
        var eventItem = await api.GetAsync<ApiEvent>($"api/events/{id}");

        if (eventItem == null)
            return NotFound();

        var enrolments = await api.GetAsync<List<ApiEnrolment>>(
            $"api/events/{id}/enrolments");

        var results = await api.GetAsync<List<ApiEventResult>>(
            $"api/events/{id}/results");

        var model = new OrganiserResultsViewModel
        {
            EventID = id,
            EventName = eventItem.EventName,
            Enrolments = enrolments ?? new List<ApiEnrolment>(),
            Results = results ?? new List<ApiEventResult>()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddResult(ResultEntryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter valid result information.";
            return RedirectToAction(nameof(Results), new { id = model.EventID });
        }

        TimeSpan? finishTime = null;

        if (!string.IsNullOrWhiteSpace(model.FinishTime))
        {
            if (!TimeSpan.TryParse(model.FinishTime, out var parsedTime)
                || parsedTime < TimeSpan.Zero
                || parsedTime >= TimeSpan.FromDays(1))
            {
                TempData["Error"] = "Please enter a valid finish time.";
                return RedirectToAction(nameof(Results), new { id = model.EventID });
            }

            finishTime = parsedTime;
        }

        var dto = new
        {
            model.EnrolmentID,
            model.FinishPosition,
            FinishTime = finishTime,
            model.ResultStatus
        };

        var response = await api.SendAsync<System.Text.Json.JsonElement>(
            HttpMethod.Post,
            $"api/events/{model.EventID}/results",
            dto
        );

        TempData[response.Ok ? "Success" : "Error"] =
            response.Ok
                ? "Result saved successfully."
                : response.Message;

        return RedirectToAction(nameof(Results), new { id = model.EventID });
    }
}
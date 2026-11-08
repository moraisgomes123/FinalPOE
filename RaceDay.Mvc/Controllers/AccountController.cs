using Microsoft.AspNetCore.Mvc;using RaceDay.Mvc.Models;using RaceDay.Mvc.Services;
namespace RaceDay.Mvc.Controllers;
public class AccountController(RaceDayApiClient api):Controller
{
 [HttpGet]public IActionResult Login()=>View();
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Login(LoginViewModel model){if(!ModelState.IsValid)return View(model);var result=await api.SendAsync<ApiLoginResult>(HttpMethod.Post,"api/auth/login",model);if(!result.Ok||result.Data is null){ModelState.AddModelError("",result.Message);return View(model);}HttpContext.Session.SetString("Token",result.Data.Token);HttpContext.Session.SetString("UserID",result.Data.UserID.ToString());HttpContext.Session.SetString("Role",result.Data.Role);HttpContext.Session.SetString("Name",$"{result.Data.FirstName} {result.Data.LastName}");return RedirectToAction("Index",result.Data.Role=="Organiser"?"Organiser":"Participant");}
 [HttpGet]public IActionResult Register()=>View();
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Register(RegisterViewModel model){if(!ModelState.IsValid)return View(model);var r=await api.SendAsync<object>(HttpMethod.Post,"api/auth/register",model);if(!r.Ok){ModelState.AddModelError("",r.Message);return View(model);}TempData["Success"]="Account created. Please sign in.";return RedirectToAction(nameof(Login));}
 [HttpPost,ValidateAntiForgeryToken]public IActionResult Logout(){HttpContext.Session.Clear();return RedirectToAction("Index","Home");}
}

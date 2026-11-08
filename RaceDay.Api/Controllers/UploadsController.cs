using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using RaceDay.Api.Data;using RaceDay.Api.Services;using System.Security.Claims;
namespace RaceDay.Api.Controllers;
[ApiController][Authorize][Route("api")]
public class UploadsController(RaceDayContext db,IBlobStorageService blobs):ControllerBase
{
 private int? UserId(){var value=User.FindFirstValue(ClaimTypes.NameIdentifier);return int.TryParse(value,out var id)?id:null;}
 private static bool ValidImage(IFormFile file)=>file.Length>0&&file.Length<=5*1024*1024&&new[]{"image/jpeg","image/png","image/webp"}.Contains(file.ContentType,StringComparer.OrdinalIgnoreCase);
 [HttpPost("events/{eventId:int}/banner")][Authorize(Roles="Organiser")][RequestSizeLimit(5*1024*1024)]
 public async Task<IActionResult> EventBanner(int eventId,IFormFile file,CancellationToken ct){if(file==null||!ValidImage(file))return BadRequest(new{message="Upload a JPG, PNG or WEBP image up to 5 MB."});var id=UserId();var item=await db.Events.SingleOrDefaultAsync(x=>x.EventID==eventId,ct);if(item==null)return NotFound();if(item.OrganiserID!=id)return Forbid();await using var stream=file.OpenReadStream();item.BannerUrl=await blobs.UploadAsync(stream,file.FileName,file.ContentType,"event-banners",ct);await db.SaveChangesAsync(ct);return Ok(new{item.EventID,item.BannerUrl});}
 [HttpPost("users/me/profile-picture")][RequestSizeLimit(5*1024*1024)]
 public async Task<IActionResult> ProfilePicture(IFormFile file,CancellationToken ct){if(file==null||!ValidImage(file))return BadRequest(new{message="Upload a JPG, PNG or WEBP image up to 5 MB."});var id=UserId();if(id==null)return Unauthorized();var user=await db.Users.FindAsync(new object[]{id.Value},ct);if(user==null)return NotFound();await using var stream=file.OpenReadStream();user.ProfilePictureUrl=await blobs.UploadAsync(stream,file.FileName,file.ContentType,"profile-pictures",ct);await db.SaveChangesAsync(ct);return Ok(new{user.UserID,user.ProfilePictureUrl});}
}

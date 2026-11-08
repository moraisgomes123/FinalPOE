using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
namespace RaceDay.Mvc.Services;
public class RaceDayApiClient
{
 private readonly HttpClient _http; private readonly IHttpContextAccessor _accessor;
 private static readonly JsonSerializerOptions JsonOptions=new(JsonSerializerDefaults.Web){PropertyNameCaseInsensitive=true};
 public RaceDayApiClient(IHttpClientFactory factory,IHttpContextAccessor accessor){_http=factory.CreateClient("RaceDayApi");_accessor=accessor;}
 private void Authorise(){_http.DefaultRequestHeaders.Authorization=null;var token=_accessor.HttpContext?.Session.GetString("Token");if(!string.IsNullOrWhiteSpace(token))_http.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);}
 public async Task<T?> GetAsync<T>(string path){Authorise();var r=await _http.GetAsync(path);if(!r.IsSuccessStatusCode)return default;return await r.Content.ReadFromJsonAsync<T>(JsonOptions);}
 public async Task<(bool Ok,string Message,T? Data)> SendAsync<T>(HttpMethod method,string path,object? body=null){Authorise();using var req=new HttpRequestMessage(method,path);if(body!=null)req.Content=JsonContent.Create(body);using var res=await _http.SendAsync(req);var raw=await res.Content.ReadAsStringAsync();if(!res.IsSuccessStatusCode)return(false,ExtractMessage(raw)??$"API request failed ({(int)res.StatusCode}).",default);T? data=default;try{data=JsonSerializer.Deserialize<T>(raw,JsonOptions);}catch{}return(true,"",data);}
 public async Task<(bool Ok,string Message)> UploadAsync(string path,IFormFile file){Authorise();using var form=new MultipartFormDataContent();await using var stream=file.OpenReadStream();using var content=new StreamContent(stream);content.Headers.ContentType=new MediaTypeHeaderValue(file.ContentType);form.Add(content,"file",Path.GetFileName(file.FileName));using var res=await _http.PostAsync(path,form);var raw=await res.Content.ReadAsStringAsync();return(res.IsSuccessStatusCode,res.IsSuccessStatusCode?"":ExtractMessage(raw)??"Image upload failed.");}
 private static string? ExtractMessage(string raw){try{return JsonDocument.Parse(raw).RootElement.TryGetProperty("message",out var m)?m.GetString():null;}catch{return null;}}
}

using LinkyFunky.Application.Interfaces;
using Web.Defaults;

namespace Web.Services;

sealed class GuestService(IHttpContextAccessor httpContextAccessor) : IGuestService
{
    HttpContext HttpContext => httpContextAccessor.HttpContext!;
    public bool IsGuest => !(HttpContext.User.Identity?.IsAuthenticated ?? false) 
                           && HttpContext.Request.Cookies.ContainsKey(CookieDefaults.GuestCookieId);

    public Guid GuestIdOrDefault
    {
        get
        {
            if (!IsGuest)
                return Guid.Empty;
            
            HttpContext.Request.Cookies.TryGetValue(CookieDefaults.GuestCookieId, out var guestCookieId);
            
            if (!string.IsNullOrWhiteSpace(guestCookieId) && Guid.TryParse(guestCookieId, out var guestId))
                return guestId;
            
            return Guid.Empty;
        }
    }
    
    public void SetGuest()
    {
        HttpContext.Response.Cookies.Append(CookieDefaults.GuestCookieId, Guid.NewGuid().ToString(), new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
    }

    public void DeleteGuest()
    {
        HttpContext.Response.Cookies.Delete(CookieDefaults.GuestCookieId);
    }
}
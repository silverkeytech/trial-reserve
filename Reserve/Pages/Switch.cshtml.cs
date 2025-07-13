using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reserve.Pages;

public class SwitchModel : PageModel
{
    public IActionResult OnGet(string culture, string redirectUrl)
    {
        if (culture != null)
        {
            HttpContext.Response.Cookies.Delete(CookieRequestCultureProvider.DefaultCookieName);
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));
        }

        if (HttpContext.Request.Query.Count < 3)
            return LocalRedirect(redirectUrl);

        var url = HttpContext.Request.QueryString.Value;
        var queryString = url?.Split("?").Last();
        var qsItems = queryString?.Substring(queryString.IndexOf("&"));

        return LocalRedirect(redirectUrl + qsItems);
    }
}
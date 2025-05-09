using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MvcPracownicy.Models;
using Microsoft.Extensions.Caching.Memory;

namespace MvcPracownicy.Controllers;

[Route("/info")]
public class InfoController : Controller
{
    private readonly ILogger<InfoController> _logger;

    private readonly IMemoryCache _memoryCache;

    public InfoController(ILogger<InfoController> logger, IMemoryCache memoryCache)
    {
        _logger = logger; 
        _memoryCache = memoryCache;
    }

    public IActionResult Info() 
    {
        ViewData["Name"] = "Patryk";
        ViewData["Age"] = 20;
        return View("Information");
    }

    [Route("date")]
    public IActionResult Date() 
    {
        return Content(DateTime.Today.ToString());
    }

    [HttpGet("process-data")]
    public IActionResult Data([
        Bind(Prefix = "str1")] String string1,
        [Bind(Prefix ="str2")] String string2)
    {
        String resultString = "Jeden argument to: " + string1 + "\nDrugi argument to: " + string2;
        return Content(resultString);
    }

    [HttpGet("form")]
    public IActionResult Form() 
    {
        if (!HttpContext.Session.Keys.Contains("dane"))
        {
            ViewData["dane"] = "Brak danych";
        }
        else 
        {
            var daneZSesji = HttpContext.Session.GetString("dane");
            ViewData["dane"] = daneZSesji ?? "brak danych (null w sesji)";
        }
        return View();
    }

    [HttpPost("form")]
    public IActionResult Form(IFormCollection form) 
    {
        String dane = form["dane"].ToString();
        HttpContext.Session.SetString("dane", dane);
        ViewData["dane"] = dane;
        return View();
    }




    // COOKIES
    // [Route("visit")]
    // public String Index()
    // {
    //     if (!HttpContext.Request.Cookies.ContainsKey("pierwszy_request")) 
    //     {
    //         CookieOptions cookieOptions = new CookieOptions();
    //         cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddDays(7));
    //         HttpContext.Response.Cookies.Append("pierwszy_request", DateTime.Now.ToString(), cookieOptions);
    //         return "Pierwsze odwiedziny!";
    //     }
    //     else 
    //     {
    //         DateTime firstVisit = DateTime.Parse(HttpContext.Request.Cookies["pierwszy_request"]!);
    //         return "Pierwszy raz odwiedziles nas: "+ firstVisit.ToString();
    //     }
    // }

    // SESSION
    // [Route("visit")]
    // public String Index() 
    // {
    //     if (!HttpContext.Session.Keys.Contains("pierwszy_request"))
    //     {
    //         HttpContext.Session.SetString("pierwszy_request", DateTime.Now.ToString());
    //         return "Pierwsze odwiedziny (dane z sesji)";
    //     }
    //     else
    //     {
    //         String firstRequest = HttpContext.Session.GetString("pierwszy_request")!;
    //         DateTime firstRequestDate = DateTime.Parse(firstRequest);
    //         return "Po raz pierwszy odwiedziłeś nas (wg sesji): " + firstRequestDate.ToString();
    //     }
    // }

    [Route("visit")]
    public String Index()
        {
        DateTime CurrentDateTime = DateTime.Now;

        if (!_memoryCache.TryGetValue("pierwszy_request", out DateTime cacheValue))
        {
            cacheValue = CurrentDateTime;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(10));

            _memoryCache.Set("pierwszy_request", cacheValue, cacheEntryOptions);
            return "Pierwsze odwiedziny (z casha)";
        }
        else
        {
            return "(z casha) Po raz pierwszy odwiedziłeś nas: " + cacheValue.ToString();
        }
    }


}
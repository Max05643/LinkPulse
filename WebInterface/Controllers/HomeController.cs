using System.Diagnostics;
using LinkPulseDefinitions;
using Microsoft.AspNetCore.Mvc;

namespace WebInterface.Controllers;

public class HomeController : Controller
{

    readonly IShortenerController controller;
    public HomeController(IShortenerController controller)
    {
        this.controller = controller;
    }

    [HttpGet]
    public IActionResult Index([FromRoute] string? param = null)
    {
        if (param == null)
        {
            return View();
        }
        else
        {
            if (controller.TryGetURLByShortenedVersion(param, out string? fullUrl))
            {
                return Redirect(fullUrl!);
            }
            else
            {
                return NotFound();
            }
        }
    }

}

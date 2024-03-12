using LinkPulseDefinitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using WebInterface.Models;

namespace WebInterface.Controllers
{
    [ApiController]
    public class ApiController : Controller
    {
        readonly IShortenerController shortenerController;
        readonly TimeSpan? expirationTime;

        public ApiController(IShortenerController shortenerController, IConfiguration configuration)
        {
            this.shortenerController = shortenerController;

            var expirationTimeConfig = configuration["Shortener:ExpirationTimeSeconds"];

            if (expirationTimeConfig == null)
            {
                expirationTime = null;
            }
            else
            {
                expirationTime = TimeSpan.FromSeconds(int.Parse(expirationTimeConfig));
            }
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("/Api/TryAdd")]
        public JsonResult TryAddNewUrl([FromForm, MaxLength(2000), RegularExpression(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)")] string url)
        {

            if (!ModelState.IsValid)
            {
                return Json(new URLShorteningResult() { Success = false });
            }

            var result = shortenerController.TryAddNewURL(url, out string? shortenedVersion);

            if (result)
            {
                return Json(new URLShorteningResult() { Success = true, ShortenedUrl = shortenedVersion, TimeToExpireInSec = (int)expirationTime!.Value.TotalSeconds });
            }
            else
            {
                return Json(new URLShorteningResult() { Success = false });
            }
        }
    }
}

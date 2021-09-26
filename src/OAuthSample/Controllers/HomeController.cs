using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OAuthSample.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult NeedLogin()
        {
            return View();
        }
    }
}

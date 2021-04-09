using System.Threading.Tasks;
using JwtAndCookie.Libs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAndCookie.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("~/Api/Demo/[action]")]
    public class DemoApiController : ControllerBase
    {
        private readonly ICurrentUserContext _currentUser;

        public DemoApiController(ICurrentUserContext currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet]
        [AllowAnonymous]
        public MessageResult Anonymous()
        {
            return MessageResult.CreateSuccess("Anonymous",_currentUser.ToString());
        }

        [HttpGet]
        [Authorize]
        public async Task<MessageResult> NeedLogin()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return MessageResult.CreateSuccess("NeedLogin", _currentUser.ToString());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public MessageResult NeedAdmin()
        {
            return MessageResult.CreateSuccess("NeedAdmin", _currentUser.ToString());
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Super")]
        public MessageResult NeedAdminSuper()
        {
            return MessageResult.CreateSuccess("NeedAdmin,Super", _currentUser.ToString());
        }

        [HttpGet]
        [Authorize(Roles = "Leader")]
        public MessageResult NeedLeader()
        {
            return MessageResult.CreateSuccess("NeedLeader", _currentUser.ToString());
        }
    }
}

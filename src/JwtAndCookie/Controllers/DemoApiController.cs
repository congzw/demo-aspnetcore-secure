using System.Threading.Tasks;
using Common;
using Common.Auth;
using Common.Auth.PermissionChecks;
using Common.Auth.PermissionChecks.Demo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAndCookie.Controllers
{
    [Route("~/Api/Demo/[action]")]
    public class DemoApiController : ControllerBase
    {
        private readonly CurrentUserContext _currentUser;

        public DemoApiController(CurrentUserContext currentUser)
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

        [HttpGet]
        public MessageResult Fallback()
        {
            return MessageResult.CreateSuccess("Fallback", _currentUser.ToString());
        }

        [HttpGet]
        [PermissionCheck(PermissionId = DemoConst.PermissionIds.GuestOp)]
        public MessageResult GuestOp()
        {
            return MessageResult.CreateSuccess("GuestOp", _currentUser.ToString());
        }

        [HttpGet]
        [PermissionCheck(PermissionId = DemoConst.PermissionIds.LoginOp)]
        public MessageResult LoginOp()
        {
            return MessageResult.CreateSuccess("LoginOp", _currentUser.ToString());
        }

        [HttpGet]
        [PermissionCheck(PermissionId = DemoConst.PermissionIds.AdminOp)]
        public MessageResult AdminOp()
        {
            return MessageResult.CreateSuccess("AdminOp", _currentUser.ToString());
        }

        [HttpGet]
        [PermissionCheck(PermissionId = DemoConst.PermissionIds.SuperOp)]
        public MessageResult SuperOp()
        {
            return MessageResult.CreateSuccess("SuperOp", _currentUser.ToString());
        }

        [HttpGet]
        public MessageResult SmartOp()
        {
            return MessageResult.CreateSuccess("SmartOp", _currentUser.ToString());
        }
    }
}

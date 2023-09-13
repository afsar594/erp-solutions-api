using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Authentication.Inerfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IloginService loginsvc;
        public LoginController(IloginService _loginsvc)
        {
            loginsvc = _loginsvc;
        }
        [HttpPost]
        [Route("login")]
        public ApiResponse<UserFile> login([FromBody] User user)
        {
            ApiResponse<UserFile> apiResponse = new ApiResponse<UserFile>()
            {
                Valid = true,
                Result = null,
                Message = ""
            };
            try
            {
                var k = loginsvc.SignIn(new UserFile { UserName = user.Username, Password = user.Password });
                if (k != null)
                {
                    apiResponse.Result = k;
                    apiResponse.Error = false;
                    apiResponse.Message = LoginMessage.Successfull;
                }
                else
                {
                    apiResponse.Valid = false;
                    apiResponse.Error = true;
                    apiResponse.Message = LoginMessage.Failed;

                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse<UserFile>()
                {
                    Valid = false,
                    Result = null,
                    Message = DbMessage.Failed,
                    Error = true
                };
            }

            return apiResponse;
        }


        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User user)
        {
            var response = loginsvc.Authenticate(new UserFile
            {
                UserName = user.Username,
                Password = user.Password
            }, ipAddress());

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            setTokenCookie(response.RefreshToken);
            response.FinancialPeriodsFsno = user.Financialperiod.FinancialPeriodsFsno;
            response.LocId = (int)user.Location.LocationMasterLocationId;
            return Ok(ApiResponse<UserFile>.Success(response));
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Convert.ToString(Request.Cookies["refreshToken"]);

            var response = loginsvc.RefreshToken(new UserFile { RefreshToken = refreshToken });

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] User user)
        {
            // accept token from request body or cookie
            var token = user.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = loginsvc.RevokeToken(new UserFile { RefreshToken = user.RefreshToken });

            if (response == null)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }
        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}

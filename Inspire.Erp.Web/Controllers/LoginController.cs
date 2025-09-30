using AutoMapper;
using Inspire.Erp.Application.Authentication.Inerfaces;
using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{    
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IloginService loginsvc;
        private readonly IMapper _mapper;
        private IRepository<UserFile> userFileRepository;
        private IRepository<LocationMaster> locationrepository;
        private IRepository<FinancialPeriods> financialrepository;
        private IConfiguration _configuration;

        public LoginController(IloginService _loginsvc, IMapper mapper,
            IRepository<LocationMaster> _locationrepository, IRepository<UserFile> _userFileRepository, IRepository<FinancialPeriods> _financialrepository, IConfiguration configuration)
        {
            loginsvc = _loginsvc;
            _mapper = mapper; userFileRepository = _userFileRepository;
            locationrepository = _locationrepository; financialrepository = _financialrepository;
            _configuration = configuration;
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
        public IActionResult Authenticate([FromBody] User user, string returnUrl = "/")
        {
           
            var response = loginsvc.Authenticate(new UserFile
            {
                UserName = user.Username,
                Password = user.Password
            }, ipAddress(), (int)user.Location.LocationMasterLocationId);

            if (response.IsValid != false)
            {
                response.IsValid = true;
                setTokenCookie(response.RefreshToken);
                response.FinancialPeriodsFsno = user.Financialperiod.FinancialPeriodsFsno;
                response.LocId = (int)user.Location.LocationMasterLocationId;
            }
            else
            {
                return Ok(response);
            }


            return Ok(ApiResponse<UserFile>.Success(response));
        }

        [AllowAnonymous]
        [HttpGet("auto-login")]
        public async Task<IActionResult> AutoLogin(string token, string redirect)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(this._configuration["ApplicationSettings:Secret"]);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Redirect(redirect);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token is invalid or expired." });
            }
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
        [HttpPost]
        [Route("InsertUser")]
        public ApiResponse<UserFileViewModel> InserUser([FromBody] UserFileViewModel user)
        {
            ApiResponse<UserFileViewModel> apiResponse = new ApiResponse<UserFileViewModel>();

            var param1 = _mapper.Map<UserFile>(user);
            param1.UserImage = ImageHelper.Base64ImageToByteArray(user.UserImageBase64);
            param1.SignImg = ImageHelper.Base64ImageToByteArray(user.SignImageBase64);
            var xs = loginsvc.InsertUser(param1);
            UserFileViewModel userdata = new UserFileViewModel();
            if (xs.IsValid == true )
            {
                userdata = _mapper.Map<UserFileViewModel>(xs);
                 userdata.UserImageBase64 = ImageHelper.ByteArrayToBase64Image(xs.UserImage);
                userdata.SignImageBase64 = ImageHelper.ByteArrayToBase64Image(xs.SignImg);
                apiResponse = new ApiResponse<UserFileViewModel>
                {
                    Valid = true,
                    Result = userdata,
                    Message = UserFileMessage.UpdateUser
                };
            }
            else
            {
                userdata = _mapper.Map<UserFileViewModel>(xs);
                apiResponse = new ApiResponse<UserFileViewModel>
                {
                    Valid = true,
                    Result = userdata,
                    Message = xs.Message
                };
            }
            return apiResponse;
        }


        [HttpPost]
        [Route("UpdateUser")]
        public ApiResponse<UserFileViewModel> UpdateUser([FromBody] UserFileViewModel user)
        {
            ApiResponse<UserFileViewModel> apiResponse = new ApiResponse<UserFileViewModel>();

            var param1 = _mapper.Map<UserFile>(user);
            param1.UserImage = ImageHelper.Base64ImageToByteArray(user.UserImageBase64);
            param1.SignImg = ImageHelper.Base64ImageToByteArray(user.SignImageBase64);
            var xs = loginsvc.UpdateUser(param1);

            UserFileViewModel userdata = new UserFileViewModel();
            if (xs.IsValid == true || xs.IsValid == null)
            {
                userdata = _mapper.Map<UserFileViewModel>(xs);
                userdata.UserImageBase64 = ImageHelper.ByteArrayToBase64Image(xs.UserImage);
                userdata.SignImageBase64 = ImageHelper.ByteArrayToBase64Image(xs.SignImg);
                apiResponse = new ApiResponse<UserFileViewModel>
                {
                    Valid = true,
                    Result = userdata,
                    Message = UserFileMessage.UpdateUser
                };
            }
            else
            {
                userdata = _mapper.Map<UserFileViewModel>(xs);
                apiResponse = new ApiResponse<UserFileViewModel>
                {
                    Valid = false,
                    Result = userdata,
                    Message = xs.Message
                };
            }
            return apiResponse;
        }

        [HttpPost]
        [Route("DeleteUser")]
        public ApiResponse<UserFileViewModel> DeleteUser([FromBody] UserFileViewModel user)
        {
            ApiResponse<UserFileViewModel> apiResponse = new ApiResponse<UserFileViewModel>();

            var param1 = _mapper.Map<UserFile>(user);

            var xs = loginsvc.DeleteUser(param1);

            UserFileViewModel userdata = new UserFileViewModel();
            userdata = _mapper.Map<UserFileViewModel>(xs);

            apiResponse = new ApiResponse<UserFileViewModel>
            {
                Valid = true,
                Result = userdata,
                Message = UserFileMessage.DeleteUser
            };
            return apiResponse;
        }


        [HttpGet]
        [Route("GetAllUserFile")]
        public ApiResponse<List<UserFile>> GetAllUserFile()
        {
            var users = _mapper.Map<List<UserFile>>(loginsvc.GetAllUserFile());
            users.ForEach(x => {
                x.UserImageBase64 = ImageHelper.ByteArrayToBase64Image(x.UserImage);
                x.SignImageBase64 = ImageHelper.ByteArrayToBase64Image(x.SignImg);
            });
            ApiResponse<List<UserFile>> apiResponse = new ApiResponse<List<UserFile>>
            {
                Valid = true,
                Result = users,
                Message = ""
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetUserById")]

        public UserFile GetUserById(long id)
        {
            var user = loginsvc.GetUserById(id);

            return user;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("LoadDropdown")]
        public ResponseInfo LoadDropdown()
        {
            var objectresponse = new ResponseInfo();

            //var LocationMaster = locationrepository.GetAsQueryable().Where(a => a.LocationMasterLocationDelStatus != true).Select(c => new
            //{
            //    c.LocationMasterLocationId,
            //    c.LocationMasterLocationName
            //}).AsEnumerable().OrderBy(x=>x.LocationMasterLocationName).ToList();
            var FinancialPeriods = financialrepository.GetAsQueryable().Where(x => x.FinancialPeriodsFsno != null).Select(k => new
            {
                k.FinancialPeriodsFsno,
                k.FinancialPeriodsStartDate,
                k.FinancialPeriodsEndDate
            });

            objectresponse.ResultSet = new
            {
               //LocationMaster = LocationMaster,
                FinancialPeriods = FinancialPeriods
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("LoadDropdownFPeriod")]
        public ResponseInfo LoadDropdownFPeriod()
        {
            var objectresponse = new ResponseInfo();

            //var LocationMaster = locationrepository.GetAsQueryable().Where(a => a.LocationMasterLocationDelStatus != true).Select(c => new
            //{
            //    c.LocationMasterLocationId,
            //    c.LocationMasterLocationName
            //}).AsEnumerable().OrderBy(x=>x.LocationMasterLocationName).ToList();
            var FinancialPeriods = financialrepository.GetAsQueryable().Where(x => x.FinancialPeriodsFsno != null).Select(k => new
            {
                k.FinancialPeriodsFsno,
                k.FinancialPeriodsStartDate,
                k.FinancialPeriodsEndDate
            });

            objectresponse.ResultSet = new
            {
                // LocationMaster = LocationMaster,
                FinancialPeriods = FinancialPeriods
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }


        [HttpGet]
        [Route("GetAllUser")]

        public ResponseInfo GetAllUser()
        {
            var objectresponse = new ResponseInfo();

            var user = userFileRepository.GetAsQueryable().AsNoTracking().Select(a => new
            {
                a.UserId,
                a.UserName
            }).OrderBy(x=>x.UserName).ToList();
            objectresponse.ResultSet = new
            {
                result = user,
            };
            objectresponse.IsSuccess = true;
            return objectresponse;
        }
    }
}

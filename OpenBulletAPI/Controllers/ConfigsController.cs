using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenBulletAPI.Models;
using OpenBulletAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBulletAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigsController : ControllerBase
    {
        private readonly ConfigService _configService;
        private readonly UserService _userService;
        private readonly AppSettings _appSettings;
        private string Auth
        {
            get
            {
                var auth = Request.Headers["Authorization"].FirstOrDefault();
                return auth == null ? "" : auth;
            }
        }

        private string[] GetGroups()
        {
            if (Auth.StartsWith("Basic"))
            {
                // Login to external API and get the groups
                throw new Exception("UserPass Authentication not implemented on this API");
            }
            else // Otherwise we assume it's an API key
            {
                // Retrieve the user
                var user = _userService.GetUser(Auth);

                // Check if the user exists
                if (user == null)
                {
                    throw new Exception("The user does not exist");
                }

                // Check the IP if the IPs whitelist is not blank
                var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                if (user.IPs != null && user.IPs.Count(i => !string.IsNullOrWhiteSpace(i)) > 0)
                {
                    if (!user.IPs.Contains(ip))
                    {
                        throw new Exception("Unauthorized IP");
                    }
                }

                // If we set AutoBindFirstIP to true, add the current IP to the array
                if (_appSettings.AutoBindFirstIP && (user.IPs == null || user.IPs.Count(i => !string.IsNullOrWhiteSpace(i)) == 0))
                {
                    user.IPs = new string[] { ip };
                    _userService.UpdateUser(user);
                }

                return user.Groups;
            }
        }

        public ConfigsController(IOptions<AppSettings> settings, ConfigService configService, UserService userService)
        {
            _configService = configService;
            _userService = userService;
            _appSettings = settings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var groups = GetGroups();

                return File(_configService.Get(groups).ToArray(), "application/zip", "Configs.zip");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("No file selected");

            if (await _configService.Upload(file.OpenReadStream(), Request.Headers["Group"].FirstOrDefault(), Request.Headers["Name"].FirstOrDefault()))
            {
                return Content("Uploaded!");
            }
            else
            {
                return Content("Failed to upload");
            }
        }
    }
}

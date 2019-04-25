using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenBulletAPI.Models;
using OpenBulletAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenBulletAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigsController : ControllerBase
    {
        private readonly ConfigService _configService;
        private readonly UserService _userService;
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
                return new string[] { };
            }
            else // Otherwise we assume it's an API key
            {
                var user = _userService.GetUser(Auth);

                if (user == null) return new string[] { };
                else return user.Groups;
            }
        }

        public ConfigsController(ConfigService configService, UserService userService)
        {
            _configService = configService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var groups = GetGroups();

            return File(_configService.Get(groups).ToArray(), "application/zip", "Configs.zip");
        }

        /*
        // GET api/configs
        // Admin-only
        [HttpGet]
        public ActionResult<List<Config>> Get()
        {
            return _configService.Get();
        }

        // GET api/configs/123
        [HttpGet("{id}", Name = "GetConfig")]
        public ActionResult<Config> Get(string id)
        {
            var config = _configService.Get(id);

            if (config == null)
            {
                return NotFound();
            }

            return config;
        }

        // POST api/configs
        [HttpPost]
        public ActionResult<Config> Create(Config config)
        {
            _configService.Create(config);

            return CreatedAtRoute("GetConfig", new { id = config.Id }, config);
        }

        // PUT api/configs/123
        [HttpPut("{id}")]
        public IActionResult Update(string id, Config configIn)
        {
            var config = _configService.Get(id);

            if (config == null)
            {
                return NotFound();
            }

            _configService.Update(id, configIn);

            return NoContent();
        }

        // DELETE api/configs/123
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var config = _configService.Get(id);

            if (config == null)
            {
                return NotFound();
            }

            _configService.Remove(config.Id);

            return NoContent();
        }
        */
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenBulletAPI.Models;
using OpenBulletAPI.Services;

namespace OpenBulletAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private string Auth
        {
            get
            {
                var auth = Request.Headers["Authorization"].FirstOrDefault();
                return auth == null ? "" : auth;
            }
        }

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET api/user
        // Admin-only
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            return _userService.Get(Auth);
        }

        // GET api/user/KEY_HERE
        // Admin-only
        [HttpGet("{key}", Name = "GetUser")]
        public ActionResult<User> Get(string key)
        {
            var user = _userService.Get(Auth, key);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST api/user
        // Admin-only
        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            _userService.Create(Auth, user);

            return CreatedAtRoute("GetUser", new { key = user.Key }, user);
        }

        // PUT api/user/KEY_HERE
        // Admin-only
        [HttpPut("{key}")]
        public IActionResult Update(string key, User userIn)
        {
            var user = _userService.Get(Auth, key);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(Auth, userIn);

            return NoContent();
        }

        // DELETE api/user/KEY_HERE
        // Admin-only
        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            var user = _userService.Get(Auth, key);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(Auth, user.Key);

            return NoContent();
        }
    }
}

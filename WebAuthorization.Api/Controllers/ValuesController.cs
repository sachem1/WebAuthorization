using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAuthorization.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;

        public ValuesController(JwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        [Authorize(Permissions.WingWell)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet]
        [Authorize(Policy = Permissions.FreerepName)]
        [Route("GetFreerep")]
        public string GetFreerep(int id)
        {
            return Permissions.FreerepName;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.ContentName)]
        [Route("GetContent")]
        public string GetContent(int id)
        {
            return Permissions.ContentName;
        }

        [HttpGet]
        [Authorize(Roles = RoleHelper.FreeRepRole)]
        [Route("GetFreerepRole")]
        public string GetFreerepRole(int id)
        {
            return RoleHelper.FreeRepRole;
        }

        [HttpGet]
        [Authorize(Roles = RoleHelper.ContentRole)]
        [Route("GetContentRole")]
        public string GetContentRole(int id)
        {
            return RoleHelper.ContentRole;
        }


        [HttpGet]
        [Route("GetFreerepToken")]
        public string GetFreerepToken()
        {
            var userModel = new UserModel()
            {
                Mobile = "13691854578",
                UserId = "C0EEEE5B1B334BDD83D9B6AE2C27DCDE",
                UserName = "jiesen",
                OpenId = "o6d4G6kd6tWK6VZr6Y7jfjgJTXqs",
                UnionId = "oK_8h6btyB7dNPfgC2NvhN5WZkKg",
                UserRoles = new List<UserRole>()
                {
                    new UserRole() { RoleName =RoleHelper.FreeRepRole, RoleId = "4" }
                }
            };
            var token = _jwtHelper.CreateToken(userModel);
            return token;
        }
        
        [HttpGet]
        [Route("GetContentToken")]
        public string GetContentToken()
        {
            var userModel = new UserModel()
            {
                Mobile = "13691854578",
                UserId = "C0EEEE5B1B334BDD83D9B6AE2C27DCDE",
                UserName = "jiesen",
                OpenId = "o6d4G6kd6tWK6VZr6Y7jfjgJTXqs",
                UnionId = "oK_8h6btyB7dNPfgC2NvhN5WZkKg",
                UserRoles = new List<UserRole>()
                {
                    new UserRole() { RoleName = RoleHelper.ContentRole, RoleId = "5" }
                }
            };
            var token = _jwtHelper.CreateToken(userModel);
            return token;
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

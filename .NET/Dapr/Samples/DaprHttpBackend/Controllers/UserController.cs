using Microsoft.AspNetCore.Mvc;

namespace dapr_http_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("list")]
        public ActionResult<IList<UserInfo>> ListUser()
        {
            _logger.LogInformation("Get User Lists");

            var userList = new List<UserInfo>
            {
                new UserInfo(0,"zhang3",10),
                new UserInfo(1,"li4",15){UserAddress = "china"},
                new UserInfo(2,"wang5",29),
                new UserInfo(3000, "zhao6",90){UserAddress="America"}
            };

            return Ok(userList);
        }
    }

    public record UserInfo(long UserId, string UserName, int UserAge)
    {
        public string UserAddress { get; set; } = "earth";
    }
}

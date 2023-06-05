using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAuth.AspNetCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }
        [Authorize(policy:"Demo1")]
        [HttpGet]
        public async Task<IActionResult> demo()
        {
            return Ok("这里拥有DEMO权限可以看见");
        }
        [Authorize(policy: "Demo2")]
        [HttpGet]
        public async Task<IActionResult> demo2()
        {
            return Ok("这里拥有DEMO2权限可以看见");
        }
        [Authorize(policy: "Demo")]
        [HttpGet]
        public async Task<IActionResult> demo3()
        {
            return Ok("这里拥有DEMO1和DEMO2权限可以看见");
        }
    }
}

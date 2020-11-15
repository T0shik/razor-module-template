using Microsoft.AspNetCore.Mvc;

namespace RazorModule.Areas.Test
{
    public class TestController : ControllerBase
    {
        [Route("/test/test/index")]
        public IActionResult Index()
        {
            return Ok("Hello World");
        }
    }
}
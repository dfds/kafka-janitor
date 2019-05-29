using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.WebApp.Controllers
{
    [Route("")]
    public class HelloController : Controller
    {
        [Route("")]
        public IActionResult Hello()
        {
            return Content("Hello from kafka janitor!", "text/plain");
        }
    }
}

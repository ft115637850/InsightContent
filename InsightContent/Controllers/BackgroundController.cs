using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsightContent.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsightContent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundController : ControllerBase
    {
        [HttpPost]
        public ActionResult Post([FromForm] BackGroundModel value)
        {
            var bgImg = value.ImgFile;
            return Ok();
        }
    }
}
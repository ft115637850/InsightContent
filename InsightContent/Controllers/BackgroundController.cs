using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using InsightContent.Entities;
using InsightContent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsightContent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackgroundController : ControllerBase
    {
        private readonly IBackgroundService backgroundSvc;
        public BackgroundController(IBackgroundService backgroundSvc)
        {
            this.backgroundSvc = backgroundSvc;
        }

        [HttpPost]
        public ActionResult<StatusCodeResult> Post([FromForm] BackGroundModel value)
        {
            this.backgroundSvc.SaveOrUpdateBackground(value);
            return Ok();
        }

        [HttpGet("Img/{id}")]
        public ActionResult<FileStreamResult> GetImg(string id)
        {
            var img = this.backgroundSvc.GetBackgroundImg(id);
            if (img == null)
                return NotFound("Image not found");

            // TO DO: ETag
            return File(img.Item1, img.Item2);
        }

        [HttpGet("Info/{id}")]
        public ActionResult<BackGroundModel> GetInfo(string id)
        {
            return this.backgroundSvc.GetBackgroundInfo(id);
        }

        [HttpDelete("{id}")]
        public ActionResult<StatusCodeResult> Delete(string id)
        {
            this.backgroundSvc.DeleteBackground(id);
            return Ok();
        }
    }
}
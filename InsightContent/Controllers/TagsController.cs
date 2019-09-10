using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using InsightContent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsightContent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagsController : ControllerBase
    {
        private readonly ITagService tagSvc;
        public TagsController(ITagService tagSvc)
        {
            this.tagSvc = tagSvc;
        }

        [HttpGet]
        public ActionResult<DataTable> Get()
        {
            return this.tagSvc.GetTags();
        }
    }
}
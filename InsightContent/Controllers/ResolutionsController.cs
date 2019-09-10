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
    public class ResolutionsController : ControllerBase
    {
        private readonly IResolutionService resolutionSvc;

        public ResolutionsController(IResolutionService resolutionSvc)
        {
            this.resolutionSvc = resolutionSvc;
        }

        [HttpGet]
        public ActionResult<DataTable> Get()
        {
            return this.resolutionSvc.GetResolutions();
        }
    }
}
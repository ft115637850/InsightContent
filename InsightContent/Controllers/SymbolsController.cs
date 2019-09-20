using System;
using System.Collections.Generic;
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
    public class SymbolsController : ControllerBase
    {
        private readonly ISymbolService symbolSvc;
        public SymbolsController(ISymbolService symbolSvc)
        {
            this.symbolSvc = symbolSvc;
        }

        [HttpPost]
        public ActionResult<StatusCodeResult> Post([FromBody] GraphicChartDataModel symsInfo)
        {
            this.symbolSvc.SaveOrUpdateGraphicChartData(symsInfo);
            return Ok();
        }
    }
}
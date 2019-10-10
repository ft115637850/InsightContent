using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
    public class GraphicChartController : ControllerBase
    {
        private readonly IGraphicChartService graphicChartSvc;
        public GraphicChartController(IGraphicChartService graphicChartSvc)
        {
            this.graphicChartSvc = graphicChartSvc;
        }

        [HttpPost]
        public ActionResult<StatusCodeResult> Post([FromBody] GraphicChartDataModel symsInfo)
        {
            try
            {
                this.graphicChartSvc.SaveOrUpdateGraphicChartData(symsInfo);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
            
            return Ok();
        }

        [HttpGet("{graphicChartId}")]
        public ActionResult<GraphicChartDataModel> Get(string graphicChartId)
        {
            return this.graphicChartSvc.LoadGraphicChartData(graphicChartId);
        }

        [HttpGet]
        public ActionResult<DataTable> Get()
        {
            return this.graphicChartSvc.GetGraphicChartList();
        }
    }
}
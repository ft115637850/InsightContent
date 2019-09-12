using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Entities
{
    public class BackGroundModel
    {
        public IFormFile ImgFile { get; set; }
        public string GraphicChartId { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string BgSizeOption { get; set; }
    }
}

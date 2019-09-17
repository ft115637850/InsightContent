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
        public int Width { get; set; }
        public int Height { get; set; }
        public string BgSizeOption { get; set; }
        public string ImgContentType { get; set; }
    }
}

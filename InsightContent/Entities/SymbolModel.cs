using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Entities
{
    public class SymbolModel
    {
        public string SymbolId { get; set; }
        public string SymbolType { get; set; }
        public string TagId { get; set; }
        public string TagName { get; set; }
        public string ViewBox { get; set; }
        public int ViewBoxWidth { get; set; }
        public int ViewBoxHeight { get; set; }
        public decimal PositionXRatio { get; set; }
        public decimal PositionYRatio { get; set; }
        public decimal WidthRatio { get; set; }
        public string StrokeRGB { get; set; }
    }
}

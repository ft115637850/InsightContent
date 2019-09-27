using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Entities
{
    public class CardModel
    {
        public string CardId { get; set; }
        public decimal PositionXRatio { get; set; }
        public decimal PositionYRatio { get; set; }
        public decimal WidthRatio { get; set; }
        public decimal HeightRatio { get; set; }
        public string StrokeRGB { get; set; }
        public string Alpha { get; set; }
        public int ZOrder { get; set; }
    }
}

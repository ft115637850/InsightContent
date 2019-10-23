using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Entities
{
    public class GraphicChartDataModel
    {
        public string GraphicChartId { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public SymbolModel[] SymbolList { get; set; }
        public CardModel[] CardList { get; set; }
    }
}

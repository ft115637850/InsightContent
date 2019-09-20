using InsightContent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public interface ISymbolService
    {
        void SaveOrUpdateGraphicChartData(GraphicChartDataModel symsInfo);
    }
}

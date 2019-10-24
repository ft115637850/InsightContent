using InsightContent.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public interface IGraphicChartService
    {
        string SaveOrUpdateGraphicChartData(GraphicChartDataModel symsInfo);
        GraphicChartDataModel LoadGraphicChartData(string graphicChartId);
        void DeleteGraphicChart(string graphicChartId);
        DataTable GetGraphicChartList();
    }
}

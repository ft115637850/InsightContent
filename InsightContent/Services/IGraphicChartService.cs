﻿using InsightContent.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public interface IGraphicChartService
    {
        void SaveOrUpdateGraphicChartData(GraphicChartDataModel symsInfo);
        GraphicChartDataModel LoadGraphicChartData(string graphicChartId);
        DataTable GetGraphicChartList();
    }
}
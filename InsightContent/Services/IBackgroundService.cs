using InsightContent.Entities;
using System;
using System.Data;

namespace InsightContent.Services
{
    public interface IBackgroundService
    {
        void SaveOrUpdateBackground(BackGroundModel model);
        void DeleteBackground(string graphicChartId);
        Tuple<byte[], string> GetBackgroundImg(string graphicChartId);
        BackGroundModel GetBackgroundInfo(string graphicChartId);
    }
}

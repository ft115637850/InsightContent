using InsightContent.Entities;
using System;
using System.Data;

namespace InsightContent.Services
{
    public interface IBackgroundService
    {
        void SaveOrUpdateBackground(BackGroundModel model);
        Tuple<byte[], string> GetBackgroundImg(string graphicChartId);
        BackGroundModel GetBackgroundInfo(string graphicChartId);
    }
}

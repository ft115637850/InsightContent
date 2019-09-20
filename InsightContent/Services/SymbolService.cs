using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataBaseAccessService;
using InsightContent.Entities;

namespace InsightContent.Services
{
    public class SymbolService : ISymbolService
    {
        private readonly IDBAccessService dbAccess;
        public SymbolService(IDBAccessService dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public void SaveOrUpdateGraphicChartData(GraphicChartDataModel symsInfo)
        {
            var newSymbolList = new DataTable("symbolinfo");
            newSymbolList.Columns.AddRange(new DataColumn[] {
                new DataColumn("id"),
                new DataColumn("graphicChartId"),
                new DataColumn("type"),
                new DataColumn("tagId"),
                new DataColumn("tagName"),
                new DataColumn("viewBox"),
                new DataColumn("viewBoxWidth"),
                new DataColumn("viewBoxHeight"),
                new DataColumn("positionXRatio"),
                new DataColumn("positionYRatio"),
                new DataColumn("widthRatio"),
                new DataColumn("strokeRGB")
            });
            foreach (var sym in symsInfo.SymbolList)
            {
                var newSym = newSymbolList.NewRow();
                newSym[0] = sym.SymbolId;
                newSym[1] = symsInfo.GraphicChartId;
                newSym[2] = sym.SymbolType;
                newSym[3] = sym.TagId;
                newSym[4] = sym.TagName;
                newSym[5] = sym.ViewBox;
                newSym[6] = sym.ViewBoxWidth;
                newSym[7] = sym.ViewBoxHeight;
                newSym[8] = sym.PositionXRatio;
                newSym[9] = sym.PositionYRatio;
                newSym[10] = sym.WidthRatio;
                newSym[11] = sym.StrokeRGB;
                newSymbolList.Rows.Add(newSym);
            }

            var preSql = "delete from symbolinfo where graphicChartId=@graphicChartId";
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@graphicChartId", symsInfo.GraphicChartId),
            };
            this.dbAccess.BulkInsert(newSymbolList, preSql, parms);
        }
    }
}

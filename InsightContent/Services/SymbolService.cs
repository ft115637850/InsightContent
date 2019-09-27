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

        public GraphicChartDataModel LoadGraphicChartData(string graphicChartId)
        {
            var sql = "select id,type,tagId,tagName,viewBox,viewBoxWidth,viewBoxHeight,positionXRatio,positionYRatio,widthRatio,strokeRGB "
                + "from symbolinfo where graphicChartId=@graphicChartId";
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@graphicChartId", graphicChartId),
            };
            var data = this.dbAccess.GetData(sql, parms);
            var symLst = new List<SymbolModel>();
            foreach(DataRow row in data.Rows)
            {
                symLst.Add(new SymbolModel {
                    SymbolId = row[0].ToString(),
                    SymbolType = row[1].ToString(),
                    TagId = row[2].ToString(),
                    TagName = row[3].ToString(),
                    ViewBox = row[4].ToString(),
                    ViewBoxWidth = Convert.ToInt16(row[5]),
                    ViewBoxHeight = Convert.ToInt16(row[6]),
                    PositionXRatio = Convert.ToDecimal(row[7]),
                    PositionYRatio = Convert.ToDecimal(row[8]),
                    WidthRatio = Convert.ToDecimal(row[9]),
                    StrokeRGB = row[10].ToString()
                });
            }

            sql = "select id,positionXRatio,positionYRatio,widthRatio,heightRatio,strokeRGB,alpha,zOrder "
                + "from cardinfo where graphicChartId=@graphicChartId";
            data = this.dbAccess.GetData(sql, parms);
            var cardLst = new List<CardModel>();
            foreach (DataRow row in data.Rows)
            {
                cardLst.Add(new CardModel
                {
                    CardId = row[0].ToString(),
                    PositionXRatio = Convert.ToDecimal(row[1]),
                    PositionYRatio = Convert.ToDecimal(row[2]),
                    WidthRatio = Convert.ToDecimal(row[3]),
                    HeightRatio = Convert.ToDecimal(row[4]),
                    StrokeRGB = row[5].ToString(),
                    Alpha = row[6].ToString(),
                    ZOrder = Convert.ToInt16(row[7])
                });
            }

            return new GraphicChartDataModel {
                GraphicChartId = graphicChartId,
                SymbolList = symLst.ToArray(),
                CardList = cardLst.ToArray()
            };
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

            var newCardList = new DataTable("cardinfo");
            newCardList.Columns.AddRange(new DataColumn[] {
                new DataColumn("id"),
                new DataColumn("graphicChartId"),
                new DataColumn("positionXRatio"),
                new DataColumn("positionYRatio"),
                new DataColumn("widthRatio"),
                new DataColumn("heightRatio"),
                new DataColumn("strokeRGB"),
                new DataColumn("alpha"),
                new DataColumn("zOrder")
            });
            foreach (var card in symsInfo.CardList)
            {
                var newCard = newCardList.NewRow();
                newCard[0] = card.CardId;
                newCard[1] = symsInfo.GraphicChartId;
                newCard[2] = card.PositionXRatio;
                newCard[3] = card.PositionYRatio;
                newCard[4] = card.WidthRatio;
                newCard[5] = card.HeightRatio;
                newCard[6] = card.StrokeRGB;
                newCard[7] = card.Alpha;
                newCard[8] = card.ZOrder;
                newCardList.Rows.Add(newCard);
            }

            var preSql = "delete from symbolinfo where graphicChartId=@graphicChartId;" +
                "delete from cardinfo where graphicChartId=@graphicChartId;";
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@graphicChartId", symsInfo.GraphicChartId),
            };
            this.dbAccess.BulkInsert(new DataTable[] { newSymbolList, newCardList }, preSql, parms);
        }
    }
}

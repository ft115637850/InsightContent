using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataBaseAccessService;
using InsightContent.Entities;

namespace InsightContent.Services
{
    public class BackgroundService : IBackgroundService
    {
        private readonly IDBAccessService dbAccess;
        public BackgroundService(IDBAccessService dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public Tuple<byte[], string> GetBackgroundImg(string graphicChartId)
        {
            var parms = new Tuple<string, object>[] {new Tuple<string, object>("@graphicChartId", graphicChartId)};
            var result = this.dbAccess.GetData("select image, imgContentType from background where graphicChartId=@graphicChartId", parms);
            if (result.Rows.Count == 0)
                return null;
            if (DBNull.Value.Equals(result.Rows[0][0]))
                return null;
            return new Tuple<byte[], string>((byte[])result.Rows[0][0], (string)result.Rows[0][1]);
        }

        public BackGroundModel GetBackgroundInfo(string graphicChartId)
        {
            var parms = new Tuple<string, object>[] { new Tuple<string, object>("@graphicChartId", graphicChartId) };
            var result = this.dbAccess.GetData("select width, height, bgSizeOption, imgContentType from background where graphicChartId=@graphicChartId", parms);
            if (result.Rows.Count == 0)
                return null;
            return new BackGroundModel {
                Width = Convert.ToInt16(result.Rows[0][0]),
                Height = Convert.ToInt16(result.Rows[0][1]),
                BgSizeOption = Convert.ToString(result.Rows[0][2]),
                ImgContentType = Convert.ToString(result.Rows[0][3])
            };
        }

        public void SaveOrUpdateBackground(BackGroundModel model)
        {
            byte[] image = null;
            string imgContentType = null;
            if (model.ImgFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    model.ImgFile.CopyTo(memoryStream);
                    image = memoryStream.ToArray();
                }
                imgContentType = model.ImgFile.ContentType;
            }
            
            var parms = new Tuple<string, object>[] {
                new Tuple<string, object>("@id", Guid.NewGuid()),
                new Tuple<string, object>("@graphicChartId", model.GraphicChartId),
                new Tuple<string, object>("@width", model.Width),
                new Tuple<string, object>("@height", model.Height),
                new Tuple<string, object>("@bgSizeOption", model.BgSizeOption),
                new Tuple<string, object>("@image", image),
                new Tuple<string, object>("@imgContentType", imgContentType),
            };

            var sql = "delete from background where graphicChartId=@graphicChartId;"
                + "insert into background(id,graphicChartId,width,height,bgSizeOption,image,imgContentType)"
                + "values(@id,@graphicChartId,@width,@height,@bgSizeOption,@image,@imgContentType);";
            this.dbAccess.ExecuteNonQuery(sql, parms);
        }
    }
}

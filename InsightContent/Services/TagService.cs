using DataBaseAccessService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public class TagService : ITagService
    {
        private readonly IDBAccessService dbAccess;
        public TagService(IDBAccessService dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public DataTable GetTags()
        {
            return this.dbAccess.GetData("select id,name,alias,units,max,min,dataType,source,description,location from taginfo", null);
        }
    }
}

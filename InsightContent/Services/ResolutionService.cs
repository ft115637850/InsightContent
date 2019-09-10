using DataBaseAccessService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public class ResolutionService : IResolutionService
    {
        private readonly IDBAccessService dbAccess;
        public ResolutionService(IDBAccessService dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public DataTable GetResolutions()
        {
            return this.dbAccess.GetData("select x,y,viewValue from canvas_resolution", null);
        }
    }
}

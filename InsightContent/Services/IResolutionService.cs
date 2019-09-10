using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public interface IResolutionService
    {
        DataTable GetResolutions();
    }
}

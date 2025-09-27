using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Application.Interfaces
{
    public interface IReportAiService
    {
        Task<string> SummarizeAsync(string reportText, CancellationToken cancellationToken = default);
    }

}

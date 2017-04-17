using Data.Entities;
using System.Collections.Generic;

namespace Data.Repositories.Contract
{
    public interface ICallLogRepository
    {
        IEnumerable<CallLog> GetCallLogs();
    }
}
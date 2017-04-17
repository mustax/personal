using System;
using System.Linq;

namespace Data.Entities.Builders
{
    public class CallLogBuilder : ICallLogBuilder
    {
        private readonly char fileValueSeparator;

        public CallLogBuilder(char fileValueSeparator = ';')
        {
            this.fileValueSeparator = fileValueSeparator;
        }

        public CallLog Build(string logEntry)
        {
            if (string.IsNullOrEmpty(logEntry))
            {
                return null;
            }

            var logEntryParts = logEntry.Split(fileValueSeparator);

            if (logEntryParts.Count() != 4)
            {
                throw new InvalidOperationException(string.Format("the log entries is not respecting the format start date{0}end date{0}source number{0}destinantion number\rSeparator: '{0}'", fileValueSeparator));
            }

            var callLog = new CallLog();
            BuildWithCallStartDate(logEntryParts, callLog);
            BuildWithCallEndDate(logEntryParts, callLog);
            BuildWithSourceNumber(logEntryParts, callLog);
            BuildWithDestinationNumber(logEntryParts, callLog);

            return callLog;
        }

        private void BuildWithSourceNumber(string[] logEntryParts, CallLog callLog)
        {
            callLog.SourceNumber = logEntryParts[2];
        }

        private void BuildWithDestinationNumber(string[] logEntryParts, CallLog callLog)
        {
            callLog.DestinationNumber = logEntryParts[3];
        }

        private void BuildWithCallEndDate(string[] logEntryParts, CallLog callLog)
        {
            DateTime callEndDate;
            DateTime.TryParse(logEntryParts[1], out callEndDate);
            callLog.CallEndDate = callEndDate;
        }

        private void BuildWithCallStartDate(string[] logEntryParts, CallLog callLog)
        {
            DateTime callStartDate;
            DateTime.TryParse(logEntryParts[0], out callStartDate);
            callLog.CallStartDate = callStartDate;
        }
    }
}
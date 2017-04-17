using Data.Entities;
using Data.Entities.Builders;
using Data.Repositories.Contract;
using System;
using System.Collections.Generic;
using System.IO;

namespace Data.Repositories
{
    public class CallLogRepository : ICallLogRepository
    {
        private readonly string logFileLocation;
        private readonly ICallLogBuilder builder;

        public CallLogRepository(string logFileLocation, ICallLogBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(logFileLocation))
            {
                throw new ArgumentNullException("logFileLocation", "logFileLocation can not be null or empty");
            }

            if (builder == null)
            {
                throw new ArgumentNullException("builder", "builder can not be null");
            }

            this.logFileLocation = logFileLocation;
            this.builder = builder;
        }

        public IEnumerable<CallLog> GetCallLogs()
        {
            var callLogEntries = ReadCallLogs();

            foreach (var entry in callLogEntries)
            {
                yield return builder.Build(entry);
            }
        }

        private IEnumerable<string> ReadCallLogs()
        {
            if (!File.Exists(logFileLocation))
            {
                throw new ArgumentException(string.Format("File '{0}' does not exits", logFileLocation));
            }

            using (var reader = new StreamReader(logFileLocation))
            {
                while (!reader.EndOfStream)
                {
                    var entry = reader.ReadLine();
                    yield return entry;
                };
            }
        }
    }
}
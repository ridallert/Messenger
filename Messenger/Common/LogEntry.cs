using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Common
{
    public class LogEntry
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public LogEntry(string type, string message, DateTime dateTime)
        {
            Type = type;
            Message = message;
            DateTime = dateTime;
        }
    }
}

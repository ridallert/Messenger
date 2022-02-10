namespace Messenger.DataObjects
{
    using System;
    public class LogEntry
    {
        #region Properties

        public EventType Type { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        #endregion //Properties
    }
}

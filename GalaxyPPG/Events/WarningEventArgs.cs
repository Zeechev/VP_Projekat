using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPPG.Events
{
    public class WarningEventArgs : EventArgs
    {
        public string WarningType { get; set; }
        public string ParticipantId { get; set; }
        public long TimestampMs { get; set; }
        public double? Value { get; set; }
        public string Message { get; set; }
    }
}

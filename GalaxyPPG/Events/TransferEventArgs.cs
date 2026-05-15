using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPPG.Events
{
    public class TransferEventArgs : EventArgs
    {
        public string ParticipantId { get; set; }
        public string DeviceId { get; set; }
        public int SampleCount { get; set; }
        public string Message { get; set; }
    }
}
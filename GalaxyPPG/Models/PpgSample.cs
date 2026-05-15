using System.Runtime.Serialization;

namespace GalaxyPPG.Models
{
    [DataContract]
    public class PpgSample
    {
        [DataMember]
        public long TimestampMs { get; set; }

        [DataMember]
        public double? PpgGreen { get; set; }

        [DataMember]
        public double? PpgRed { get; set; }

        [DataMember]
        public double? PpgIr { get; set; }

        [DataMember]
        public double? AccX { get; set; }

        [DataMember]
        public double? AccY { get; set; }

        [DataMember]
        public double? AccZ { get; set; }

        [DataMember]
        public double? HeartRate { get; set; }

        [DataMember]
        public double? IBI_ms { get; set; }

        [DataMember]
        public string ParticipantId { get; set; }

        [DataMember]
        public int RowIndex { get; set; }
    }
}
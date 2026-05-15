using System.Runtime.Serialization;

namespace GalaxyPPG.Faults
{
    [DataContract]
    public class DataFormatFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
using System.Runtime.Serialization;

namespace GalaxyPPG.Faults
{
    [DataContract]
    public class ValidationFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
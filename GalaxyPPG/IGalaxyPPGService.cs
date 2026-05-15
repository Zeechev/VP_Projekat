using System.ServiceModel;
using GalaxyPPG.Models;
using GalaxyPPG.Faults;

namespace GalaxyPPG
{
    [ServiceContract]
    public interface IGalaxyPPGService
    {
        [OperationContract]
        [FaultContract(typeof(DataFormatFault))]
        [FaultContract(typeof(ValidationFault))]
        void StartSession(SessionMeta meta);

        [OperationContract]
        [FaultContract(typeof(DataFormatFault))]
        [FaultContract(typeof(ValidationFault))]
        void PushSample(PpgSample sample);

        [OperationContract]
        void EndSession();
    }
}
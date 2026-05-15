using System;
using System.ServiceModel;

namespace GalaxyPPG.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IGalaxyPPGService> factory = null;
            IGalaxyPPGService proxy = null;

            try
            {
                factory = new ChannelFactory<IGalaxyPPGService>("GalaxyPPGService");
                proxy = factory.CreateChannel();

                Console.WriteLine("Klijent povezan na servis.");

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greska u klijentu: " + ex.Message);
            }
            finally
            {
                if (proxy != null)
                {
                    ICommunicationObject communicationObject =
                        proxy as ICommunicationObject;

                    if (communicationObject != null)
                    {
                        if (communicationObject.State == CommunicationState.Faulted)
                            communicationObject.Abort();
                        else
                            communicationObject.Close();
                    }
                }

                if (factory != null)
                {
                    if (factory.State == CommunicationState.Faulted)
                        factory.Abort();
                    else
                        factory.Close();
                }
            }

            Console.ReadLine();
        }
    }
}
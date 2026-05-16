using GalaxyPPG.Client.IO;
using GalaxyPPG.Models;
using System;
using System.IO;
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
                Console.Write("Unesi putanju do Dataset foldera: ");
                string datasetPath = Console.ReadLine();

                Console.Write("Unesi ucesnika (npr. P02): ");
                string participantId = Console.ReadLine();

                string participantPath = Path.Combine(datasetPath, participantId);

                factory = new ChannelFactory<IGalaxyPPGService>("GalaxyPPGService");
                proxy = factory.CreateChannel();

                Console.WriteLine("Klijent povezan na servis.");

                SessionMeta meta = new SessionMeta
                {
                    ParticipantId = participantId,
                    DeviceId = "GalaxyWatch",
                    SampleRateHz = 25,
                    TimestampOffsetMs = 0
                };

                proxy.StartSession(meta);

                int sentCount = 0;

                using (CsvGalaxyReader reader = new CsvGalaxyReader())
                {
                    foreach (PpgSample sample in reader.ReadParticipant(participantPath, participantId))
                    {
                        try
                        {
                            proxy.PushSample(sample);
                            sentCount++;
                        }
                        catch (FaultException<GalaxyPPG.Faults.ValidationFault> ex)
                        {
                            Console.WriteLine(
                                "Odbijen red " + sample.RowIndex + ": " + ex.Detail.Message);
                            File.AppendAllText(
                                "rejected_client.csv",
                                "Red " + sample.RowIndex + ": " + ex.Detail.Message + Environment.NewLine);
                        }
                        catch (FaultException<GalaxyPPG.Faults.DataFormatFault> ex)
                        {
                            Console.WriteLine(
                                "Odbijen red " + sample.RowIndex + ": " + ex.Detail.Message);
                            File.AppendAllText(
                                "rejected_client.csv",
                                "Red " + sample.RowIndex + ": " + ex.Detail.Message + Environment.NewLine);
                        }
                    }
                }

                proxy.EndSession();

                Console.WriteLine("Prenos zavrsen na klijentu.");
                Console.WriteLine("Ukupno poslato uzoraka: " + sentCount);
            }
            catch (FaultException<GalaxyPPG.Faults.ValidationFault> ex)
            {
                Console.WriteLine("Validation fault: " + ex.Detail.Message);
            }
            catch (FaultException<GalaxyPPG.Faults.DataFormatFault> ex)
            {
                Console.WriteLine("Data format fault: " + ex.Detail.Message);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("WCF greska: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greska u klijentu: " + ex.Message);
            }
            finally
            {
                if (proxy != null)
                {
                    ICommunicationObject communicationObject = proxy as ICommunicationObject;

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
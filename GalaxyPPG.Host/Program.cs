using System;
using System.ServiceModel;

namespace GalaxyPPG.Host
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(GalaxyPPGService)))
            {
                try
                {
                    host.Open();

                    Console.WriteLine("GalaxyPPG WCF servis je pokrenut.");
                    Console.WriteLine("Pritisni ENTER za zaustavljanje servisa.");

                    Console.ReadLine();

                    host.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Greska pri radu hosta: " + ex.Message);
                    host.Abort();
                }
            }
        }
    }
}
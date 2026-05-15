using GalaxyPPG.Faults;
using GalaxyPPG.IO;
using GalaxyPPG.Models;
using System;
using System.ServiceModel;
using GalaxyPPG.Events;
using System.Configuration;


namespace GalaxyPPG
{
    public class GalaxyPPGService : IGalaxyPPGService
    {
        private ServerSessionWriter writer;
        private int brojPrimljenihUzoraka;

        public event EventHandler<TransferEventArgs> OnTransferStarted;
        public event EventHandler<SampleReceivedEventArgs> OnSampleReceived;
        public event EventHandler<TransferEventArgs> OnTransferCompleted;
        public event EventHandler<WarningEventArgs> OnWarningRaised;
        public void StartSession(SessionMeta meta)
        {
            
            if (meta == null)
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault { Message = "Session meta ne sme biti null." });
            }

            if (string.IsNullOrWhiteSpace(meta.ParticipantId) ||
                string.IsNullOrWhiteSpace(meta.DeviceId))
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { Message = "ParticipantId i DeviceId su obavezni." });
            }

            Console.WriteLine("Session started.");
            Console.WriteLine("Participant: " + meta.ParticipantId);
            Console.WriteLine("Device: " + meta.DeviceId);
            writer = new ServerSessionWriter();
            writer.Start(meta);
            brojPrimljenihUzoraka = 0;

            Console.WriteLine("Prenos u toku...");

            OnTransferStarted?.Invoke(this, new TransferEventArgs
            {
                ParticipantId = meta.ParticipantId,
                DeviceId = meta.DeviceId,
                SampleCount = 0,
                Message = "Prenos je pokrenut."
            });
        }

        public void PushSample(PpgSample sample)
        {
            if (writer == null)
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault { Message = "Sesija nije pokrenuta. Prvo pozvati StartSession." });
            }

            ValidateSample(sample);
            writer.WriteSample(sample);
            brojPrimljenihUzoraka++;
            OnSampleReceived?.Invoke(this, new SampleReceivedEventArgs
            {
                Sample = sample
            });

            Console.WriteLine("Primljen uzorak broj: " + brojPrimljenihUzoraka);

        }

        public void EndSession()
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }

            Console.WriteLine("Prenos zavrsen.");
            Console.WriteLine("Ukupno primljenih uzoraka: " + brojPrimljenihUzoraka);
            OnTransferCompleted?.Invoke(this, new TransferEventArgs
            {
                SampleCount = brojPrimljenihUzoraka,
                Message = "Prenos je zavrsen."
            });

            Console.WriteLine("Session ended.");
        }




        private void ValidateSample(PpgSample sample)
        {
            if (sample == null)
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault { Message = "Sample ne sme biti null." });
            }

            if (sample.HeartRate.HasValue &&
                (sample.HeartRate.Value < 30 || sample.HeartRate.Value > 220))
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { Message = "HeartRate mora biti u opsegu [30, 220] bpm." });
            }

            if (sample.PpgGreen.HasValue && sample.PpgGreen.Value < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { Message = "PpgGreen mora biti >= 0." });
            }

            if (sample.PpgRed.HasValue && sample.PpgRed.Value < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { Message = "PpgRed mora biti >= 0." });
            }

            if (sample.PpgIr.HasValue && sample.PpgIr.Value < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { Message = "PpgIr mora biti >= 0." });
            }

            if (sample.IBI_ms.HasValue &&
                (sample.IBI_ms.Value < 250 || sample.IBI_ms.Value > 2000))
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { Message = "IBI_ms mora biti u opsegu [250, 2000] ms." });
            }
        }
        private void RaiseWarning(string type, PpgSample sample, double? value, string message)
        {
            OnWarningRaised?.Invoke(this, new WarningEventArgs
            {
                WarningType = type,
                ParticipantId = sample.ParticipantId,
                TimestampMs = sample.TimestampMs,
                Value = value,
                Message = message
            });

            Console.WriteLine("WARNING: " + type + " | " + message);
        }


    }
}
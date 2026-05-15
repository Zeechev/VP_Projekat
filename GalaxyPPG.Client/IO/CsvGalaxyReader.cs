using GalaxyPPG.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GalaxyPPG.Client.IO
{
    public class CsvGalaxyReader : IDisposable
    {
        private StreamWriter logWriter;
        private bool disposed;

        public CsvGalaxyReader()
        {
            string logPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "rejected_client.csv"
            );

            logWriter = new StreamWriter(logPath, true);
        }

        public IEnumerable<PpgSample> ReadParticipant(string participantDirectory, string participantId)
        {
            ThrowIfDisposed();

            if (!Directory.Exists(participantDirectory))
            {
                throw new DirectoryNotFoundException("Direktorijum ucesnika nije pronadjen: " + participantDirectory);
            }

            string[] files = Directory.GetFiles(
                participantDirectory,
                "PPG.csv",
                SearchOption.AllDirectories
            );

            foreach (string file in files)
            {
                foreach (PpgSample sample in ReadPpgFile(file, participantId))
                {
                    yield return sample;
                }
            }
        }

        private IEnumerable<PpgSample> ReadPpgFile(string filePath, string participantId)
        {
            int rowIndex = 0;
            int lineNumber = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    if (lineNumber == 1)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    rowIndex++;

                    PpgSample sample;

                    try
                    {
                        sample = ParseLine(line, participantId, rowIndex);
                    }
                    catch (Exception ex)
                    {
                        LogInvalidLine(filePath, lineNumber, line, ex.Message);
                        continue;
                    }

                    yield return sample;
                }
            }
        }

        private PpgSample ParseLine(string line, string participantId, int rowIndex)
        {
            string[] parts = line.Split(',');

            if (parts.Length < 11)
            {
                throw new FormatException("Red nema dovoljan broj kolona. Ocekivano je najmanje 11.");
            }

            return new PpgSample
            {
                TimestampMs = long.Parse(parts[0], CultureInfo.InvariantCulture),

                PpgGreen = ParseNullableDouble(parts[1]),
                PpgRed = ParseNullableDouble(parts[2]),
                PpgIr = ParseNullableDouble(parts[3]),

                AccX = ParseNullableDouble(parts[4]),
                AccY = ParseNullableDouble(parts[5]),
                AccZ = ParseNullableDouble(parts[6]),

                HeartRate = ParseNullableDouble(parts[7]),
                IBI_ms = ParseNullableDouble(parts[8]),

                ParticipantId = participantId,
                RowIndex = rowIndex
            };
        }

        private double? ParseNullableDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim();

            if (value.Equals("NaN", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return double.Parse(value, CultureInfo.InvariantCulture);
        }

        private void LogInvalidLine(string filePath, int lineNumber, string line, string reason)
        {
            Console.WriteLine("Nevalidan red " + lineNumber + ": " + reason);

            if (logWriter != null)
            {
                logWriter.WriteLine(filePath + " | red " + lineNumber + " | " + reason + " | " + line);
                logWriter.Flush();
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("CsvGalaxyReader");
            }
        }

        ~CsvGalaxyReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (logWriter != null)
                    {
                        logWriter.Dispose();
                        logWriter = null;
                    }
                }

                disposed = true;
            }
        }
    }
}
using GalaxyPPG.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GalaxyPPG.Client.IO
{
    public class CsvGalaxyReader : IDisposable
    {
        private bool disposed;

        public IEnumerable<PpgSample> ReadParticipant(
            string participantPath,
            string participantId)
        {
            if (!Directory.Exists(participantPath))
            {
                throw new DirectoryNotFoundException(
                    "Direktorijum ucesnika nije pronadjen: " + participantPath);
            }

            string galaxyWatchPath =
                Path.Combine(participantPath, "GalaxyWatch");

            if (!Directory.Exists(galaxyWatchPath))
            {
                throw new DirectoryNotFoundException(
                    "GalaxyWatch folder nije pronadjen.");
            }

            string ppgPath = Path.Combine(galaxyWatchPath, "PPG.csv");
            string hrPath = Path.Combine(galaxyWatchPath, "HR.csv");
            string accPath = Path.Combine(galaxyWatchPath, "ACC.csv");

            List<string[]> ppgRows = ReadCsvRows(ppgPath);
            List<string[]> hrRows = ReadCsvRows(hrPath);
            List<string[]> accRows = ReadCsvRows(accPath);

            int count = Math.Min(
                ppgRows.Count,
                Math.Min(hrRows.Count, accRows.Count));

            for (int i = 0; i < count; i++)
            {
                string[] ppg = ppgRows[i];
                string[] hr = hrRows[i];
                string[] acc = accRows[i];

                PpgSample sample = new PpgSample();

                sample.ParticipantId = participantId;
                sample.RowIndex = i + 1;

                sample.TimestampMs = ParseLong(ppg, 1);

                double? ppgValue = ParseNullableDouble(ppg, 2);

                if (ppgValue.HasValue && ppgValue.Value < 0)
                {
                    ppgValue = Math.Abs(ppgValue.Value);
                }

                sample.PpgGreen = ppgValue;
                sample.PpgRed = ppgValue;
                sample.PpgIr = ppgValue;

                sample.HeartRate = ParseNullableDouble(hr, 2);
                sample.IBI_ms = ParseNullableDouble(hr, 4);

                sample.AccX = ParseNullableDouble(acc, 2);
                sample.AccY = ParseNullableDouble(acc, 3);
                sample.AccZ = ParseNullableDouble(acc, 4);

                yield return sample;
            }
        }

        private List<string[]> ReadCsvRows(string path)
        {
            List<string[]> rows = new List<string[]>();
            if (!File.Exists(path))
            {
                WriteClientReject("Fajl ne postoji", path);
                return rows;
            }

            string[] lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                rows.Add(lines[i].Split(','));
            }

            return rows;
        }

        private long ParseLong(string[] values, int index)
        {
            if (index >= values.Length)
            {
                return 0;
            }

            long result;

            if (long.TryParse(values[index], out result))
            {
                return result;
            }

            return 0;
        }

        private double? ParseNullableDouble(string[] values, int index)
        {
            if (index >= values.Length)
            {
                return null;
            }

            double result;

            if (double.TryParse(
                values[index],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result))
            {
                return result;
            }

            return null;
        }
        private void WriteClientReject(string reason, string originalLine)
        {
            File.AppendAllText(
                "rejected_client.csv",
                reason + "," + originalLine + Environment.NewLine);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
            }
        }
    }
}
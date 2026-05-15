using GalaxyPPG.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GalaxyPPG.Client.IO
{
    public class CsvGalaxyReader
    {
        private readonly string rejectedLogPath = "rejected_client.csv";

        public IEnumerable<PpgSample> ReadParticipant(string participantDirectory, string participantId)
        {
            string[] files = Directory.GetFiles(participantDirectory, "PPG.csv", SearchOption.AllDirectories);

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

            using (StreamReader reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // preskakanje zaglavlja

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    rowIndex++;

                    PpgSample sample;

                    try
                    {
                        sample = ParseLine(line, participantId, rowIndex);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(
                            rejectedLogPath,
                            $"{filePath}; red {rowIndex}; {ex.Message}; {line}{Environment.NewLine}"
                        );

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
                throw new FormatException("Red nema 11 kanala.");

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
            if (string.IsNullOrWhiteSpace(value) || value == "NaN")
                return null;

            return double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
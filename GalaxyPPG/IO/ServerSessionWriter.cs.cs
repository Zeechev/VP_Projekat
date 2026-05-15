using GalaxyPPG.Models;
using System;
using System.Globalization;
using System.IO;

namespace GalaxyPPG.IO
{
    public class ServerSessionWriter : IDisposable
    {
        private StreamWriter sessionWriter;
        private StreamWriter rejectsWriter;
        private bool disposed;

        public void Start(SessionMeta meta)
        {
            string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");

            string folderPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                meta.ParticipantId,
                meta.DeviceId,
                dateFolder
            );

            Directory.CreateDirectory(folderPath);

            string sessionPath = Path.Combine(folderPath, "session.csv");
            string rejectsPath = Path.Combine(folderPath, "rejects.csv");

            sessionWriter = new StreamWriter(
                new FileStream(sessionPath, FileMode.Append, FileAccess.Write)
            );

            rejectsWriter = new StreamWriter(
                new FileStream(rejectsPath, FileMode.Append, FileAccess.Write)
            );

            sessionWriter.WriteLine("TimestampMs,PpgGreen,PpgRed,PpgIr,AccX,AccY,AccZ,HeartRate,IBI_ms,ParticipantId,RowIndex");
            sessionWriter.Flush();

            rejectsWriter.WriteLine("Reason,OriginalLine");
            rejectsWriter.Flush();
        }

        public void WriteSample(PpgSample sample)
        {
            ThrowIfDisposed();

            string line = string.Join(",",
                sample.TimestampMs.ToString(CultureInfo.InvariantCulture),
                NullableToString(sample.PpgGreen),
                NullableToString(sample.PpgRed),
                NullableToString(sample.PpgIr),
                NullableToString(sample.AccX),
                NullableToString(sample.AccY),
                NullableToString(sample.AccZ),
                NullableToString(sample.HeartRate),
                NullableToString(sample.IBI_ms),
                sample.ParticipantId,
                sample.RowIndex.ToString(CultureInfo.InvariantCulture)
            );

            sessionWriter.WriteLine(line);
            sessionWriter.Flush();
        }

        public void WriteReject(string reason, string originalLine)
        {
            ThrowIfDisposed();

            rejectsWriter.WriteLine(reason + "," + originalLine);
            rejectsWriter.Flush();
        }

        private string NullableToString(double? value)
        {
            if (!value.HasValue)
            {
                return "NaN";
            }

            return value.Value.ToString(CultureInfo.InvariantCulture);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("ServerSessionWriter");
            }
        }

        ~ServerSessionWriter()
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
                    if (sessionWriter != null)
                    {
                        sessionWriter.Dispose();
                        sessionWriter = null;
                    }

                    if (rejectsWriter != null)
                    {
                        rejectsWriter.Dispose();
                        rejectsWriter = null;
                    }
                }

                disposed = true;
            }
        }
    }
}
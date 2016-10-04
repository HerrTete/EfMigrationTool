using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EfMigrationTool.Core
{
    public class MigrationDecoder
    {
        public string GetEdmxContentForMigration(IMigrationMetadata migrationMetadata)
        {
            string edmxContent = null;

            try
            {
                var compressedBytes = Convert.FromBase64String(migrationMetadata.Target);
                edmxContent = GetEdmxContentForMigration(compressedBytes);
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Exception[GetEdmxContentForMigration]");
                Trace.WriteLine(exception);
            }

            return edmxContent;
        }
        public string GetEdmxContentForMigration(byte[] compressedBytes)
        {
            string edmxContent = null;

            try
            {
                var memoryStream = new MemoryStream(compressedBytes);
                var gzip = new GZipStream(memoryStream, CompressionMode.Decompress);
                var reader = new StreamReader(gzip);
                edmxContent = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                reader = null;
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Exception[GetEdmxContentForMigration]");
                Trace.WriteLine(exception);
            }

            return edmxContent;
        }
        public byte[] GetBytesFromEdmxContent(string edmxContent)
        {
            byte[] bytes = null;

            try
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(outStream, CompressionMode.Compress))
                    using (MemoryStream srcStream = new MemoryStream(Encoding.Default.GetBytes(edmxContent)))
                    {
                        srcStream.CopyTo(gzipStream);
                    }
                    bytes = outStream.ToArray();
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Exception[GetBytesFromEdmxContent]");
                Trace.WriteLine(exception);
            }

            return bytes;
        }


    }
}

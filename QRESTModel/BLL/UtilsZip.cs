using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRESTModel.BLL
{
    public static class UtilsZip
    {
        public static byte[] CreateZIPFileFromMemoryStream(MemoryStream memStreamIn, string zipEntryName)
        {
            var outputMemStream = new MemoryStream();
            using (var zipStream = new ZipOutputStream(outputMemStream))
            {
                // 0-9, 9 being the highest level of compression
                zipStream.SetLevel(3);

                ZipEntry newEntry = new ZipEntry(zipEntryName);
                newEntry.DateTime = DateTime.Now;

                zipStream.PutNextEntry(newEntry);

                StreamUtils.Copy(memStreamIn, zipStream, new byte[4096]);
                zipStream.CloseEntry();

                // Stop ZipStream.Dispose() from also Closing the underlying stream.
                zipStream.IsStreamOwner = false;
            }

            outputMemStream.Position = 0;

            byte[] byteArrayOut = outputMemStream.ToArray();

            return byteArrayOut;
        }
    }
}

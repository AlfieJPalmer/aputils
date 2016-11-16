using SevenZip;
using System;
using System.IO;
using System.Threading.Tasks;

namespace aputils
{

    public static class FileOps
    {

        public static void Copy(this FileInfo src, FileInfo dst, Action<int> progressCB)
        {
            const int bufferSize = 1024 * 1024;  // 1MB
            byte[] buffer = new byte[bufferSize], buffer2 = new byte[bufferSize];
            bool swap = false;
            int progress = 0, reportedProgress = 0, read = 0;
            long len = src.Length;
            float flen = len;
            Task writer = null;

            using (var source = src.OpenRead())
            using (var dest = dst.OpenWrite())
            {
                dest.SetLength(source.Length);
                for (long size = 0; size < len; size += read)
                {
                    if ((progress = ((int)((size / flen) * 100))) != reportedProgress)
                        progressCB(reportedProgress = progress);

                    read = source.Read(swap ? buffer : buffer2, 0, bufferSize);
                    writer?.Wait();
                    writer = dest.WriteAsync(swap ? buffer : buffer2, 0, read);
                    swap = !swap;
                }
                writer?.Wait();
            }
            if (reportedProgress == 100) { Console.WriteLine("\n\n\noutlier\n\n\n"); /*debug*/ }
            progressCB(++reportedProgress); // final percent
        }

        public static void Move(this FileInfo src, FileInfo dst, Action<int> progressCB)
        {
            Copy(src, dst, progressCB); // Copy File Asynchronously
            src.Delete();               // Perform delete on source file
        }

        public static void Delete(this FileInfo src)
        {
            src.Delete();               // Perform delete operation
        }

        public static void CompressionProgressCB(object sender, EventArgs e, Action<int> progressCB)
        {
            progressCB(((ProgressEventArgs)e).PercentDone);
        }

        private static string CompressLZMA(string input, string output, Action<int> progressCB)
        {
            SevenZipBase.SetLibraryPath(@"C:\Users\APalmer\Downloads\7z1604-extra\7za.dll");
            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.CompressionMode = CompressionMode.Create;
            compressor.TempFolderPath = Path.GetTempPath();
            compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
            compressor.Compressing += (sender, e) => CompressionProgressCB(sender, e, progressCB);
            compressor.CompressDirectory(input, output);

            return "";
        }

        public static string CompressFile(string input, string output, Action<int> progressCB, InArchiveFormat method = InArchiveFormat.Zip)
        {
            switch (method)
            {
                case InArchiveFormat.Lzma:
                    {
                        CompressLZMA(input, output, progressCB);
                        break;
                    }
                default:
                    {
                        // invalid decomp algorithm
                        break;
                    }
            }


            return "";
        }

    }
}

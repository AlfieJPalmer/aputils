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
        
        public enum DecompressionMethod
        {
            ZIP,
            GZIP,
            RAR,
            LZMA
        }

        private static string DecompressZIP(string input, string output)
        {
            return "";
        }

        private static string DecompressGZIP(string input, string output)
        {
            return "";
        }

        private static string DecompressRAR(string input, string output)
        {
            return "";
        }

        private static string DecompressLZMA(string input, string output)
        {
             return "";
        }

        public static string DecompressFile(string input, string output, DecompressionMethod method)
        {
            switch (method)
            {
                case DecompressionMethod.ZIP:
                    {
                        DecompressZIP(input, output);
                        break;
                    }
                case DecompressionMethod.GZIP:
                    {
                        DecompressGZIP(input, output);
                        break;
                    }
                case DecompressionMethod.RAR:
                    {
                        DecompressRAR(input, output);
                        break;
                    }
                case DecompressionMethod.LZMA:
                    {
                        DecompressLZMA(input, output);
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

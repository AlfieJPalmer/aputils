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
            const int bufferSize = 1048576;  // 1MB
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
                for (long sz = 0; sz < len; sz += read)
                {
                    if ((progress = ((int)((sz / flen) * 100))) != reportedProgress)
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

        public static void CompressDir(string dir, string archName, Action<int> progressCB, OutArchiveFormat fmt = OutArchiveFormat.SevenZip, CompressionLevel lvl = CompressionLevel.Ultra, CompressionMethod mtd = CompressionMethod.Deflate )
        {
            if(fmt == OutArchiveFormat.XZ || fmt == OutArchiveFormat.GZip || fmt == OutArchiveFormat.BZip2)
            {
                Log.Lg("Cannot compress a directory to format: " + fmt.ToString() + ", it can only be applied to files");
                return;
            }

            FileAttributes att = File.GetAttributes(dir);
            if (!att.HasFlag(FileAttributes.Directory))
            {
                Log.Lg("Input file: " + dir + " is not a directory");
                return;
            }

            Compress(new string[] { dir }, archName, progressCB, fmt, lvl, mtd, true);
        }

        public static void CompressFile(string file, string archName, Action<int> progressCB, OutArchiveFormat fmt = OutArchiveFormat.GZip, CompressionLevel lvl = CompressionLevel.Normal, CompressionMethod mtd = CompressionMethod.Default)
        {
            if(!File.Exists(file))
            {
                Log.Lg(file + " does not exist");
                return;
            }

            FileAttributes att = File.GetAttributes(file);
            if(att.HasFlag(FileAttributes.Directory))
            {
                Log.Lg("Input file: " + file + " is a directory");
                return;
            }

            if (archName == string.Empty) // change function to do this by default
                archName = file;

            Compress(new string[] { file }, archName, progressCB, fmt, lvl, mtd, false);
        }

        public static void CompressFiles(string[] files, string archName, Action<int> progressCB, OutArchiveFormat fmt = OutArchiveFormat.GZip, CompressionLevel lvl = CompressionLevel.Normal, CompressionMethod mtd = CompressionMethod.Default)
        {
            foreach(string s in files)
            {
                if(!File.Exists(s)) // temp: convert to exception handling
                {
                    Log.Lg(s + " does not exist");
                    return;
                }
            }

            if (archName == string.Empty)
                archName = files[0];

            if (fmt == OutArchiveFormat.XZ || fmt == OutArchiveFormat.GZip || fmt == OutArchiveFormat.BZip2)
            {
                foreach (string s in files)
                {
                    archName = s;
                    CompressFile(s, archName, progressCB, fmt, lvl, mtd);
                }
            }
            else
            {
                Compress(files, archName, progressCB, fmt, lvl, mtd, false);
            }
        }

        private static void Compress(string[] input, string output, Action<int> progressCB, OutArchiveFormat fmt, CompressionLevel lvl, CompressionMethod mtd, bool isDir = true)
        {
            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.CompressionMode = CompressionMode.Create;
            compressor.TempFolderPath = Path.GetTempPath();
            compressor.CompressionLevel = lvl;
            compressor.ArchiveFormat = fmt;
            compressor.CompressionMethod = mtd;
            compressor.Compressing += (s, e) => progressCB(e.PercentDone); // Event only fires on LZMA

            string ext = string.Empty;
            switch(fmt)
            {
                case OutArchiveFormat.SevenZip: ext = ".7z"; break;
                case OutArchiveFormat.Zip: ext = ".zip"; break;
                case OutArchiveFormat.GZip: ext = ".gz"; break;
                case OutArchiveFormat.BZip2: ext = ".bz2"; break;
                case OutArchiveFormat.Tar: ext = ".tar"; break;
                case OutArchiveFormat.XZ: ext = ".xz";  break;
                default: break;
            }

            output = Utils.ConvertPath(output);
            for (int i = 0; i < input.Length; ++i)
                input[i] = Utils.ConvertPath(input[i]);

            if (isDir)
                compressor.CompressDirectory(input[0], output + ext);
            else
                compressor.CompressFiles(output + ext, input); // support backslash path only
        }

        //todo: private, more decompression functions
        public static void Decompress(string[] input, string output, Action<int> progressCB, InArchiveFormat fmt, bool isDir = true, string password = "")
        {
            if (isDir)
            {
                if (output == string.Empty)
                {
                    output = input[0];

                    int ind = output.LastIndexOf('.');
                    output = ind == -1 ? output : output.Substring(0, ind);
                }

                string inputFile = Utils.ConvertPath(input[0]);
                output = Utils.ConvertPath(output);

                

                using (var extractor = new SevenZipExtractor(inputFile))
                {
                    extractor.ExtractArchive(output);
                }
            }
        }

    }
}

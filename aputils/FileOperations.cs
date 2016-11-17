using SevenZip;
using System;
using System.IO;
using System.Text;
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

        public static void CompressDir(string dir, string archName, Action<int> progressCB, OutArchiveFormat fmt = OutArchiveFormat.SevenZip, CompressionLevel lvl = CompressionLevel.Ultra, CompressionMethod mtd = CompressionMethod.Deflate)
        {
            if (fmt == OutArchiveFormat.XZ || fmt == OutArchiveFormat.GZip || fmt == OutArchiveFormat.BZip2)
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
            if (!File.Exists(file))
            {
                Log.Lg(file + " does not exist");
                return;
            }

            FileAttributes att = File.GetAttributes(file);
            if (att.HasFlag(FileAttributes.Directory))
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
            foreach (string s in files)
            {
                if (!File.Exists(s)) // temp: convert to exception handling
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
            switch (fmt)
            {
                case OutArchiveFormat.SevenZip: ext = ".7z"; break;
                case OutArchiveFormat.Zip: ext = ".zip"; break;
                case OutArchiveFormat.GZip: ext = ".gz"; break;
                case OutArchiveFormat.BZip2: ext = ".bz2"; break;
                case OutArchiveFormat.Tar: ext = ".tar"; break;
                case OutArchiveFormat.XZ: ext = ".xz"; break;
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

        public static void Decompress(string file, Action<int> progressCB, string password = "")
        {
            if (!File.Exists(file)) // if this is the only clause, rewrite to be inverted if(ex){}
            {
                Log.Lg(file + " does not exist");
                return;
            }

            int ind = file.LastIndexOf('.');
            string output = ind == -1 ? file : file.Substring(0, ind); // strip extention
            string ext = Path.GetExtension(file);

            if (ext.ToLower() == ".gz" || ext.ToLower() == ".bz2" || ext.ToLower() == "xz")
                Decompress(new string[] { file }, output, progressCB, password, false); // extract to file
            else
                Decompress(new string[] { file }, output, progressCB, password, true); // extract to dir
        }

        // add pw support
        public static void Decompress(string[] files, Action<int> progressCB, string[] passwords = null)
        {
            //passwords = passwords ?? new string[files.Length];

            foreach (string s in files)
                Decompress(s, progressCB);
        }

        // .gz fires two progress events? mb. extract & create
        private static void Decompress(string[] input, string output, Action<int> progressCB, string password = "", bool isDir = true)
        {
            if (output == string.Empty)
            {
                output = input[0];

                int ind = output.LastIndexOf('.');
                output = ind == -1 ? output : output.Substring(0, ind); // strip extention
            }

            string inputFile = Utils.ConvertPath(input[0]);
            output = Utils.ConvertPath(output);

            if (isDir)
            {
                using (var extractor = new SevenZipExtractor(inputFile))
                {
                    extractor.Extracting += (s, e) => progressCB(e.PercentDone); // Event only fires on LZMA
                    extractor.ExtractArchive(output);
                }
            }
            else
            {
                using (var extractor = new SevenZipExtractor(inputFile))
                {
                    extractor.Extracting += (s, e) => progressCB(e.PercentDone); // Event only fires on LZMA
                    extractor.ExtractFile(0, File.Create(output));
                }
            }
        }


        // -----------

        public static void ReadFirstBytes(string path, long bytes)
        {
            using (var reader = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                long fileLength = new FileInfo(path).Length;
                if (fileLength < bytes)
                    bytes = fileLength;

                byte[] byteArr = new byte[bytes];

                reader.Read(byteArr, 0, (int)bytes);
                reader.Close();

                Console.WriteLine(Encoding.Default.GetString(byteArr));
            }


        }

        public static void ReadLastBytes(string path, long bytes)
        {
            using (var reader = new StreamReader(path))
            {
                if (reader.BaseStream.Length < bytes)
                    bytes = reader.BaseStream.Length;
                
                reader.BaseStream.Seek(-bytes, SeekOrigin.End);
                string line;
                while ((line = reader.ReadLine()) != null)
                    Console.WriteLine(line);
                
            }
        }


    }
}
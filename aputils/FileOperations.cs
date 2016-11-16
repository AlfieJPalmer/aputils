namespace aputils
{
    public class FileOps
    {
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

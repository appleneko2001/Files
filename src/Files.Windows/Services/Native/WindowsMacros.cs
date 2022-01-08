using System;

namespace Files.Windows.Services.Native
{
    public class WindowsMacros
    {
        public static void Decode(IntPtr input, out int a, out int b)
        {
            if (DecodeA == null || DecodeB == null)
            {
                switch (IntPtr.Size)
                {
                    // is x86_64 arch
                    case 8:
                        DecodeA = DecodeAImpl64;
                        DecodeB = DecodeBImpl64;
                        break;
                    // is x86 arch
                    case 4:
                        DecodeA = DecodeAImpl86;
                        DecodeB = DecodeBImpl86;
                        break;
                    // unknown arch
                    default:
                        throw new InvalidOperationException("Unknown architecture or unsupported. (Not x86 or x86_64 either)");
                }
            }

            a = DecodeA(input);
            b = DecodeB(input);
        }
        
        private static Func<IntPtr, int> DecodeA;
        private static Func<IntPtr, int> DecodeB;

        private static int DecodeAImpl86(IntPtr param)
        {
            var d = (int) param;
            return d & 0xFFFF;
        }
        
        private static int DecodeBImpl86(IntPtr param)
        {
            var d = (int) param;
            return d >> 16;
        }
        
        private static int DecodeAImpl64(IntPtr param)
        {
            var d = (long) param;
            return (int)(d & 0xFFFF);
        }
        
        private static int DecodeBImpl64(IntPtr param)
        {
            var d = (long) param;
            return (int)(d >> 16);
        }
    }
}
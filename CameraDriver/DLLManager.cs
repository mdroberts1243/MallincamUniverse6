using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ASCOM.MallincamUniverse_I.Camera
{

    public class DLLManager
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        private IntPtr hModule;

        // Load the DLL
        public bool LoadDll(string dllPath)
        {
            hModule = LoadLibrary(dllPath);
            if (hModule == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine($"Failed to load DLL. Error: {error}");
                return false;
            }
            Console.WriteLine("DLL loaded successfully.");
            return true;
        }

        // Free the DLL
        public bool UnloadDll()
        {
            if (hModule != IntPtr.Zero)
            {
                bool result = FreeLibrary(hModule);
                if (!result)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to unload DLL. Error: {error}");
                    return false;
                }
                hModule = IntPtr.Zero;
                Console.WriteLine("DLL unloaded successfully.");
            }
            return true;
        }

        // Get a function pointer from the DLL
        public IntPtr GetFunctionPointer(string functionName)
        {
            IntPtr pFunction = GetProcAddress(hModule, functionName);
            if (pFunction == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine($"Failed to get function pointer. Error: {error}");
            }
            return pFunction;
        }

        // Get the module handle of the loaded DLL
        public IntPtr GetModuleHandle()
        {
            return hModule;
        }
    }

}

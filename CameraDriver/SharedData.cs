using System;

namespace ASCOM.MallincamUniverse_I.Camera
{
    internal static class SharedData
    {
        // The Data Ready Callback and CameraLost Callback address needs to be dynamic
        //   because it is assigned to the window by frmMain.cs 
        //   before CameraHardware.cs runs, then properly addressed
        //   by CameraHardware Initializer and then pulled dynamically 
        //   by the WndProc event processor.
        public static Action OnDataReadyCallback { get; set; }
        public static Action OnCameraLostCallback { get; set; }
        // The WindowHandle of the frmMain.cs window.  Used by
        // NativeListener as it's Parent.
        public static IntPtr WindowHandle { get; set; }

        // Cleanup method to clear shared data properties
        public static void Cleanup() 
        { 
            OnDataReadyCallback = null; 
            OnCameraLostCallback = null; 
            WindowHandle = IntPtr.Zero; 
        }
    }
}

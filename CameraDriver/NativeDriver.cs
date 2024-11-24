using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

//
// NativeDriver class defines the constants, enumerations and function calls extracted from the TS413.DLL
//
// Original provided by Kyle Goodwin
//
namespace ASCOM.MallincamUniverse_I.Camera
{
    class NativeDriver
    {
        public const int WM_RECEIVE_DATA = (NativeDriver.WM_USER + 101);
        public const int WM_LOST_CAMERA = (NativeDriver.WM_USER + 102);
        public const int WM_USER = 1024;

        public enum tagTS_RUNMODE
        {
            /// RUNMODE_PLAY -> 0
            RUNMODE_PLAY = 0,
            RUNMODE_PAUSE,
            RUNMODE_STOP,
        }

        public enum tagTSCCD413_RESOLUTION
        {
            TS413II_3032_2018 = 0, // full resolution, 1x1 binning (no binning)
            TS413II_1516_1008, // 2x2 binning
            TS413II_1008_672,  // 3x3 binning
            TS413II_756_504,   // 4x4 binning
        }

        public enum tagTS_CAMERA_STATUS
        {
            /// STATUS_OK -> 1
            STATUS_OK = 1,
            /// STATUS_INTERNAL_ERROR -> 0
            STATUS_INTERNAL_ERROR = 0,
            /// STATUS_NO_DEVICE_FIND -> -1
            STATUS_NO_DEVICE_FIND = -1,
            /// STATUS_NOT_ENOUGH_SYSTEM_MEMORY -> -2
            STATUS_NOT_ENOUGH_SYSTEM_MEMORY = -2,
            /// STATUS_HW_IO_ERROR -> -3
            STATUS_HW_IO_ERROR = -3,
            /// STATUS_PARAMETER_INVALID -> -4
            STATUS_PARAMETER_INVALID = -4,
            /// STATUS_PARAMETER_OUT_OF_BOUND -> -5
            STATUS_PARAMETER_OUT_OF_BOUND = -5,
            /// STATUS_FILE_CREATE_ERROR -> -6
            STATUS_FILE_CREATE_ERROR = -6,
            /// STATUS_FILE_INVALID -> -7
            STATUS_FILE_INVALID = -7,
        }

        public enum tagTS_MIRROR_DIRECTION
        {
            /// MIRROR_DIRECTION_HORIZONTAL -> 0
            MIRROR_DIRECTION_HORIZONTAL = 0,
            /// MIRROR_DIRECTION_VERTICAL -> 1
            MIRROR_DIRECTION_VERTICAL = 1,
        }

        public enum tagTS_SPEED_MODE
        {
            /// SPEED_MODE_NORMAL -> 0
            SPEED_MODE_NORMAL = 0,
            SPEED_MODE_A,
            SPEED_MODE_B,
            SPEED_MODE_C,
            SPEED_MODE_D,
            SPEED_MODE_E,
            SPEED_MODE_F,
            SPEED_MODE_G,
            SPEED_MODE_MAX,
        }

        public enum tagTS_FILE_TYPE
        {
            /// FILE_JPG -> 1
            FILE_JPG = 1,
            /// FILE_BMP -> 2
            FILE_BMP = 2,
            /// FILE_RAW -> 4
            FILE_RAW = 4,
            /// FILE_TIFF -> 8
            FILE_TIFF = 8,
        }

        public enum tagTS_DATA_TYPE
        {

            /// DATA_TYPE_RAW -> 0
            DATA_TYPE_RAW = 0,
            /// DATA_TYPE_RGB24 -> 1
            DATA_TYPE_RGB24 = 1,
        }

        public enum tagTS_PARAMETER_TEAM
        {
            /// PARAMETER_TEAM_A -> 0
            PARAMETER_TEAM_A = 0,
            /// PARAMETER_TEAM_B -> 1
            PARAMETER_TEAM_B = 1,
            /// PARAMETER_TEAM_C -> 2
            PARAMETER_TEAM_C = 2,
            /// PARAMETER_TEAM_D -> 3
            PARAMETER_TEAM_D = 3,
        }

        /// Return Type: int
        ///pImageBuffer: BYTE*
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public unsafe delegate int SNAP_PROC(byte* pImageBuffer);

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct HWND__
        {
            /// int
            public int unused;
        }

        static NativeDriver()
        {
            if (CameraHardware.dllManager == null)
            {
                throw new Exception("DLLManager is not initialized.");
            }
        }

        private static T GetFunctionDelegate<T>(string functionName) where T : Delegate
        {
            IntPtr pFunction = CameraHardware.dllManager.GetFunctionPointer(functionName);
            if (pFunction == IntPtr.Zero)
            {
                throw new Exception($"Failed to get function pointer for {functionName}");
            }
            return Marshal.GetDelegateForFunctionPointer<T>(pFunction);
        }

        // MDR: Here is the new definitions to support dynamic loading

        // Define the delegates for the DLL functions
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraInitDelegate(byte uiResolution, System.IntPtr hWndDisplay, System.IntPtr hReceive);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraUnInitDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraPlayDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraStopDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraGetImageSizeDelegate(ref int pWidth, ref int pHeight);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraGetImageDataDelegate(IntPtr pRaw);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraDecodeDelegate(IntPtr pRaw, IntPtr pRGB24, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bDataWide);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetResolutionDelegate(byte uiResolution);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetAeStateDelegate([MarshalAs(UnmanagedType.Bool)] bool bState);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetExposureTimeDelegate(uint uiExposureTime);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraGetExposureTimeDelegate(ref uint pExposureTime);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraGetRowTimeDelegate(ref float pRowTime);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetLongExpPowerDelegate([MarshalAs(UnmanagedType.Bool)] bool bPower);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetAnalogGainDelegate(ushort usAnalogGain);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraGetAnalogGainDelegate(ref ushort pAnalogGain);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetAWBStateDelegate([MarshalAs(UnmanagedType.Bool)] bool bAWBState);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetGainDelegate(ushort RGain, ushort GGain, ushort BGain);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetGammaDelegate(sbyte uiGamma);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetContrastDelegate(sbyte uiContrast);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetColorEnhancementDelegate([MarshalAs(UnmanagedType.Bool)] bool bEnable);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetSaturationDelegate(byte uiSaturation);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetMonochromeDelegate([MarshalAs(UnmanagedType.Bool)] bool bEnable);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetMirrorDelegate(tagTS_MIRROR_DIRECTION uiDir, [MarshalAs(UnmanagedType.Bool)] bool bEnable);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetDataWideDelegate([MarshalAs(UnmanagedType.Bool)] bool bDataWide);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetVideoMessageDelegate(uint nMessage);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraSetFrameSpeedDelegate(byte uiSpeed);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate tagTS_CAMERA_STATUS CameraDisplayEnableDelegate([MarshalAs(UnmanagedType.Bool)] bool bEnable);

        // Camera Init Function
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///pCallBackFun: SNAP_PROC
        ///uiResolution: BYTE->unsigned char
        ///hWndDisplay: HWND->HWND__*
        ///hReceive: HWND->HWND__*
        public static tagTS_CAMERA_STATUS CameraInit(byte uiResolution, System.IntPtr hWndDisplay, System.IntPtr hReceive)
        {
            var func = GetFunctionDelegate<CameraInitDelegate>("TS413IICameraInit");
            return func(uiResolution, hWndDisplay, hReceive);
        }

        // Camera Un Init Function
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        public static tagTS_CAMERA_STATUS CameraUnInit()
        {
            var func = GetFunctionDelegate<CameraUnInitDelegate>("TS413IICameraUnInit");
            return func();
        }

        // Camera Play Mode
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        public static tagTS_CAMERA_STATUS CameraPlay()
        {
            var func = GetFunctionDelegate<CameraPlayDelegate>("TS413IICameraPlay");
            return func();
        }

        // Camera Stop Mode
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        public static tagTS_CAMERA_STATUS CameraStop()
        {
            var func = GetFunctionDelegate<CameraStopDelegate>("TS413IICameraStop");
            return func();
        }

        // Camera Get Image Size
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///pWidth: int*
        ///pHeight: int*
        public static tagTS_CAMERA_STATUS CameraGetImageSize(ref int pWidth, ref int pHeight)
        {
            var func = GetFunctionDelegate<CameraGetImageSizeDelegate>("TS413IICameraGetImageSize");
            return func(ref pWidth, ref pHeight);
        }

        // Camera Get Image Data
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///pRaw: BYTE*
        public static tagTS_CAMERA_STATUS CameraGetImageData(IntPtr pRaw)
        {
            var func = GetFunctionDelegate<CameraGetImageDataDelegate>("TS413IICameraGrabFrame");
            return func(pRaw);
        }

        // Camera Decode 
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///pRaw: BYTE*
        public static tagTS_CAMERA_STATUS CameraDecode(IntPtr pRaw, IntPtr pRGB24, int nWidth, int nHeight, bool bDataWide)
        {
            var func = GetFunctionDelegate<CameraDecodeDelegate>("TS413IICameraDecode");
            return func(pRaw, pRGB24, nWidth, nHeight, bDataWide);
        }

        // Camera Set Resolution
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///uiResolution: BYTE->unsigned char
        public static tagTS_CAMERA_STATUS CameraSetResolution(byte uiResolution)
        {
            var func = GetFunctionDelegate<CameraSetResolutionDelegate>("TS413IICameraSetResolution");
            return func(uiResolution);
        }

        // Camera Set AE State
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///bState: BOOL->int
        public static tagTS_CAMERA_STATUS CameraSetAeState(bool bState)
        {
            var func = GetFunctionDelegate<CameraSetAeStateDelegate>("TS413IICameraSetAeState");
            return func(bState);
        }

        // Camera Set Exposure Time
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///uiExposureTime: DWORD->unsigned int
        public static tagTS_CAMERA_STATUS CameraSetExposureTime(uint uiExposureTime)
        {
            var func = GetFunctionDelegate<CameraSetExposureTimeDelegate>("TS413IICameraSetExposureTime");
            return func(uiExposureTime);
        }

        // Camera Get Exposure Time
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///pExposureTime: DWORD*
        public static tagTS_CAMERA_STATUS CameraGetExposureTime(ref uint pExposureTime)
        {
            var func = GetFunctionDelegate<CameraGetExposureTimeDelegate>("TS413IICameraGetExposureTime");
            return func(ref pExposureTime);
        }

        // Camera Get Row Time
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///pRowTime: FLOAT*
        public static tagTS_CAMERA_STATUS CameraGetRowTime(ref float pRowTime)
        {
            var func = GetFunctionDelegate<CameraGetRowTimeDelegate>("TS413IICameraGetRowTime");
            return func(ref pRowTime);
        }

        // Camera Set Long Exposure Power
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///bPower: BOOL->int
        public static tagTS_CAMERA_STATUS CameraSetLongExpPower(bool bPower)
        {
            var func = GetFunctionDelegate<CameraSetLongExpPowerDelegate>("TS413IICameraSetLongExpPower");
            return func(bPower);
        }

        // Camera Set Analog Gain
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///usAnalogGain: USHORT->unsigned short
        public static tagTS_CAMERA_STATUS CameraSetAnalogGain(ushort usAnalogGain)
        {
            var func = GetFunctionDelegate<CameraSetAnalogGainDelegate>("TS413IICameraSetAnalogGain");
            return func(usAnalogGain);
        }

        // Camera Get Analog Gain
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///pAnalogGain: USHORT*
        public static tagTS_CAMERA_STATUS CameraGetAnalogGain(ref ushort pAnalogGain)
        {
            var func = GetFunctionDelegate<CameraGetAnalogGainDelegate>("TS413IICameraGetAnalogGain");
            return func(ref pAnalogGain);
        }

        // Camera Set AWB State 
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///bAWBState: BOOL->int
        public static tagTS_CAMERA_STATUS CameraSetAWBState(bool bAWBState)
        {
            var func = GetFunctionDelegate<CameraSetAWBStateDelegate>("TS413IICameraSetAWBState");
            return func(bAWBState);
        }

        // Camera Set Gain
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///RGain: USHORT->unsigned short
        ///GGain: USHORT->unsigned short
        ///BGain: USHORT->unsigned short
        public static tagTS_CAMERA_STATUS CameraSetGain(ushort RGain, ushort GGain, ushort BGain)
        {
            var func = GetFunctionDelegate<CameraSetGainDelegate>("TS413IICameraSetGain");
            return func(RGain, GGain, BGain);
        }
        // Camera Set Gamma
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///uiGamma: BYTE->unsigned char
        public static tagTS_CAMERA_STATUS CameraSetGamma(sbyte uiGamma)
        {
            var func = GetFunctionDelegate<CameraSetGammaDelegate>("TS413IICameraSetGamma");
            return func(uiGamma);
        }

        // Camera Set Contrast
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///uiContrast: BYTE->unsigned char
        public static tagTS_CAMERA_STATUS CameraSetContrast(sbyte uiContrast)
        {
            var func = GetFunctionDelegate<CameraSetContrastDelegate>("TS413IICameraSetContrast");
            return func(uiContrast);
        }

        // Camera Set Color Enhancement
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///bEnable: BOOL->int
        public static tagTS_CAMERA_STATUS CameraSetColorEnhancement(bool bEnable)
        {
            var func = GetFunctionDelegate<CameraSetColorEnhancementDelegate>("TS413IICameraSetColorEnhancement");
            return func(bEnable);
        }

        // Camera Set Saturation
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///uiSaturation: BYTE->unsigned char
        public static tagTS_CAMERA_STATUS CameraSetSaturation(byte uiSaturation)
        {
            var func = GetFunctionDelegate<CameraSetSaturationDelegate>("TS413IICameraSetSaturation");
            return func(uiSaturation);
        }

        // Camera Set Monochrome
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///bEnable: BOOL->int
        public static tagTS_CAMERA_STATUS CameraSetMonochrome(bool bEnable)
        {
            var func = GetFunctionDelegate<CameraSetMonochromeDelegate>("TS413IICameraSetMonochrome");
            return func(bEnable);
        }

        // Camera Set Mirror
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///uiDir: TS_MIRROR_DIRECTION->tagTS_MIRROR_DIRECTION
        ///bEnable: BOOL->int
        public static tagTS_CAMERA_STATUS CameraSetMirror(tagTS_MIRROR_DIRECTION uiDir, bool bEnable)
        {
            var func = GetFunctionDelegate<CameraSetMirrorDelegate>("TS413IICameraSetMirror");
            return func(uiDir, bEnable);
        }

        // Camera Set Frame Speed
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///uiSpeed: BYTE->unsigned char
        public static tagTS_CAMERA_STATUS CameraSetFrameSpeed(byte uiSpeed)
        {
            var func = GetFunctionDelegate<CameraSetFrameSpeedDelegate>("TS413IICameraSetFrameSpeed");
            return func(uiSpeed);
        }

        // Camera Set Data Wide (True/False)
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///bDataWide: BOOL->int
        public static tagTS_CAMERA_STATUS CameraSetDataWide(bool bDataWide)
        {
            var func = GetFunctionDelegate<CameraSetDataWideDelegate>("TS413IICameraSetDataWide");
            return func(bDataWide);
        }

        // Camera Set Video Message
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///nMessage: UINT->unsigned int
        public static tagTS_CAMERA_STATUS CameraSetVideoMessage(uint nMessage)
        {
            var func = GetFunctionDelegate<CameraSetVideoMessageDelegate>("TS413IICameraSetVideoMessage");
            return func(nMessage);
        }

        // Camera Display Enable
        /// Return Type: TS_CAMERA_STATUS->tagTS_CAMERA_STATUS
        ///bEnable: BOOL->int
        public static tagTS_CAMERA_STATUS CameraDisplayEnable(bool bEnable)
        {
            var func = GetFunctionDelegate<CameraDisplayEnableDelegate>("TS413IICameraDisplayEnable");
            return func(bEnable);
        }




    }
}

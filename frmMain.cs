using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using ASCOM.MallincamUniverse_I.Camera;

namespace ASCOM.LocalServer
{
    public partial class FrmMain : Form
    {
        private delegate void SetTextCallback(string text);

        public FrmMain()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.Load += FrmMain_Load;  // MDR: added

            // MDR: Subscribe to the ApplicationExit event
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
        }

        // below code is to set up my listener for the DLL callbacks from TS413.DLL
        private void FrmMain_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("FrmMain_Load started");
            try
            {
                // Initialize the callbacks here before initializing CustomListener
                SharedData.OnDataReadyCallback = CameraHardware.OnDataReady;
                SharedData.OnCameraLostCallback = CameraHardware.OnCameraLost;
                // Pass the handle to SharedData for CameraHardware to use later
                SharedData.WindowHandle = this.Handle;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during FrmMain_Load: {ex.Message}");
            }
        }

        // WndProc for catching the custom events
        protected override void WndProc(ref Message m)
        {
            const int DBT_DEVICEARRIVAL = 0x8000; 
            const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

            string message = $"Message received: {m.Msg}, LParam: {m.LParam}, WParam: {m.WParam}, HWnd: {m.HWnd}, Thread ID: {Thread.CurrentThread.ManagedThreadId}";
            System.Diagnostics.Debug.WriteLine(message); // Add debug output

            switch (m.Msg)
            {
                case NativeDriver.WM_RECEIVE_DATA:
                    SharedData.OnDataReadyCallback?.Invoke();
                    break;
                case NativeDriver.WM_LOST_CAMERA:
                    SharedData.OnCameraLostCallback?.Invoke();
                    break;
                case 0x0219: // WM_DEVICECHANGE
                    switch ((int)m.WParam)
                    {
                        case DBT_DEVICEARRIVAL:
                            System.Diagnostics.Debug.WriteLine("Device connected");
                            // Handle device connection
                            SharedData.OnCameraLostCallback?.Invoke();
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            System.Diagnostics.Debug.WriteLine("Device disconnected");
                            // Handle device disconnection
                            SharedData.OnCameraLostCallback?.Invoke();
                            break; 
                        default:
                            System.Diagnostics.Debug.WriteLine($"Unhandled device change event: {m.WParam}"); 
                            break; 
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        // Event handler for ApplicationExit
        private void OnApplicationExit(object sender, EventArgs e)
        {
            // Call cleanup methods
            SharedData.Cleanup();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Detach event handlers
            this.Load -= FrmMain_Load;

            // Dispose components
            if (components != null)
            {
                components.Dispose();
            }

            base.OnFormClosing(e);
        }

    }
}

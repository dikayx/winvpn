using System.Windows;
using System.Windows.Interop;
using WinVPN.Interop; // WindowsNativeHandler

namespace WinVPN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SourceInitialized += Window_SourceInitialized;

            InitializeComponent();
        }

        // The following code is used to handle the window resizing and moving.
        // It is essential together with the WindowsNativeHandler.cs file for properly resizing.
        void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WindowsNativeHandler.WmGetMinMaxInfo(hwnd, lParam, (int)MinWidth, (int)MinHeight);
                    handled = true;
                    break;
            }

            return (IntPtr)0;
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void ButtonMax_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Minimized ? WindowState.Normal : WindowState.Minimized;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
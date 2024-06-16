using System;
using System.Runtime.InteropServices;

public static class ScreenHelper
{
    public static (int Width, int Height, int X, int Y) GetTotalScreenSize()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetWindowsScreenSize();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return GetMacOSScreenSize();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetLinuxScreenSize();
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported platform");
        }
    }

    private static (int Width, int Height, int X, int Y) GetWindowsScreenSize()
    {
        int totalWidth = 0;
        int totalHeight = 0;
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) =>
        {
            MonitorInfo mi = new MonitorInfo();
            mi.cbSize = Marshal.SizeOf(mi);
            if (GetMonitorInfo(hMonitor, ref mi))
            {
                minX = Math.Min(minX, mi.rcMonitor.left);
                minY = Math.Min(minY, mi.rcMonitor.top);
                maxX = Math.Max(maxX, mi.rcMonitor.right);
                maxY = Math.Max(maxY, mi.rcMonitor.bottom);
            }
            return true;
        }, IntPtr.Zero);

        totalWidth = maxX - minX;
        totalHeight = maxY - minY;

        return (totalWidth, totalHeight, minX, minY);
    }

    private static (int Width, int Height, int X, int Y) GetMacOSScreenSize()
    {
        // Implement macOS logic if needed
        throw new NotImplementedException("macOS implementation is not available");
    }

    private static (int Width, int Height, int X, int Y) GetLinuxScreenSize()
    {
        // Implement Linux logic if needed
        throw new NotImplementedException("Linux implementation is not available");
    }

    // Windows P/Invoke
    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MonitorInfo
    {
        public int cbSize;
        public Rect rcMonitor;
        public Rect rcWork;
        public int dwFlags;
    }
}

using System.Runtime.InteropServices;

namespace InventoryHotkeys
{
    static class VirtualNumpad
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_KEYDOWN = 0x0000;

        public enum Numpad
        {
            _1 = 0x61,
            _2 = 0x62,
            _4 = 0x64,
            _5 = 0x65,
            _7 = 0x67,
            _8 = 0x68
        }

        public static void PressDown(Numpad numpad)
        {
            keybd_event((byte)numpad, 0, KEYEVENTF_KEYDOWN, 0);
        }
    }
}

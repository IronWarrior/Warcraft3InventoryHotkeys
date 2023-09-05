﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InventoryHotkeys
{
    /// <summary>
    /// Attaches to Windows keypress event hooks and forwards the calls.
    /// </summary>
    class InterceptKeys : IDisposable
    {
        public event Action<int> OnPress;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private IntPtr hookID = IntPtr.Zero;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public void Register()
        {
            Debug.Assert(hookID == IntPtr.Zero, "Attempting to register hook that has already been registered.");

            hookID = SetHook(HookCallback);
        }

        public void Dispose()
        {
            if (hookID != IntPtr.Zero)
                UnhookWindowsHookEx(hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;

            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                try
                {
                    OnPress?.Invoke(vkCode);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
    }
}

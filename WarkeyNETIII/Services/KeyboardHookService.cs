﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WarkeyNETIII.Items;

namespace WarkeyNETIII.Services
{
    public static class KeyboardHookService
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;

        //LEFT ALT for lParam
        private const int VK_LMENU = 0x00A4;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static void Initialize()
        {
            _hookID = SetHook(_proc);
        }

        public static void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // iswar3foreground have to put here
            // because if not in foreground no need to do anything
            // else need to have pointer to mainservice iswar3foreground
            // and pass in here when initialize
            // which is not possible with C#
            if (MainService.IsWar3Foreground && !ChatboxService.IsChatting && nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    GlobalKeyDown(null, new HotkeyItem()
                    {
                        Alt = false,
                        Key = KeyInterop.KeyFromVirtualKey(vkCode)
                    });
                }
                else if (wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    if (vkCode != VK_LMENU)
                    {
                        GlobalKeyDown(null, new HotkeyItem()
                        {
                            Alt = true,
                            Key = KeyInterop.KeyFromVirtualKey(vkCode)
                        });
                    }
                }

            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public static event EventHandler<HotkeyItem> GlobalKeyDown;

    }
}

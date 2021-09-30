using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Raptiller
{
    class InputReceiver
    {
        private struct KBDLLHOOKSTRUCT
        {
            public Int32 vkCode;
            public Int32 scanCode;
            public Int32 flags;
            public Int32 time;
            IntPtr dwExtraInfo;
        }

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private const int WH_KEYBOARD_LL = 13;
        private const int LLKHF_INJECTED = 0x00000010;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private LowLevelKeyboardProc lowLevelKeyBoardProcess;

        private IntPtr hookID = IntPtr.Zero;

        public InputReceiver()
        {
            lowLevelKeyBoardProcess = HookCallback;
            HookKeyBoard();

            InputModifier.Initialize();
        }

        ~InputReceiver()
        {
            UnHookKeyBoard();
        }

        private void HookKeyBoard()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookID = SetWindowsHookEx(WH_KEYBOARD_LL, lowLevelKeyBoardProcess, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private void UnHookKeyBoard()
        {
            UnhookWindowsHookEx(hookID);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KBDLLHOOKSTRUCT keyStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            if (nCode >= 0)
            {
                if ((keyStruct.flags & LLKHF_INJECTED) == 0)
                {
                    if (wParam == (IntPtr)InputReceiver.WM_KEYDOWN || wParam == (IntPtr)InputReceiver.WM_SYSKEYDOWN)
                    {
                        InputModifier.SendKey(keyStruct.vkCode, keyStruct.scanCode, keyStruct.flags, true);
                    }

                    if (wParam == (IntPtr)InputReceiver.WM_KEYUP || wParam == (IntPtr)InputReceiver.WM_SYSKEYUP)
                    {
                        InputModifier.SendKey(keyStruct.vkCode, keyStruct.scanCode, keyStruct.flags, false);
                    }
                    return (System.IntPtr)1;
                }
            }

            if (wParam == (IntPtr)InputReceiver.WM_KEYDOWN || wParam == (IntPtr)InputReceiver.WM_SYSKEYDOWN)
            {
                Debug.WriteLine(((Keys)keyStruct.vkCode).ToString() + " is pressed");
            }
            else if (wParam == (IntPtr)InputReceiver.WM_KEYUP || wParam == (IntPtr)InputReceiver.WM_SYSKEYUP)
            {
                Debug.WriteLine(((Keys)keyStruct.vkCode).ToString() + " is released");
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
    }
}

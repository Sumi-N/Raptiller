using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Raptiller
{
    class IMEChecker
    {
        public struct HKL
        {
            public Int16 languageIdentifiers;
        }

        public struct RECT
        {
            Int64 left;
            Int64 top;
            Int64 right;
            Int64 bottom;
        }

        public struct GUITHREADINFO
        {
            Int32  cbSize;
            Int32  flags;
            IntPtr hwndActive;
            IntPtr hwndFocus;
            IntPtr hwndCapture;
            IntPtr hwndMenuOwner;
            IntPtr hwndMoveSize;
            IntPtr hwndCaret;
            RECT   rcCaret;
        }

        private const int WM_IME_CONTROL = 0x0283;
        private const int IMC_GETOPENSTATUS = 0x0005;
        private const int IMC_SETOPENSTATUS = 0x0006;

        private const int KEYBOARD_JAPANESE = 1041;
        private const int KEYBOARD_ENGLISH = 1033;

        [DllImport("user32")]
        public static extern HKL GetKeyboardLayout(Int32 idThread);

        [DllImport("user32")]
        public static extern bool GetKeyboardLayoutNameA(Int32 idThread);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern Int32 GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

        [DllImport("user32.dll")]
        static extern bool GetGUIThreadInfo(Int32 idThread, IntPtr pgui);

        [DllImport("user32.dll")]
        static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, IntPtr dwExtraInfo);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        static extern System.Boolean GetThreadPreferredUILanguages(

           System.UInt32 dwFlags,

           ref System.UInt32 pulNumLanguages,

           out System.IntPtr pwszLanguagesBuffer,

           ref System.UInt32 pcchLanguagesBuffer

        );

        [DllImport("Imm32.dll")]
        static extern IntPtr ImmGetDefaultIMEWnd(IntPtr unnamedParam1);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public static void ModifyJapaneseIME(int vk, int sc)
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            Int32 foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
            HKL KeyboardLayout = GetKeyboardLayout(foregroundProcess);

            // If current IME is English
            if (KeyboardLayout.languageIdentifiers == KEYBOARD_ENGLISH)
            {
                return;
            }
            // If current IME is Japanese
            else if (KeyboardLayout.languageIdentifiers == KEYBOARD_JAPANESE)
            {
                IntPtr imeWindow = ImmGetDefaultIMEWnd(foregroundWindow);
                SendMessage(imeWindow, WM_IME_CONTROL, (IntPtr)IMC_SETOPENSTATUS, (IntPtr)1);

                //if (vk == 160)
                //{
                //    keybd_event((Byte)vk, (Byte)sc, (Int32)0x02, (IntPtr)0xFFC3D450);
                //}
                keybd_event((Byte)vk, (Byte)sc, (Int32)0x02, (IntPtr)0);
            }
            else
            {
                return;
            }

            //uint MUI_MERGE_USER_FALLBACK = 0x20;

            //uint numLang = 0;

            //IntPtr pwszLangBuf = new IntPtr();

            //uint langBuf = 2323;

            //bool b = GetThreadPreferredUILanguages(MUI_MERGE_USER_FALLBACK, ref numLang, out pwszLangBuf, ref langBuf);

            //string tmp = Marshal.PtrToStringAuto(pwszLangBuf);
        }
    }
}

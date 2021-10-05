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
        private struct HKL
        {
            public Int16 languageIdentifiers;
        }

        private const int WM_IME_CONTROL = 0x0283;
        private const int IMC_GETOPENSTATUS = 0x0005;
        private const int IMC_SETOPENSTATUS = 0x0006;

        private const int KEYBOARD_JAPANESE = 1041;
        private const int KEYBOARD_ENGLISH = 1033;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

        [DllImport("user32.dll")]
        private static extern HKL GetKeyboardLayout(Int32 idThread);

        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr unnamedParam1);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public static void ModifyJapaneseIME(ref KeyBoardType type)
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            Int32 foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
            HKL KeyboardLayout = GetKeyboardLayout(foregroundProcess);

            if (KeyboardLayout.languageIdentifiers == KEYBOARD_ENGLISH)
            {
                type = KeyBoardType.English;
            }
            else if (KeyboardLayout.languageIdentifiers == KEYBOARD_JAPANESE)
            {
                type = KeyBoardType.Japanese;

                IntPtr imeWindow = ImmGetDefaultIMEWnd(foregroundWindow);
                SendMessage(imeWindow, WM_IME_CONTROL, (IntPtr)IMC_SETOPENSTATUS, (IntPtr)1);
            }

            return;
        }
    }
}

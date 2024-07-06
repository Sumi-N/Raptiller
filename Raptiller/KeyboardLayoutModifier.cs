using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace Raptiller
{
    class KeyboardLayoutModifier
    {
        private const int MAPVK_VK_TO_VSC = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;

        private struct HKL
        {
            public Int16 languageIdentifiers;
        }

        private struct KEYDBINPUT
        {
            public Int16 wVk;
            public Int16 wScan;
            public Int32 dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
            public Int32 __filler1;
            public Int32 __filler2;
        }

        private struct INPUT
        {
            public Int32 type;
            public KEYDBINPUT ki;
        }

        private const int WM_IME_CONTROL = 0x0283;
        private const int IMC_GETCONVERSIONMODE = 0x0001;
        private const int IMC_SETCONVERSIONMODE = 0x0002;
        private const int IMC_GETOPENSTATUS = 0x0005;
        private const int IMC_SETOPENSTATUS = 0x0006;

        private const int KEYBOARD_JAPANESE = 1041;
        private const int KEYBOARD_ENGLISH = 1033;

        private const int IME_CMODE_ALPHANUMERIC = 0x00;
        private const int IME_CMODE_HIRAGANA = 0x09;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

        [DllImport("user32.dll")]
        private static extern HKL GetKeyboardLayout(Int32 idThread);

        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr unnamedParam1);

        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr unnamedParam1);

        [DllImport("imm32.dll")]
        private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("imm32.dll")]
        private static extern bool ImmGetConversionStatus(IntPtr hIMC, out uint lpfdwConversion, out uint lpfdwSentence);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SendInput(int cInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        public extern static int MapVirtualKey(int wCode, int wMapType);

        public static void Check(ref KeyBoardType type)
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
                IntPtr imeWindow = ImmGetDefaultIMEWnd(foregroundWindow);
                IntPtr status = SendMessage(imeWindow, WM_IME_CONTROL, (IntPtr)IMC_GETCONVERSIONMODE, (IntPtr)0);

                if (status == (IntPtr)IME_CMODE_ALPHANUMERIC)
                {
                    type = KeyBoardType.Japanese_AlphabetNumeric;
                }
                else if (status == (IntPtr)IME_CMODE_HIRAGANA)
                {
                    type = KeyBoardType.Japanese_Hiragana;
                }
            }
            else
            {
                Debug.WriteLine("Unexpected KeyboardLayout is set!");
            }
        }

        public static void Modify(KeyBoardType currentKeyboardType)
        {
            if (currentKeyboardType == KeyBoardType.English)
            {
                // The keyboard sometime becomes to CapsLock state after transitioning to English layout.
                // To prevent that, check if the keyboard is in CapsLocked and unlock the state if it is.
                if (Control.IsKeyLocked(Keys.CapsLock))
                {
                    INPUT input = new INPUT();
                    int key = (int)System.Windows.Forms.Keys.CapsLock;
                    int vsc = MapVirtualKey(key, MAPVK_VK_TO_VSC);

                    input.type = INPUT_KEYBOARD;
                    input.ki.dwFlags = 0;
                    input.ki.wVk = (Int16)key;
                    input.ki.wScan = (Int16)vsc;

                    SendInput(1, ref input, Marshal.SizeOf(input));
                }
            }
            else if (currentKeyboardType == KeyBoardType.Japanese_AlphabetNumeric)
            {
                IntPtr foregroundWindow = GetForegroundWindow();
                IntPtr imeWindow = ImmGetDefaultIMEWnd(foregroundWindow);
                SendMessage(imeWindow, WM_IME_CONTROL, (IntPtr)IMC_SETCONVERSIONMODE, (IntPtr)9);
            }

            return;
        }

        public static void Exec(ref KeyBoardType type)
        {
            Check(ref type);
            Modify(type);
        }
    }
}

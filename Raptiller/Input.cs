using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Raptiller
{
    public class Input
    {
        [DllImport("user32")]
        public static extern int SendInput(int cInputs, ref INPUT pInputs, int cbSize);

        public const int INPUT_KEYBOARD = 1;
        public const int KEYEVENTF_KEYUP = 0x0002;

        public struct KEYDBINPUT
        {
            public Int16 wVk;
            public Int16 wScan;
            public Int32 dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
            public Int32 __filler1;
            public Int32 __filler2;
        }

        public struct INPUT
        {
            public Int32 type;
            public KEYDBINPUT ki;
        }

        public static void SendKey(Keys vk, int sc, int flags, bool isPress)
        {
            INPUT input = new INPUT();
            input.type = INPUT_KEYBOARD;
            input.ki.dwFlags = isPress ? 0 : KEYEVENTF_KEYUP;
            input.ki.dwFlags = (input.ki.dwFlags | flags);
            input.ki.wVk = (Int16)vk;
            input.ki.wScan = (Int16)sc;
            SendInput(1, ref input, Marshal.SizeOf(input));
        }

        public static void SendKeyArray(bool isPress, params Keys[] vks)
        {
            INPUT test = new INPUT();
            INPUT[] inputs = new INPUT[vks.Length];
            for (int i = 0; i < vks.Length; i++)
            {
                inputs[i].type = INPUT_KEYBOARD;
                inputs[i].ki.dwFlags = isPress ? 0 : KEYEVENTF_KEYUP;
                inputs[i].ki.wVk = (Int16)vks[i];
            }
            SendInput(vks.Length, ref inputs[0], Marshal.SizeOf(test));
            //if (uSent != ARRAYSIZE(inputs))
            //{
            //    OutputString(L"SendInput failed: 0x%x\n", HRESULT_FROM_WIN32(GetLastError()));
            //}
        }

        public static void Test()
        {
            //SendKey(Keys.LShiftKey, 0, true);
            //SendKey(Keys.Left, 0, true);
            ////SendKey(Keys.Left, true);
            ////SendKey(Keys.Left, true);
            ////SendKey(Keys.Left, true);
            //SendKey(Keys.Left, 0, false);
            //SendKey(Keys.LShiftKey, 0, false);
        }
    }
}
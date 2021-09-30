using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;

namespace Raptiller
{
    class InputModifier
    {
        private static Dictionary<Keys, bool> InputStates = new Dictionary<Keys, bool>();

        public static void Initialize()
        {
            for( int i = 0; i < 256; i++)
            {
                InputStates.Add((Keys)i, false);
            }

            Console.WriteLine(String.Join(Environment.NewLine, InputStates));
        }

        public static void SendKey(IntPtr wParam, IntPtr lParam)
        {
            KBDLLHOOKSTRUCT keyStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            if (wParam == (IntPtr)InputReceiver.WM_KEYDOWN || wParam == (IntPtr)InputReceiver.WM_SYSKEYDOWN)
            {
                InputStates[(Keys)keyStruct.vkCode] = true;
            }

            if (wParam == (IntPtr)InputReceiver.WM_KEYUP || wParam == (IntPtr)InputReceiver.WM_SYSKEYUP)
            {
                InputStates[(Keys)keyStruct.vkCode] = false;
            }

            if (InputStates[Keys.LShiftKey])
            {
                if((Keys)keyStruct.vkCode == Keys.B)
                {
                    Input.PressKey(Keys.U);
                }
            }
            else
            {
                if (wParam == (IntPtr)InputReceiver.WM_KEYDOWN || wParam == (IntPtr)InputReceiver.WM_SYSKEYDOWN)
                {
                    Input.PressKey((Keys)keyStruct.vkCode);
                }

                if (wParam == (IntPtr)InputReceiver.WM_KEYUP || wParam == (IntPtr)InputReceiver.WM_SYSKEYUP)
                {
                    Input.ReleaseKey((Keys)keyStruct.vkCode);
                }
            }

            return;
        }
    }
}

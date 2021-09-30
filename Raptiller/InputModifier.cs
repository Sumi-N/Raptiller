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
            InputStates = Enum.GetValues(typeof(Keys))
                           .Cast<Keys>()
                           .ToDictionary(t => (Keys)t, t => (bool)t);

            Console.WriteLine(String.Join(Environment.NewLine, InputStates));
        }

        public static void SendKey(IntPtr wParam, IntPtr lParam)
        {
            KBDLLHOOKSTRUCT keyStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            if (wParam == (IntPtr)InputReceiver.WM_KEYDOWN || wParam == (IntPtr)InputReceiver.WM_SYSKEYDOWN)
            {
                //Input.PressKey((Keys)keyStruct.vkCode);
            }

            if (wParam == (IntPtr)InputReceiver.WM_KEYUP || wParam == (IntPtr)InputReceiver.WM_SYSKEYUP)
            {
                //Input.ReleaseKey((Keys)keyStruct.vkCode);
            }

            if ()
            {

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

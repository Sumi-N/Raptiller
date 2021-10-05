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
        [DllImport("user32.dll")]
        private static extern int SendInput(int cInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, IntPtr dwExtraInfo);

        private const int INPUT_KEYBOARD = 1;
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        public struct KeyInfo
        {
            public Keys virtualKey;
            public int  scanCode;
            public bool isPressed;
            public bool isModified;
            public bool shouldSend;
            public int  flags;
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

        private static Dictionary<Keys, bool> InputStates = new Dictionary<Keys, bool>();

        public static void Initialize()
        {
            for (int i = 0; i < 256; i++)
            {
                InputStates.Add((Keys)i, false);
            }
        }

        public static void Modify(KeyBoardType type, ref KeyInfo info)
        {            
            InputStates[info.virtualKey] = info.isPressed;

            if(type == KeyBoardType.Japanese)
            {
                // When Shift + Caps is pressed in Japanese Keyboard, the system will automatically translate the key input to 
                // the virtual keycode equals to System.Windows.Forms.Keys.D0 | System.Windows.Forms.Keys.Oem3, which is equivalent to 240 in Integer.
                // The statement below prevent that to be happened and translate it to work as same as the English KeyBoard.
                if (info.virtualKey == (Keys)240)
                {
                    InputStates[Keys.CapsLock] = true;
                    info.isModified = true;
                    info.shouldSend = false;
                }
            }

            if (InputStates[Keys.CapsLock])
            {
                switch (info.virtualKey)
                {
                    case Keys.B:
                        info.virtualKey = Keys.Left;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        info.isModified = true;
                        break;

                    case Keys.F:
                        info.virtualKey = Keys.Right;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        info.isModified = true;
                        break;

                    case Keys.P:                        
                        info.virtualKey = Keys.Up;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        info.isModified = true;
                        break;

                    case Keys.N:                        
                        info.virtualKey = Keys.Down;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        info.isModified = true;
                        break;

                    case Keys.D:                        
                        info.virtualKey = Keys.Delete;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        info.isModified = true;
                        break;

                    case Keys.H:                        
                        info.virtualKey = Keys.Back;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        info.isModified = true;
                        break;

                    case Keys.E:                        
                        info.virtualKey = Keys.End;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        info.isModified = true;
                        break;

                    case Keys.A:                        
                        info.virtualKey = Keys.Home;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        info.isModified = true;
                        break;

                    case Keys.CapsLock:
                        info.isModified = true;
                        info.shouldSend = false;
                        return;

                    default:
                        info.flags |= 0x00;
                        info.isModified = true;
                        break;
                }
            }

            return;
        }

        private static INPUT SeindingInput = new INPUT();

        public static void SendKey(KeyInfo info)
        {            
            SeindingInput.type = INPUT_KEYBOARD;
            SeindingInput.ki.dwFlags = info.isPressed ? 0 : KEYEVENTF_KEYUP;
            SeindingInput.ki.dwFlags |= info.flags;
            SeindingInput.ki.wVk = (Int16)info.virtualKey;
            SeindingInput.ki.wScan = (Int16)info.scanCode;
            SendInput(1, ref SeindingInput, Marshal.SizeOf(SeindingInput));
        }
    }
}
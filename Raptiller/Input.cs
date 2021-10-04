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
                if (InputStates[Keys.LShiftKey])
                {
                    keybd_event((Byte)Keys.LShiftKey, (Byte)0, (Int32)0x02, (IntPtr)0);
                }
            }

            if (InputStates[Keys.CapsLock])
            {
                switch (info.virtualKey)
                {
                    case Keys.B:
                        info.virtualKey = Keys.Left;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        break;

                    case Keys.F:
                        info.virtualKey = Keys.Right;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        break;

                    case Keys.P:                        
                        info.virtualKey = Keys.Up;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        break;

                    case Keys.N:                        
                        info.virtualKey = Keys.Down;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        break;

                    case Keys.D:                        
                        info.virtualKey = Keys.Delete;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        break;

                    case Keys.H:                        
                        info.virtualKey = Keys.Back;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        break;

                    case Keys.E:                        
                        info.virtualKey = Keys.End;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        break;

                    case Keys.A:                        
                        info.virtualKey = Keys.Home;
                        info.flags |= KEYEVENTF_EXTENDEDKEY;
                        break;

                    case Keys.CapsLock:
                        return;

                    default:
                        info.flags |= 0x00;
                        break;
                }
                info.isModified = true;
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
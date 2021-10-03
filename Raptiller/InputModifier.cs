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
        private static Keys CurrentKey;

        public static void Initialize()
        {
            for( int i = 0; i < 256; i++)
            {
                InputStates.Add((Keys)i, false);
            }
        }

        public static void SendKey(int vk, int sc, int flags, bool isPress)
        {
            CurrentKey = (Keys)vk;
            
            InputStates[CurrentKey] = isPress;

            if (InputStates[Keys.CapsLock])
            {
                switch (CurrentKey)
                {
                    case Keys.B:
                        Input.SendKey(Keys.Left, sc, 1, isPress);
                        break;

                    case Keys.F:
                        Input.SendKey(Keys.Right, sc, 1, isPress);
                        break;

                    case Keys.P:
                        Input.SendKey(Keys.Up, sc, 1, isPress);
                        break;

                    case Keys.N:
                        Input.SendKey(Keys.Down, sc, 1, isPress);
                        break;

                    case Keys.D:
                        Input.SendKey(Keys.Delete, sc, 1, isPress);
                        break;

                    case Keys.H:
                        Input.SendKey(Keys.Back, sc, 1, isPress);
                        break;

                    case Keys.E:
                        Input.SendKey(Keys.End, sc, 1, isPress);
                        break;

                    case Keys.A:
                        Input.SendKey(Keys.Home, sc, 1, isPress);
                        break;

                    case Keys.CapsLock:
                        return;                        

                    default:
                        Input.SendKey(CurrentKey, sc, 0, isPress);
                        break;
                }
                return;
            }
            else
            {
                Input.SendKey(CurrentKey, sc, flags, isPress);                
            }

            return;
        }
    }
}

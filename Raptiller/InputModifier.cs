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

            Console.WriteLine(String.Join(Environment.NewLine, InputStates));
        }

        public static void SendKey(int vk, bool isPress)
        {            
            CurrentKey = (Keys)vk;
            
            InputStates[CurrentKey] = isPress;

            if (InputStates[Keys.LShiftKey])
            {
                switch (CurrentKey)
                {
                    case Keys.B:
                        Input.SendKey(Keys.Left, isPress);
                        break;

                    case Keys.F:
                        Input.SendKey(Keys.Right, isPress);
                        break;

                    case Keys.P:
                        Input.SendKey(Keys.Up, isPress);
                        break;

                    case Keys.N:
                        Input.SendKey(Keys.Down, isPress);
                        break;

                    case Keys.D:
                        Input.SendKey(Keys.Delete, isPress);
                        break;

                    case Keys.H:
                        Input.SendKey(Keys.Back, isPress);
                        break;

                    case Keys.E:
                        Input.SendKey(Keys.End, isPress);
                        break;

                    case Keys.A:
                        Input.SendKey(Keys.Home, isPress);
                        break;

                    default:
                        Input.SendKey(CurrentKey, isPress);
                        break;
                }              
                return;
            }
            else
            {
                Input.SendKey(CurrentKey, isPress);                
            }

            return;
        }
    }
}

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
    public class KeyPressArgs : EventArgs
    {
        public System.Windows.Input.Key Key;

        public KeyPressArgs(System.Windows.Input.Key key)
        {            
            Key = key;
        }
    }
}

public class KeyboardHook
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public enum Modifiers
    {
        None    = 0x0000,
        Alt     = 0x0001,
        Control = 0x0002,
        Shift   = 0x0004,
        Win     = 0x0008
    }

    int    modifier;
    int    key;
    IntPtr hWnd;
    int    id;

    public KeyboardHook(int modifiers, Keys key, Form f)
    {
        this.modifier = modifiers;
        this.key = (int)key;
        this.hWnd = f.Handle;
        id = this.GetHashCode();
    }

    public override int GetHashCode()
    {
        return modifier ^ key ^ hWnd.ToInt32();
    }


    public bool Register()
    {
        return RegisterHotKey(hWnd, id, modifier, key);
    }
    public bool Unregister()
    {
        return UnregisterHotKey(hWnd, id);
    }
}

public class KeyboardManager
{
    public const int INPUT_KEYBOARD = 1;
    public const int KEYEVENTF_KEYUP = 0x0002;

    public struct KEYDBINPUT
    {
        public Int16 wVk;
        public Int16 wScan;
        public Int32 dwFlags;
        public Int32 time;
        public Int32 dwExtraInfo;
        public Int32 __filler1;
        public Int32 __filler2;
    }

    public struct INPUT
    {
        public Int32 type;
        public KEYDBINPUT ki;
    }

    [DllImport("user32")]
    public static extern int SendInput(int cInputs, ref INPUT pInputs, int cbSize);

    public static void HoldKey(Keys vk)
    {
        INPUT input = new INPUT();
        input.type = INPUT_KEYBOARD;
        input.ki.dwFlags = 0;
        input.ki.wVk = (Int16)vk;
        SendInput(1, ref input, Marshal.SizeOf(input));
    }

    public static void ReleaseKey(Keys vk)
    {
        INPUT input = new INPUT();
        input.type = INPUT_KEYBOARD;
        input.ki.dwFlags = KEYEVENTF_KEYUP;
        input.ki.wVk = (Int16)vk;
        SendInput(1, ref input, Marshal.SizeOf(input));
    }

    public static void PressKey(Keys vk)
    {
        HoldKey(vk);
        ReleaseKey(vk);
    }
}
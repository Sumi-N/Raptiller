using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Raptiller
{
    public partial class RaptillerForm : Form
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        public struct tagKBDLLHOOKSTRUCT
        {
            public Int32 vkCode;
            public Int32 scanCode;
            public Int32 flags;
            public Int32 time;
            IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public event EventHandler<KeyPressArgs> OnKeyPressed;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public RaptillerForm()
        {
            InitializeComponent();

            MakeThisApplicationDeamon();

            this.Activated += new EventHandler(Form_Activated);

            _proc = HookCallback;
                        
            
            OnKeyPressed += _listener_OnKeyPressed;
            HookKeyboard();
        }

        ~RaptillerForm()
        {
            
        }

        private void Exit_Click(object sender, EventArgs e)
        {            
            Application.Exit();
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void MakeThisApplicationDeamon()
        {
            ShowInTaskbar = false;

            var menuItem = new ToolStripMenuItem();
            menuItem.Text = "&Exit";
            menuItem.Click += new EventHandler(Exit_Click);

            var menu = new ContextMenuStrip();
            menu.Items.Add(menuItem);

            var icon = new NotifyIcon();
            icon.Icon = new Icon("frog.ico");
            icon.Visible = true;
            icon.Text = "Raptiller";
            icon.ContextMenuStrip = menu;
        }

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == 0x0312)
        //    {
        //        HandleHotkey();
        //        int id = m.WParam.ToInt32();
        //        Console.WriteLine(string.Format("Hotkey #{0} pressed", id));
        //    }
        //    base.WndProc(ref m);
        //}

        //private void HandleHotkey()
        //{
        //    KeyboardManager.PressKey(Keys.Q);
        //}

        public void HookKeyboard()
        {
            _hookID = SetHook(_proc);
        }

        public void UnHookKeyboard()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            tagKBDLLHOOKSTRUCT keyStruct = (tagKBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(tagKBDLLHOOKSTRUCT));
            var isInjected = 0x01 & (keyStruct.flags >> 4);

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                if (isInjected == 0)
                {
                    KeyboardManager.HoldKey((Keys)keyStruct.vkCode);
                    return (System.IntPtr)1;
                }
                else
                {

                }
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
            {
                if (isInjected == 0)
                {
                    KeyboardManager.ReleaseKey((Keys)keyStruct.vkCode);
                    return (System.IntPtr)1;
                }
                else
                {
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        void _listener_OnKeyPressed(object sender, KeyPressArgs e)
        {
            Console.WriteLine(e.Key.ToString());
        }
    }
}

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
        private InputReceiver inputReceiver;

        public RaptillerForm()
        {
            InitializeComponent();

            ShowInTaskbar = false;

            var menuItem = new ToolStripMenuItem();
            menuItem.Text = "&Exit";
            menuItem.Click += new EventHandler(Exit_Click);

            var menu = new ContextMenuStrip();
            menu.Items.Add(menuItem);

            var icon = new NotifyIcon();
            //icon.Icon = new Icon("frog.ico");
            icon.Visible = true;
            icon.Text = "Raptiller";
            icon.ContextMenuStrip = menu;

            this.Activated += new EventHandler(Form_Activated);

            inputReceiver = new InputReceiver();
        }

        private void Exit_Click(object sender, EventArgs e)
        {            
            Application.Exit();
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}

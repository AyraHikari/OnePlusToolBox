using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace OnePlusToolBox
{
    public partial class Pairing : Form
    {
        public Pairing()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/k adb pair " + textBox1.Text;
            // process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.data_ip = textBox1.Text;
        }

        private void Pairing_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.data_ip;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/k adb pair " + textBox1.Text;
                // process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
                this.Close();
            }
        }
    }
}

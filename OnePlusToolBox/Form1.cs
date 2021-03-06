﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace OnePlusToolBox
{
    public partial class Form1 : Form
    {
        string AppVersion = "0.1.";
        int RevisionNumber = (int)(DateTime.UtcNow - new DateTime(2010, 1, 1)).TotalDays;
        int ADBrecommend = 41;

        string connectIP = "";
        bool isWirelessConnected = false;

        string deviceID = "null";
        string deviceDisplay = "";
        string deviceModel = "";
        string deviceName = "";
        string deviceMan = "";
        string deviceLabel = "";
        string deviceDName = "";
        int deviceSDK = 0;
        string deviceOS = "";
        string AndroidVersion = "";

        bool hasParalel = false;

        public string adb(string args)
        {
            Process process = new Process();
            process.StartInfo.FileName = "adb.exe";
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            try
            {
                process.Start();
            } catch
            {
                var confirmResult = MessageBox.Show("No ADB found!\nWant download latest?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    Process.Start("https://developer.android.com/studio/releases/platform-tools");
                }
                Environment.Exit(1);
            }
            string output = process.StandardOutput.ReadToEnd();
            //string err = process.StandardError.ReadToEnd();
            Console.Write(output);
            process.WaitForExit();
            return output;
        }
        public bool pair()
        {
            Pairing form2 = new Pairing();
            form2.ShowDialog();
            Application.Restart();
            return true;
        }

        public bool connect(string deviceIP = "")
        {
            if (DeviceList.Text == "No device found" & deviceIP == "") { return false; }
            if (deviceIP != "")
            {
                connectIP = deviceIP;
            }
            while (true)
            {
                if (deviceIP != "")
                {
                    var con = adb("connect " + deviceIP);
                    if (con.Contains("unable"))
                    {
                        var confirmResult = MessageBox.Show(con,
                            "Connection failed",
                            MessageBoxButtons.RetryCancel);
                        if (confirmResult == DialogResult.Cancel)
                        {
                            break;
                        }
                    }
                    else if (con.Contains("failed to connect"))
                    {
                        var confirmResult = MessageBox.Show("ADB need pair to your wireless device, click Yes to start pair",
                            "Need pair",
                            MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            pair();
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (DeviceList.Text == "No device found" & deviceIP != "") { DeviceList.Text = deviceIP; }
                var connectID = DeviceList.Text;
                if (deviceIP != "")
                {
                    connectID = deviceIP;
                }
                var devmodel = adb("-s " + connectID + " shell getprop ro.product.model");
                if (devmodel.Contains("error") | devmodel == "")
                {
                    var confirmResult = MessageBox.Show(devmodel,
                        "Connection failed",
                        MessageBoxButtons.RetryCancel);
                    if (confirmResult == DialogResult.Cancel)
                    {
                        break;
                    }
                }
                else
                {

                    try
                    {
                        deviceLabel = outsplit(devmodel);
                        var temp0 = adb("-s " + connectID + " shell getprop ro.display.series");
                        deviceDisplay = outsplit(temp0);
                        var temp01 = adb("-s " + connectID + " shell getprop ro.product.model");
                        deviceModel = outsplit(temp01);
                        var temp1 = adb("-s " + connectID + " shell getprop ro.product.manufacturer");
                        deviceMan = outsplit(temp1);
                        var temp2 = adb("-s " + connectID + " shell getprop ro.product.name");
                        deviceDName = outsplit(temp2);
                        var temp3 = adb("-s " + connectID + " shell getprop ro.product.build.version.sdk");
                        int sdk = 0;
                        Int32.TryParse(outsplit(temp3), out sdk);
                        deviceSDK = sdk;
                        var temp4 = adb("-s " + connectID + " shell getprop ro.oxygen.version");
                        deviceOS = outsplit(temp4);
                    }
                    catch { }
                    switch (deviceSDK)
                    {
                        case 0:
                            AndroidVersion = "Unsupported";
                            break;
                        case 28:
                            AndroidVersion = "Android 9";
                            break;
                        case 29:
                            AndroidVersion = "Android 10";
                            break;
                        case 30:
                            AndroidVersion = "Android 11";
                            break;
                        case 31:
                            AndroidVersion = "Android 12";
                            break;
                    }
                    textBox3.Text = deviceDisplay;
                    textBox4.Text = deviceModel;
                    textBox5.Text = deviceMan;
                    textBox6.Text = deviceDName;
                    textBox7.Text = AndroidVersion;
                    textBox8.Text = deviceOS;
                    deviceID = DeviceList.Text;
                    deviceName = " (" + outsplit(devmodel) + ")";
                    BotomDevice.Text = "Device connected: " + deviceID + " (" + outsplit(devmodel) + ")";
                    groupBox6.Enabled = true;
                    BotomDevice.ForeColor = Color.Green;
                    break;
                }
            }
            
            return true;
        }
    
        public bool refresh()
        {
            DeviceList.Items.Clear();
            var devices = adb("devices -l");
            var splited = devices.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string temp = "";
            foreach (string dev in splited)
            {
                var data = dev.Split(new string[] { "  " }, StringSplitOptions.None);
                if (data.Length >= 2)
                {
                    DeviceList.Items.Add(data[0]);
                    if (temp == "") { temp = data[0]; }
                }
            }
            if (DeviceList.Items.Count >= 1)
            {
                DeviceList.Text = temp;
                DeviceList.Enabled = true;
            }
            else
            {
                DeviceList.Text = "No device found";
                DeviceList.Enabled = false;
            }
            BotomDevice.Text = "Device connected: " + deviceID + deviceName;
            return true;
        }

        public string outsplit(string args)
        {
            var ret = args.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            return ret[0];
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void ConnectADB_Click(object sender, EventArgs e)
        {
            connect();
        }

        private void DeviceList_Load(object sender, EventArgs e)
        {
            this.Text = "OnePlus ToolBox (" + AppVersion + RevisionNumber.ToString() + ")";
            textBox2.Text = Properties.Settings.Default.data_ip;
            DeviceList.Items.Clear();
            // Check ADB version
            var ver = adb("version");
            var cur = ver.Split(new string[] { "version " }, StringSplitOptions.None);
            var curr = cur[1].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var currr = curr[0].Split(new string[] { "." }, StringSplitOptions.None);
            int release = 0;
            int beta = 0;
            int alpha = 0;
            Int32.TryParse(outsplit(currr[0]), out release);
            Int32.TryParse(outsplit(currr[1]), out beta);
            Int32.TryParse(outsplit(currr[2]), out alpha);
            if (alpha < ADBrecommend)
            {
                var confirmResult = MessageBox.Show("You are using ADB version " + curr[0] + ", it's recommend to use latest one.\nIf you're still using this one, some features may unavaiable.\n\nWant download latest?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                button12.Enabled = false;
                if (confirmResult == DialogResult.Cancel)
                {
                    this.Close();
                } else if (confirmResult == DialogResult.Yes)
                {
                    Process.Start("https://developer.android.com/studio/releases/platform-tools");
                }
            }
            refresh();
            connect();
        }

        private void RefreshADB_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                if (deviceID == "null") { }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string clog = "Console log:";
            clog += "\n> Refreshing...";
            PSlog.Text = clog;

            var chek = adb("shell pm list users");
            var ret = chek.Split(new string[] { "UserInfo{" }, StringSplitOptions.None);
            foreach (string ce in ret) { 
                if (ce.Contains("}")) { var usr = ce.Split(new string[] { "}" }, StringSplitOptions.None); var usrid = usr[0].Split(new string[] { ":" }, StringSplitOptions.None);
                    if (usrid[0] == "999") { hasParalel = true; } }
            }

            clog += "\n> Searching apps...";
            PSlog.Text = clog;
            var pak1 = adb("shell pm list packages");
            treeView1.Nodes.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            var spak1 = pak1.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            treeView1.Nodes.Add("Apps");
            foreach (string lpak1 in spak1)
            {
                var pakname1 = lpak1.Split(new string[] { "package:" }, StringSplitOptions.None);
                if (pakname1.Length > 1)
                {
                    treeView1.Nodes[0].Nodes.Add(pakname1[1]);
                    /*
                    var getact = adb("shell \"cmd package resolve-activity --brief " + pakname[1] + " | tail -n 1\"");
                    if (!getact.Contains("No activity found") & outsplit(getact) != "")
                    {
                        comboBox2.Items.Add(pakname[1]);
                    } */
                    comboBox2.Items.Add(pakname1[1]);
                }
            }

            if (hasParalel)
            {
                clog += "\n> Paralel storage detected";
                clog += "\n> Searching paralel apps...";
                PSlog.Text = clog;
                
                var pak = adb("shell pm list packages --user 999");
                var spak = pak.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                treeView1.Nodes.Add("Paralel Apps");
                foreach (string lpak in spak)
                {
                    var pakname = lpak.Split(new string[] { "package:" }, StringSplitOptions.None);
                    if (pakname.Length > 1)
                    {
                        treeView1.Nodes[1].Nodes.Add(pakname[1]);
                        /*
                        var getact = adb("shell \"cmd package resolve-activity --brief " + pakname[1] + " | tail -n 1\"");
                        if (!getact.Contains("No activity found") & outsplit(getact) != "")
                        {
                            comboBox2.Items.Add(pakname[1]);
                        }
                        */
                        comboBox3.Items.Add(pakname[1]);
                    }
                }
                button1.Enabled = true;
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
            } else
            {
                clog += "\n> Paralel storage not detected!";
            }

            clog += "\n> Everything was done!";
            PSlog.Text = clog;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string clog = "Console log:\n";
            clog += "> Deleting paralel storage...\n";
            PSlog.Text = clog;

            var is_success = adb("shell su -c \"pm remove-user 999\"");
            if (is_success.Contains("Error"))
            {
                clog += "> Failed, maybe already removed?";
                PSlog.Text = clog;
            } else
            {
                clog += "> " + is_success;
                PSlog.Text = clog;
            }
            button1.Enabled = false;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            connect(textBox2.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.data_ip = textBox2.Text;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                connect(textBox2.Text);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            adb("disconnect");
            MessageBox.Show("Disconnected everything!");
            BotomDevice.Text = "Device connected: null";
            BotomDevice.ForeColor = SystemColors.ControlText;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "adb.exe";
            process.StartInfo.Arguments = "-s " + deviceID + " shell";
            // process.StartInfo.UseShellExecute = false;
            process.Start();
        }

        private void PSlog_TextChanged(object sender, EventArgs e)
        {
            PSlog.SelectionStart = PSlog.Text.Length;
            PSlog.ScrollToCaret();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Install from App list") { return; }
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            
            string clog = "Console log:\n";
            clog += "> Pulling apps...\n";
            PSlog.Text = clog;

            var ret = adb("shell pm path " + comboBox2.Text);
            bool exists = System.IO.Directory.Exists("temp");
            if (exists) { System.IO.Directory.Delete(@"temp", true); }
            System.IO.Directory.CreateDirectory("temp");

            var pakname1 = ret.Split(new string[] { "package:" }, StringSplitOptions.None);
            if (pakname1.Length > 1)
            {
                foreach (string pack in pakname1)
                {
                    var pakfname = pack.Split(new string[] { "/" }, StringSplitOptions.None);
                    var o = adb("pull " + outsplit(pack) + " temp/" + outsplit(pakfname[pakfname.Length - 1]));
                }
            } else {
                clog += "> Failed, command failed (1)\n";
                PSlog.Text = clog;
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
                return;
            }

            clog += "> Installing...\n";
            PSlog.Text = clog;

            var apps = "";
            string[] apks = System.IO.Directory.GetFiles(@"temp/", "*.apk");
            foreach (string pack in apks) { apps += pack + " "; }
            var inst = adb("install-multiple --user 999 " + apps);
            System.IO.Directory.Delete(@"temp", true);
            clog += "> " + inst;
            PSlog.Text = clog;

            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "Select apk from paralel storage") { return; }
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            string clog = "Console log:";
            clog += "\n> Uninstalling " + comboBox3.Text + "...";
            PSlog.Text = clog;

            var ret = adb("shell pm uninstall --user 999 " + comboBox3.Text);
            clog += "\n> " + ret;
            PSlog.Text = clog;

            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
        }
    }
}

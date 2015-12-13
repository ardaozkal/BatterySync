using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace BatterySync
{
    public partial class Form1 : Form
    {
        string phpdirectory = "FILL THIS PLACE"; //http://yourserverhere.com/folder/sendbattery.php
        string devicename = ""; //Don't touch it.
        string devicenamedirectory = Directory.GetCurrentDirectory() + "\\devname.txt";
        WebClient wc = new WebClient();

        public Form1()
        {   
            InitializeComponent();

            if (phpdirectory == "FILL THIS PLACE")
            {
                timer1.Enabled = false;
                label1.Text = "Not working, change php link @line18@code.";
                MessageBox.Show("Please change the php placeholder to your website in code, at line 18.");
            }
            else
            {
                if (File.Exists(devicenamedirectory))
                {
                    devicename = File.ReadAllText(devicenamedirectory);
                    textBox1.Text = devicename;
                    timer1_Tick(null, null);
                }
                else
                {
                    label1.Text = "Not working, please set name.";
                    MessageBox.Show("No devicename found. The app will not work without one. Please write the devicename into the box and click set name.");
                    timer1.Enabled = false;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                var texttosend = "{chargestatus}: {batterypercent} percent";
                foreach (ManagementObject mo in new ManagementObjectSearcher(new ObjectQuery("Select * FROM Win32_Battery")).Get())
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        if (property.Name == "BatteryStatus")
                        {
                            texttosend = texttosend.Replace("{chargestatus}", (((UInt16)property.Value == 2) ? "Charging" : "Not charging"));
                        }
                        if (property.Name == "EstimatedChargeRemaining")
                        {
                            texttosend = texttosend.Replace("{batterypercent}", property.Value.ToString());
                        }
                    }
                }
                label1.Text = "Battery Status: " + texttosend;
                // v SPAGHETTI CODE RIP RAM. Y I NO DISPOSE.
                wc.DownloadString(phpdirectory + "?devicename=" + devicename + "&content=" + texttosend);
            }
            catch
            {

            }
        }

        List<string> bannedchars = new List<string> { " ", ".", "{", "}" , "/", "\\", ":", "*", "?", "\"", ">", "<", "|" };

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (string bannedchar in bannedchars)
            {
                textBox1.Text.Replace(bannedchar, "");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText(devicenamedirectory, textBox1.Text);
            devicename = textBox1.Text;
            timer1.Enabled = true;
            timer1_Tick(null, null);
        }
    }
}

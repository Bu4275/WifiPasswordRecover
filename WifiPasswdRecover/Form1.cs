using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
namespace WifiPasswdRecover
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private string ExcuteCmd(object args)
        {
            string result = "";
            ProcessStartInfo start_info =
                            new ProcessStartInfo("cmd.exe", args.ToString());
            start_info.UseShellExecute = false;
            start_info.CreateNoWindow = true;
            start_info.RedirectStandardOutput = true;
            start_info.RedirectStandardError = true;

            // Make the process and set its start information.
            using (Process proc = new Process())
            {
                // Start the process.
                proc.StartInfo = start_info;
                proc.Start();
                result = proc.StandardOutput.ReadToEnd();
                proc.Close();
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Get SSID
            List<string> SSIDList = new List<string>();
            string info = ExcuteCmd("/c netsh wlan show profile");
            Regex rex = new Regex(@"所有使用者設定檔 : [\w!@#$%^&*()_+-]*");
            MatchCollection matches = rex.Matches(info);
            foreach (Match match in matches)
            {
                string SSID = match.ToString().Split(' ')[2];
                Console.Write(SSID + "\n");
                SSIDList.Add(SSID);
            }

            //Find Password
            info = "";
            foreach (string SSID in SSIDList)
            {
                string passwd = ExcuteCmd("/c netsh wlan show profile name=\"" + SSID + "\" key=clear |findstr \"金鑰內容 key\"");
                if (!passwd.Equals(""))
                {
                    info += string.Format("{0}\r\n{1}", SSID, passwd);
                }
            }
            textBox1.Text = info;

        }
    }
}

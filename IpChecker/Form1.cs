using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Timers;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace IpChecker {
    public partial class Form1 : Form {
        private System.Timers.Timer timer;
        public Form1() {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);

            timer = new System.Timers.Timer(5000);
            timer.Elapsed += OnTimedEvent_Tick;
            timer.AutoReset = true;
            timer.Enabled = true;

            // Initialize DataGridView
            dgvStatus.Columns.Add("IP", "IP/Website");
            dgvStatus.Columns.Add("Status", "Status");
        }


        private void btnAdd_Click(object sender, EventArgs e) {
            // Add IP/Website to DataGridView
            dgvStatus.Rows.Add(txtUrl.Text, "Checking...");
            
        }

        private void OnTimedEvent_Tick(object source, ElapsedEventArgs e) {
            // Check online status for each IP/Website
            foreach (DataGridViewRow row in dgvStatus.Rows) {
                string ipOrWebsite = (string)row.Cells["IP"].Value;
                if (!string.IsNullOrEmpty(ipOrWebsite)) {
                    bool isOnline = PingHost(ipOrWebsite);
                    string status = isOnline ? "Online" : "Offline";

                    // Use Invoke to update the DataGridView on the UI thread
                    this.Invoke((MethodInvoker)delegate {
                        row.Cells["Status"].Value = status;
                    });
                }
            }
        }


        private bool PingHost(string nameOrAddress) {
            bool pingable = false;
            Ping pinger = null;

            try {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException) {
                // Discard PingExceptions and return false;
            }
            finally {
                if (pinger != null) {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
            var psi = new ProcessStartInfo {
                FileName = "https://www.instagram.com/developing_off/",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            var lines = dgvStatus.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Cells["IP"].Value != null)
                .Select(r => r.Cells["IP"].Value.ToString())
                .ToArray();
            File.WriteAllLines("ips.txt", lines);
        }

        private void Form1_Load(object sender, EventArgs e) {
            if (File.Exists("ips.txt")) {
                var lines = File.ReadAllLines("ips.txt");
                foreach (var line in lines) {
                    dgvStatus.Rows.Add(line, "Checking...");
                }
            }
        }




    }
}

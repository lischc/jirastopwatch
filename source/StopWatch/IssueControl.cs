﻿/**************************************************************************
Copyright 2015 Carsten Gehling

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**************************************************************************/
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StopWatch
{
    internal class IssueControl : UserControl
    {
        public string JiraBaseUrl { get; set; }

        public string IssueKey
        {
            get
            {
                return tbJira.Text;
            }

            set
            {
                tbJira.Text = value;
                UpdateSummary();
            }
        }

        public event EventHandler TimerStarted;

        #region public methods
        public IssueControl(JiraClient jiraClient)
            : base()
        {
            InitializeComponent();

            this.jiraClient = jiraClient;
            this.watchTimer = new WatchTimer();
        }


        public void UpdateOutput()
        {
            tbTime.Text = TimeSpanToJiraTime(watchTimer.TimeElapsed);

            if (watchTimer.Running)
            {
                btnStartStop.Text = "Pause";
                tbTime.BackColor = Color.PaleGreen;
            }
            else {
                btnStartStop.Text = "Start";
                tbTime.BackColor = SystemColors.Control;
            }
        }


        public void Pause()
        {
            watchTimer.Pause();
            UpdateOutput();
        }
        #endregion


        #region private methods
        private string TimeSpanToJiraTime(TimeSpan ts)
        {
            if (ts.Hours > 0)
                return String.Format("{0:%h}h {0:%m}m", ts);

            return String.Format("{0:%m}m", ts);
        }


        private void OpenJira(string issue)
        {
            if (jiraClient == null)
                return;
            if (tbJira.Text == "")
                return;

            jiraClient.OpenIssueInBrowser(tbJira.Text);
        }


        private void UpdateSummary()
        {
            lblSummary.Text = "";

            if (jiraClient == null)
                return;
            if (tbJira.Text == "")
                return;

            Task.Factory.StartNew(
                () => {
                    string key = "";
                    string summary = "";
                    this.Invoke(new Action(
                        () => key = tbJira.Text
                    ));
                    summary = jiraClient.GetIssueSummary(tbJira.Text);
                    this.Invoke(new Action(
                        () => lblSummary.Text = summary
                    ));
                }
            );
        }


        private void InitializeComponent()
        {
            this.tbJira = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.tbTime = new System.Windows.Forms.TextBox();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblSummary = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbJira
            // 
            this.tbJira.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbJira.Location = new System.Drawing.Point(0, 0);
            this.tbJira.Name = "tbJira";
            this.tbJira.Size = new System.Drawing.Size(155, 28);
            this.tbJira.TabIndex = 0;
            this.tbJira.Leave += new System.EventHandler(this.tbJira_Leave);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(156, 0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(58, 28);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // tbTime
            // 
            this.tbTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTime.Location = new System.Drawing.Point(250, 0);
            this.tbTime.Name = "tbTime";
            this.tbTime.Size = new System.Drawing.Size(95, 28);
            this.tbTime.TabIndex = 2;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(346, 0);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(58, 28);
            this.btnStartStop.TabIndex = 3;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(444, 0);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(58, 28);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblSummary
            // 
            this.lblSummary.Location = new System.Drawing.Point(0, 29);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(503, 23);
            this.lblSummary.TabIndex = 5;
            // 
            // IssueControl
            // 
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.tbTime);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.tbJira);
            this.Name = "IssueControl";
            this.Size = new System.Drawing.Size(503, 55);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        #region private eventhandlers
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text == "Start") {
                this.watchTimer.Start();

                if (this.TimerStarted != null)
                    this.TimerStarted(this, e);
            }
            else {
                this.watchTimer.Pause();
            }
            UpdateOutput();
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            this.watchTimer.Reset();
            UpdateOutput();
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenJira(tbJira.Text);
        }


        private void tbJira_Leave(object sender, EventArgs e)
        {
            UpdateSummary();
        }
        #endregion


        #region private members
        private TextBox tbJira;
        private Button btnOpen;
        private TextBox tbTime;
        private Button btnStartStop;
        private Button btnReset;
        private Label lblSummary;

        private WatchTimer watchTimer;

        private JiraClient jiraClient;
        #endregion
    }
}
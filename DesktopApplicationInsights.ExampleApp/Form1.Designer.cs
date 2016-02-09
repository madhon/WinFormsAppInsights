namespace DesktopApplicationInsights.ExampleApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.telemetryButton1 = new DesktopApplicationInsights.TelemetryButton();
            this.SuspendLayout();
            // 
            // telemetryButton1
            // 
            this.telemetryButton1.EventName = null;
            this.telemetryButton1.IsTimed = false;
            this.telemetryButton1.Location = new System.Drawing.Point(56, 51);
            this.telemetryButton1.Name = "telemetryButton1";
            this.telemetryButton1.Size = new System.Drawing.Size(130, 34);
            this.telemetryButton1.TabIndex = 0;
            this.telemetryButton1.TelemetryClientName = null;
            this.telemetryButton1.Text = "telemetryButton1";
            this.telemetryButton1.UseVisualStyleBackColor = true;
            this.telemetryButton1.Click += new System.EventHandler(this.telemetryButton1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 338);
            this.Controls.Add(this.telemetryButton1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private TelemetryButton telemetryButton1;
    }
}


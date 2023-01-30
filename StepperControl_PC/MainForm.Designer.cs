namespace StepperControl_PC
{
    partial class MainForm
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
            this.txt_log = new System.Windows.Forms.TextBox();
            this.panel_motorsControl = new System.Windows.Forms.Panel();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_connect_serial_port = new System.Windows.Forms.Button();
            this.btn_UpdatePorts = new System.Windows.Forms.Button();
            this.cmb_serial_ports = new System.Windows.Forms.ComboBox();
            this.txt_command_code = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.pb_connection = new System.Windows.Forms.PictureBox();
            this.Label26 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.pb_power = new System.Windows.Forms.PictureBox();
            this.Label28 = new System.Windows.Forms.Label();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_connection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_power)).BeginInit();
            this.SuspendLayout();
            // 
            // txt_log
            // 
            this.txt_log.Dock = System.Windows.Forms.DockStyle.Right;
            this.txt_log.Location = new System.Drawing.Point(637, 0);
            this.txt_log.Margin = new System.Windows.Forms.Padding(2);
            this.txt_log.Multiline = true;
            this.txt_log.Name = "txt_log";
            this.txt_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_log.Size = new System.Drawing.Size(282, 964);
            this.txt_log.TabIndex = 52;
            // 
            // panel_motorsControl
            // 
            this.panel_motorsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel_motorsControl.AutoScroll = true;
            this.panel_motorsControl.BackColor = System.Drawing.SystemColors.Control;
            this.panel_motorsControl.Location = new System.Drawing.Point(12, 110);
            this.panel_motorsControl.MinimumSize = new System.Drawing.Size(620, 600);
            this.panel_motorsControl.Name = "panel_motorsControl";
            this.panel_motorsControl.Size = new System.Drawing.Size(621, 842);
            this.panel_motorsControl.TabIndex = 54;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.btn_connect_serial_port);
            this.GroupBox1.Controls.Add(this.btn_UpdatePorts);
            this.GroupBox1.Controls.Add(this.cmb_serial_ports);
            this.GroupBox1.Controls.Add(this.txt_command_code);
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.pb_connection);
            this.GroupBox1.Controls.Add(this.Label26);
            this.GroupBox1.Controls.Add(this.Label22);
            this.GroupBox1.Controls.Add(this.pb_power);
            this.GroupBox1.Controls.Add(this.Label28);
            this.GroupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox1.Location = new System.Drawing.Point(11, 11);
            this.GroupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.GroupBox1.Size = new System.Drawing.Size(622, 94);
            this.GroupBox1.TabIndex = 53;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "SERIAL CONNECTION";
            // 
            // btn_connect_serial_port
            // 
            this.btn_connect_serial_port.Location = new System.Drawing.Point(20, 58);
            this.btn_connect_serial_port.Margin = new System.Windows.Forms.Padding(2);
            this.btn_connect_serial_port.Name = "btn_connect_serial_port";
            this.btn_connect_serial_port.Size = new System.Drawing.Size(162, 24);
            this.btn_connect_serial_port.TabIndex = 7;
            this.btn_connect_serial_port.Text = "CONNECT";
            this.btn_connect_serial_port.UseVisualStyleBackColor = true;
            this.btn_connect_serial_port.Click += new System.EventHandler(this.btn_connect_serial_port_Click_1);
            // 
            // btn_UpdatePorts
            // 
            this.btn_UpdatePorts.Location = new System.Drawing.Point(186, 30);
            this.btn_UpdatePorts.Margin = new System.Windows.Forms.Padding(2);
            this.btn_UpdatePorts.Name = "btn_UpdatePorts";
            this.btn_UpdatePorts.Size = new System.Drawing.Size(86, 52);
            this.btn_UpdatePorts.TabIndex = 4;
            this.btn_UpdatePorts.Text = "UPDATE PORTS";
            this.btn_UpdatePorts.UseVisualStyleBackColor = true;
            this.btn_UpdatePorts.Click += new System.EventHandler(this.btn_UpdatePorts_Click);
            // 
            // cmb_serial_ports
            // 
            this.cmb_serial_ports.FormattingEnabled = true;
            this.cmb_serial_ports.Location = new System.Drawing.Point(91, 30);
            this.cmb_serial_ports.Margin = new System.Windows.Forms.Padding(2);
            this.cmb_serial_ports.Name = "cmb_serial_ports";
            this.cmb_serial_ports.Size = new System.Drawing.Size(92, 25);
            this.cmb_serial_ports.TabIndex = 3;
            // 
            // txt_command_code
            // 
            this.txt_command_code.Enabled = false;
            this.txt_command_code.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_command_code.Location = new System.Drawing.Point(559, 43);
            this.txt_command_code.Margin = new System.Windows.Forms.Padding(2);
            this.txt_command_code.Name = "txt_command_code";
            this.txt_command_code.Size = new System.Drawing.Size(54, 20);
            this.txt_command_code.TabIndex = 49;
            this.txt_command_code.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(16, 32);
            this.Label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(78, 17);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "Serial Port:";
            // 
            // pb_connection
            // 
            this.pb_connection.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pb_connection.Location = new System.Drawing.Point(559, 24);
            this.pb_connection.Margin = new System.Windows.Forms.Padding(2);
            this.pb_connection.Name = "pb_connection";
            this.pb_connection.Size = new System.Drawing.Size(52, 15);
            this.pb_connection.TabIndex = 28;
            this.pb_connection.TabStop = false;
            // 
            // Label26
            // 
            this.Label26.AutoSize = true;
            this.Label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label26.Location = new System.Drawing.Point(472, 24);
            this.Label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label26.Name = "Label26";
            this.Label26.Size = new System.Drawing.Size(59, 13);
            this.Label26.TabIndex = 18;
            this.Label26.Text = "Connected";
            // 
            // Label22
            // 
            this.Label22.AutoSize = true;
            this.Label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label22.Location = new System.Drawing.Point(473, 46);
            this.Label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(82, 13);
            this.Label22.TabIndex = 22;
            this.Label22.Text = "Command Code";
            // 
            // pb_power
            // 
            this.pb_power.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pb_power.Location = new System.Drawing.Point(559, 67);
            this.pb_power.Margin = new System.Windows.Forms.Padding(2);
            this.pb_power.Name = "pb_power";
            this.pb_power.Size = new System.Drawing.Size(52, 15);
            this.pb_power.TabIndex = 29;
            this.pb_power.TabStop = false;
            // 
            // Label28
            // 
            this.Label28.AutoSize = true;
            this.Label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label28.Location = new System.Drawing.Point(472, 67);
            this.Label28.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(37, 13);
            this.Label28.TabIndex = 27;
            this.Label28.Text = "Power";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 964);
            this.Controls.Add(this.panel_motorsControl);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.txt_log);
            this.MinimumSize = new System.Drawing.Size(920, 950);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_connection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_power)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.TextBox txt_log;
        internal System.Windows.Forms.Panel panel_motorsControl;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Button btn_connect_serial_port;
        internal System.Windows.Forms.Button btn_UpdatePorts;
        internal System.Windows.Forms.ComboBox cmb_serial_ports;
        internal System.Windows.Forms.TextBox txt_command_code;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.PictureBox pb_connection;
        internal System.Windows.Forms.Label Label26;
        internal System.Windows.Forms.Label Label22;
        internal System.Windows.Forms.PictureBox pb_power;
        internal System.Windows.Forms.Label Label28;
    }
}


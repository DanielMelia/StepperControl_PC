using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace StepperControl_PC
{
    public partial class MainForm : Form
    {
        DUETimer_StteperArray myMotorArray;
        Timer timer_StatusUpdate;

        public MainForm()
        {
            InitializeComponent();

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            myMotorArray = new DUETimer_StteperArray();
            myMotorArray.InitialiseMotors();


            UpdateAvailablePorts();
            timer_StatusUpdate = new Timer();
            timer_StatusUpdate.Tick += InternalTimer_Tick;
            timer_StatusUpdate.Interval = 500;
            timer_StatusUpdate.Start();

            MotorControlPanel testMotorControlPanel0 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 0);
            testMotorControlPanel0.CreateMotorControlGroupBox(new Point(4, 4));
            MotorControlPanel testMotorControlPanel1 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 1);
            testMotorControlPanel1.CreateMotorControlGroupBox(new Point(4, 119));

            MotorControlPanel testMotorControlPanel2 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 2);
            testMotorControlPanel2.CreateMotorControlGroupBox(new Point(4, 234));
            MotorControlPanel testMotorControlPanel3 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 3);
            testMotorControlPanel3.CreateMotorControlGroupBox(new Point(4, 349));
            MotorControlPanel testMotorControlPanel4 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 4);
            testMotorControlPanel4.CreateMotorControlGroupBox(new Point(4, 464));
            MotorControlPanel testMotorControlPanel5 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 5);
            testMotorControlPanel5.CreateMotorControlGroupBox(new Point(4, 579));
            MotorControlPanel testMotorControlPanel6 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 6);
            testMotorControlPanel6.CreateMotorControlGroupBox(new Point(4, 694)); 
            MotorControlPanel testMotorControlPanel7 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 7);
            testMotorControlPanel7.CreateMotorControlGroupBox(new Point(4, 809));
            MotorControlPanel testMotorControlPanel8 = new MotorControlPanel(ref myMotorArray, ref panel_motorsControl, 8);
            testMotorControlPanel8.CreateMotorControlGroupBox(new Point(4, 924));

            MotorControlPanel.UpdatePanelsStatus();
            MotorControlPanel.TimerStart();


        }

        private void InternalTimer_Tick(object sender, EventArgs e)
        {
            Log.DisplayLog(ref txt_log);

            if (myMotorArray.IsConnected)
                pb_connection.BackColor = Color.Green;
            else
                pb_connection.BackColor = Color.Red;

            txt_command_code.Text = myMotorArray.CommandCode.ToString();

            MotorControlPanel.UpdatePanelsStatus();

        }

        private void UpdateAvailablePorts()
        {
            cmb_serial_ports.Enabled = true;
            cmb_serial_ports.Items.Clear();

            string[] ports = SerialPort.GetPortNames();

            foreach(string port in ports)
            {
                cmb_serial_ports.Items.Add(port);
            }

            if (cmb_serial_ports.Items.Count > 0)
                cmb_serial_ports.SelectedIndex = 0;
        }

        private async void btn_connect_serial_port_Click(object sender, EventArgs e)
        {
            if (myMotorArray.IsConnected)
                return;

            string portName = cmb_serial_ports.SelectedItem.ToString();
            await myMotorArray.EstablishSerialConnection(portName);
            InititaliseTestMotor();

        }

        private async void InititaliseTestMotor()
        {
            myMotorArray.SetMotorName(0, "motor0");
            await myMotorArray.SetAxisParameters(0,53,51,49,35,37,1000,1,100);
            myMotorArray.InitialiseMotionParameters(0, 10, 50, 50);
        }

        private async void btn_connect_serial_port_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (myMotorArray.IsConnected)
                    return;
                if (cmb_serial_ports.SelectedIndex >= 0)
                {
                    string portName = cmb_serial_ports.SelectedItem.ToString();
                    await myMotorArray.EstablishSerialConnection(portName);
                    InititaliseTestMotor();
                }
                else
                {
                    MessageBox.Show("No Port Selected!");
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }

        private void btn_UpdatePorts_Click(object sender, EventArgs e)
        {
            UpdateAvailablePorts();
        }

    }
}

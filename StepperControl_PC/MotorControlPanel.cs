using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StepperControl_PC
{
    internal class MotorControlPanel
    {
        public int motor_id;
        private Panel parentPanel;


        private GroupBox myGB;  // Default : Size(801,139)

        // Buttons
        private Button btn_MovelVel_Neg;  // Default : Location(15,32) , Size(53,49)
        private Button btn_MovelVel_Pos;  // Default : Location(281,32) , Size(53,49)
        private Button btn_MovelStep_Neg;  // Default : Location(74,32) , Size(53,49)
        private Button btn_MovelStep_Pos;  // Default : Location(219,32) , Size(53,49)
        private Button btn_MovelAbs;  // Default : Location(510,22) , Size(64,56)
        private Button btn_Stop;  // Default : Location(510,22) , Size(64,56)
        private Button btn_Home;  // Default : Location(704,22) , Size(85,56)

        // Labels
        private Label lbl_step_size;
        private Label lbl_absTrgPos;
        private Label lbl_vel_mms;

        private Label lbl_currentPos;
        private Label lbl_TrgPos;
        private Label lbl_ControlMode;

        private Label lbl_LimSw1;
        private Label lbl_LimSw2;

        // TextBoxes
        private TextBox txt_step_um;
        private TextBox txt_absTrgPos_mm;
        private TextBox txt_currentPos;
        private TextBox txt_TrgPos;
        private TextBox txt_ControlMode;

        // Numeric Up and Down
        private NumericUpDown nud_vel_mms;

        // Picture Boxes
        private PictureBox pb_IsInit;
        private PictureBox pb_IsHomed;

        private PictureBox pb_LimSw1;
        private PictureBox pb_LimSw2;

        public static System.Windows.Forms.Timer myTimer;
        private static List<MotorControlPanel> TotalPanels;
        private static DUETimer_StteperArray myMotorArray;

        public MotorControlPanel(ref DUETimer_StteperArray motorArray,ref Panel parent, int motor_id)
        {
            parentPanel = parent;
            this.motor_id = motor_id;
            myMotorArray = motorArray;
            if (TotalPanels == null)
                TotalPanels = new List<MotorControlPanel>();
            TotalPanels.Add(this);
        }

        public GroupBox CreateMotorControlGroupBox(Point myLoc)
        {
            Font Font10p_Reg = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
            Font Font10p_Bold = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            Font Font8p_Reg = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

            // Initialise Move Buttons
            Image image_MovelVelNeg = Image.FromFile(System.IO.Directory.GetCurrentDirectory() + @"\Images\arrow_double_left.png");
            btn_MovelVel_Neg = new Button() { Location = new Point(11, 26), Size = new Size(40, 40), BackgroundImage = image_MovelVelNeg, BackgroundImageLayout = ImageLayout.Stretch };
            btn_MovelVel_Neg.Click += MoveVelNeg;

            Image image_MovelVelPos = Image.FromFile(System.IO.Directory.GetCurrentDirectory() + @"\Images\arrow_double_right.png");
            btn_MovelVel_Pos = new Button() { Location = new Point(211, 26), Size = new Size(40, 40), BackgroundImage = image_MovelVelPos, BackgroundImageLayout = ImageLayout.Stretch };
            btn_MovelVel_Pos.Click += MoveVelPos;

            Image image_MovelStepNeg = Image.FromFile(System.IO.Directory.GetCurrentDirectory() + @"\Images\arrow_single_left.png");
            btn_MovelStep_Neg = new Button() { Location = new Point(56, 26), Size = new Size(40, 40), BackgroundImage = image_MovelStepNeg, BackgroundImageLayout = ImageLayout.Stretch };
            btn_MovelStep_Neg.Click += MoveStepNeg;

            Image image_MovelStepPos = Image.FromFile(System.IO.Directory.GetCurrentDirectory() + @"\Images\arrow_single_right.png");
            btn_MovelStep_Pos = new Button() { Location = new Point(164, 26), Size = new Size(40, 40), BackgroundImage = image_MovelStepPos, BackgroundImageLayout = ImageLayout.Stretch };
            btn_MovelStep_Pos.Click += MoveStepPos;

            // Initialise Home Button
            btn_Home = new Button() { Location = new Point(528, 18), Size = new Size(64, 46), Text = "HOME", Font = Font10p_Bold };
            btn_Home.Click += HomeMotor;

            // Initialise Absolute Position Button
            btn_MovelAbs = new Button() { Location = new Point(382, 18), Size = new Size(48, 46), Text = "GO", Font = Font10p_Bold };
            btn_MovelAbs.Click += MoveAbs;

            // Initialise Stop Button
            btn_Stop = new Button() { Location = new Point(436, 18), Size = new Size(64, 46), Text = "STOP", Font = Font10p_Bold };
            btn_Stop.Click += StopMotor;

            txt_step_um = new TextBox() { Text = "100", Font = Font10p_Reg, Location = new Point(100, 43), Size = new Size(60, 23) };

            txt_absTrgPos_mm = new TextBox() { Text = "0", Font = Font10p_Reg, Location = new Point(269, 41), Size = new Size(102, 23) };

            nud_vel_mms = new NumericUpDown() { Font = Font10p_Reg, Location = new Point(119, 76), Size = new Size(85, 23), Maximum = 30 };

            lbl_step_size = new Label() { Text = "Step(um)", Font = Font10p_Reg, Location = new Point(96, 22), AutoSize = false, Size = new Size(68, 20) };

            lbl_absTrgPos = new Label() { Text = "Target Pos (um)", Font = Font10p_Reg, Location = new Point(266, 18), Size = new Size(111, 20) };

            lbl_vel_mms = new Label() { Text = "Velocity (mm/s):", Font = Font10p_Reg, Location = new Point(10, 77), AutoSize = false, Size = new Size(109, 19) };

            pb_IsInit = new PictureBox() { Location = new Point(326, 90), Size = new Size(14, 15), BackColor = SystemColors.ControlDark };
            pb_IsHomed = new PictureBox() { Location = new Point(345, 90), Size = new Size(14, 15), BackColor = SystemColors.ControlDark };
            pb_LimSw1 = new PictureBox() { Location = new Point(528, 90), Size = new Size(26, 15), BackColor = SystemColors.ControlDark };
            pb_LimSw2 = new PictureBox() { Location = new Point(566, 90), Size = new Size(26, 15), BackColor = SystemColors.ControlDark };


            txt_currentPos = new TextBox() { Font = Font8p_Reg, Location = new Point(363, 88), Size = new Size(48, 20), Enabled = false };

            txt_TrgPos = new TextBox() { Font = Font8p_Reg, Location = new Point(414, 88), Size = new Size(48, 20), Enabled = false };

            txt_ControlMode = new TextBox() { Font = Font8p_Reg, Location = new Point(465, 88), Size = new Size(48, 20), Enabled = false };


            lbl_currentPos = new Label() { Text = "Pos", Font = Font8p_Reg, Location = new Point(363, 72), AutoSize = false, Size = new Size(46, 14) };
            lbl_TrgPos = new Label() { Text = "Trg", Font = Font8p_Reg, Location = new Point(414, 72), AutoSize = false, Size = new Size(46, 14) };
            lbl_ControlMode = new Label() { Text = "Mode", Font = Font8p_Reg, Location = new Point(465, 72), AutoSize = false, Size = new Size(46, 14) };

            lbl_LimSw1 = new Label() { Text = "LS1", Font = Font8p_Reg, Location = new Point(528, 74), AutoSize = false, Size = new Size(26, 14) };
            lbl_LimSw2 = new Label() { Text = "LS2", Font = Font8p_Reg, Location = new Point(566, 74), AutoSize = false, Size = new Size(26, 14) };

            // Create GroupBox
            myGB = new GroupBox();
            myGB.Size = new Size(601, 113);
            myGB.Text = "Motor " + motor_id.ToString() + " Control";
            myGB.Location = myLoc;
            myGB.Controls.Add(btn_Home);
            myGB.Controls.Add(btn_MovelAbs);
            myGB.Controls.Add(btn_Stop);
            myGB.Controls.Add(btn_MovelVel_Neg);
            myGB.Controls.Add(btn_MovelVel_Pos);
            myGB.Controls.Add(btn_MovelStep_Neg);
            myGB.Controls.Add(btn_MovelStep_Pos);

            myGB.Controls.Add(txt_step_um);
            myGB.Controls.Add(txt_absTrgPos_mm);
            myGB.Controls.Add(nud_vel_mms);
            myGB.Controls.Add(lbl_step_size);
            myGB.Controls.Add(lbl_absTrgPos);
            myGB.Controls.Add(lbl_vel_mms);

            myGB.Controls.Add(pb_IsInit);
            myGB.Controls.Add(pb_IsHomed);
            myGB.Controls.Add(txt_currentPos);
            myGB.Controls.Add(txt_TrgPos);
            myGB.Controls.Add(txt_ControlMode);

            myGB.Controls.Add(lbl_currentPos);
            myGB.Controls.Add(lbl_TrgPos);
            myGB.Controls.Add(lbl_ControlMode);

            myGB.Controls.Add(pb_LimSw1);
            myGB.Controls.Add(pb_LimSw2);

            myGB.Controls.Add(lbl_LimSw1);
            myGB.Controls.Add(lbl_LimSw2);


            parentPanel.Controls.Add(myGB);

            return myGB;
        }


        private async void HomeMotor(object sender, EventArgs e)
        {
            btn_Home.Enabled = false;
            await myMotorArray.HomeMotor(motor_id, 1);
            btn_Home.Enabled = true;

        }

        private async void MoveAbs(object sender, EventArgs e)
        {
            try
            {
                double trgPos_mm;
                if (!double.TryParse(txt_absTrgPos_mm.Text, out trgPos_mm))
                {
                    MessageBox.Show("Error Reading Target Position!"); return;
                }
                if (trgPos_mm < 0 | trgPos_mm > myMotorArray.Motors[motor_id].max_travel_range)
                    MessageBox.Show("Target Position Out Of Limits!");

                btn_MovelAbs.Enabled = false;
                await myMotorArray.MoveToAbsolutePosition(motor_id, trgPos_mm);
                btn_MovelAbs.Enabled = true;
            }
            catch (Exception ex)
            {
            }
        }

        private void StopMotor(object sender, EventArgs e)
        {
            myMotorArray.StopAxis(motor_id, 0);
        }

        private async void MoveStepNeg(object sender, EventArgs e)
        {
            try
            {
                double step_um;
                if (!double.TryParse(txt_step_um.Text, out step_um))
                {
                    MessageBox.Show("Wrong Step um parameter!"); return;
                }
                double step_mm = step_um / 1000;
                if (step_mm <= 0)
                {
                    MessageBox.Show("Select a step size higher than 0"); return;
                }

                btn_MovelStep_Neg.Enabled = false;
                await myMotorArray.MoveRelativeDistance(motor_id, -step_mm);
                btn_MovelStep_Neg.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void MoveStepPos(object sender, EventArgs e)
        {
            try
            {
                double step_um;
                if (!double.TryParse(txt_step_um.Text, out step_um))
                {
                    MessageBox.Show("Error Reading Step_um parameter!"); return;
                }
                double step_mm = step_um / 1000;
                if (step_mm <= 0)
                {
                    MessageBox.Show("Select a step size higher than 0"); return;
                }

                btn_MovelStep_Pos.Enabled = false;
                await myMotorArray.MoveRelativeDistance(motor_id, step_mm);
                btn_MovelStep_Pos.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MoveVelNeg(object sender, EventArgs e)
        {
            double trg_vel_mms = (double)nud_vel_mms.Value;
            if (trg_vel_mms <= 0)
            {
                MessageBox.Show("Select a velocity higher than 0"); return;
            }
            if (myMotorArray.Motors[motor_id].CurrentControlMode == MotorConfig.MotorControlMode.STP)
                myMotorArray.MoveSpeedMode(motor_id, false, trg_vel_mms);
            else
                myMotorArray.StopAxis(motor_id, 0);
        }

        private void MoveVelPos(object sender, EventArgs e)
        {
            double trg_vel_mms = (double)nud_vel_mms.Value;
            if (trg_vel_mms <= 0)
            {
                MessageBox.Show("Select a velocity higher than 0"); return;
            }
            if (myMotorArray.Motors[motor_id].CurrentControlMode == MotorConfig.MotorControlMode.STP)
                myMotorArray.MoveSpeedMode(motor_id, true, trg_vel_mms);
            else
                myMotorArray.StopAxis(motor_id, 0);
        }

        public static void UpdatePanelsStatus()
        {
            if (TotalPanels == null)
                return;
            if (TotalPanels.Count <= 0)
                return;

            foreach (var mcp in TotalPanels)
            {
                if (myMotorArray.Motors[mcp.motor_id].IsInit)
                {
                    mcp.myGB.Enabled = true; mcp.myGB.Text = myMotorArray.Motors[mcp.motor_id].Name + " Control";
                }
                else
                {
                    mcp.myGB.Enabled = false; mcp.myGB.Text = myMotorArray.Motors[mcp.motor_id].Name;
                }
            }
        }

        public static void TimerStart(int interval = 100)
        {
            myTimer = new Timer();
            myTimer.Interval = interval;
            myTimer.Tick += timer_FullStatusUpdate;
            myTimer.Start();
        }

        private static void timer_FullStatusUpdate(object sender, EventArgs e)
        {
            if (TotalPanels == null)
                return;
            if (TotalPanels.Count <= 0)
                return;

            foreach (var mcp in TotalPanels)
            {
                if (myMotorArray.Motors[mcp.motor_id].IsInit)
                    mcp.pb_IsInit.BackColor = Color.Green;
                else
                    mcp.pb_IsInit.BackColor = Color.Red;

                if (myMotorArray.Motors[mcp.motor_id].IsHomed)
                    mcp.pb_IsHomed.BackColor = Color.Green;
                else
                    mcp.pb_IsHomed.BackColor = Color.Red;

                mcp.txt_currentPos.Text = myMotorArray.Motors[mcp.motor_id].position.ToString("0.000");
                mcp.txt_TrgPos.Text = myMotorArray.Motors[mcp.motor_id].targetPosition.ToString("0.000");
                mcp.txt_ControlMode.Text = myMotorArray.Motors[mcp.motor_id].CurrentControlMode.ToString();
            }
        }

    }
}

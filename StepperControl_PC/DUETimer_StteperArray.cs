using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace StepperControl_PC
{
    internal class DUETimer_StteperArray
    {
        public string COM_Port_Name { get { return _COM_Port_Name; } }

        private string _COM_Port_Name;
        public bool IsConnected { get { return _IsConnected; } }
        private bool _IsConnected;
        public bool IsPowerOn { get; }
        public bool IsPowerEnabled { get; }
        public char CommandCode { get { return _CommandCode; } }
        private char _CommandCode;


        public MotorConfig[] Motors { get { return _Motors; } }
        private MotorConfig[] _Motors;

        private int _timeSinceLastSerialReceived;

        private bool DataRequestSent;

        public SerialPort ArduinoSerialPort;
        
        public System.Windows.Forms.Timer InternalTimer;

        public string SerialIn;

        public DUETimer_StteperArray()
        {
            ArduinoSerialPort = new SerialPort();
            ArduinoSerialPort.DataReceived += ArduinoSerialPort_DataReceived;            
            InternalTimer = new System.Windows.Forms.Timer();
            InternalTimer.Tick += InternalTimer_Tick;
            _timeSinceLastSerialReceived += 2;
            InternalTimer.Interval = 1000;
            InternalTimer.Enabled = true;
            InternalTimer.Start();
            _Motors = new MotorConfig[9];
        }


        #region Timer
        private void InternalTimer_Tick(object sender, EventArgs e)
        {
            LL_checkArduinoConnection();
        }

        /// <summary>
        /// Checks if Arduino is sending data
        /// </summary>
        /// <returns></returns>
        private bool LL_checkArduinoConnection()
        {
            try
            {
                // == Place this function inside the Timer == '
                // Increase the counter every timer click. The counter should be reset to 0 everytime data is received (LL_readPort). If not, the counter will keep increasing
                _timeSinceLastSerialReceived += 1;
                if (_timeSinceLastSerialReceived > 1)
                {
                    if (_IsConnected) { _IsConnected = false; }
                }
                else
                {
                    _IsConnected = true;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region SerialPort
        
        private void ArduinoSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
                if (ArduinoSerialPort.IsOpen)
                {
                    string inStr = ArduinoSerialPort.ReadLine();
                    // Read data...
                    ReadSerialData(inStr);
                    _timeSinceLastSerialReceived = 0;    // Reset counter 
                }
                    
            }
            catch
            {

            }
            // Event handling code here
        }

        private bool ReadSerialData(string inStr)
        {
            try
            {
                string[] sub_command = inStr.Split(';');

                if(sub_command.Length >= 20)
                {
                    if (!Char.TryParse(sub_command[0], out _CommandCode))
                        Log.SendToLog("Error Parsing Command Code");


                    for(int m = 0; m < Motors.Length; m++)
                    {
                        double temp_position;
                        Double.TryParse(sub_command[m + 1], out temp_position);
                        _Motors[m].position = temp_position;

                        if(_Motors[m].position < -99) _Motors[m].IsHomed = false;
                        else _Motors[m].IsHomed = true;

                        MotorConfig.MotorControlMode tmp_motorControlMode;
                        Enum.TryParse(sub_command[m + 10], out tmp_motorControlMode);
                        _Motors[m].CurrentControlMode = tmp_motorControlMode;
                    }
                }
                else
                {
                    if (DataRequestSent)
                        DataRequestSent = false;
                }

                SerialIn = inStr + " (" + sub_command.Length.ToString() + ") - " + _CommandCode.ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Serial Connection

        public async Task<bool> EstablishSerialConnection(string portName)
        {

            Task<bool> t_connect = Task.Factory.StartNew(()=> Connect(portName));
            bool t_connect_result = await t_connect;
            return t_connect_result;
        }

        private bool Connect(string portName, int TimeOut = 5000, int delay = 200)
        {
            // Check that the port field name is not emtpy
            if (portName == "")
            {
                MessageBox.Show("Enter Port Name");
                _IsConnected = false;
                return false;
            }
            try
            {
                bool checkPort = SerialPort.GetPortNames().Any(x => x == portName);
                if (!checkPort)
                {
                    Log.SendToLog(portName + " port Not available - could Not connect to Arduino");
                    _IsConnected = false;
                    return false;
                }

                if (ArduinoSerialPort.IsOpen)
                    ArduinoSerialPort.Close();

                ArduinoSerialPort.PortName = portName;
                ArduinoSerialPort.BaudRate = 115200;
                ArduinoSerialPort.Parity = Parity.None;
                ArduinoSerialPort.DataBits = 8;
                ArduinoSerialPort.StopBits = StopBits.One;
                ArduinoSerialPort.Handshake = Handshake.None;
                ArduinoSerialPort.ReadTimeout = 5000;
                ArduinoSerialPort.Encoding = System.Text.Encoding.Default;

                ArduinoSerialPort.Open();
                if (!ArduinoSerialPort.IsOpen)
                {
                    Log.SendToLog("Connection Failed - Couldn't open the port");
                    _IsConnected = false;
                    return false;
                }

                _IsConnected = SerialBeginTransmission();
                _COM_Port_Name = portName;
                return _IsConnected;

            }
            catch(Exception ex)
            {
                MessageBox.Show("Connection Failed - " + ex.Message);
                return false;
            }

        }

        private bool SerialBeginTransmission(int delay = 200, int max_retries = 3)
        {
            try
            {
                if (!ArduinoSerialPort.IsOpen)  // Check that the port is open
                    ArduinoSerialPort.Open();

                string command = "I:1;";    // Command for starting transmision and setting System Axes Configuration

                DataRequestSent = true; // Set flag to indicate that a data request has been sent

                ArduinoSerialPort.Write(command + Environment.NewLine);    // Send I:1; followed by the AxisConfig to start serial transmission

                Log.SendToLog(command);

                int elapsedTime = 0;
                int retries = 0;

                // 'Wait for confirmation from Arduino that the command has been executed
                while (DataRequestSent) //  When the serial port receives Arduino reply, the DataRequestSent flag will be set to false
                {
                    Thread.Sleep(delay);
                    elapsedTime += delay;
                    if (elapsedTime >= 1000) // ' If no response after one second, send command again
                    {
                        ArduinoSerialPort.Write(command + Environment.NewLine);
                        Log.SendToLog("RETRY: " + command);
                        retries += 1;
                    }
                    if (retries > max_retries)  //  Exit loop after max number of tries, and return FALSE
                    {
                        Log.SendToLog("SerialBeginTransmission - Arduino not responding");
                        return false;
                    }
                }

                retries = 0;
                // At this point, Arduino should be connected and transmiting. If not, try resending the command
                while (!_IsConnected)
                {
                    Thread.Sleep(delay);
                    elapsedTime += delay;
                    if (elapsedTime >= 1000) // ' If no response after one second, send command again
                    {
                        ArduinoSerialPort.Write(command + Environment.NewLine);
                        Log.SendToLog("RETRY: " + command);
                        retries += 1;
                    }
                    if (retries > max_retries)  //  Exit loop after max number of tries, and return FALSE
                    {
                        Log.SendToLog("SerialBeginTransmission - TimeOut");
                        return false;
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("SerialBeginTransmission - " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Motion Control

        public async Task<bool> HomeMotor(int motor_id, double velocity_mms, bool direction = false)
        {
            try
            {
                int freq = Convert_Vel_mms_To_Hz(motor_id, velocity_mms);
                int dir_int = Convert.ToInt32(direction);
                Task<bool> t_home = Task.Factory.StartNew(() => _HomeMotor(motor_id, false, freq, dir_int, 0));
                bool t_home_result = await t_home;
                return t_home_result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Home Motor - " + ex.Message);
                return false;
            }
        }

        private bool _HomeMotor(int motor_id, bool home_ovr, int freq, int dir, double start_pos, int TimeOut = 180000, int delay = 200)
        {
            try
            {
                if (!IsConnected)
                {
                    Log.SendToLog("HomeXYTable - Arduino Not Connected"); return false;
                }    // Check Arduino Connection
                if (motor_id < 0 | motor_id > Motors.Length - 1)
                {
                    Log.SendToLog("Home Motor - Wrong Axis Id"); return false;
                }
                // Check motor is not moving

                // Send command to Arduino to start Homing all three axes
                string command;
                if (!home_ovr)
                    command = "H:" + motor_id.ToString() + ";0;" + freq.ToString() + ";" + dir.ToString() + ";";
                else
                    command = "H:" + motor_id.ToString() + ";1;" + start_pos.ToString("0.000") + ";";

                if (!ArduinoSerialPort.IsOpen)
                    ArduinoSerialPort.Open();    // Check that the port is open...

                ArduinoSerialPort.Write(command + Environment.NewLine);    // ...and send the command
                Log.SendToLog(command);

                // Wait For Homing to Finish
                int elapsedTime = 0;
                while (!Motors[motor_id].IsHomed)
                {
                    Thread.Sleep(delay);
                    elapsedTime += delay;
                    if (elapsedTime > TimeOut)
                    {
                        Log.SendToLog("Home Motor " + motor_id.ToString() + " FAIL - TimeOut!"); return false;
                    }
                }
                string ErrStr = "";
                if (!_Motors[motor_id].SetTargetPosition(Motors[motor_id].position, ref ErrStr))
                {
                    Log.SendToLog("Home Motor FAIL - Target Position Out of Range : " + ErrStr); return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Home Motor - " + ex.Message);
                return false;
            }
        }

        private int Convert_Vel_mms_To_Hz(int motor_id, double vel_mm_s)
        {
            
            return Convert.ToInt32(vel_mm_s * Motors[motor_id].pulses_per_rev / Motors[motor_id].screw_pitch);
        }
       
        public async Task<bool> MoveRelativeDistance(int motor_id, double dist_mm, bool homeOvr = false, double speed_mms = 0, double acc_mms2 = 0, double dec_mms2 = 0, int TimeOut = 60000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {
                if (Motors[motor_id].CurrentControlMode != MotorConfig.MotorControlMode.STP)
                    return false;
                Task<bool> t_move = Task.Factory.StartNew(() => _MoveRel(motor_id, dist_mm, homeOvr, speed_mms, acc_mms2, dec_mms2, TimeOut, delay, max_count, max_tries));
                bool t_move_result = await t_move;
                return t_move_result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Move Relative Distance - " + ex.Message);
                return false;
            }
        }

        private bool _MoveRel(int motor_id, double dist_mm, bool homeOvr = false, double speed_mms = 0, double acc_mms2 = 0, double dec_mms2 = 0, int TimeOut = 60000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {
                for (int i = 0; i <= 3; i++)
                {
                    if (IsConnected)
                        break;
                    Thread.Sleep(100);
                    if (i == 3 & !IsConnected)
                    {
                        Log.SendToLog("MoveRel - Arduino Not Connected"); return false;
                    }
                }

                if (motor_id < 0 | motor_id > _Motors.Length - 1)
                {
                    Log.SendToLog("MoveRel - Wrong Axis Id"); return false;
                }

                if (!homeOvr)
                {
                    if (!Motors[motor_id].IsHomed)
                    {
                        Log.SendToLog("MoveRel FAIL - Motor " + motor_id.ToString() + " not homed"); return false;
                    }
                    string ErrStr = "";
                    if (!_Motors[motor_id].SetTargetPosition(Motors[motor_id].position + dist_mm, ref ErrStr))
                    {
                        Log.SendToLog("MoveRel FAIL - Target Position Out of Range : " + ErrStr); return false;
                    }
                }

                if (speed_mms == 0)
                    speed_mms = _Motors[motor_id].SpeedDefault_mms;
                if (acc_mms2 == 0)
                    acc_mms2 = _Motors[motor_id].AccDefault_mms2;
                if (dec_mms2 == 0)
                    dec_mms2 = _Motors[motor_id].DecDefault_mms2;

                // Send command to move to the target position
                char cc = 'R';   // Temporary variable Command Code (First Char)
                string command = "R:" + motor_id.ToString() + ";" + dist_mm.ToString("0.000") + ";" + speed_mms.ToString("0.000") + ";" + acc_mms2.ToString("0.000") + ";" + dec_mms2.ToString("0.000") + ";";

                if (ArduinoSerialPort.IsOpen == false)
                    ArduinoSerialPort.Open();    // Check that the port is open...

                _CommandCode = '\0'; // Reset Data Member Command Code
                ArduinoSerialPort.Write(command + Environment.NewLine);    // ...and send the command
                Log.SendToLog(command);

                int count = 0;    // TimeOut variables
                int tries = 0;
                int elapsedTime = 0;
                // WAIT FOR START: wait for confirmation from Arduino that the command has been received and executing
                while (_CommandCode != cc) // _command_code is Updated inside the Serial Port Data Received event
                {
                    Thread.Sleep(delay);
                    if (count >= max_count)
                    {
                        ArduinoSerialPort.Write(command + Environment.NewLine);
                        Log.SendToLog("RETRY: " + command + " , " + _CommandCode + " , " + cc);
                        tries = tries + 1;
                        count = 0;
                    }
                    count = count + 1;
                    if (tries > max_tries)
                    {
                        Log.SendToLog("MoveRel - Command not sent!"); return false;
                    } // After max number of tries return FALSE
                }
                Thread.Sleep(200);
                elapsedTime = 0;

                // WAIT FOR FINISH: wait until the execution has finished
                while (_Motors[motor_id].CurrentControlMode != MotorConfig.MotorControlMode.STP) // Wait until the platform stops moving
                {
                    Thread.Sleep(delay);
                    elapsedTime += delay;
                    if (elapsedTime >= TimeOut)
                    {
                        // StopAllMotion()
                        Log.SendToLog("MoveRel - TimeOut!");
                        return false;
                    }
                }

                if (!homeOvr)
                {
                    if (!_Motors[motor_id].IsInTarget())
                    {
                        Log.SendToLog("MoveRel Motor " + motor_id.ToString() + " - Target Position Not Reached"); return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("MoveRel - Exception! : " + ex.Message);
                return false;
            }
        }

        public async Task<bool> MoveToAbsolutePosition(int motor_id, double abs_trgPos_mm, double speed_mms = 0, double acc_mms2 = 0, double dec_mms2 = 0, int TimeOut = 60000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {
                if (Motors[motor_id].CurrentControlMode != MotorConfig.MotorControlMode.STP)
                    return false;
                Task<bool> t_move = Task.Factory.StartNew(() => _MoveAbs(motor_id, abs_trgPos_mm, speed_mms, acc_mms2, dec_mms2, TimeOut, delay, max_count, max_tries));
                bool t_move_result = await t_move;
                return t_move_result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Move To Absolute Position - " + ex.Message);
                return false;
            }
        }

        private bool _MoveAbs(int motor_id, double abs_trgPos_mm, double speed_mms = 0, double acc_mms2 = 0, double dec_mms2 = 0, int TimeOut = 60000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {
                for (int i = 0; i <= 3; i++)
                {
                    if (IsConnected)
                        break;
                    Thread.Sleep(100);
                    if (i == 3 & !IsConnected)
                    {
                        Log.SendToLog("MoveAbs - Arduino Not Connected"); return false;
                    }
                }
                if (motor_id < 0 | motor_id > _Motors.Length - 1)
                {
                    Log.SendToLog("MoveAbs - Wrong Axis Id"); return false;
                }
                if (!_Motors[motor_id].IsHomed)
                {
                    Log.SendToLog("MoveAbs - Motor Not Homed"); return false;
                }
                string ErrStr = "";
                if (!_Motors[motor_id].SetTargetPosition(abs_trgPos_mm, ref ErrStr))
                {
                    Log.SendToLog("MoveAbs FAIL - Target Position Out of Range : " + ErrStr); return false;
                }

                if (speed_mms == 0)
                    speed_mms = _Motors[motor_id].SpeedDefault_mms;
                if (acc_mms2 == 0)
                    acc_mms2 = _Motors[motor_id].AccDefault_mms2;
                if (dec_mms2 == 0)
                    dec_mms2 = _Motors[motor_id].DecDefault_mms2;

                // Send command to move to the target position
                char cc = 'L';   // Temporary variable Command Code (First Char)
                string command = "L:" + motor_id.ToString() + ";" + abs_trgPos_mm.ToString("0.000") + ";" + speed_mms.ToString("0.000") + ";" + acc_mms2.ToString("0.000") + ";" + dec_mms2.ToString("0.000") + ";";

                if (ArduinoSerialPort.IsOpen == false)
                    ArduinoSerialPort.Open();    // Check that the port is open...

                _CommandCode = '\0'; // Reset Data Member Command Code
                ArduinoSerialPort.Write(command + Environment.NewLine);    // ...and send the command
                Log.SendToLog(command);

                int count = 0;    // TimeOut variables
                int tries = 0;
                int elapsedTime = 0;
                // WAIT FOR START: wait for confirmation from Arduino that the command has been received and executing
                while (_CommandCode != cc) // _command_code is Updated inside the Serial Port Data Received event
                {
                    Thread.Sleep(delay);
                    if (count >= max_count)
                    {
                        ArduinoSerialPort.Write(command + Environment.NewLine);
                        Log.SendToLog("RETRY: " + command + " , " + _CommandCode + " , " + cc);
                        tries = tries + 1;
                        count = 0;
                    }
                    count = count + 1;
                    if (tries > max_tries)
                    {
                        Log.SendToLog("MoveAbs - Command not sent!"); return false;
                    } // After max number of tries return FALSE
                }
                Thread.Sleep(200);
                elapsedTime = 0;

                // WAIT FOR FINISH: wait until the execution has finished
                while (Motors[motor_id].CurrentControlMode != MotorConfig.MotorControlMode.STP) // Wait until the platform stops moving
                {
                    Thread.Sleep(delay);
                    elapsedTime += delay;
                    if (elapsedTime >= TimeOut)
                    {
                        // StopAllMotion()
                        Log.SendToLog("MoveAbs - TimeOut!");
                        return false;
                    }
                }

                if (!_Motors[motor_id].IsInTarget())
                {
                    Log.SendToLog("MoveAbs Motor " + motor_id.ToString() + " - Target Position Not Reached"); return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Move To Absolute Position - " + ex.Message);
                return false;
            }
        }

        public async Task<bool> MoveSpeedMode(int motor_id, bool direction, double speed_mms = 0, double acc_mms2 = 0, int TimeOut = 60000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {
                if (Motors[motor_id].CurrentControlMode != MotorConfig.MotorControlMode.STP)
                    return false;
                Task<bool> t_move = Task.Factory.StartNew(() => _MoveVel(motor_id, direction, speed_mms, acc_mms2, TimeOut, delay, max_count, max_tries));
                bool t_move_result = await t_move;
                return t_move_result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Move Speed Mode - " + ex.Message);
                return false;
            }
        }
        private bool _MoveVel(int motor_id, bool direction, double speed_mms = 0, double acc_mms2 = 0, int TimeOut = 60000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {
                for (int i = 0; i <= 3; i++)
                {
                    if (IsConnected)
                        break;
                    Thread.Sleep(100);
                    if (i == 3 & !IsConnected)
                    {
                        Log.SendToLog("MoveVel - Arduino Not Connected"); return false;
                    }
                }
                if (motor_id < 0 | motor_id > _Motors.Length - 1)
                {
                    Log.SendToLog("MoveAbs - Wrong Axis Id"); return false;
                }
                if (!_Motors[motor_id].IsHomed)
                {
                    Log.SendToLog("MoveVel - Motor Not Homed"); return false;
                }

                if (speed_mms == 0)
                    speed_mms = _Motors[motor_id].SpeedDefault_mms;
                if (acc_mms2 == 0)
                    acc_mms2 = _Motors[motor_id].AccDefault_mms2;

                int dir_int;
                if (direction)
                    dir_int = 1;
                else
                    dir_int = 0;

                // Send command to move to the target position
                char cc = 'V';   // Temporary variable Command Code (First Char)
                string command = "V:" + motor_id.ToString() + ";" + dir_int.ToString() + ";" + speed_mms.ToString("0.000") + ";" + acc_mms2.ToString("0.000") + ";";

                if (ArduinoSerialPort.IsOpen == false)
                    ArduinoSerialPort.Open();    // Check that the port is open...

                _CommandCode = '\0'; // Reset Data Member Command Code
                ArduinoSerialPort.Write(command + Environment.NewLine);    // ...and send the command
                Log.SendToLog(command);

                int count = 0;    // TimeOut variables
                int tries = 0;
                // WAIT FOR START: wait for confirmation from Arduino that the command has been received and executing
                while (_CommandCode != cc) // _command_code is Updated inside the Serial Port Data Received event
                {
                    Thread.Sleep(delay);
                    if (count >= max_count)
                    {
                        ArduinoSerialPort.Write(command + Environment.NewLine);
                        Log.SendToLog("RETRY: " + command);
                        tries = tries + 1;
                        count = 0;
                    }
                    count = count + 1;
                    if (tries > max_tries)
                    {
                        Log.SendToLog("MoveVel - Command not sent!"); return false;
                    } // After max number of tries return FALSE
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Move Speed Mode - " + ex.Message);
                return false;
            }
        }

        public async Task<bool> StopAxis(int motor_id, int mode, int decelerationSteps = 0, int TimeOut = 2000, int delay = 100, int max_count = 5, int max_tries = 3)
        {
            try
            {
                Task<bool> t_stop = Task.Factory.StartNew(() => _Stop(motor_id, mode, decelerationSteps, TimeOut, delay, max_count, max_tries));
                bool t_stop_result = await t_stop;
                return t_stop_result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stop Axis - " + ex.Message);
                return false;
            }
        }

        private bool _Stop(int motor_id, int mode, int decelerationSteps, int TimeOut = 60000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {

                // Send command to move to the target position
                char cc = 'S';   // Temporary variable Command Code (First Char)
                string command;

                if (mode == 0)
                    command = "S:" + motor_id.ToString() + ";0;";
                else if (mode == 1)
                    command = "S:" + motor_id.ToString() + ";1;" + decelerationSteps.ToString() + ";";
                else if (mode == 2)
                    command = "S:" + motor_id.ToString() + ";2;" + _Motors[motor_id].DecDefault_mms2.ToString("0.0") + ";";
                else
                {
                    Log.SendToLog("Wrong Stop Mode");
                    return false;
                }

                if (ArduinoSerialPort.IsOpen == false)
                    ArduinoSerialPort.Open();    // Check that the port is open...
                _CommandCode = '\0'; // Reset Data Member Command Code
                ArduinoSerialPort.Write(command + Environment.NewLine);    // ...and send the command
                Log.SendToLog(command);

                int count = 0;    // TimeOut variables
                int tries = 0;
                int elapsedTime = 0;
                // WAIT FOR START: wait for confirmation from Arduino that the command has been received and executing
                while (_CommandCode != cc) // _command_code is Updated inside the Serial Port Data Received event
                {
                    Thread.Sleep(delay);
                    if (count >= max_count)
                    {
                        ArduinoSerialPort.Write(command + Environment.NewLine);
                        Log.SendToLog("RETRY: " + command + " , " + _CommandCode + " , " + cc);
                        tries = tries + 1;
                        count = 0;
                    }
                    count = count + 1;
                    if (tries > max_tries)
                    {
                        Log.SendToLog("MoveAbs - Command not sent!"); return false;
                    } // After max number of tries return FALSE
                }
                Thread.Sleep(200);
                elapsedTime = 0;

                // WAIT FOR FINISH: wait until the execution has finished
                while (_Motors[motor_id].CurrentControlMode != MotorConfig.MotorControlMode.STP) // Wait until the platform stops moving
                {
                    Thread.Sleep(delay);
                    elapsedTime += delay;
                    if (elapsedTime >= TimeOut)
                    {
                        // StopAllMotion()
                        Log.SendToLog("MoveAbs - TimeOut!");
                        return false;
                    }
                }
                string ErrStr = "";
                _Motors[motor_id].SetTargetPosition(_Motors[motor_id].position,ref ErrStr);


                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stop Axis - " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Set Motor Parameters

        public bool InitialiseMotors()
        {
            try
            {
                if (_Motors == null)
                    _Motors = new MotorConfig[9];

                for (int m = 0; m <= _Motors.Length; m++)
                    _Motors[m] = new MotorConfig()
                    {
                        Name = "<Empty>"
                    };

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetMotorName(int motor_id, string motor_name)
        {
            try
            {
                if (_Motors[motor_id] == null)
                    _Motors[motor_id] = new MotorConfig();
                _Motors[motor_id].Name = motor_name;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InitialiseMotionParameters(int motor_id, double speed, double acceleration, double deceleration)
        {
            try
            {
                if (motor_id < 0 | motor_id > Motors.Length - 1)
                {
                    Log.SendToLog("InitialiseMotionMarameters - Wrong Axis Id"); return false;
                }
                if (_Motors[motor_id] == null)
                    _Motors[motor_id] = new MotorConfig();
                _Motors[motor_id].SpeedDefault_mms = speed;
                _Motors[motor_id].AccDefault_mms2 = acceleration;
                _Motors[motor_id].DecDefault_mms2 = deceleration;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> SetAxisParameters(int axis_id, int pls_pin, int dir_pin, int ena_pin, int h_sw_pin, int f_sw_pin, int ppr, double pitch_mm, 
            double travel_mm, int TimeOut = 2000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {
                Task<bool> t_set = Task.Factory.StartNew(() => _SetAxis(axis_id, pls_pin, dir_pin, ena_pin, h_sw_pin, f_sw_pin, ppr, pitch_mm, travel_mm, TimeOut, delay, max_count, max_tries));
                bool t_set_result = await t_set;
                return t_set_result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Set Axis Parameters - " + ex.Message);
                return false;
            }
        }

        private bool _SetAxis(int axis_id, int pls_pin, int dir_pin, int ena_pin, int h_sw_pin, int f_sw_pin, int ppr, double pitch_mm, double travel_mm, 
            int TimeOut = 2000, int delay = 200, int max_count = 5, int max_tries = 3)
        {
            try
            {
                if (!IsConnected)
                {
                    Log.SendToLog("SetXAxis - Arduino Not Connected"); return false;
                }    // Check Arduino Connection

                if (axis_id < 0 | axis_id > 8)
                {
                    Log.SendToLog("SetAxis - Wrong Axis Id"); return false;
                }

                char cc = 'C';   // Temporary variable Command Code (First Char)
                                 // Full Command to Set X Axis Parameters
                string command = "C:" + axis_id.ToString() + ";" + pls_pin.ToString() + ";" + dir_pin.ToString() + ";" + ena_pin.ToString() + ";" + h_sw_pin.ToString() + ";" + f_sw_pin.ToString() + ";" + ppr.ToString() + ";" + pitch_mm.ToString("0.0") + ";" + travel_mm.ToString("0.0") + ";";

                if (!ArduinoSerialPort.IsOpen)
                    ArduinoSerialPort.Open(); // check if port is open

                _CommandCode = '\0'; // Reset Data Member Command Code
                ArduinoSerialPort.Write(command + Environment.NewLine);    // Send Command to Set X Axis Parameters
                Log.SendToLog(command);
                int count = 0;    // TimeOut variables
                int tries = 0;
                // WAIT FOR START: wait for confirmation from Arduino that the command has been received and executing
                while (CommandCode != cc) // _command_code is Updated inside the Serial Port Data Received event
                {
                    Thread.Sleep(delay);
                    if (count >= max_count)
                    {
                        ArduinoSerialPort.Write(command + Environment.NewLine);
                        Log.SendToLog("RETRY: " + command);
                        tries = tries + 1;
                        count = 0;
                    }
                    count = count + 1;
                    if (tries > max_tries)
                    {
                        Log.SendToLog("SetAxis " + axis_id.ToString() + " - Command not sent!"); return false;
                    } // After max number of tries return FALSE
                }

                _Motors[axis_id].IsInit = true;
                _Motors[axis_id].max_travel_range = travel_mm;
                _Motors[axis_id].pulses_per_rev = ppr;                   
                _Motors[axis_id].screw_pitch = pitch_mm;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SetAxis " + axis_id.ToString() + " - " + ex.Message);
                return false;
            }
        }

        #endregion
    }
}

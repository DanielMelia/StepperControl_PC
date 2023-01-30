using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepperControl_PC
{
    public class MotorConfig
    {

        public enum MotorControlMode
        {
            STP,
            HOM,
            POS,
            VEL,
            UNDEF
        }

        public string Name { get; set; }

        public double targetPosition { get { return _targetPosition; } }
        private double _targetPosition;

        public double position { get; set; }
        public bool IsHomed { get; set; }
        public bool IsInit { get; set; }

        public int pulses_per_rev { get; set; }

        public double screw_pitch { get; set; }

        public double max_travel_range { get; set; }

        public double SpeedDefault_mms { get; set; }
        public double AccDefault_mms2 { get; set; }
        public double DecDefault_mms2 { get; set; }

        public MotorControlMode CurrentControlMode { get; set; }    

        public bool SetTargetPosition(double trgPos, ref string errStr)
        {
            if (!IsHomed)
            {
                errStr = "SetTargetPosition FAIL - Motor not homed";
                return false;   
            }

            if (trgPos >= 0 && trgPos <= max_travel_range)
            {
                _targetPosition = trgPos;
                return true;
            }
            else
            {
                errStr = "TargetPos : " + trgPos.ToString("0.000") + " ; Limits : 0 / " + max_travel_range.ToString("0.000");
                return false;
            }
        }

        public bool IsInTarget(double max_error = 0.15)
        {
            try
            {
                if (!IsHomed)
                {
                    Log.SendToLog("IsInTarget FAIL - Motor not homed"); return false;
                }
                if (Math.Abs(position - targetPosition) > max_error)
                {
                    Log.SendToLog("Motor Out of target by: " + Math.Abs(position - targetPosition).ToString("0.000")); return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}

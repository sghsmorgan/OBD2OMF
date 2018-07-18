using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBD2OMF
{
    class OBDMessage
    {
        public enum Measurements
        {
            Speed,
            RPM,
            CoolantTemp,
            ThrottlePosition,
            IntakeAirTemperature,
            FuelPressure,
            FuelTankLevel,
            NODATA
        }

        private Measurements measurement;

        public Measurements Measurement
        {
            get { return measurement; }
            set { measurement = value; }
        }

        private int? value;

        public int? Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private DateTime timestamp;

        public DateTime TimeStamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OBD2OMF
{
    public class Simulator : IDisposable
    {
        private int _speedValue;                //in km/h
        private int _rpmValue;                  //RPM
        private int _coolantTempValue;          //in C
        private int _fuelLevelValue;            //in %
        private int _airIntakeTempValue;        //In C
        private int _throttlePositionValue;     //In %
        private int _fuelPressureValue;         //In psi
        private volatile bool _keepGoing = false;
        private object simulatorLock = new object();
        System.Timers.Timer fuelTimer = new System.Timers.Timer();
        System.Timers.Timer speedTimer = new System.Timers.Timer();
        System.Timers.Timer coolantTimer = new System.Timers.Timer();
       
        public Simulator()
        {
            _speedValue = 0;
            _rpmValue = 500;
            _coolantTempValue = 0;
            _fuelLevelValue = 100;
            _airIntakeTempValue = 0;
            _throttlePositionValue = 10;        //Idling throttle position
            _fuelPressureValue = 45;
            _keepGoing = true;
            Thread simulatorThread = new Thread(SimulateData);
            simulatorThread.Start();
        }
        
        /// <summary>
        /// Simulates the data on a separate thread
        /// </summary>
        internal void SimulateData()
        {
            lock (simulatorLock)
            {
                while (_keepGoing)
                {
                    SimulateSpeed();
                    SimulateCoolantTemp();
                    SimulateFuelLevel();
                    SimulateRPM();
                    SimulateFuelPressure();
                    SimulateAirIntakeTemp();
                    SimulateThrottlePosition();
                    Thread.Sleep(2500);
                }
            }
        }

        /// <summary>
        /// Simulates values for speed
        /// </summary>
        private void SimulateSpeed()
        {
            if (!speedTimer.Enabled)
            {
                speedTimer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
                speedTimer.Elapsed += delegate (object source, System.Timers.ElapsedEventArgs e)
                {
                    if (_speedValue < 70)
                    {
                        _speedValue = _speedValue + 2;
                    }
                };
                speedTimer.Start();
            }
        }

        /// <summary>
        /// Simulates values for RPM
        /// </summary>
        private void SimulateRPM()
        {
            Random randomRpm = new Random();
            _rpmValue = randomRpm.Next(500, 7000);      //Some value between idling and max rpm
        }

        /// <summary>
        /// Simulates values for coolant temperature
        /// </summary>
        private void SimulateCoolantTemp()
        {
            int maxCoolantTemp = 210;
            if (!coolantTimer.Enabled)
            {
                coolantTimer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
                coolantTimer.Elapsed += delegate (object source, System.Timers.ElapsedEventArgs e)
                {
                    if (_coolantTempValue < maxCoolantTemp)
                    {
                        _coolantTempValue = _coolantTempValue + 20;
                    }
                };
                coolantTimer.Start();
            }
        }

        /// <summary>
        /// Simulates values for fuel level
        /// </summary>
        private void SimulateFuelLevel()
        {
            if (!fuelTimer.Enabled)
            {
                fuelTimer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
                fuelTimer.Elapsed += delegate (object source, System.Timers.ElapsedEventArgs e)
                {
                    if (_fuelLevelValue > 0)
                    {
                        _fuelLevelValue--;
                    }
                };
                fuelTimer.Start();
            }
        }

        /// <summary>
        /// Simulates values for fuel pressure
        /// </summary>
        private void SimulateFuelPressure()
        {
            Random randomFuelPressure = new Random();
            _fuelPressureValue = randomFuelPressure.Next(30, 45);      //30-45 psi
        }
        
        /// <summary>
        /// Simulates Air intake temperature
        /// </summary>
        private void SimulateAirIntakeTemp()
        {
            Random randomAirIntakeTemp = new Random();
            _airIntakeTempValue = randomAirIntakeTemp.Next(60, 100);      //60-100 degrees fahrenheit. This is the outside temp going into the intake
        }

        /// <summary>
        /// Simulates Throttle Position
        /// </summary>
        private void SimulateThrottlePosition()
        {
            Random randomThrottlePosition= new Random();
            _throttlePositionValue = randomThrottlePosition.Next(10, 100);      //10-100 percent of the throttle position
        }

        /// <summary>
        /// Gets the data from the OBD simulator
        /// </summary>
        /// <param name="PID">The given PID that you are wanting to retrieve a value for</param>
        /// <returns>The value for the given PID</returns>
        public int? GetData(string PID)
        {
            PID = PID.Trim().ToUpper();
            switch (PID)
            {
                case OBDPid.SpeedPID:
                    return _speedValue;
                case OBDPid.RpmPID:
                    return _rpmValue;
                case OBDPid.CoolantTempPID:
                    return _coolantTempValue;
                case OBDPid.FuelLevelPID:
                    return _fuelLevelValue;
                case OBDPid.ThrottlePositionPID:
                    return _throttlePositionValue;
                case OBDPid.AirIntakeTempPID:
                    return _airIntakeTempValue;
                case OBDPid.FuelPressurePID:
                    return _fuelPressureValue;
                default:
                    return null;    //No data for given PID
            }
        }

        public void Dispose()
        {
            _keepGoing = false;
        }
    }

    /// <summary>
    /// Defined OBD PIDs
    /// </summary>
    internal class OBDPid
    {
        public const string SpeedPID = "010D";
        public const string RpmPID = "010C";
        public const string CoolantTempPID = "0105";
        public const string FuelLevelPID = "012F";
        public const string ThrottlePositionPID = "0111";
        public const string AirIntakeTempPID = "010F";
        public const string FuelPressurePID = "010A";
    }
}

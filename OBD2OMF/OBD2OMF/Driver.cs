using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngressServiceAPI;
using IngressServiceAPI.API;
using System.Threading;

namespace OBD2OMF
{
    class Driver
    {
        private static volatile bool keepRunning = true;
        private static Simulator simulator;
        static void Main(string[] args)
        {
            simulator = new Simulator();
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Driver.keepRunning = false;
            };

            List<string> PIDs = new List<string>()
            {
                OBDPid.SpeedPID,
                OBDPid.RpmPID,
                OBDPid.CoolantTempPID,
                OBDPid.FuelLevelPID,
                OBDPid.ThrottlePositionPID,
                OBDPid.AirIntakeTempPID,
                OBDPid.FuelPressurePID
            };

            IngressClient client = new IngressClient(OMFVars.omfEndpoint, OMFVars.producerToken);

            Container[] containers = DefineTypes(PIDs, client);


            while (Driver.keepRunning)
            {
                List<OBDMessage> messages = GetOBDMessages(PIDs);
                
                Process(messages,containers,client);

                Thread.Sleep(5000);
                

            }
            Console.WriteLine("Collection End");


        }

        private static Container[] DefineTypes(List<String> pIDs, IngressClient client)
        {

            client.CreateTypes(new string[] { FleetStaticType.JsonSchema, VehicleStaticType.JsonSchema, ValueStaticType.JsonSchema });
            client.CreateTypes(new string[] { MeasurementAttribute.JsonSchema });
            Container[] containers =
            {
                new Container() {Id = OBDMessage.Measurements.Speed.ToString(), TypeId = nameof(MeasurementAttribute) },
                new Container() {Id= OBDMessage.Measurements.CoolantTemp.ToString(), TypeId = nameof(MeasurementAttribute) },
                new Container() {Id= OBDMessage.Measurements.FuelPressure.ToString(), TypeId = nameof(MeasurementAttribute) },
                new Container() {Id= OBDMessage.Measurements.FuelTankLevel.ToString(), TypeId = nameof(MeasurementAttribute) },
                new Container() {Id= OBDMessage.Measurements.IntakeAirTemperature.ToString(), TypeId = nameof(MeasurementAttribute) },
                new Container() {Id= OBDMessage.Measurements.RPM.ToString(), TypeId = nameof(MeasurementAttribute) },
                new Container() {Id= OBDMessage.Measurements.ThrottlePosition.ToString(), TypeId = nameof(MeasurementAttribute) }

            };

            client.CreateContainers(containers);

            DefineAssets(pIDs, client, containers);
            return containers;

        }

        private static void DefineAssets(List<string> pIDs, IngressClient client,Container[] containers)
        {
            //send asset for fleet
            AssetLinkValues<FleetStaticType> assetFleet = new AssetLinkValues<FleetStaticType>()
            {
                typeid = "Fleet",
                Values = new List<FleetStaticType> { new FleetStaticType() { index = "SouthEastFleet", name = "SouthEastFleet" } }
            };
            client.SendValuesAsync(new AssetLinkValues<FleetStaticType>[] { assetFleet
    }).Wait();

            //vehicle asset
            AssetLinkValues<VehicleStaticType> assetVehicle = new AssetLinkValues<VehicleStaticType>()
            {
                typeid = "Vehicle",
                Values = new List<VehicleStaticType> { new VehicleStaticType() { index = OMFVars.vehicleName, name = OMFVars.vehicleName } }
            };
            client.SendValuesAsync(new AssetLinkValues<VehicleStaticType>[] { assetVehicle
}).Wait();
            //Measurement Assets
            List<ValueStaticType> values = new List<ValueStaticType>();
            foreach (string pid in pIDs)
            {
                ValueStaticType valueStatic = new ValueStaticType() { index = GetMeasurement(pid).ToString(), name = GetMeasurement(pid).ToString(), PIDString = pid };
                values.Add(valueStatic);
            }
            AssetLinkValues<ValueStaticType> assetValue = new AssetLinkValues<ValueStaticType>()
            {
                typeid = "ValueStaticType",
                Values = values            
            };
            
            client.SendValuesAsync(new AssetLinkValues<ValueStaticType>[] { assetValue }).Wait();

            List<AFLink<StaticElement,StaticElement>> aflinks = new List<AFLink<StaticElement, StaticElement>>();
            AFLink<StaticElement, StaticElement> aFLink = new AFLink<StaticElement, StaticElement> { source = new StaticElement() { typeid = nameof(FleetStaticType), index = "_ROOT" }, target = new StaticElement() { typeid = nameof(FleetStaticType), index = "SouthEastFleet" } };
            aflinks.Add(aFLink);
            aFLink  = new AFLink<StaticElement, StaticElement> { source = new StaticElement() { typeid = nameof(FleetStaticType), index = "SouthEastFleet" }, target = new StaticElement() { typeid = nameof(VehicleStaticType), index = OMFVars.vehicleName } };
            aflinks.Add(aFLink);
            foreach ( var v in values)
            {
                aFLink = new AFLink<StaticElement, StaticElement> { source = new StaticElement() { typeid = nameof(VehicleStaticType), index = OMFVars.vehicleName }, target = new StaticElement() { typeid = nameof(ValueStaticType), index = v.index } };
                aflinks.Add(aFLink);
            }
            //send asset parent-child links
            AssetLinkValues<AFLink<StaticElement, StaticElement>> assetLink = new AssetLinkValues<AFLink<StaticElement, StaticElement>>()
            {
                typeid = "__Link",
                Values = aflinks
            };

            client.SendValuesAsync(new AssetLinkValues<AFLink<StaticElement, StaticElement>>[] { assetLink }).Wait();

            List<AFLink<StaticElement,DynamicElement >> dataLinks = new List<AFLink<StaticElement, DynamicElement>>();
            
            for(int i=0; i<containers.Length; i++)
            {
                dataLinks.Add(new AFLink<StaticElement, DynamicElement>() { source = new StaticElement() { typeid = nameof(ValueStaticType), index = values[i].index }, target = new DynamicElement() { containerid = containers[i].Id } });
            }
            AssetLinkValues<AFLink<StaticElement, DynamicElement >> dataLink = new AssetLinkValues<AFLink<StaticElement, DynamicElement>>()
            {
                typeid = "__Link",
                Values =dataLinks
            };
            client.SendValuesAsync(new AssetLinkValues<AFLink<StaticElement, DynamicElement>>[] { dataLink }).Wait();

        }

        private static void Process(List<OBDMessage> messages, Container[] containers, IngressClient client)
        {
            List<MeasurementAttribute> values = new List<MeasurementAttribute>();



            foreach (OBDMessage message in messages)
            {
                values.Add(new MeasurementAttribute() { Measurement = message.Value.GetValueOrDefault(), timestamp = message.TimeStamp });
            }

            List<DataValues> dataValues = new List<DataValues>();
            for(int i=0;i<containers.Length;i++)
            {
                dataValues.Add(new DataValues() { ContainerId = containers[i].Id, Values = new List<MeasurementAttribute> { values[i] } });
            }

            client.SendValuesAsync(dataValues.ToArray());


        }

        private static List<OBDMessage> GetOBDMessages(List<string> PIDs)
        {
            List<OBDMessage> messages = new List<OBDMessage>();
            foreach (var pid in PIDs)
            {
                OBDMessage message = new OBDMessage();
                message.Value = simulator.GetData(pid);
                message.TimeStamp = DateTime.Now;
                message.Measurement = GetMeasurement(pid);
                Console.WriteLine($"PID: {pid} ({message.Measurement}) \n Value: {message.Value} \n {message.TimeStamp} \n");
                messages.Add(message);
            }
            return messages;
        }

        private static OBDMessage.Measurements GetMeasurement(string PID)
        {
            switch (PID)
            {
                case OBDPid.SpeedPID:
                    return OBDMessage.Measurements.Speed;
                case OBDPid.RpmPID:
                    return OBDMessage.Measurements.RPM;
                case OBDPid.CoolantTempPID:
                    return OBDMessage.Measurements.CoolantTemp;
                case OBDPid.FuelLevelPID:
                    return OBDMessage.Measurements.FuelTankLevel;
                case OBDPid.ThrottlePositionPID:
                    return OBDMessage.Measurements.ThrottlePosition;
                case OBDPid.AirIntakeTempPID:
                    return OBDMessage.Measurements.IntakeAirTemperature;
                case OBDPid.FuelPressurePID:
                    return OBDMessage.Measurements.FuelPressure;
                default:
                    return OBDMessage.Measurements.NODATA;    //No data for given PID
            }
        }

    }

    public static class OMFVars
    {
        public static string omfEndpoint = @"https://abathon5520.osisoft.int:5470/ingress/messages";
        public static string producerToken = @"uid=b2f7bfd6-08cf-4ba7-a01c-f2749be34757&crt=20180717195947978&sig=PKn45Ypzlh6Apqr5ZUmEwToCewC//yPlEQzyCujxyZo=";

        public static string vehicleName = "Truck1";

    }
}

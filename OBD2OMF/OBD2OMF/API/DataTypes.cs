/*************************************************************************************
# Copyright 2018 OSIsoft, LLC
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# <http://www.apache.org/licenses/LICENSE-2.0>
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

# NOTE: this script was designed using the v1.0
# version of the OMF specification, as outlined here:
# http://omf-docs.osisoft.com/en/v1.0
# For more info, see OMF Developer Companion Guide:
# http://omf-companion-docs.osisoft.com
****************************************************************************************/

using System;

namespace IngressServiceAPI
{
    /// <summary>
    /// Sample data structure to represent data objects in the target system.
    /// </summary>
    /// 

    class FleetStaticType
    {
        public string index { get; set; }
        public string name { get; set; }

        public const string JsonSchema =
            @"{
                ""id"": ""FleetStaticType"",
                ""name"": ""FleetStaticType"",
                ""classification"": ""static"",
                ""type"": ""object"",
                ""description"": ""Fleet Vehicle belongs to"",
                ""properties"": {
                    ""index"": {
                        ""type"": ""string"",
                        ""isindex"": true,
                        ""name"": ""not in use"",
                        ""description"": ""not in use""
                    },
                    ""name"": {
                        ""type"": ""string"",
                        ""isname"": true,
                        ""name"": ""not in use"",
                    }
                }
            }";
    }

    class VehicleStaticType
    {
        public string index { get; set; }
        public string name { get; set; }

        public const string JsonSchema =
            @"{
                ""id"": ""VehicleStaticType"",
                ""name"": ""VehicleStaticType"",
                ""classification"": ""static"",
                ""type"": ""object"",
                ""description"": ""Vehicle that data is being polled from"",
                ""properties"": {
                    ""index"": {
                        ""type"": ""string"",
                        ""isindex"": true,
                        ""name"": ""not in use"",
                        ""description"": ""not in use""
                    },
                    ""name"": {
                        ""type"": ""string"",
                        ""isname"": true,
                        ""name"": ""not in use"",
                        ""description"": ""not in use""
                    }
                }
            }";
    }

    class ValueStaticType
    {
        public string index { get; set; }
        public string name { get; set; }
        public string PIDString { get; set; }

        public const string JsonSchema =
    @"{
                ""id"": ""ValueStaticType"",
                ""name"": ""ValueStaticType"",
                ""classification"": ""static"",
                ""type"": ""object"",
                ""description"": ""Element Containing PID and Value Attributes"",
                ""properties"": {
                    ""index"": {
                        ""type"": ""string"",
                        ""isindex"": true,
                        ""name"": ""not in use"",
                        ""description"": ""not in use""
                    },
                    ""name"": {
                        ""type"": ""string"",
                        ""isname"": true,
                        ""name"": ""not in use"",
                        ""description"": ""not in use""
                    },
                    ""PIDString"": {
                        ""type"": ""string"",
                        ""name"": ""PIDString"",
                        ""description"": ""String defining PID""
                    }
                }
            }";

    }


    class MeasurementAttribute
    {
        public DateTime timestamp { get; set; }
        public int Measurement { get; set; }

        public const string JsonSchema =
            @"{
               ""id"": ""MeasurementAttribute"",
                ""name"": ""MeasurementAttribute"",
                ""classification"": ""dynamic"",
                ""type"": ""object"",
                ""description"": ""not in use"",
                ""properties"": {
                    ""timestamp"": {
                        ""format"": ""date-time"",
                        ""type"": ""string"",
                        ""isindex"": true,
                        ""name"": ""not in use"",
                        ""description"": ""not in use""
                    },
                    ""Measurement"": {
                        ""type"": ""integer"",
                        ""name"": ""Measurement"",
                        ""description"": ""Value representing the OBD Measurement""
                    }
                }
            }";
    }



}

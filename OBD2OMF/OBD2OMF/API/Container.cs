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

namespace IngressServiceAPI.API
{
    /// <summary>
    /// This defines the relationship between a container and a type. Id and TypeId
    /// property names are defined in the OMF spec.  This class is serialized into 
    /// an OMF message.
    /// </summary>
    public class Container
    {
        public string Id { get; set; }
        public string TypeId { get; set; }
    }
}
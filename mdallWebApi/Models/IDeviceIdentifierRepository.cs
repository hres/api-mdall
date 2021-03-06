﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface IDeviceIdentifierRepository
    {
        IEnumerable<DeviceIdentifier> GetAll(string state="",string deviceIdentifierName="", int licenceId=0, int deviceId=0);
        IEnumerable<DeviceIdentifier> Get(int id);
    }
}
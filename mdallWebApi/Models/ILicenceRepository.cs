﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface ILicenceRepository
    {
        IEnumerable<Licence> GetAll(string status = "", string licenceName = "");

        IEnumerable<Licence> GetAllLicenceByCompanyId(int company_id, string status = "");
        

        Licence Get(int id, string status = "");
        
    }
}
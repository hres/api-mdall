using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface ISbdLocationRepository
    {
        IEnumerable<SbdLocation> GetAll();
        SbdLocation Get(int id);
    }
}

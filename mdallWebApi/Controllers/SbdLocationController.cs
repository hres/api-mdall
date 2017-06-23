using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using MdallWebApi.Models;

namespace MdallWebApi.Controllers
{
    //[EnableCors(origins: "https://api.hres.ca/mdall/api-v1/", headers: "*", methods: "*")]
    public class SbdLocationController : ApiController
    {
        static readonly ISbdLocationRepository databasePlaceholder = new SbdLocationRepository();

        public IEnumerable<SbdLocation> GetAllSbdLocation()
        {

            return databasePlaceholder.GetAll();
        }


        public SbdLocation GetSbdLocationById(int id)
        {
            SbdLocation location = databasePlaceholder.Get(id);
            if (location == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return location;
        }
    }
}
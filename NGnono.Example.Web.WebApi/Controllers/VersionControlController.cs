using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NGnono.Example.Web.WebApi.Support.Filters;

namespace NGnono.Example.Web.WebApi.Controllers
{
    //[RoutePrefix("api/v2")]
    public class VersionControlV2Controller : ApiController
    {
        // [DecodeFilter]
        [ExecInfoActionFilter]
        [Route("api/v2/vc/{name}_{version}")]
        [Route("api/v3/vc/{name}_{version}")]
        public IEnumerable<string> Get2(string name, string version)
        {
            return new[]
                {
                    name,
                    version
                };
        }
    }


    public class VersionControlController : ApiController
    {
        public VersionControlController()
        {
        }

        // GET api/versioncontrol
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/versioncontrol/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/versioncontrol
        public void Post([FromBody]string value)
        {
        }

        // PUT api/versioncontrol/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/versioncontrol/5
        public void Delete(int id)
        {
        }


        // [DecodeFilter]
        [ExecInfoActionFilter]
        [Route("api/vc/{name}_{version}")]
        [SimpleDataRole("storeid")]
        public dynamic Get2(string name, string version, int? storeId)
        {
            return new
                {
                    name,
                    version,
                    storeId
                };
        }
    }
}

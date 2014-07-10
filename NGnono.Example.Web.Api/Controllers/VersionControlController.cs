using System.Collections.Generic;
using System.Web.Http;

namespace NGnono.Example.Web.Api.Controllers
{
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


        [Route("api/vc/{name}_{version}")]
        public IEnumerable<string> Get2(string name, string version)
        {
            return new[]
                {
                    name,
                    version
                };
        }
    }
}

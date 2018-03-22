using System.Web.Http;
using ToDoApp.WebApi.Filters;

namespace ToDoApp.WebApi.Controllers
{
    [Authenticate]
    [RoutePrefix("api/tests")]
    public class TestsController : ApiController
    {

        public TestsController()
        {
            
        }

        [HttpGet]
        public string Get()
        {
            return "Get Test";
        }

        [HttpGet]
        public string Get(string id)
        {
            return "Get Test Id: " + int.Parse(id);
        }

        [HttpPost]
        public void Post(string value)
        {
        }

        [HttpPut]
        public void Put(int id, string value)
        {
        }

        [HttpDelete]
        public void Delete(int id)
        {
        }
    }
}

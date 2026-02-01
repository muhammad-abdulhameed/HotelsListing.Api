using HotelsListing.Api.DomainObj;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelsListing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        public  Hotel[] Hotels=new Hotel[] 
        {
            new Hotel{Id="0",Name="continital",Country= "Egypt",Rating=4.5},
            new Hotel{Id="1",Name="continital",Country= "UAE",Rating=3.5},
            new Hotel{Id="2",Name="continital",Country= "Sudan",Rating=5.0},
        
        };

        // GET: api/<HotelsController>
        [HttpGet]
        public ActionResult<IEnumerable<Hotel[]>>  Get()
        {
            return Ok(Hotels);
        }

        // GET api/<HotelsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HotelsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HotelsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HotelsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MoesTavern.Contracts;
using MoesTavern.ServiceBusFacade.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoesTavern.ServiceBusFacade.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class WaitersController : Controller
    {
        private ServiceBusProxy Proxy { get; }

        public WaitersController(ServiceBusProxy proxy)
        {
            Proxy = proxy;
        }

        [HttpPost("serve")]
        public async Task<ActionResult> ServeDrink([FromBody]Drink drink)
        {
            await Proxy.SendAsync(drink, new KeyValuePair<string, object>("OrderedFor", drink.OrderedFor));

            return Ok();
        }

        [HttpGet("orders/next")]
        public async Task<ActionResult<Order>> GetNextOrder()
        {
            Order nextOrder = await Proxy.GetAsync<Order>();

            if (nextOrder == null)
            {
                return NotFound();
            }

            return Ok(nextOrder);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using MoesTavern.Contracts;
using MoesTavern.ServiceBusFacade.Integration;
using System.Threading.Tasks;

namespace MoesTavern.ServiceBusFacade.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private ServiceBusProxy Proxy { get; }

        public CustomersController(ServiceBusProxy proxy)
        {
            Proxy = proxy;
        }

        [HttpGet("{customerName}")]
        public async Task<ActionResult<CustomerInfo>> GetCustomerInfoAsync(string customerName)
        {
            long? messageCount = await Proxy.GetMessageCountAsync(customerName);

            if (!messageCount.HasValue)
            {
                return NotFound($"{customerName}? Never heard of'em.");
            }

            return Ok(new CustomerInfo
            {
                DrinksLeft = messageCount.GetValueOrDefault()
            });
        }

        [HttpPut("{customerName}")]
        public async Task<ActionResult> TakeASeatAsync(string customerName)
        {
            await Proxy.SubscribeAsync(customerName);

            return Created($"api/customers/{customerName}", new CustomerInfo());
        }

        [HttpDelete("{customerName}")]
        public async Task<ActionResult> LeaveAsync(string customerName)
        {
            await Proxy.UnsubscribeAsync(customerName);

            return NotFound();
        }

        [HttpPost("order")]
        public async Task<ActionResult> SendOrderAsync([FromBody]Order order)
        {
            await Proxy.SendAsync(order);

            return Ok();
        }

        [HttpGet("{customerName}/next")]
        public async Task<ActionResult<Drink>> GetNextDrinkAsync(string customerName)
        {
            Drink nextDrink = await Proxy.GetAsync<Drink>(customerName);

            if (nextDrink == null)
            {
                return NotFound();
            }

            return Ok(nextDrink);
        }
    }
}
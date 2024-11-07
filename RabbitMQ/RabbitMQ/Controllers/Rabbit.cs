using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class RabbitController : ControllerBase
    {
        [HttpPost]
        public IActionResult Send(object message)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = "host.docker.internal", Password = "rabbitmq", UserName = "rabbitmq" };

                using (var connection = factory.CreateConnection())
                using(var channel = connection.CreateModel())
                {
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                    channel.BasicPublish(
                        exchange: "", 
                        routingKey: "TestQueue", 
                        body: body, 
                        basicProperties: null);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return StatusCode(500);
            }

            return Ok("The message was sent");
        }
    }
}

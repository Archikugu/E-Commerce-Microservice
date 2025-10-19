using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace MultiShop.RabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateMessage()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",

            };

            var connection = await connectionFactory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("Stack1", false, false, false, arguments: null);

            var messageContent = "Hi this RabbitMQ Message";
            var byteMessageContent = Encoding.UTF8.GetBytes(messageContent);

            var props = new BasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = DeliveryModes.Persistent;

            await channel.BasicPublishAsync<BasicProperties>(
                exchange: "",
                routingKey: "Stack1",
                mandatory: false,
                basicProperties: props,
                body: byteMessageContent
            );

            return Ok("Message added stack");
        }

        [HttpGet("read")]
        public async Task<IActionResult> ReadMessage()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            var connection = await connectionFactory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("Stack1", false, false, false, arguments: null);

            var result = await channel.BasicGetAsync("Stack1", autoAck: true);
            if (result == null)
            {
                return NotFound("No message in queue");
            }

            var bytes = result.Body.ToArray();
            var text = Encoding.UTF8.GetString(bytes);
            return Ok(new { message = text });
        }
    }
}

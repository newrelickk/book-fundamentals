using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewRelic.Api.Agent;
using StackExchange.Redis;

namespace PublisherWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublisherController : ControllerBase
    {

        public PublisherController()
        {
            
        }

        [HttpGet]
        public async Task Get([FromQuery]string message = "Hello Redis")
        {
            var dict = new Dictionary<string, string>();
            NewRelic.Api.Agent.NewRelic.GetAgent().CurrentTransaction.InsertDistributedTraceHeaders(dict,
                (dict, key, value) => { dict[key] = value; });
            dict["message"] = message;
            var jsonString = JsonSerializer.Serialize(dict);
            using var redis = await ConnectionMultiplexer.ConnectAsync("redis:6379");
            var subscriber = redis.GetSubscriber();
            await subscriber.PublishAsync("urn:redissubscriber:userchange", jsonString, CommandFlags.FireAndForget);
        }
    }
}

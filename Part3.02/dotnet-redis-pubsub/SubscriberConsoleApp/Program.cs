using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using NewRelic.Api.Agent;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace SubscriberConsoleApp
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync(args);
        }

        public async Task Run(
            [Option("r", "redis configuration.")] string configuration = "redis:6379")
        {
            using var redis = await ConnectionMultiplexer.ConnectAsync(configuration);
            
            var subscriber = redis.GetSubscriber();
            await subscriber.SubscribeAsync("urn:redissubscriber:userchange", Handler);
            // you can write infinite-loop while stop request(Ctrl+C or docker terminate).
            try
            {
                while (!Context.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        Console.WriteLine("Subscribing redis...");
                    }
                    catch (Exception ex)
                    {
                        // error occured but continue to run(or terminate).
                        Console.WriteLine(ex);
                    }

                    // wait for next time
                    await Task.Delay(TimeSpan.FromMinutes(1), Context.CancellationToken);
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                NewRelic.Api.Agent.NewRelic.NoticeError(ex);
            }
            finally
            {
                await subscriber.UnsubscribeAsync("urn:redissubscriber:userchange");
            }
        }

        [Transaction]
        private void Handler(RedisChannel channel, RedisValue val)
        {
            Console.WriteLine($"received {val}");
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(val);
            NewRelic.Api.Agent.NewRelic.GetAgent().CurrentTransaction.AcceptDistributedTraceHeaders(dict, 
                (carrier, key) => carrier.TryGetValue(key, out string v) ? new[] { v } : Array.Empty<string>(), 
                TransportType.Other);
            Console.WriteLine($"working");
            Task.Delay(TimeSpan.FromSeconds(3), Context.CancellationToken).GetAwaiter().GetResult();
            Console.WriteLine($"finished");
        }
    }
}

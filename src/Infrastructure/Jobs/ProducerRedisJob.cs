// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Dtos;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Constants;
using Newtonsoft.Json;
using Quartz;

namespace NetCa.Infrastructure.Jobs;

/// <summary>
/// ProducerRedisJob
/// </summary>
public class ProducerRedisJob : BaseJob<ProducerRedisJob>
{
    private readonly IProducerService _producerService;
    private readonly IRedisService _redisService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProducerRedisJob"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="producerService"></param>
    /// <param name="redisService"></param>
    /// <param name="logger"></param>
    public ProducerRedisJob(
        IServiceScopeFactory serviceScopeFactory,
        IProducerService producerService,
        IRedisService redisService,
        ILogger<ProducerRedisJob> logger)
        : base(logger, serviceScopeFactory)
    {
        _producerService = producerService;
        _redisService = redisService;
    }

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task Execute(IJobExecutionContext context)
    {
        try
        {
            Logger.LogDebug("Process produce message from Redis");

            for (var i = 0; i < ProcessConstants.DefaultTotalMaxProcess; i++)
            {
                try
                {
                    var value = await _redisService.ListLeftPopAsync(RedisConstants.SubKeyProduceMessage).ConfigureAwait(false);

                    if (value == null)
                    {
                        break;
                    }

                    var redisMessage = JsonConvert.DeserializeObject<MessageBrokerDto>(value, JsonExtensions.SerializerSettings());

                    await _producerService.SendAsync(redisMessage.Name, redisMessage.Value).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.LogError("Error when running worker produce message from Redis: {Message}", e.Message);

                    throw;
                }
            }

            Logger.LogDebug("Produce message from Redis success");
        }
        catch (Exception e)
        {
            Logger.LogError("Error when running worker produce message from Redis: {Message}", e.Message);
        }

        Logger.LogDebug("Produce message from Redis done");
    }
}

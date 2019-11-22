﻿using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Newtonsoft.Json;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Database.Leagues.Commands;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchLeagueStandingConsumer : IConsumer<ILeagueStandingFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchLeagueStandingConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<ILeagueStandingFetchedMessage> context)
        {
            var message = context.Message;
            var standings = JsonConvert.SerializeObject(message.LeagueStanding);

            var command = new InsertOrUpdateStandingCommand(
                message.LeagueStanding.League.Id,
                message.LeagueStanding.LeagueSeason.Id,
                message.LeagueStanding.Type.DisplayName,
                standings,
                message.Language);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}
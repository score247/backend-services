namespace Soccer.Core.Domain.Matches
{
    using Score247.Shared.Base;
    using Soccer.Core.Domain.Matches.Entities;

    public interface ILiveMatchRepository : IBaseRepository<LiveMatchEntity>
    {
    }

    public class LiveMatchRepository : SoccerBaseRepository<LiveMatchEntity>, ILiveMatchRepository
    {
        public LiveMatchRepository(SoccerContext soccerContext) : base(soccerContext)
        {
        }
    }
}
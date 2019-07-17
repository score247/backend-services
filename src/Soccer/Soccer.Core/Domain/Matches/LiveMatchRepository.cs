namespace Soccer.Core.Domain.Matches
{
    using Soccer.Core.Domain.Matches.Entities;

    public interface ILiveMatchRepository : IBaseRepository<LiveMatchEntity>
    {
    }

    public class LiveMatchRepository : BaseRepository<LiveMatchEntity>, ILiveMatchRepository
    {
        public LiveMatchRepository(SoccerContext soccerContext) : base(soccerContext)
        {
        }
    }
}
namespace Soccer.Core.Domain.Matches
{
    using Soccer.Core.Domain.Matches.Entities;

    public interface IMatchRepository : IBaseRepository<MatchEntity>
    {
    }

    public class MatchRepository : BaseRepository<MatchEntity>, IMatchRepository
    {
        public MatchRepository(SoccerContext soccerContext) : base(soccerContext)
        {
        }
    }
}
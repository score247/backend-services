namespace Soccer.Core.Domain.Matches
{
    using Score247.Shared.Base;
    using Soccer.Core.Domain.Matches.Entities;

    public interface IMatchRepository : IBaseRepository<MatchEntity>
    {
    }

    public class MatchRepository : SoccerBaseRepository<MatchEntity>, IMatchRepository
    {
        public MatchRepository(SoccerContext soccerContext) : base(soccerContext)
        {
        }
    }
}
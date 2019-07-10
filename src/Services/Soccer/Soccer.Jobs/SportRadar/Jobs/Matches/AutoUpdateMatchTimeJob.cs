namespace LiveScore.Soccers.Jobs
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAutoUpdateMatchTimeJob
    {
        Task UpdateMatchTime();
    }

    public class AutoUpdateMatchTimeJob : IAutoUpdateMatchTimeJob
    {
        public Task UpdateMatchTime()
        {
            throw new System.NotImplementedException();
        }
    }
}
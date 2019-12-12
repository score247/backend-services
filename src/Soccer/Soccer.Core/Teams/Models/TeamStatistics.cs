namespace Soccer.Core.Teams.Models
{
    using MessagePack;
    using Newtonsoft.Json;

    [MessagePackObject(keyAsPropertyName: true)]
    public class TeamStatistic
    {
        public TeamStatistic()
        {
        }

        public TeamStatistic(
            int redCards,
            int yellowRedCards)
        {
            RedCards = redCards;
            YellowRedCards = yellowRedCards;
        }

#pragma warning disable S107 // Methods should not have too many parameters

        [SerializationConstructor, JsonConstructor]
        public TeamStatistic(
            int possession,
            int freeKicks,
            int throwIns,
            int goalKicks,
            int shotsBlocked,
            int shotsOnTarget,
            int shotsOffTarget,
            int cornerKicks,
            int fouls,
            int shotsSaved,
            int offSides,
            int yellowCards,
            int injuries,
            int redCards,
            int yellowRedCards)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            Possession = possession;
            FreeKicks = freeKicks;
            ThrowIns = throwIns;
            GoalKicks = goalKicks;
            ShotsBlocked = shotsBlocked;
            ShotsOnTarget = shotsOnTarget;
            ShotsOffTarget = shotsOffTarget;
            CornerKicks = cornerKicks;
            Fouls = fouls;
            ShotsSaved = shotsSaved;
            Offsides = offSides;
            YellowCards = yellowCards;
            Injuries = injuries;
            RedCards = redCards;
            YellowRedCards = yellowRedCards;
        }

#pragma warning disable S109 // Magic numbers should not be used
        public int Possession { get; }

        public int FreeKicks { get; }

        public int ThrowIns { get; }

        public int GoalKicks { get; }

        public int ShotsBlocked { get; }

        public int ShotsOnTarget { get; }

        public int ShotsOffTarget { get; }

        public int CornerKicks { get; }

        public int Fouls { get; }

        public int ShotsSaved { get; }

        public int Offsides { get; }

        public int YellowCards { get; }

        public int Injuries { get; }

        public int RedCards { get; }

        public int YellowRedCards { get; }

#pragma warning restore S109 // Magic numbers should not be used
    }
}
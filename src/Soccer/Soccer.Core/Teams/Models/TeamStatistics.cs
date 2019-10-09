using MessagePack;

namespace Soccer.Core.Teams.Models
{
    using MessagePack;
    using Newtonsoft.Json;

    [MessagePackObject]
    public class TeamStatistic
    {
        public TeamStatistic() { }

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
            int offsides, 
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
            Offsides = offsides;
            YellowCards = yellowCards;
            Injuries = injuries;
            RedCards = redCards;
            YellowRedCards = yellowRedCards;
        }

        [Key(0)]
        public int Possession { get; }

        [Key(1)]
        public int FreeKicks { get; }

        [Key(2)]
        public int ThrowIns { get; }

        [Key(3)]
        public int GoalKicks { get; }

        [Key(4)]
        public int ShotsBlocked { get; }

        [Key(5)]
        public int ShotsOnTarget { get; }

        [Key(6)]
        public int ShotsOffTarget { get; }


        [Key(7)]
        public int CornerKicks { get; }


        [Key(8)]
        public int Fouls { get; }


        [Key(9)]
        public int ShotsSaved { get; }


        [Key(10)]
        public int Offsides { get; }


        [Key(11)]
        public int YellowCards { get; }


        [Key(12)]
        public int Injuries { get; }


        [Key(13)]
        public int RedCards { get; }


        [Key(14)]
        public int YellowRedCards { get; }
    }
}
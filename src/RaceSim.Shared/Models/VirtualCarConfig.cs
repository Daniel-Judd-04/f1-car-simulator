namespace RaceSim.Shared;

public class VirtualCarConfig
{
    public float MaxEngineForce { get; } = 52.0f;
    public float MaxBrakeForce { get; } = 80.0f;
    public float SpeedPerGear { get; } = 40.0f;
    public float IdleRpm { get; } = 1500.0f;
    public float MaxRpm { get; } = 11500.0f;
    public float FuelConsumptionRatePerKm { get; } = 0.65f;
    public float CoolingRate { get; } = 23.0f;
    public Random Random { get; } = new Random(123);
    private Dictionary<RaceFlag, float> flagSwitchProbabilities = new Dictionary<RaceFlag, float>
    {
        {RaceFlag.Chequered, 0F}, // Never
        {RaceFlag.Green, 0.001F}, // 100s
        {RaceFlag.Yellow, 0.01F}, // 10s
        {RaceFlag.Red, 0.005F}, // 20s
        {RaceFlag.Black_With_Orange_Disc, 0F}, // Never
    };
    private Dictionary<SegmentType, Dictionary<RaceFlag, float>> flagOccurrenceProbabilities = new Dictionary<SegmentType, Dictionary<RaceFlag, float>>
    {
        {SegmentType.Straight, new Dictionary<RaceFlag, float>
        {
            {RaceFlag.Green, 0.95F},
            {RaceFlag.Yellow, 0.04F},
            {RaceFlag.Red, 0.01F},
        }
        },
        {SegmentType.BrakingZone, new Dictionary<RaceFlag, float>
        {
            {RaceFlag.Green, 0.90F},
            {RaceFlag.Yellow, 0.08F},
            {RaceFlag.Red, 0.02F},
        }
        },
        {SegmentType.CornerApex, new Dictionary<RaceFlag, float>
        {
            {RaceFlag.Green, 0.80F},
            {RaceFlag.Yellow, 0.16F},
            {RaceFlag.Red, 0.04F},
        }
        },
        {SegmentType.CornerExit, new Dictionary<RaceFlag, float>
        {
            {RaceFlag.Green, 0.90F},
            {RaceFlag.Yellow, 0.06F},
            {RaceFlag.Red, 0.04F},
        }
        },
    };

    public float RequiredFuel(Track track)
    {    
        float raceLengthKm = track.TotalLength() / 1000f;
        float variance = 0.05F;
        
        return (raceLengthKm * FuelConsumptionRatePerKm * (track.AvgThrottleUsage() * (1+variance))) + 1;
    }

    public float Randomise(float num, float factor = 0.01F, float variance = 0.1F)
    {
        float rand = (float)((Random.NextDouble()*factor)+1);
        float randVar = (float)(variance*Random.NextDouble())-(variance/2);
        return (num+randVar) * rand;
    }

    public RaceFlag ChooseFlag(RaceFlag flag, SegmentType segmentType)
    {
        float switchProbability = flagSwitchProbabilities[flag];
        if (Random.NextDouble() >= switchProbability) // Stay
        {
            return flag;
        }

        Dictionary<RaceFlag, float> occurrenceProbabilities = flagOccurrenceProbabilities[segmentType];

        float totalAlternativeWeight = 0.0F;
        foreach ((RaceFlag f, float p) in occurrenceProbabilities)
        {
            if (f != flag) 
            {
                totalAlternativeWeight += p;
            }
        }
        if (totalAlternativeWeight <= 0.0F) return flag;
        float rand = (float)(Random.NextDouble() * totalAlternativeWeight);

        float cumulativeWeight = 0.0F;
        foreach ((RaceFlag f, float p) in occurrenceProbabilities)
        {
            if (f == flag) continue;

            cumulativeWeight += p;
            if (rand < cumulativeWeight)
            {
                return f;
            }
        }

        return flag;
    }
}
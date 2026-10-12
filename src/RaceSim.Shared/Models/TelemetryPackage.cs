namespace RaceSim.Shared;

public class TelemetryPackage
{
    public string CarId { get; init; } = "CAR-01";
    public string Status { get; init; } = "";
    public DateTimeOffset CreatedAt { get; init; }

    public RaceFlag Flag { get; init; } = RaceFlag.Black_With_Orange_Disc;
    public bool IsNewLap { get; init; }
    public int LapNumber { get; init; }
    public float LapTimeMs { get; init; }

    public float Throttle { get; init; }
    public float Brake { get; init; }
    public float SteeringAngle { get; init; }

    public float Speed { get; init; }
    public int Gear { get; init; }
    public int EngineRevs { get; init; }
    public float EngineTemperature { get; init; }

    public float FuelRemainingLitres { get; init; }
    public float DistanceTravelled { get; init; }

    public TyreMetrics[] Tyres { get; init; } = new TyreMetrics[4];
}

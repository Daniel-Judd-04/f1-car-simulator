using System.ComponentModel;
using System.Dynamic;

namespace RaceSim.Shared;

public class ParametersPackage
{
    public Track Track { get; init; } = null!;
    public float InitialFuel { get; init; }
    public float MaxSpeed { get; init; }
    public float LapLength { get; init; }
    public int IntervalMs { get; init; } = 100;

    // Factory Method to encapsulate mapping calculations
    public static ParametersPackage Create(VirtualCarConfig config, Track track)
    {
        float lapLengthMeters = track.Length();
        float totalDistanceMeters = lapLengthMeters * track.Laps;
        float theoreticalMaxSpeed = (float)Math.Sqrt(config.MaxEngineForce / 0.0005f);

        return new ParametersPackage
        {
            Track = track,
            LapLength = lapLengthMeters,
            InitialFuel = config.RequiredFuel(track),
            MaxSpeed = theoreticalMaxSpeed,
        };
    }
}
namespace RaceSim.Shared;

public class TrackSegment
{
    public float StartDistance { get; set; }
    public float EndDistance { get; set; }
    public SegmentType Type { get; set; }
    public float TargetSteering { get; set; }

    public TrackSegment(float start, float end, SegmentType type, float targetSteering)
    {
        StartDistance = start;
        EndDistance = end;
        Type = type;
        TargetSteering = targetSteering;
    }

    public TrackSegment(float start, float end, SegmentType type)
    {
        StartDistance = start;
        EndDistance = end;
        Type = type;
        TargetSteering = 0.0F;
    }

    public TrackSegment(float start, float end)
    {
        StartDistance = start;
        EndDistance = end;
        Type = SegmentType.Straight;
        TargetSteering = 0.0F;
    }

    public float GetTargetThrottle()
    {
        return Type switch
        {
            SegmentType.Straight => 1.0F,
            SegmentType.BrakingZone => 0.0F,
            SegmentType.CornerApex => 0.15F,
            SegmentType.CornerExit => 0.6F,
            _ => 0.0F,
        };
    }

    public float GetTargetBrake()
    {
        return Type switch
        {
            SegmentType.Straight => 0.0F,
            SegmentType.BrakingZone => 1.0F,
            SegmentType.CornerApex => 0.0F,
            SegmentType.CornerExit => 0.0F,
            _ => 1.0F,
        };
    }

    public float GetTargetSteering()
    {
        return Type switch
        {
            SegmentType.Straight => 0.0F,
            SegmentType.BrakingZone => 0.0F,
            SegmentType.CornerApex => TargetSteering,
            SegmentType.CornerExit => TargetSteering * 0.5F,
            _ => 0.0F,
        };
    }
}
namespace RaceSim.Shared;

public class Track
{
    public TrackSegment[] Segments { get; }
    public int Laps { get; }
    public string Name { get; }


    public Track(TrackSegment[] segments, int laps, string name)
    {
        Segments = segments;
        Laps = laps;
        Name = name;
    }

    public float Length()
    {
        float sum = 0.0F;
        foreach (TrackSegment segment in Segments)
        {
            sum += segment.EndDistance - segment.StartDistance;
        }
        return sum;
    }

    public float TotalLength()
    {
        return this.Length() * Laps;
    }

    public TrackSegment GetSegmentAt(float distanceTravelled)
    {
        float distance = this.GetLapDistance(distanceTravelled);

        foreach (TrackSegment segment in Segments)
        {
            if (distance >= segment.StartDistance && distance < segment.EndDistance)
            {
                return segment;
            }
        }
        Console.WriteLine($"Could not find segment at distance: {distance}");
        return Segments[0];
    }

    public float GetLapDistance(float distanceTravelled)
    {
        return distanceTravelled % this.Length();
    }

    public float AvgThrottleUsage()
    {
        if (Segments == null || Segments.Length == 0)
        {
            return 0.0F;
        }

        float totalTrackLength = 0.0F;
        float accumulatedThrottleDistance = 0.0F;

        foreach (var segment in Segments)
        {
            float segmentLength = segment.EndDistance - segment.StartDistance;
            
            if (segmentLength > 0) 
            {
                totalTrackLength += segmentLength;
                // Weight the throttle value by the length it is active
                accumulatedThrottleDistance += segmentLength * segment.GetTargetThrottle();
            }
        }

        if (totalTrackLength <= 0.0F) return 0.0F;
        return accumulatedThrottleDistance / totalTrackLength;
    }
}
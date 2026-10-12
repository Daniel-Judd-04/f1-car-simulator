using System;

namespace RaceSim.Generator;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting F1 Telemetry Generator");

        TelemetrySender sender = new TelemetrySender();
        sender.StartStreaming();
    }
}
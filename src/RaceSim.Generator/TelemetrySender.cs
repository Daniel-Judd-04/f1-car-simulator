using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using RaceSim.Shared;

namespace RaceSim.Generator;

public class TelemetrySender
{
    private readonly string _ipAddress;
    private readonly int _port;

    private readonly Track silverstone = new Track(
    new TrackSegment[]
    {
        // --- SECTION 1: Hamilton Straight to Wellington Straight ---
        new(0, 250), // Hamilton Straight (Start/Finish Line zone)
        
        // Turn 1: Abbey (Fast Right)
        new(250, 280, SegmentType.BrakingZone),
        new(280, 340, SegmentType.CornerApex, 15.0F),
        new(340, 400, SegmentType.CornerExit, 15.0F),
        
        // Turn 2: Farm Curve (Flat out sweeping Left)
        new(400, 550, SegmentType.CornerApex, -10.0F),
        new(550, 700), // Short acceleration run
        
        // Turn 3: Village (Slow Right)
        new(700, 770, SegmentType.BrakingZone),
        new(770, 830, SegmentType.CornerApex, 60.0F),
        new(830, 880, SegmentType.CornerExit, 60.0F),
        
        // Turn 4: The Loop (Tight 180° Left Hairpin - Slowest point on track)
        new(880, 920, SegmentType.BrakingZone),
        new(920, 970, SegmentType.CornerApex, -85.0F),
        new(970, 1020, SegmentType.CornerExit, -85.0F),
        
        // Turn 5: Aintree (Left kink accelerating hard onto straight)
        new(1020, 1100, SegmentType.CornerApex, -20.0F),
        new(1100, 1750), // Wellington Straight (First DRS Zone)

        // --- SECTION 2: Brooklands to National Straight ---
        // Turn 6: Brooklands (Hard Left entry)
        new(1750, 1830, SegmentType.BrakingZone),
        new(1830, 1910, SegmentType.CornerApex, -50.0F),
        new(1910, 1950, SegmentType.CornerExit, -50.0F),
        
        // Turn 7: Luffield (Long, patient 180° Right-hander)
        new(1950, 1990, SegmentType.BrakingZone),
        new(1990, 2120, SegmentType.CornerApex, 55.0F),
        new(2120, 2180, SegmentType.CornerExit, 55.0F),
        
        // Turn 8: Woodcote (Sweeping Right toward Old Pit Straight)
        new(2180, 2300, SegmentType.CornerApex, 25.0F),
        new(2300, 2750), // National Pits Straight (Old Pit Straight line)

        // --- SECTION 3: The High-Speed Complex (Copse to Hangar) ---
        // Turn 9: Copse (Blistering high-speed Right)
        new(2750, 2790, SegmentType.BrakingZone), // Minor weight-transfer tap
        new(2790, 2880, SegmentType.CornerApex, 35.0F),
        new(2880, 2960, SegmentType.CornerExit, 35.0F),
        new(2960, 3100), // Run up to Maggotts
        
        // Turns 10 & 11: Maggotts (Ultra-fast Left to Right entry)
        new(3100, 3170, SegmentType.CornerApex, -30.0F),
        new(3170, 3250, SegmentType.CornerApex, 35.0F),
        
        // Turns 12 & 13: Becketts (Heavy lateral force Right to Left)
        new(3250, 3310, SegmentType.BrakingZone),
        new(3310, 3380, SegmentType.CornerApex, 55.0F),
        new(3380, 3450, SegmentType.CornerApex, -45.0F),
        
        // Turn 14: Chapel (Left exit opening up the engine blast)
        new(3450, 3540, SegmentType.CornerExit, -25.0F),
        new(3540, 4350), // Hangar Straight (Longest flat-out section)

        // --- SECTION 4: Final Sector (Stowe to Club) ---
        // Turn 15: Stowe (High-speed trailing brake Right)
        new(4350, 4430, SegmentType.BrakingZone),
        new(4430, 4520, SegmentType.CornerApex, 40.0F),
        new(4520, 4600, SegmentType.CornerExit, 40.0F),
        new(4600, 4950), // Vale Straight
        
        // Turn 16: Vale (Hard deceleration into sharp Left)
        new(4950, 5040, SegmentType.BrakingZone),
        new(5040, 5090, SegmentType.CornerApex, -70.0F),
        
        // Turns 17 & 18: Club (Double-apex Right sweeping to the line)
        new(5090, 5160, SegmentType.CornerApex, 45.0F),
        new(5160, 5260, SegmentType.CornerApex, 30.0F),
        new(5260, 5350, SegmentType.CornerExit, 30.0F),
        
        // Final dash to finish line
        new(5350, 5891) 
    },
    52, 
    "Silverstone Circuit"
);

    public TelemetrySender(string ipAddress = "127.0.0.1", int port = 5000)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    public void StartStreaming()
    {
        IPAddress targetIp = IPAddress.Parse(_ipAddress);
        IPEndPoint endPoint = new IPEndPoint(targetIp, _port);
        
        using UdpClient client = new UdpClient();
        
        Console.WriteLine($"Starting stream to {_ipAddress}:{_port}.");

        int packetCount = 1;

        VirtualCar car = new VirtualCar("VER-3", silverstone, new VirtualCarConfig());

        int intervalMs = new ParametersPackage().IntervalMs;
        while (true)
        {
            car.Update();

            TelemetryPackage package = car.CreateSnapshot();

            string jsonString = JsonSerializer.Serialize(package);
            byte[] data = Encoding.UTF8.GetBytes(jsonString);

            client.Send(data, data.Length, endPoint);
            Console.WriteLine($" - {packetCount}: Sent data for CAR-{package.CarId}");

            packetCount++;
            Thread.Sleep(intervalMs); 
        }
    }
}


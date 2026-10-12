using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using RaceSim.Shared;

var builder = WebApplication.CreateBuilder(args);

// Core Thread-Safe States
builder.Services.AddSingleton<TelemetryState>();
builder.Services.AddSingleton<SessionState>();

// Background Simulation Consumer
builder.Services.AddHostedService<UdpTelemetryListener>();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", 
    p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseCors("AllowAll");

#region HTTP Minimal API Endpoints

// GET Live Telemetry Stream
app.MapGet("/api/telemetry", (TelemetryState state) =>
{
    var latest = state.GetLatest();
    return latest is null ? Results.NotFound("No telemetry received yet.") : Results.Ok(latest);
});

var silverstone = new Track(
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

// GET Static Metadata / Configurations
app.MapGet("/api/parameters", (SessionState state) =>
{
    var paramsPkg = state.GetParameters();
    if (paramsPkg == null)
    {
        state.Initialize(ParametersPackage.Create(new VirtualCarConfig(), silverstone));
        paramsPkg = state.GetParameters();
    }
    return paramsPkg is null ? Results.NotFound("Session parameters not initialized.") : Results.Ok(paramsPkg);
});

#endregion

app.Run();

#region Infrastructure Storage Classes

public class TelemetryState
{
    private readonly object _lock = new();
    private TelemetryPackage? _latestPackage;

    public void Update(TelemetryPackage package)
    {
        lock (_lock) { _latestPackage = package; }
    }

    public TelemetryPackage? GetLatest()
    {
        lock (_lock) { return _latestPackage; }
    }
}

public class SessionState
{
    private readonly object _lock = new();
    private ParametersPackage? _parameters;

    public void Initialize(ParametersPackage parameters)
    {
        lock (_lock) { _parameters = parameters; }
    }

    public ParametersPackage? GetParameters()
    {
        lock (_lock) { return _parameters; }
    }
}

// DTO wrapper for initial ingestion
public record SessionInitializationRequest(
    string TrackName, 
    int NumberOfLaps, 
    VirtualCarConfig CarConfig, 
    TrackSegment[] TrackSegments
);

public class UdpTelemetryListener : BackgroundService
{
    private readonly TelemetryState _state;
    private readonly int _port = 5000;

    public UdpTelemetryListener(TelemetryState state)
    {
        _state = state;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udpClient = new UdpClient(_port);
        Console.WriteLine($"[Engine] Listening for UDP Telemetry on port {_port}...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = await udpClient.ReceiveAsync(stoppingToken);
                string jsonString = Encoding.UTF8.GetString(result.Buffer);
                var package = JsonSerializer.Deserialize<TelemetryPackage>(jsonString);

                if (package != null)
                {
                    _state.Update(package);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Console.WriteLine($"Error receiving UDP packet: {ex.Message}");
            }
        }
    }
}
#endregion
using System.Diagnostics;
using System.Xml.Schema;

namespace RaceSim.Shared;

public class VirtualCar
{
    public Track Track { get; }
    public VirtualCarConfig Config { get; }

    public string CarId { get; }
    private bool _isStopped = false;
    private string _status = "NOT_STARTED";

    private RaceFlag _flag = RaceFlag.Green;
    private bool _isNewLap = true;
    private int _lapNumber = 1;
    private float _lapTimeMs = 0.0F;

    private float _throttle = 0.0F;
    private float _brake = 0.0F;
    private float _steeringAngle = 0;

    private float _speed = 0.0F;
    private int _gear = 1;
    private int _rpm = 1000;
    private float _engineTemperature = 30.0f;

    private float _remainingFuel = 0.0F;
    private float _distanceTravelled = 0.0F;

    private TyreMetrics[] _currentTyres;
    private float _tyreTemperature = 30F;
    
    private float _previousLapDistance = 0.0F;
    

    public VirtualCar(string carId, Track track, VirtualCarConfig config)
    {
        CarId = carId;
        Track = track;
        Config = config;

        _remainingFuel = config.RequiredFuel(track);

        _currentTyres = new TyreMetrics[]
        {
            new() { Temperature = 20.0F, PutOnAt = DateTimeOffset.Now},
            new() { Temperature = 20.0F, PutOnAt = DateTimeOffset.Now},
            new() { Temperature = 20.0F, PutOnAt = DateTimeOffset.Now},
            new() { Temperature = 20.0F, PutOnAt = DateTimeOffset.Now},
        };
    }

    public void Update()
    {
        if (_isStopped) return;
        if (_remainingFuel <= 0.0F)
        {
            Stop();
            _flag = RaceFlag.Black_With_Orange_Disc;
            _status = "DNF";
            _isStopped = true;
            return;
        }

        float _currentLapDistance = Track.GetLapDistance(_distanceTravelled);
        bool isNewLap = _previousLapDistance > _currentLapDistance;
        _previousLapDistance = _currentLapDistance;

        if (isNewLap)
        {
            _lapNumber++;
            _isNewLap = true;

            if (_lapNumber > Track.Laps)
            {
                Stop();
                _flag = RaceFlag.Chequered;
                _status = "FINISHED";
                _isStopped = true;
                return;
            }
        } else
        {
            if (_isNewLap)
            { // Last tick was a new lap
                _isNewLap = false;    
                _lapTimeMs = 100F; // Account for this tick
            }
            
        }
        _lapTimeMs += 100;
        _status = "RUNNING";

        TrackSegment segment = Track.GetSegmentAt(_distanceTravelled);
        _flag = Config.ChooseFlag(_flag, segment.Type);
        float targetThrottle = segment.GetTargetThrottle();
        float targetBrake = segment.GetTargetBrake();
        float targetSteering = segment.GetTargetSteering();

        _throttle += (targetThrottle - _throttle) * 0.3F;
        _brake += (targetBrake - _brake) * 0.8f;
        _steeringAngle += (targetSteering - _steeringAngle) * 0.4f;
        _steeringAngle = Config.Randomise(_steeringAngle, 0.1F);

        float drag = 0.0005f * _speed * _speed;
        float engineForce = _throttle * Config.MaxEngineForce;
        float brakeForce = _brake * Config.MaxBrakeForce;

        float netAcceleration = engineForce - brakeForce - drag;
        _speed = Math.Max(0, _speed + (netAcceleration * 0.1f)); // 0.1s tick delta
        if (netAcceleration < 0 && _speed < 30) _speed=30;
        _distanceTravelled += (_speed/3.6f) * 0.1f;

        _gear = Math.Clamp(1 + (int)(_speed / Config.SpeedPerGear), 1, 8);
        float gearProgress = (_speed - ((_gear - 1) * Config.SpeedPerGear)) / Config.SpeedPerGear;
        if (_gear == 8)
        {
            gearProgress = Math.Min(gearProgress, 1.0f);
        }
        _rpm = (int)(Config.IdleRpm + (gearProgress * (Config.MaxRpm - Config.IdleRpm)));

        float baseIdleRate = 0.05f;
        float activeThrottleFactor = _throttle * (1.0f - baseIdleRate);

        float effectiveBurnRate = (baseIdleRate + activeThrottleFactor) * Config.FuelConsumptionRatePerKm;

        _remainingFuel -= (effectiveBurnRate * _speed * 0.1f) / 3600f;

        float rpmNormalized = _rpm / Config.MaxRpm;
        float combustionHeat = rpmNormalized * (5.5f + _throttle * 5f);

        float airflowCooling = _speed * 0.018f;
        float radiantCooling = (_engineTemperature - 30.0f) * 0.013f;
        float totalCooling = airflowCooling + radiantCooling;

        float netThermalDelta = combustionHeat - totalCooling;
        _engineTemperature += netThermalDelta * 0.1f;

        _engineTemperature = Config.Randomise(Math.Clamp(_engineTemperature, 30.0f, 130.0f), variance:0);

        float tireFrictionHeat = (_speed * 0.09f) + (Math.Abs(_steeringAngle) * 0.37f);
        _tyreTemperature = Config.Randomise(
            Math.Clamp(_tyreTemperature + (tireFrictionHeat - Config.CoolingRate) * 0.1f, 30f, 130f)
            );
        _currentTyres = new TyreMetrics[]
        {
            new() { Temperature = Config.Randomise(_tyreTemperature, 0.1F), PutOnAt = _currentTyres[0].PutOnAt},
            new() { Temperature = Config.Randomise(_tyreTemperature, 0.1F), PutOnAt = _currentTyres[1].PutOnAt},
            new() { Temperature = Config.Randomise(_tyreTemperature, 0.1F), PutOnAt = _currentTyres[2].PutOnAt},
            new() { Temperature = Config.Randomise(_tyreTemperature, 0.1F), PutOnAt = _currentTyres[3].PutOnAt},
        };
    }
    

    public TelemetryPackage CreateSnapshot()
    {
        return new TelemetryPackage
        {
            CarId = this.CarId,
            Status = _status,
            CreatedAt = DateTimeOffset.Now,
            Flag = _flag,
            IsNewLap = _isNewLap,
            LapNumber = _lapNumber,
            LapTimeMs = _lapTimeMs,
            Speed = _speed,
            Gear = _gear,
            EngineRevs = _rpm,
            Throttle = _throttle,
            Brake = _brake,
            EngineTemperature = _engineTemperature,
            SteeringAngle = _steeringAngle,
            FuelRemainingLitres = _remainingFuel,
            DistanceTravelled = _distanceTravelled,
            Tyres = _currentTyres
        };
    }

    public void Stop()
    {
        _speed = 0.0F;
        _gear = 0;
        _rpm = 1000;

        _steeringAngle = 0.0F;
        _throttle = 0.0F;
        _brake = 0.0F;
    }

    public void Reset()
    {
        Stop();

        _distanceTravelled = 0.0F;
        _remainingFuel = Config.RequiredFuel(Track);
        _lapNumber = 1;

        _currentTyres = new TyreMetrics[]
        {
            new() { Temperature = 30.0F, PutOnAt = DateTimeOffset.Now},
            new() { Temperature = 30.0F, PutOnAt = DateTimeOffset.Now},
            new() { Temperature = 30.0F, PutOnAt = DateTimeOffset.Now},
            new() { Temperature = 30.0F, PutOnAt = DateTimeOffset.Now},
        };
    }

}
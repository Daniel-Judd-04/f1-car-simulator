
export interface TyreMetric {
    temperature: number;
    putOnAt: string;
}

export interface TelemetryPackage {
    carId: string;
    status: string;
    createdAt: string;

    flag: RaceFlag;
    isNewLap: boolean;
    lapNumber: number;
    lapTimeMs: number;

    throttle: number;
    brake: number;
    steeringAngle: number;

    speed: number;
    gear: number;
    engineRevs: number;
    engineTemperature: number;
    
    fuelRemainingLitres: number;
    distanceTravelled: number;
    
    tyres: TyreMetric[];
}

export interface ParametersPackage {
    track: Track;
    initialFuel: number;
    maxSpeed: number;
    lapLength: number;
    intervalMs: number;
}

export interface Track {
    laps: number;
    name: string;
    segments: TrackSegment[]
}

export interface TrackSegment {
    startDistance: number;
    endDistance: number;
    type: number;
    targetSteering: number;
}

export type RaceFlag = 
    | 'Chequered'
    | 'Green'
    | 'Yellow'
    | 'Red'
    | 'White'
    | 'Blue'
    | 'Black'
    | 'Yellow_And_Red_Striped'
    | 'Black_With_Orange_Disc'
    | 'Black_And_White';
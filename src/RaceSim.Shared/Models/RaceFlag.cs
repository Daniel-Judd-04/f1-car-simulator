using System.Text.Json.Serialization;

namespace RaceSim.Shared;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RaceFlag
{
    Chequered,
    Green,
    Yellow,
    Red,
    Black_With_Orange_Disc,
}
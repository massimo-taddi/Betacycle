using System.Reflection.Emit;

namespace BetaCycleAPI.Models.Enums
{
    public enum DBCheckResponse
    {
        NotFound = 0,
        FoundNotMigrated,
        FoundMigrated,
        FoundAdmin
    }
}

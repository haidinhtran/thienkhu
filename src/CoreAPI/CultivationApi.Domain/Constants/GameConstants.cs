namespace CultivationApi.Domain.Constants;

public static class CharacterStates
{
    public const string Idle = "IDLE";
    public const string InExploration = "IN_EXPLORATION";
    public const string InCombat = "IN_COMBAT";
}

public static class AuditLogTypes
{
    public const string ExpGain = "EXP_GAIN";
    public const string ExplorationReward = "EXPLORATION_REWARD";
    public const string ItemConsume = "ITEM_CONSUME";
    public const string Breakthrough = "BREAKTHROUGH";
}

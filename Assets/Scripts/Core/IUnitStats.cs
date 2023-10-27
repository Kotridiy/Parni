namespace Assets.Scripts.Core
{
    public interface IUnitStats
    {
        int MaxHealth { get; }
        float Health { get; set; }
        float HealthPercent { get; }

        float Damage { get; }
        float Range { get; }
        float AttackSpeed { get; }

        int VisualRange { get; }
        int MemoryVolume { get; }
        float ProgressFactor { get; }
        float Caution { get; }

        float SpeedMultiplier { get; }
        int Weight { get; }
        int Worth { get; }
    }
}

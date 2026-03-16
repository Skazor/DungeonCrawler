namespace DungeonCrawler;

public enum ClassType { Warrior, Mage, Rogue, Paladin, Hunter }

public class CharacterClass
{
    public ClassType Type { get; }
    public string Name { get; }
    public string Description { get; }
    public int MaxHealth { get; }
    public int AttackPower { get; }
    public int Defense { get; }  // reduserer skade man tar
    public double CritChance { get; }  // 0.0 - 1.0, sjanse for kritisk treff
    public double MoveDelay { get; }  // lavere = raskere

    public CharacterClass(ClassType type)
    {
        Type = type;
        switch (type)
        {
            case ClassType.Warrior:
                Name = "WARRIOR";
                Description = "High HP and defense. Dominates in melee combat.";
                MaxHealth = 150;
                AttackPower = 18;
                Defense = 8;
                CritChance = 0.05;
                MoveDelay = 0.15;
                break;
            case ClassType.Mage:
                Name = "MAGE";
                Description = "Low HP but devastating magical damage.";
                MaxHealth = 70;
                AttackPower = 35;
                Defense = 2;
                CritChance = 0.10;
                MoveDelay = 0.15;
                break;
            case ClassType.Rogue:
                Name = "ROGUE";
                Description = "Fast and deadly. High crit chance.";
                MaxHealth = 90;
                AttackPower = 22;
                Defense = 4;
                CritChance = 0.35;
                MoveDelay = 0.10;
                break;
            case ClassType.Paladin:
                Name = "PALADIN";
                Description = "Tank with self-healing abilities.";
                MaxHealth = 130;
                AttackPower = 14;
                Defense = 12;
                CritChance = 0.05;
                MoveDelay = 0.18;
                break;
            case ClassType.Hunter:
                Name = "HUNTER";
                Description = "Ranged attacks and traps. Keeps enemies at distance.";
                MaxHealth = 100;
                AttackPower = 20;
                Defense = 5;
                CritChance = 0.20;
                MoveDelay = 0.12;
                break;
        }
    }
}
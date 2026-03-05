namespace DungeonCrawler.Interfaces;

/*Interface som sier: "Dette er noe som kan ta skade i spillet".
  Brukes av spilleren (Player) og fiender (Enemy) for å behandle dem likt i kamplogikken.
  Fordelen med dette er at det blir en felles liste over ting som kan skades, og bruke polymorfisme.
  HealthPotion implementerer ikke dette interfacet da den ikke kan ta skade.*/
public interface IDamageable
{
    /*Hvor mye helse (HP) tingen har igjen.
      Kan leses fra utsiden, men endres kun via TakeDamage().*/
    int Health { get; }

    /*Sjekker raskt om tingen fortsatt lever (HP > 0).
      Brukes for å vite om en fiende skal fjernes eller om spilleren har tapt.*/
    bool IsAlive { get; }

    /*Hvor mye skade tingen gjør når den angriper.
      Spilleren bruker sin AttackPower mot fiender, fiender bruker sin mot spilleren.*/
    int AttackPower { get; }

    /*Reduserer helsen med gitt mengde skade.
      Implementeres i hver klasse (Player og Enemy), vanligvis med sjekk mot 0 HP.
      "amount" Hvor mye skade som påføres*/
    void TakeDamage(int amount);
}
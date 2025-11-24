using System;
using System.Runtime.CompilerServices;

public abstract class Spaceship
{
    public int Energy { get; protected set; }
    public int Shields { get; protected set; }


    protected Spaceship(int energy, int shields)
    {
        this.Energy = energy;
        this.Shields = shields;

    }

    public virtual void TakeDamage(int damage)
    {
        if (damage <= Shields)
        {
            Shields -= damage;
            return;
        }

        int remainingDamage = damage - Shields;
        Shields = 0;
        Energy = Math.Max(0, Energy - remainingDamage);
    }

    public abstract void Attack(Spaceship target);

    public virtual bool IsFlyable()
    {
        return Energy > 0;
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Energy: {Energy}, Shields: {Shields}]";
    }

}




public class  Scout : Spaceship 
{
    float dodgeChance;
    int laserDamage;
    Random random;

    public Scout(int energy, int shields, float dodgeChance, int laserDamage, Random random)
    : base(energy, shields)
    {
        this.dodgeChance = Math.Clamp(dodgeChance, 0f, 1f);
        this.laserDamage = laserDamage;
        this.random = random; // koristi vanjski generator
    }


    public override void Attack(Spaceship target)
    {
        if(target.Shields == 0)
        {
            target.TakeDamage(laserDamage);
        }
        
    }

    public override void TakeDamage(int damage)
    {
        if(random.NextDouble() < dodgeChance)
        {
            return;
        }
        else        
        {
            base.TakeDamage(damage);
        }
    }

    public override string ToString() 
    { 
        return $"{base.ToString()}, DodgeChance: {dodgeChance}, LaserDamage: {laserDamage}";
    }

}












public class Bomber : Spaceship
{
    int bombCount;
    int bombDamage;
    int extraShield;


    private const float BurstScale = 0.3f;
    public Bomber(int energy, int shields, int bombCount, int bombDamage, int extraShield) : base(energy, shields + extraShield)
    {
        this.bombCount = bombCount;
        this.bombDamage = bombDamage;
        this.extraShield = extraShield;
    }

    public override void Attack(Spaceship target)
    {
       if(bombCount>0)
       {
            int dmg = bombDamage;

            if (target.Shields == 0)
            {
                dmg= (int)(dmg*(1+BurstScale));
            }

            target.TakeDamage(dmg);
                        bombCount--;
        }
    }


    public override void TakeDamage(int damage)
    {
        if(extraShield>0)
        {
            int shieldAbsorb = Math.Min(extraShield, damage);
            extraShield -= shieldAbsorb;
            damage -= shieldAbsorb;
        }

            if(damage>0)
            {
                base.TakeDamage(damage);
        }
    }


    public override string ToString()
    {
        return $"{base.ToString()}, BombCount: {bombCount}, BombDamage: {bombDamage}, ExtraShield: {extraShield}";
    }


}







class Program
{
    static void Main()
    {
        QuickTest();
    }

    public static void QuickTest()
    {
        Random generator = new Random();
        Spaceship[] ships = new Spaceship[]
        {
            new Scout(10, 0, 0.5f, 2, generator),
            new Bomber(20, 5, 3, 4, 2),
        };

        Console.WriteLine("Prije napada:");
        foreach (var ship in ships)
            Console.WriteLine(ship);

        // Scout napada Bombera
        Console.WriteLine("\nScout napada Bombera:");
        ships[0].Attack(ships[1]);
        // Bomber napada Scouta
        Console.WriteLine("\nBomber napada Scouta:");
        ships[1].Attack(ships[0]);
        // Scout ponovno napada Bombera
        Console.WriteLine("\nScout ponovno napada Bombera:");
        ships[0].Attack(ships[1]);

        Console.WriteLine("\nNakon 3 napada:");
        foreach (var ship in ships)
            Console.WriteLine(ship);
    }
}

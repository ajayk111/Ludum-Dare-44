﻿using System.Collections.Generic;
using UnityEngine;

public class Assassination : AttackCard
{
	public Assassination()
	{
        NumTargets = 1;
        AttackDamage = 0;
        HealthCost = 4;
        Description = "Assassination\n4 HP\nDestroy a single target with 50% or less health remaining.";
        CardArt = Resources.Load<Sprite>("CardArt/assassination");
    }

    // Casts the card effect on an enemy with 25% or less health
    public override void Cast(List<Entity> enemies)
    {
        Player P = Player.PlayerRef;
        if (enemies.Count == 1 && enemies[0].CurrentHealth <= enemies[0].MaxHealth/2f && P.CanCast(this))
        {
            P.TakeCastDamage(HealthCost);
            enemies[0].CurrentHealth = 0;
        }
    }
}

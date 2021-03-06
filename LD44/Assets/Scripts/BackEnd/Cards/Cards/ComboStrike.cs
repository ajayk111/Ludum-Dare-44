﻿using System.Collections.Generic;
using UnityEngine;

public class ComboStrike : AttackCard
{
	public ComboStrike()
	{
        NumTargets = 1;
        AttackDamage = 5;
        HealthCost = 5;
        Description = "Combo Strike\n3 HP\nDeal 5 damage to a single enemy. " +
                      "If they are above 50% health afterwards, deal 5 more.";
        CardArt = Resources.Load<Sprite>("CardArt/combo_strike");
    }

    // Casts the card effect on a single enemy, twice if enemy still has 50% health
    public override void Cast(List<Entity> enemies)
    {
        Player P = Player.PlayerRef;
        if (enemies.Count == 1 && P.CanCast(this))
        {
            P.TakeCastDamage(HealthCost);
            enemies[0].TakeHitDamage(AttackDamage);
            if(enemies[0].CurrentHealth >= enemies[0].MaxHealth / 2)
            {
                enemies[0].TakeHitDamage(AttackDamage);
            }
        }
    }
}

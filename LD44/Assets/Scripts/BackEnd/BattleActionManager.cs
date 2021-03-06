﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class BattleActionManager
{
    public static BattleActionState State = BattleActionState.HandleTurn;
    private static CardMB _activeCard;
    private static List<Entity> _targets = new List<Entity>();

    public static void ClearActive()
    {
        if (State == BattleActionState.HandleTurn)
            return;

        _activeCard = null;
        _targets.Clear();
        State = BattleActionState.SelectCard;
    }

    public static void Click(CardMB cardMb)
    {
        if (State != BattleActionState.SelectCard)
            return;
        if (!Player.PlayerRef.CanCast(cardMb.MeCard))
            return;

        _activeCard = cardMb;
        State = BattleActionState.SelectTargets;
    }

    public static void Click(Entity entity)
    {
        if (State != BattleActionState.SelectTargets || _activeCard == null)
            return;

        _targets.Add(entity);

        if (_activeCard.MeCard.NumTargets > _targets.Count)
            return;

        State = BattleActionState.HandleTurn;
        if (_activeCard.MeCard.NumTargets < _targets.Count)
        {
            Debug.Log("Too many targets! Card: " + _activeCard.GetType().FullName);
            _activeCard.MeCard.Cast(_targets.Take(_activeCard.MeCard.NumTargets).ToList());
        }
        else if (_activeCard.MeCard.NumTargets == _targets.Count)
        {
            _activeCard.MeCard.Cast(_targets);
        }

        Deck.Discards.Add(_activeCard.MeCard);
        _activeCard.DestroyCard();
        _activeCard = null;
        _targets.Clear();
        Deck.DrawCard();
        RoomMB.ActiveRoom.StartCoroutine(WaitAndAttackPlayer());
    }

    private static IEnumerator WaitAndAttackPlayer()
    {
        yield return new WaitForSeconds(1.6f);
        if (BattleManager.BattleManagerRef.Enemies.Count != 0)
        {
            List<Entity> enemies = BattleManager.BattleManagerRef.Enemies;
            Entity attacking = enemies[Random.Range(0, enemies.Count)];
            attacking.MeEntityMB.Attack();
            yield return new WaitForSeconds(0.7f);
            Player.PlayerRef.TakeHitDamage(attacking.GetAttackDamage());
            yield return new WaitForSeconds(1);

            if (CheckSpellsUsable())
                State = BattleActionState.SelectCard;
        }
    }

    public static bool CheckSpellsUsable()
    {
        // If can't use spells, dump hand, draw new
        foreach (CardMB cMb in Deck.Hand)
            if (Player.PlayerRef.CanCast(cMb.MeCard))
                return true;

        if (Player.PlayerRef.CurrentHealth > 0)
        {
            int handSize = Deck.Hand.Count;
            for (int i = handSize - 1; i >= 0; i--)
                Deck.Discard(Deck.Hand[i]);

            Deck.DrawCard();
            Deck.DrawCard();
            Deck.DrawCard();

            // If still can't use cards after a new draw 3, then lose
            foreach (CardMB cMb in Deck.Hand)
                if (Player.PlayerRef.CanCast(cMb.MeCard))
                    return true;
        }

        
        BattleManager.EndGameObject.gameObject.SetActive(true);
        BattleManager.EndGameObject.Activate(false);
        return false;
    }
}

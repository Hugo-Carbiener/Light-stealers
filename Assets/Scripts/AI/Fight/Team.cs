using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Team
{
    public Factions faction { get; private set; }
    public List<FightModule> fighters { get; private set; }
    private int currentFighterIndex;

    public Team(Factions faction, List<FightModule> fighters)
    {
        this.faction = faction;
        this.fighters = fighters;
        currentFighterIndex = 0;
    }

    public IEnumerator PlayTurn(Team ennemyTeam)
    {
        Debug.Log("FIGHT : " + fighters + " team starts " + currentFighterIndex + "nth fighter turn");
        yield return fighters[currentFighterIndex].PlayTurn(ennemyTeam);
        currentFighterIndex++;
        if (currentFighterIndex == fighters.Count) currentFighterIndex = 0;
    }

    public void OnFightEnd()
    {
        fighters.ForEach(fighter => fighter.OnFightEnd());
    }

    public void AddFighter(FightModule fighter)
    {
        fighters.Add(fighter);
    }

    public bool IsAlive()
    {
        return fighters.Exists(fighter => fighter.IsAlive());
    }
}

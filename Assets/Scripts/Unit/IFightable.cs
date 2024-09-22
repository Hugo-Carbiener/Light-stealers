using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFightable
{
    public void OnDeath();
    public FightModule GetFightModule();
    public Vector2Int GetPosition();
    public bool IsBuilding();
    public bool IsTroop();
    public bool IsValidTargetForFight(Factions attackerFaction);
}

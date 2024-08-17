using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Basic component for game agents
 */
public class Unit : MonoBehaviour, IFightable
{
    [Header("Modules")]
    [SerializeField] private AIAgent agent;
    [SerializeField] private MovementModule movementModule;
    [SerializeField] private FightModule fightModule;
    [Header("Consumption")]
    [SerializeField] private int foodAmount;
    [Header("Spawn")]
    [SerializeField] private DayNightCyclePhases troopAppearAtStartOfPhase;

    private void Awake()
    {
        //Assert.IsNotNull(behaviorModule);
        Assert.IsNotNull(movementModule);
        Assert.IsNotNull(fightModule);
    }

    private Vector2Int position;

    public void OnApparition(Vector2Int startingPos)
    {
        if (fightModule.GetFaction() == Factions.Monsters)
        {
            System.Random rnd = new System.Random();
            int index = rnd.Next(FractureManager.Instance.fractures.Count);
            SetPosition(FractureManager.Instance.fractures[index].coordinates);
            return;
        }
        SetPosition(startingPos);
    }
    
    public void OnDeath()
    {
        Debug.Log("FIGHT : " + this.gameObject.name + " died during the fight(" + fightModule.health + "hp)");
        if (fightModule)
        {
            Fight fight = FightManager.Instance.GetFight(fightModule);
            if (fight != null)
            {
                fight.RemoveFighter(fightModule);
            }
            fightModule.OnDeath.Invoke();
        }
        UnitManager.Instance.DeactivateUnit(this);
    }

    public int GetFoodConsummed() { return foodAmount; }

    public AIAgent GetAgent() { return agent; }   
    public MovementModule GetMovementModule() { return movementModule; }
    public FightModule GetFightModule() { return fightModule; }
    public Vector2Int GetPosition() { return position; }
    public void SetPosition(Vector2Int position)
    {
        this.position = position;
        movementModule.currentCell = position;
        transform.position = TilemapManager.Instance.groundTilemap.CellToWorld((Vector3Int) position);
    }
    public DayNightCyclePhases GetTroopApparitionPhase() { return troopAppearAtStartOfPhase; }

    public bool Attack(Vector2Int location)
    {
        if (movementModule.currentCell != location) return false;
        return fightModule.Attack(location, agent.assignedTask);
    }

    public bool IsValidTargetForFight(Factions attackerFaction)
    {
        return fightModule.IsValidTarget()
            && attackerFaction != fightModule.GetFaction();
    }

    public bool IsBuilding()
    {
        return false;
    }

    public bool IsTroop()
    {
        return true;    
    }
}

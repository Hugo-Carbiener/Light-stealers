using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Basic component for game agents
 */
public class Unit : MonoBehaviour, ITargettable
{
    [Header("Modules")]
    [SerializeField] private BehaviorModule behaviorModule;
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

    public Vector2Int position
    {
        get { return position; }

        set
        {
            movementModule.currentCell = value;
            transform.position = TilemapManager.Instance.groundTilemap.CellToWorld((Vector3Int) value);
        }
    }

    public void OnApparition(Vector2Int startingPos)
    {
        if (fightModule.GetFaction() == Factions.Monsters)
        {
            System.Random rnd = new System.Random();
            int index = rnd.Next(FractureManager.Instance.fractures.Count);
            position = FractureManager.Instance.fractures[index].coordinates;
            return;
        }
        position = startingPos;
    }
    
    public void OnDeath()
    {
        UnitManager.Instance.DeactivateUnit(this);
    }

    public int GetFoodConsummed() { return foodAmount; }

    public BehaviorModule GetBehaviorModule() { return behaviorModule; }   
    public MovementModule GetMovementModule() { return movementModule; }
    public FightModule GetFightModule() { return fightModule; }
    public Vector2Int GetPosition() { return position; }
    public DayNightCyclePhases GetTroopApparitionPhase() { return troopAppearAtStartOfPhase; }

    public bool Attack(Vector2Int location)
    {
        if (movementModule.currentCell != location) return false;
        return fightModule.Attack(location);
    }
}

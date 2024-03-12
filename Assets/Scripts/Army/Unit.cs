using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Basic component for game agents
 */
public class Unit : MonoBehaviour
{
    [Header("Modules")]
    [SerializeField] private BehaviorModule behaviorModule;
    [SerializeField] private MovementModule movementModule;
    [Header("Consumption")]
    [SerializeField] private int foodAmount;

    private void Awake()
    {
        Assert.IsNotNull(behaviorModule);
        Assert.IsNotNull(movementModule);
    }

    public Vector2Int position
    {
        get { return this.position; }

        set
        {
            movementModule.currentCell = value;
        }
    }
    
    public void die()
    {
        gameObject.SetActive(false);
        ArmyManager.Instance.armySize--;
        ArmyManager.Instance.armyUnits.Remove(this);
    }

    public int getFoodConsummed() { return foodAmount; }
}

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
    [SerializeField] private Movement movementModule;
    [Header("Consumption")]
    [SerializeField] private int foodAmount;

    private void Awake()
    {
        Assert.IsNotNull(behaviorModule);
        Assert.IsNotNull(movementModule);
    }

    public Vector2Int position { get; set; } = Vector2Int.zero;
    
    public void die()
    {
        gameObject.SetActive(false);
        ArmyManager.Instance.armySize--;
        ArmyManager.Instance.armyTroops.Remove(this);
    }

    public int getFoodConsummed() { return foodAmount; }
}

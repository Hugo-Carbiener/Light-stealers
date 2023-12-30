using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Basic component for game agents
 */
public class Troop : MonoBehaviour
{
    [Header("Consumption")]
    [SerializeField] private int foodAmount;

    public Vector2Int position { get; set; }

    public void die()
    {
        gameObject.SetActive(false);
        ArmyManager.Instance.armySize--;
        ArmyManager.Instance.armyTroops.Remove(this);
    }

    public int getFoodConsummed() { return foodAmount; }
}

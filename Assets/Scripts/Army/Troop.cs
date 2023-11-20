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

    public void die()
    {
        gameObject.SetActive(false);
        ArmyManager.Instance.armySize--;
        ArmyManager.Instance.armyTroops.Remove(this);
        HousingUIManager.Instance.UpdateUIComponent();
    }

    public int getFoodConsummed() { return foodAmount; }
}

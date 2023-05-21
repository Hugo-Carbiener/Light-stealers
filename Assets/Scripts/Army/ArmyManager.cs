using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generic manager for gamr agents
 */
public class ArmyManager : MonoBehaviour
{
    private static ArmyManager instance;

    public static ArmyManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<ArmyManager>();
            }
            return instance;
        }
    }

    [Header("Housing variables")]
    [SerializeField] private int initialHousingSize;
    public int housingSize { private set; get; }
    public int armySize { private set; get; } 

    [Header("Army variables")]
    [SerializeField] private GameObject basicTroopPrefab;

    // pool variables
    private List<Troop> armyTroopPool;

    private void Awake()
    {
        armySize = 0;
        housingSize = initialHousingSize;
        armyTroopPool = new List<Troop>();
    }

    private void initArmyPool()
    {
        for (int i = 0; i < housingSize; i++)
        {
            instantiateNewBasicTroopInPool();
        }
    }

    private void updateArmyPool()
    {
        if (armyTroopPool.Count >= housingSize) return;
        for (int i = armyTroopPool.Count; i < housingSize; i++)
        {
            instantiateNewBasicTroopInPool();
        }
    }

    private void instantiateNewBasicTroopInPool()
    {
        if (armyTroopPool.Count >= housingSize) return;

        GameObject instantiatedObject = Instantiate(basicTroopPrefab);
        instantiatedObject.SetActive(false);
        
        Troop troop;
        if (instantiatedObject.TryGetComponent<Troop>(out troop)) {
            armyTroopPool.Add(troop);
        } else
        {
            Debug.LogError("Troop does not have Troop component.");
        }
    }

    private Troop? getFirstAvailableTroop()
    {
        foreach (Troop troop in armyTroopPool)
        {
            if (!troop.gameObject.activeInHierarchy)
            {
                return troop;
            }
        }
        return null;
    }
}

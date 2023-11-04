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

    [Header("Components")]
    [SerializeField] private Transform ArmyPoolContainer;

    [Header("Housing variables")]
    [SerializeField] private int initialHousingSize;
    public int housingSize { private set; get; }
    public int armySize { set; get; } 

    [Header("Army variables")]
    [SerializeField] private GameObject basicTroopPrefab;
    [SerializeField] private DayNightCyclePhases createTroopsAtStartOfPhase;
    [SerializeField] private Vector3 startingPos; // will be bound to the building generation troops in the future

    // pool variables
    private List<Troop> armyTroopPool;      // all troops, active and inactive
    public List<Troop> armyTroops { get; private set; }         // active troops

    private void Awake()
    {
        armySize = 0;
        housingSize = initialHousingSize;
        armyTroopPool = new List<Troop>();
        armyTroops = new List<Troop>();
        initArmyPool();
        HousingUIManager.Instance.UpdateUIComponent();
    }

    private void Start()
    {
        DayNightCycleManager.OnCyclePhaseStart += dailyArmyUpdate;
    }


    /**
     * Init pool
     */
    private void initArmyPool()
    {
        for (int i = 0; i < housingSize; i++)
        {
            instantiateNewBasicTroopInPool();
        }
    }

    /**
     * Add troops to the pool according to the housing manager
     */
    private void updateArmyPool()
    {
        if (armyTroopPool.Count >= housingSize) return;
        for (int i = armyTroopPool.Count; i < housingSize; i++)
        {
            instantiateNewBasicTroopInPool();
        }
    }

    /**
     * Init a troop in the pool
     */
    private void instantiateNewBasicTroopInPool()
    {
        if (armyTroopPool.Count >= housingSize) return;

        GameObject instantiatedObject = Instantiate(basicTroopPrefab, ArmyPoolContainer);
        instantiatedObject.SetActive(false);
        
        Troop troop;
        if (instantiatedObject.TryGetComponent<Troop>(out troop)) {
            armyTroopPool.Add(troop);
        } else
        {
            Debug.LogError("Troop does not have Troop component.");
        }
    }

    /**
     * Army pool getter
     */
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

    /**
     * instantiate a troop
     */
    private void wakeTroop(Troop troop)
    {
        if (troop.gameObject.activeInHierarchy) return;

        troop.gameObject.SetActive(true);
        troop.transform.position = startingPos;
        armyTroops.Add(troop);
        armySize++;
        HousingUIManager.Instance.UpdateUIComponent();
    }

    /**
     * Goes through the troops and apply their food consumption. If there is not enough food, the troop dies
     */
    private void armyConsumption()
    {
        foreach (Troop troop in armyTroops)
        {
            if (!ResourceManager.Instance.modifyResources(ResourceTypes.Food, -troop.getFoodConsummed()))
            {
                troop.die();
            }
        }
    }

    /**
     * Temporary while troops appear each day
     */
    private void dailyArmyUpdate(DayNightCyclePhases phaseToInstanciateArmy)
    {
        if (phaseToInstanciateArmy == createTroopsAtStartOfPhase)
        {
            // army cannot be housed, house must have been destroyed during the day
            if (armySize > housingSize)
            {
                int i = 0;
                while (i < armySize - housingSize)
                {
                    Troop troop = armyTroops[0];
                    troop.die();
                    i++;
                }
            } else if (armySize < housingSize)
            {
                while (armySize < housingSize)
                {
                    Troop troop = getFirstAvailableTroop();
                    if (troop == null)
                    {
                        Debug.LogError("Could not find active troop, pool is not big enough.");
                        return;
                    }
                    wakeTroop(troop);
                }
            }
            armyConsumption();
        }
    }
}

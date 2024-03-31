using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Manager in chagre of keeping track of housing capacities. Housing capacity for monsters corresponds to the amount spawn during each wave.
 */
public class HousingManager : MonoBehaviour
{
    private static HousingManager instance;

    public static HousingManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<HousingManager>();
            }
            return instance;
        }
    }

    [Header("Housing variables")]
    [SerializeField] private SerializableDictionary<Factions, int> initialHousingSizes;

    public Dictionary<Factions, int> housingSizes { private set; get; }

    private void Awake()
    {
        housingSizes = initialHousingSizes.ToDictionnary();
    }

    public void UpdateHousingSize(Factions faction, int newHousingSize)
    {
        housingSizes[faction] = newHousingSize;
        UnitManager.Instance.UpdateUnitPool(faction);
    }
}

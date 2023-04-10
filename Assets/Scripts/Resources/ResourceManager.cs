using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ResourceManager>();
            }

            return _instance;
        }
    }

    [SerializeField] private GameObject resourceBox;
    private Dictionary<ResourceTypes, int> resources;

    private Text foodText;
    private Text woodText;
    private Text stoneText;

    private void Start()
    {
        if (!resourceBox) resourceBox = GameObject.Find("Resources Box");
        resources = new Dictionary<ResourceTypes, int>();
        foreach (ResourceTypes resource in ResourceTypes.GetValues(typeof(ResourceTypes)))
        {
            resources.Add(resource, 50);
        }

        Text[] texts = resourceBox.GetComponentsInChildren<Text>();
        foodText = texts[0];
        woodText = texts[1];
        stoneText = texts[2];

        UpdateUI();
    }

    public int getResource(ResourceTypes resourceType) { return resources[resourceType]; }

    public void ModifyResources(ResourceTypes resourceType, int amount)
    {
        int resourceAmount;
        if (resources.TryGetValue(resourceType, out resourceAmount))
        {
            resources[resourceType] = resourceAmount + amount;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        foodText.text = resources[ResourceTypes.Food].ToString();
        woodText.text = resources[ResourceTypes.Wood].ToString();
        stoneText.text = resources[ResourceTypes.Stone].ToString();
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class Building : MonoBehaviour
{
    private Vector3Int coordinates;

    private Vector3 worldCoordinates;

    [Header("Cost")]
    [SerializeField] private SerializableDictionary<ResourceTypes, int> costs;
    [Header("Construction")]
    [SerializeField] private float constructionDuration;
    private float constructionTimer = 0;
    public bool isConstructed { private set; get; } = false;
    public UnityEvent OnConstructionFInished { private set; get; }

    [Header("Deconstruction")]
    [SerializeField] private bool buildingCanBeDeconstructed;

    private void Awake()
    {
        Assert.AreNotEqual(costs.Count(), 0);
        OnConstructionFInished = new UnityEvent();
    }
    
    public void startConstruction()
    {
        StartCoroutine(startConstructionCoroutine());
    }

    private IEnumerator startConstructionCoroutine()
    {
        while (constructionTimer < constructionDuration)
        {
            constructionTimer += Time.deltaTime;
            yield return null;
        }
        isConstructed = true;
        OnConstructionFInished.Invoke();
    }

    public Vector3Int getCoordinates() { return coordinates; }

    public void setCoordinates(Vector3Int coords)
    {
        coordinates = coords;
        worldCoordinates = TilemapManager.Instance.buildingsTilemap.CellToWorld(coordinates);
    }

    public int getCost(ResourceTypes resourceType) { return costs.At(resourceType); }

    public float getCOnstructionDuration() { return constructionDuration; }

    public float getConstructionProgression() { return constructionTimer / constructionDuration; }

    public bool canBeDeconstructed() { return buildingCanBeDeconstructed; }

    
}

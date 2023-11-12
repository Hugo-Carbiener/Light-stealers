using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class BuildingConstructionUIManager : UIManager
{
    [Header("Button template")]
    [SerializeField] private VisualTreeAsset button;
    [Header("Icons")]
    [SerializeField] SerializableDictionary<BuildingType, Sprite> iconDictionnary;
    private static Camera mainCamera;

    private static BuildingConstructionUIManager _instance;
    public static BuildingConstructionUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BuildingConstructionUIManager>();
            }

            return _instance;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        Assert.AreEqual(iconDictionnary.Count(), Enum.GetNames(typeof(BuildingType)).Length);
        root = document.rootVisualElement;
    }

    /**
     * Add all the necessary buttons to the UI Component.
     */
    public void UpdateUIComponent(List<BuildingType> buildingsToDisplay)
    {
        buildingsToDisplay.ForEach(building => AddButton(building));
    }

    /**
     * Generate a custom button and add it to the UI component.
     */
    private void AddButton(BuildingType buildingType)
    {
        TemplateContainer buttonToAdd = button.Instantiate();
        InitButton(buttonToAdd, buildingType);
        VisualElement buttonContainer = root.Q<VisualElement>("BuildingsContainer");
        if (buttonContainer == null)
        {
            Debug.LogError("Could not find Visual element button container in Building construction panel");
            return;
        }

        Button buttonElement = buttonToAdd.Q<Button>("Button");
        if (buttonElement == null)
        {
            Debug.LogError("Could not find Button element in Building construction panel button");
            return;
        }
        buttonElement.clickable.clicked += delegate { BuildingFactory.Instance.build(buildingType); };  
        buttonContainer.Add(buttonToAdd);
    }

    /**
     * Opens the building construction panel and initialise it.
     */
    public void OpenBuildingConstrutionUI(List<BuildingType> buildingsToDisplay, Vector3Int cellPosition)
    {
        ResetUIComponent();
        UpdateUIComponent(buildingsToDisplay);
        SetPosition(TilemapManager.Instance.selectionTilemap.CellToWorld(cellPosition));
        SetVisibility(DisplayStyle.Flex);
    }

    /**
     * Hides the building construction panel.
     */
    public void CloseBuildingConstructionUI()
    {
        SetVisibility(DisplayStyle.None);
        ResetUIComponent();
    }

    private void InitButton(TemplateContainer button, BuildingType buildingType)
    {
        VisualElement icon = button.Q<VisualElement>("Icon");
        Label buildingName = button.Q<Label>("BuildingName");

        icon.style.backgroundImage = new StyleBackground(iconDictionnary.At(buildingType));
        buildingName.text = buildingType.ToString();
    }

    public void ResetUIComponent()
    {
        VisualElement buttonContainer = root.Q<VisualElement>("BuildingsContainer");
        buttonContainer.Clear();
    }

    public void UpdateWorldPosition()
    {
        if (root.style.display == DisplayStyle.None) return;

        Vector3Int cellPosition = TileSelectionManager.Instance.GetSelectedCellData().GetVector3Coordinates();
        Vector3 worldPosition = TilemapManager.Instance.selectionTilemap.CellToWorld(cellPosition);
        SetPosition(worldPosition);
    }
}

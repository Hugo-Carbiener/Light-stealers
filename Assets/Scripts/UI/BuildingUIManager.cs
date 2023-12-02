using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class BuildingUIManager : UIManager
{
    [Header("Button template")]
    [SerializeField] private VisualTreeAsset button;
    [Header("Icons")]
    [SerializeField] SerializableDictionary<BuildingType, Sprite> iconDictionnary;
    [Header("Deconstruction menu")]
    [SerializeField] private string deconstructionButtonLabel;
    [SerializeField] private Sprite deconstructionIcon;

    private static BuildingUIManager _instance;
    public static BuildingUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BuildingUIManager>();
            }

            return _instance;
        }
    }

    void Start()
    {
        Assert.AreEqual(iconDictionnary.Count(), Enum.GetNames(typeof(BuildingType)).Length);
        root = document.rootVisualElement;
    }

    // CONSTRUCTION

    /**
     * Add all the necessary buttons to the UI Component.
     */
    public void UpdateBuildingConstructionUIComponent(List<Building> buildingsToDisplay, Vector2Int cellPosition)
    {
         buildingsToDisplay.ForEach(building => AddBuildingButton(building, cellPosition));
    }

    /**
     * Generate a custom button and add it to the UI component.
     */
    private void AddBuildingButton(Building buildingType, Vector2Int cellPosition)
    {
        TemplateContainer buttonToAdd = button.Instantiate();
        InitBuildingButton(buttonToAdd, buildingType);
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
        buttonElement.clickable.clicked += delegate { BuildingFactory.Instance.Build(buildingType.type, cellPosition); };  
        buttonElement.clickable.clicked += delegate { CloseUI(); };
        buttonContainer.Add(buttonToAdd);
    }

    /**
     * Opens the building construction panel and initialise it.
     */
    public void OpenBuildingConstrutionUI(List<Building> buildingsToDisplay, Vector2Int cellPosition)
    {
        ResetUIComponent();
        if (buildingsToDisplay.Count == 0)
        {
            CloseUI();
            return;
        }
        UpdateBuildingConstructionUIComponent(buildingsToDisplay, cellPosition);
        SetPosition(TilemapManager.Instance.selectionTilemap.CellToWorld((Vector3Int) cellPosition));
        SetVisibility(DisplayStyle.Flex);
    }

    private void InitBuildingButton(TemplateContainer button, Building building)
    {
        VisualElement icon = button.Q<VisualElement>("Icon");
        Label buildingName = button.Q<Label>("BuildingName");
        Label foodAmountLabel = button.Q<Label>("FoodAmount");
        Label woodAmountLabel = button.Q<Label>("WoodAmount");
        Label stoneAmountLabel = button.Q<Label>("StoneAmount");

        icon.style.backgroundImage = new StyleBackground(iconDictionnary.At(building.type));
        buildingName.text = building.type.ToString();
        foodAmountLabel.text = building.GetCost(ResourceTypes.Food).ToString();
        woodAmountLabel.text = building.GetCost(ResourceTypes.Wood).ToString();
        stoneAmountLabel.text = building.GetCost(ResourceTypes.Stone).ToString();
        
        VisualElement costContainer = button.Q<VisualElement>("CostContainer");
        costContainer.style.display = DisplayStyle.Flex;
    }

    // DECONSTRUCTION

    /**
     * Add all the necessary buttons to the UI Component.
     */
    public void UpdateBuildingDeconstructionUIComponent(Vector2Int cellPosition)
    {
        AddDeconstructionButton(cellPosition);
    }

    /**
     * Generate a deconstruction button and add it to the UI component.
     */
    private void AddDeconstructionButton(Vector2Int cellPosition)
    {
        TemplateContainer buttonToAdd = button.Instantiate();
        InitDeconstructionButton(buttonToAdd);
        VisualElement buttonContainer = root.Q<VisualElement>("BuildingsContainer");
        if (buttonContainer == null)
        {
            Debug.LogError("Could not find Visual element button container in Building deconstruction panel");
            return;
        }

        Button buttonElement = buttonToAdd.Q<Button>("Button");
        if (buttonElement == null)
        {
            Debug.LogError("Could not find Button element in Building deconstruction panel button");
            return;
        }
        buttonElement.clickable.clicked += delegate { BuildingFactory.Instance.DeconstructBuilding(cellPosition); };
        buttonElement.clickable.clicked += delegate { CloseUI(); };
        buttonContainer.Add(buttonToAdd);
    }

    /**
    * Opens the building construction panel and initialise it.
    */
    public void OpenBuildingDeconstrutionUI(Vector2Int cellPosition)
    {
        ResetUIComponent();
        UpdateBuildingDeconstructionUIComponent(cellPosition);
        SetPosition(TilemapManager.Instance.selectionTilemap.CellToWorld((Vector3Int) cellPosition));
        SetVisibility(DisplayStyle.Flex);
    }

    private void InitDeconstructionButton(TemplateContainer button)
    {
        VisualElement icon = button.Q<VisualElement>("Icon");
        Label buildingName = button.Q<Label>("BuildingName");

        icon.style.backgroundImage = new StyleBackground(deconstructionIcon);
        buildingName.text = deconstructionButtonLabel;

        VisualElement costContainer = button.Q<VisualElement>("CostContainer");
        costContainer.style.display = DisplayStyle.None;
    }

    // MISC

    /**
    * Hides the building construction panel.
    */
    public void CloseUI()
    {
        SetVisibility(DisplayStyle.None);
        ResetUIComponent();
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

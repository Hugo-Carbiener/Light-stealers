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
    public void UpdateBuildingConstructionUIComponent(List<BuildingType> buildingsToDisplay)
    {
        if (buildingsToDisplay.Count == 0)
        {
            CloseBuildingConstructionUI();
            return;
        }

        buildingsToDisplay.ForEach(building => AddBuildingButton(building));
    }

    /**
     * Generate a custom button and add it to the UI component.
     */
    private void AddBuildingButton(BuildingType buildingType)
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
        buttonElement.clickable.clicked += delegate { BuildingFactory.Instance.Build(buildingType); };  
        buttonContainer.Add(buttonToAdd);
    }

    // DECONSTRUCTION

    /**
     * Opens the building construction panel and initialise it.
     */
    public void OpenBuildingConstrutionUI(List<BuildingType> buildingsToDisplay, Vector3Int cellPosition)
    {
        ResetUIComponent();
        UpdateBuildingConstructionUIComponent(buildingsToDisplay);
        SetPosition(TilemapManager.Instance.selectionTilemap.CellToWorld(cellPosition));
        SetVisibility(DisplayStyle.Flex);
    }

    private void InitBuildingButton(TemplateContainer button, BuildingType buildingType)
    {
        VisualElement icon = button.Q<VisualElement>("Icon");
        Label buildingName = button.Q<Label>("BuildingName");

        icon.style.backgroundImage = new StyleBackground(iconDictionnary.At(buildingType));
        buildingName.text = buildingType.ToString();
    }

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
    }

    // MISC

    /**
    * Hides the building construction panel.
    */
    public void CloseBuildingConstructionUI()
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

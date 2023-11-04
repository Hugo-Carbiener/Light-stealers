using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Assertions;
using System;

public class BuildingConstructionUIManager : UIManager
{
    [Header("Button template")]
    [SerializeField] private VisualTreeAsset button;
    [Header("Icons")]
    [SerializeField] SerializableDictionary<BuildingType, Sprite> iconDictionnary;

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
        Assert.AreEqual(iconDictionnary.Count(), Enum.GetNames(typeof(BuildingType)).Length);
        root = document.rootVisualElement;
    }

    public void UpdateUIComponent(List<BuildingType> buildingsToDisplay)
    {
        buildingsToDisplay.ForEach(building => AddButton(building));
    }

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
        buttonContainer.Add(buttonToAdd);
    }

    private void InitButton(TemplateContainer button, BuildingType buildingType)
    {
        VisualElement icon = button.Q<VisualElement>("Icon");
        Label buildingName = button.Q<Label>("BuildingName");

        icon.style.backgroundImage = new StyleBackground(iconDictionnary.At(buildingType));
        buildingName.text = button.ToString();
    }
}

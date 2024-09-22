using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;


public class MainMenuUIManager : UIManager, IPassiveUI
{
    private static readonly string BOOK_BUTTON_KEY = "BookButton";
    private static readonly string BUILDING_BUTTON_KEY = "BuildingButton";
    private static readonly string BATTLEFIELD_BUTTON_KEY = "BattlefieldButton";
    public Button bookButton { get; private set; }
    public Button buildingButton { get; private set; }
    public Button battlefieldtButton { get; private set; }

    private Dictionary<IActiveUI, Button> buttons;

    private static MainMenuUIManager _instance;
    public static MainMenuUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MainMenuUIManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        root = document.rootVisualElement;
        bookButton = root.Q<Button>(BOOK_BUTTON_KEY);
        buildingButton = root.Q<Button>(BUILDING_BUTTON_KEY);
        battlefieldtButton = root.Q<Button>(BATTLEFIELD_BUTTON_KEY);
        buttons = new Dictionary<IActiveUI, Button>
        {
            { BookUIManager.Instance, bookButton },
            { BuildingUIManager.Instance, buildingButton },
            { BattlefieldUIManager.Instance, battlefieldtButton }
        };
    }

    void Start()
    {
        InitButtons();
        UpdateUIComponent();
    }

    private void InitButtons()
    {
        buttons.ToList().ForEach(element => InitButton(element.Key, element.Value));
    }

    private void InitButton(IActiveUI manager, Button button)
    {
        if (button == null)
        {
            Debug.LogError("Could not find button element : " + nameof(button));
            return;
        }
        button.clickable.clicked += delegate { manager.ToggleUIComponent(); };
    }

    public void UpdateUIComponent()
    {
        UpdateButtonStates();
    }

    /**
     * Enable buttons depending on the context
     */
    private  void UpdateButtonStates()
    {
        buttons.ToList().ForEach(element => SetEnabled(element.Value, element.Key.CanBeOpened()));
    }
}

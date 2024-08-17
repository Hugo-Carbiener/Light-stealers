using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattlefieldUIManager : UIManager, IActiveUI
{
    private static readonly string ALLY_TROOP_CONTAINER = "AlliesContainer";
    private static readonly string ENEMY_TROOP_CONTAINER = "EnemiesContainer";
    private static readonly string BUILDING_SPRITE_CONTAINER = "BuildingSpriteContainer";
    private static readonly string TROOP_SPRITE_CONTAINER = "Troop";

    private static BattlefieldUIManager _instance;
    public static BattlefieldUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BattlefieldUIManager>();
            }

            return _instance;
        }
    }

    [Header("Troop template")]
    [SerializeField] private VisualTreeAsset troopElement;
    public Fight currentFight { get; set; }

    private void Awake()
    {
        root = document.rootVisualElement;
    }

    void Start()
    {
        SetVisibility(DisplayStyle.None);
    }

    public bool CanBeOpened()
    {
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        return selectedCell != null
            && selectedCell.fight != null
            && !BookUIManager.Instance.IsVisible()
            && !BuildingUIManager.Instance.IsVisible();
    }

    public void CloseUIComponent()
    {
        SetVisibility(root, DisplayStyle.None);
        ResetUIComponent();
    }

    public void OpenUIComponent()
    {
        InitUIComponent();
        SetVisibility(DisplayStyle.Flex);
    }

    public void ResetUIComponent()
    {
        VisualElement alliesTroopContainer = root.Q<VisualElement>(ALLY_TROOP_CONTAINER);
        VisualElement enemiesTroopContainer = root.Q<VisualElement>(ENEMY_TROOP_CONTAINER);
        alliesTroopContainer.Clear();
        enemiesTroopContainer.Clear();
        MainMenuUIManager.Instance.UpdateUIComponent();
    }

    public void ToggleUIComponent()
    {
        if (IsVisible())
        {
            CloseUIComponent();
        }
        else
        {
            OpenUIComponent();
        }
    }

    private void InitUIComponent()
    {
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        if (selectedCell == null)
        {
            Debug.LogError("Opened Battlefield with no tile selected.");
            return;
        }

        Fight fight = selectedCell.fight;
        if (fight == null)
        {
            Debug.LogError("Opened Battlefield with no fight on the selected tile.");
            return;
        }

        VisualElement allyTroopContainer = root.Q<VisualElement>(ALLY_TROOP_CONTAINER);
        VisualElement enemyTroopContainer = root.Q<VisualElement>(ENEMY_TROOP_CONTAINER);

        currentFight = fight;
        InitTroops(fight, Factions.Villagers, allyTroopContainer);
        InitTroops(fight, Factions.Monsters, enemyTroopContainer);
        InitBuilding(selectedCell);
        fight.OnFighterAdded.AddListener(AddTroop);
        SetVisibility(root, DisplayStyle.Flex);
        MainMenuUIManager.Instance.UpdateUIComponent();
    }

    private void InitTroops(Fight fight, Factions faction, VisualElement troopContainer)
    {
        int troopCounter = 0;
        foreach (FightModule fighter in fight.teams[faction].fighters)
        {
            TemplateContainer troopElementToAdd = InitTroopElement(fighter, faction == Factions.Villagers, troopCounter);
            if (troopElementToAdd == null) continue;

            troopContainer.Add(troopElementToAdd);
            AddEventListeners(fighter, troopElementToAdd, false);
            troopCounter++;
        }
    }
    private void InitBuilding(CellData cell)
    {
        Building building = cell.building;
        if (building)
        {
            VisualElement buildingSpriteContainer = root.Q<VisualElement>(BUILDING_SPRITE_CONTAINER);
            if (buildingSpriteContainer == null)
            {
                Debug.LogError("Could not find building sprite container in building container of battlefield.");
                return;
            }
            buildingSpriteContainer.style.backgroundImage = new StyleBackground(GameAssets.i.buildingSprites[building.GetBuildingType()]);
            AddEventListeners(building.GetFightModule(), buildingSpriteContainer, true);
        }
    }

    private TemplateContainer InitTroopElement(FightModule fighter, bool isFacingRight, int troopCounter)
    {
        if (fighter.GetActor().IsBuilding()) return null; 
        
        TemplateContainer troopElementToAdd = troopElement.Instantiate();
        VisualElement spriteContainer = troopElementToAdd.Q<VisualElement>(TROOP_SPRITE_CONTAINER);
        if (spriteContainer == null)
        {
            Debug.LogError("Could not find troop sprite container in troop element of battlefield.");
            return null;
        }

        spriteContainer.style.backgroundImage = new StyleBackground(fighter.GetComponent<SpriteRenderer>().sprite);
        if (isFacingRight)
        {
            spriteContainer.transform.scale = spriteContainer.transform.scale + new Vector3(-2, 0, 0);
        }
        return troopElementToAdd;
    }

    private void AddEventListeners(FightModule fighter, VisualElement container, bool isBuilding)
    {
        if (isBuilding)
        {
            fighter.OnDamaged.AddListener(damageValue => OnDamaged(container, damageValue));
            fighter.OnDeath.AddListener(delegate { OnDeath(container); });
        } else
        {
            fighter.OnAttack.AddListener(attackingFighter => OnAttack(container));
            fighter.OnDamaged.AddListener(damageValue => OnDamaged(container, damageValue));
            fighter.OnDeath.AddListener(delegate { OnDeath(container); });
            fighter.OnFlee.AddListener(delegate { RemoveTroop(container); });
        }
    }

    // EVENT ACTIONS

    /**
    * Method used to add a troop while the battlefield is already open.
    * When first opening the battlefield, it is generated with all currently present troops.
    */
    private void AddTroop(FightModule fighter)
    {
        if (!IsVisible()) return;

        VisualElement troopContainer = null;
        Factions faction = fighter.GetFaction();
        switch (faction)
        {
            case Factions.Villagers:
                troopContainer = root.Q<VisualElement>(ALLY_TROOP_CONTAINER);
                break;
            case Factions.Monsters:
                troopContainer = root.Q<VisualElement>(ENEMY_TROOP_CONTAINER);
                break;
        }
        TemplateContainer troopElementToAdd = InitTroopElement(fighter, faction == Factions.Villagers, troopContainer.childCount);
        if (troopElementToAdd == null) return;
        AddEventListeners(fighter, troopElementToAdd, false);
        troopContainer.Add(troopElementToAdd);
    }

    private void RemoveTroop(VisualElement troopContainer)
    {
        troopContainer.parent.Remove(troopContainer);
    }

    private void OnAttack(VisualElement troopContainer)
    {

    }

    private void OnDamaged(VisualElement troopContainer, int damageValue)
    {
        string text = "-" + damageValue.ToString();

        Vector2 min = RuntimePanelUtils.ScreenToPanel(root.panel, Vector2.zero);
        Vector2 max = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(Screen.width, Screen.height));

        float screenX = (troopContainer.transform.position.x - min.x) / (max.x - min.x) * Screen.width;
        float screenY = (troopContainer.transform.position.y - min.y) / (max.y - min.y) * Screen.height;
        PopUpLauncher.LaunchPopUp(this, GameAssets.i.battleFieldPopUpIndicator, text, new Vector3(screenX, screenY, 0));
    }

    private void OnDeath(VisualElement troopContainer)
    {

    }

    public void UpdateVisibility() {}
}

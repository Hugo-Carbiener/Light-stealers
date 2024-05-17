using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleReportUIManager : UIManager, IActiveUI
{
    private static readonly string DAY_COUNT_LABEL_KEY = "DayCountLabel";
    private static readonly string ALLY_STAMP_CONTAINER_KEY = "AllyStampContainer";
    private static readonly string ENEMY_STAMP_CONTAINER_KEY = "EnemyStampContainer";
    private static readonly string TROOP_CASUALTIES_STAMP_CONTAINER_KEY = "TroopCasualtiesStampContainer";
    private static readonly string BUILDING_CASUALTIES_STAMP_CONTAINER_KEY = "BuildingCasualtiesStampContainer";
    private static readonly string OUTCOME_STAMP_CONTAINER_KEY = "OutcomeStampContainer";
    private static readonly string STAMP_ICON_CONTAINER_KEY = "StampIconContainer";
    private static readonly string STAMP_AMOUNT_LABEL_KEY = "StampAmountLabel";

    private Label dayCountLabel;
    private VisualElement allyStampsContainer;
    private VisualElement enemyStampsContainer;
    private VisualElement troopCasualtiesStampsContainer;
    private VisualElement buildingCasualtiesStampsContainer;
    private VisualElement outcomeStampContainer;

    [Header("Stamp template")]
    [SerializeField] private VisualTreeAsset stampTemplate;
    [Header("Icons")]
    [SerializeField] private SerializableDictionary<Factions, Sprite> iconDictionnary;
    [SerializeField] private Sprite victoryStamp;
    [SerializeField] private Sprite defeatStamp;

    public Fight fight { get; private set; }

    private static BattleReportUIManager _instance;
    public static BattleReportUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BattleReportUIManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        Assert.AreNotEqual(iconDictionnary.Count(), 0);
        Assert.IsNotNull(victoryStamp);
        Assert.IsNotNull(defeatStamp);
        root = document.rootVisualElement;
        dayCountLabel = root.Q<Label>(DAY_COUNT_LABEL_KEY);
        allyStampsContainer = root.Q<VisualElement>(ALLY_STAMP_CONTAINER_KEY);
        enemyStampsContainer = root.Q<VisualElement>(ENEMY_STAMP_CONTAINER_KEY);
        troopCasualtiesStampsContainer = root.Q<VisualElement>(TROOP_CASUALTIES_STAMP_CONTAINER_KEY);
        buildingCasualtiesStampsContainer = root.Q<VisualElement>(BUILDING_CASUALTIES_STAMP_CONTAINER_KEY);
        outcomeStampContainer = root.Q<VisualElement>(OUTCOME_STAMP_CONTAINER_KEY);
    }

    void Start()
    {
        SetVisibility(root, DisplayStyle.None);
    }

    public void OpenUIComponent()
    {
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        if (selectedCell == null || selectedCell.fight == null) return;

        GenerateBattleReport(selectedCell.fight);
        GenerateBattleReportOutcome(selectedCell.fight);
        SetVisibility(root, DisplayStyle.Flex);
        MainMenuUIManager.Instance.UpdateUIComponent();
    }

    public void CloseUIComponent()
    {
        ResetUIComponent();
        SetVisibility(root, DisplayStyle.None);
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

    public void UpdateVisibility() { }

    public void ResetUIComponent()
    {
        fight = null;
        allyStampsContainer.Clear();
        enemyStampsContainer.Clear();
        troopCasualtiesStampsContainer.Clear();
        buildingCasualtiesStampsContainer.style.backgroundImage = null;
        outcomeStampContainer.style.backgroundImage = null;
        MainMenuUIManager.Instance.UpdateUIComponent();
    }

    public bool CanBeOpened()
    {
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        return selectedCell != null 
            && selectedCell.fight != null 
            && !BookUIManager.Instance.IsVisible() 
            && !BuildingUIManager.Instance.IsVisible(); 
    }

    private void GenerateBattleReport(Fight fight)
    {
        this.fight = fight;
        SetDay(fight);
        ApplyStamps(fight.teams[Factions.Villagers], allyStampsContainer);
        ApplyStamps(fight.teams[Factions.Monsters], enemyStampsContainer);
    }

    public void GenerateBattleReportOutcome(Fight fight)
    {
        if (fight.status == Status.Done)
        {
            ApplyCasualtiesStamps(fight.casualties);
            ApplyOutcomeStamp(fight, outcomeStampContainer);
        }
    }

    private void SetDay(Fight fight)
    {
        UpdateText(dayCountLabel, fight.startDay.ToString());
    }

    private void ApplyStamps(Team team, VisualElement stampsContainer)
    {
        // TODO - change this to accept multiple troop types 
        int troopAmount = team.fighters.Count;
        TemplateContainer censusToAdd = stampTemplate.Instantiate();
        VisualElement stampIconContainer = censusToAdd.Q<VisualElement>(STAMP_ICON_CONTAINER_KEY);
        Label stampAmountLabel = censusToAdd.Q<Label>(STAMP_AMOUNT_LABEL_KEY);
        stampIconContainer.style.backgroundImage = new StyleBackground(iconDictionnary[team.faction]);
        UpdateText(stampAmountLabel, troopAmount.ToString());
        stampsContainer.Add(censusToAdd);
    }

    private void ApplyCasualtiesStamps(List<IFightable> fighters)
    {
        // TODO - change this to accept multiple troop types 
        int casualtyAmount = fighters.Count;
        foreach (IFightable fighter in fighters)
        {
            if (fighter is Building building)
            {
                Sprite buildingIcon = BuildingUIManager.Instance.GetIconDictionnary()[building.GetBuildingType()];
                buildingCasualtiesStampsContainer.style.backgroundImage = new StyleBackground(buildingIcon);
                casualtyAmount--;
            }
            TemplateContainer casualtyCensusToAdd = stampTemplate.Instantiate();
            VisualElement stampIconContainer = casualtyCensusToAdd.Q<VisualElement>(STAMP_ICON_CONTAINER_KEY);
            Label stampAmountLabel = casualtyCensusToAdd.Q<Label>(STAMP_AMOUNT_LABEL_KEY);
            stampIconContainer.style.backgroundImage = new StyleBackground(iconDictionnary[Factions.Villagers]);
            UpdateText(stampAmountLabel, casualtyAmount.ToString());
            troopCasualtiesStampsContainer.Add(casualtyCensusToAdd);
        }
    }

    private void ApplyOutcomeStamp(Fight fight, VisualElement stampContainer)
    {
        if (fight.winningFaction == Factions.Villagers)
        {
            stampContainer.style.backgroundImage = new StyleBackground(victoryStamp);
        } else
        {
            stampContainer.style.backgroundImage = new StyleBackground(defeatStamp);
        }
    }
}

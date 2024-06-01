using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FightManager : MonoBehaviour
{
    private static FightManager _instance;
    public static FightManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<FightManager>();
            }

            return _instance;
        }
    }

    [Header("Fight cooldowns")]
    [SerializeField] private float setupDuration;

    public List<Fight> fights { get; private set; }

    private void Awake()
    {
        fights = new List<Fight>();
    }

    public void StartFight(List<Team> teams, CellData fightCell)
    {
        Fight fight = new Fight(teams);
        fightCell.fight = fight;

        // UI
        MainMenuUIManager.Instance.UpdateUIComponent();
        BuildingUIManager.Instance.UpdateVisibility();

        // defense task
        Task existingTask = TaskManager.Instance.GetTask(fightCell.coordinates, TaskType.Defense);
        Task defenseTask = existingTask != null ? existingTask : new Task(fightCell.coordinates, TaskManager.INFINITE_CAPACITY, TaskType.Defense);
        TaskManager.Instance.RegisterNewTask(defenseTask);

        fights.Add(fight);
    }

    public float getSetupDuration() { return setupDuration; }

    public Fight GetFight(FightModule fighter)
    {
        return fights.First(fight => fight.ContainsFighter(fighter));
    }
}

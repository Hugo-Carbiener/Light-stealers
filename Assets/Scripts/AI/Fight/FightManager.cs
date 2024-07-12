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

    public void StartFight(List<Team> teams, CellData fightCell, Task attackTask)
    {
        // defense task
        Task defenseTask = CreateDefenseTask(fightCell);

        Fight fight = new Fight(teams, fightCell, attackTask, defenseTask);
        fightCell.fight = fight;

        // UI
        MainMenuUIManager.Instance.UpdateUIComponent();
        BuildingUIManager.Instance.UpdateVisibility();


        fights.Add(fight);
    }

    /**
     * To be called when creating a new fight to creating the corresponding defense task.
     */
    private Task CreateDefenseTask(CellData fightCell)
    {
        Task existingTask = TaskManager.Instance.GetTask(fightCell.coordinates, TaskType.Defense);
        Task defenseTask = existingTask != null ? existingTask : new Task(fightCell.coordinates, TaskManager.INFINITE_CAPACITY, TaskType.Defense);
        TaskManager.Instance.RegisterNewTask(defenseTask);
        return defenseTask;
    }

    public float getSetupDuration() { return setupDuration; }

    public Fight GetFight(FightModule fighter)
    {
        return fights.FirstOrDefault(fight => fight.ContainsFighter(fighter));
    }
}

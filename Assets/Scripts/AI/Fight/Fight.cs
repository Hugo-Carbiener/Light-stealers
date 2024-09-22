using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

public class Fight
{
    public Dictionary<Factions, Team> teams { get; private set; } = new Dictionary<Factions, Team>();
    public List<IFightable> casualties { get; private set; } = new List<IFightable>();
    public int startDay { get; private set; }
    public Factions winningFaction { get; private set; }
    public Status status { get; private set; }
    public CellData cell { get; private set; }
    public Task attackTask { get; private set; }
    public Task defenseTask { get; private set; }
    
    public UnityEvent OnFightEndEvent { get; private set; } = new UnityEvent();
    public UnityEvent<FightModule> OnFighterAdded { get; private set; } = new UnityEvent<FightModule>();

    public Fight(List<Team> teams, CellData cell, Task attackTask, Task defenseTask)
    {
        status = Status.Pending;
        startDay = DayNightCycleManager.Instance.day;
        foreach (Team team in teams)
        {
            this.teams.Add(team.faction, team);
        }
        this.cell = cell;
        this.attackTask = attackTask;
        this.defenseTask = defenseTask;
        Init();
    }

    public void Init()
    {
        FightManager.Instance.StartCoroutine(SetupCoroutine());
    }

    private IEnumerator SetupCoroutine()
    {
        float timer = 0;
        float setupDuration = FightManager.Instance.getSetupDuration();
        while (timer < setupDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        status = Status.InProgress;
        FightManager.Instance.StartCoroutine(MainLoopCoroutine());
    }

    private IEnumerator MainLoopCoroutine()
    {
        Factions currentFaction = Extensions.RandomValue<Factions>();
        while (TeamsAreAlive())
        {
            for (int i = 0; i < Enum.GetValues(typeof(Factions)).Length; i++)
            {
                Team currentTeam = teams[currentFaction];
                if (!currentTeam.IsAlive()) break;
                yield return currentTeam.PlayTurn(teams[currentFaction.Next()]);
                currentFaction = currentFaction.Next();
            }
        }
        OnFightEnd();
    }

    private void OnFightEnd()
    {
        status = Status.Done;
        winningFaction = teams.Values.First(team => team.IsAlive()).faction;

        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();  
        if (selectedCell != null && selectedCell.fight != null && selectedCell.fight == this)
        {
            MainMenuUIManager.Instance.UpdateUIComponent();
        }

        BattleReportUIManager battleReportUI = BattleReportUIManager.Instance;
        if (this == battleReportUI.fight && battleReportUI.IsVisible())
        {
            battleReportUI.GenerateBattleReportOutcome(this);
        }

        cell.fight = null;

        TaskManager.Instance.tasks.Remove(attackTask);
        TaskManager.Instance.tasks.Remove(defenseTask);

        OnFightEndEvent.Invoke();
    }

    public void AddFighter(FightModule fighter)
    {
        teams[fighter.GetFaction()].AddFighter(fighter);
        this.OnFighterAdded.Invoke(fighter);
    }

    public void RemoveFighter(FightModule fighter)
    {
        GetTeamByFighter(fighter).RemoveFighter(fighter);
        if (!fighter.IsAlive())
        {
            casualties.Add(fighter.GetActor());
        } else
        {
            Debug.Log(fighter.gameObject.name + " flee");
            fighter.OnFlee.Invoke();
        }
    }

    public Team GetTeamByFighter(FightModule fighter)
    {
        return teams.Values.First(team => team.ContainsFighter(fighter));
    }

    public bool ContainsFighter(FightModule fighter)
    {
        return teams[fighter.GetFaction()].ContainsFighter(fighter);
    }

    private bool TeamsAreAlive()
    {
        return teams.Values.All(team => team.IsAlive() && team.HasFighters());
    }
}

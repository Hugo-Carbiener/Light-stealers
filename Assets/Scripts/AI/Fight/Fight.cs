using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Fight
{
    public Dictionary<Factions, Team> teams { get; private set; } = new Dictionary<Factions, Team>();
    public List<IFightable> casualties { get; private set; } = new List<IFightable>();
    public int startDay { get; private set; }
    public Factions winningFaction { get; private set; }
    public Status status { get; private set; }

    public Fight(List<Team> teams)
    {
        status = Status.Pending;
        startDay = DayNightCycleManager.Instance.day;
        foreach (Team team in teams)
        {
            this.teams.Add(team.faction, team);
        }
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
                yield return currentTeam.PlayTurn(teams[currentFaction.Next()]);
                currentFaction = currentFaction.Next();
            }
        }
        OnFightEnd();
    }

    private void OnFightEnd()
    {
        status = Status.Done;
        foreach (Team team in teams.Values)
        {
            if (team.IsAlive() && winningFaction == null) winningFaction = team.faction;
            team.OnFightEnd(this);
        }

        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();  
        if (selectedCell != null && selectedCell.fight != null && selectedCell.fight == this)
        {
            MainMenuUIManager.Instance.UpdateUIComponent();
        }
    }

    public void AddFighter(FightModule fighter)
    {
        teams[fighter.GetFaction()].AddFighter(fighter);
    }

    private bool TeamsAreAlive()
    {
        return teams.Values.All(team => team.IsAlive());
    }
}

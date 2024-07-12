using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : Behavior
{
    private readonly FightModule fightModule;
    private readonly Vector2Int attackLocation;

    public AttackBehavior(AIAgent agent, FightModule fightModule, Vector2Int location) : base(agent)
    {
        this.fightModule = fightModule;
        attackLocation = location;
    }

    protected override void InitAction(Vector2Int targetCell)
    {
        if (!fightModule || targetCell != attackLocation)
        {
            agent.EndTask();
            return;
        }

        ExecuteAction(targetCell);
    }

    protected override void ExecuteAction(Vector2Int targetCell)
    {
        if (fightModule.Attack(targetCell, agent.assignedTask))
        {
            TilemapManager.Instance.GetCellData(targetCell).fight.OnFightEndEvent.AddListener(agent.EndTask);
        }
        else
        {
            agent.EndTask();
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerBehavior : BehaviorModule
{
    private FightModule fightModule;

    private void Awake()
    {
        fightModule = unit.GetFightModule();
    }

    private void Update()
    {
        UpdateMovementDestination(assignedTask);
    }

    protected override void InitAction(Vector2Int targetCell)
    {
        if (!fightModule) return;

        ExecuteAction(targetCell);
    }

    protected override void ExecuteAction(Vector2Int targetCell)
    {
        if (fightModule.Attack(targetCell))
        {
            TilemapManager.Instance.GetCellData(targetCell).fight.OnFightEndEvent.AddListener(EndTask);
        } else
        {
            EndTask();
        }
    }

    private void UpdateMovementDestination(Task task)
    {
        if (task == null) return;

        CellData targetCell = TilemapManager.Instance.GetCellData(task.location);
        if (targetCell != null && targetCell.fight != null && targetCell.fight.status != Status.Done) return;
        else
        {
            unit.GetMovementModule().CancelMovement();
            EndTask();
        }
    }
}

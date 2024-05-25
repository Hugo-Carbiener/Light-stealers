using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerAIAgent : AIAgent
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBehavior : BehaviorModule, ITaskAutoGeneration
{
    private ITargettable target;

    private void Update()
    {
        UpdateMovementDestination(assignedTask);
    }

    protected override void ExecuteAction(Task task)
    {
        throw new System.NotImplementedException();
    }

    protected override void InitAction(Vector2Int targetCell)
    {
        throw new System.NotImplementedException();
    }

    public Task GenerateTask(Unit unit)
    {
        target = GetTarget(unit.GetMovementModule());
        return target == null ? null : new Task(target.GetPosition(), TaskType.MonsterAttack);
    }

    private ITargettable GetTarget(MovementModule movementModule)
    {
        ITargettable closestTarget = null;
        int distanceToTarget = TilemapManager.Instance.GetTilemapColumns() * TilemapManager.Instance.GetTilemapRows();

        List<ITargettable> targets = Enumerable.Concat<ITargettable>(BuildingFactory.Instance.buildingsConstructed, UnitManager.Instance.armyUnits).ToList();
        foreach (ITargettable target in targets)
        {
            if (target == null || target.GetFightModule().IsAttackable()) continue;

            int distanceToThisTarget = Utils.GetTileDistance(movementModule.currentCell, target.GetPosition());
            if (distanceToThisTarget >= distanceToTarget) continue;

            distanceToTarget = distanceToThisTarget;
            closestTarget = target;
        }
        return closestTarget;
    }

    private void UpdateMovementDestination(Task task)
    {
        if (task.location == target.GetPosition()) return;
        
        unit.GetMovementModule().CancelMovement();
        task.location = target.GetPosition();

        if (InitMovement(task))
        {
            ExecuteMovement(task);
        }
        else
        {
            EndTask();
        }   
    }
}

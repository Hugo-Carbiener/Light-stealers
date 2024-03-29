using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBehavior : BehaviorModule, ITaskAutoGeneration
{
    protected override void ExecuteAction(Task task)
    {
        throw new System.NotImplementedException();
    }

    protected override void ExecuteMovement(Task task)
    {
        throw new System.NotImplementedException();
    }

    protected override void InitAction(Task task)
    {
        throw new System.NotImplementedException();
    }

    public Task GenerateTask(Unit unit)
    {
        ITargettable target = GetTarget(unit.GetMovementModule());
        return new Task(target.GetPosition(), TaskType.MonsterAttack);
    }

    private ITargettable GetTarget(MovementModule movementModule)
    {
        ITargettable closestTarget = null;
        int distanceToTarget = TilemapManager.Instance.GetTilemapColumns() * TilemapManager.Instance.GetTilemapRows();

        List<ITargettable> targets = Enumerable.Concat<ITargettable>(BuildingFactory.Instance.buildingsConstructed, ArmyManager.Instance.armyUnits).ToList();
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
}

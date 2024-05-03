using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBehavior : BehaviorModule, ITaskAutoGeneration
{
    private ITargettable target;
    private FightModule fightModule;

    private void Update()
    {
        UpdateMovementDestination(assignedTask);
    }

    protected override void InitAction(Vector2Int targetCell)
    {
        if (!fightModule) fightModule = gameObject.GetComponent<FightModule>();
        if (!fightModule) return;

        ExecuteAction(targetCell);
    }

    protected override void ExecuteAction(Vector2Int targetCell)
    {
        fightModule.Attack(targetCell);
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

        List<ITargettable> targets = Enumerable.Concat<ITargettable>(BuildingFactory.Instance.buildingsConstructed, UnitManager.Instance.GetAllActiveUnits()).ToList();
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

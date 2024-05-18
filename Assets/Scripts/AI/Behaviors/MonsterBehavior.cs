using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBehavior : BehaviorModule, ITaskAutoGeneration
{
    private IFightable target;
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
        }
        else
        {
            EndTask();
        }
    }

    public Task GenerateTask(Unit unit)
    {
        target = GetTarget(unit.GetMovementModule());
        return target == null ? null : new Task(target.GetPosition(), TaskType.MonsterAttack);
    }

    private IFightable GetTarget(MovementModule movementModule)
    {
        IFightable closestTarget = null;
        int distanceToTarget = TilemapManager.Instance.GetTilemapColumns() * TilemapManager.Instance.GetTilemapRows();

        List<IFightable> targets = Enumerable.Concat<IFightable>(BuildingFactory.Instance.buildingsConstructed, UnitManager.Instance.GetAllActiveUnits())
            .Where(target => target.GetFightModule() != null)
            .Where(target => target.GetFightModule().GetFaction() != fightModule.GetFaction())
            .ToList();
       
        foreach (IFightable currentTarget in targets)
        {
            if (currentTarget == null || !currentTarget.GetFightModule().IsAttackable()) continue;

            int distanceToThisTarget = Utils.GetTileDistance(movementModule.currentCell, currentTarget.GetPosition());
            if (distanceToThisTarget >= distanceToTarget) continue;

            distanceToTarget = distanceToThisTarget;
            closestTarget = currentTarget;
        }
        return closestTarget;
    }

    private void UpdateMovementDestination(Task task)
    {
        if (task == null || target == null || task.location == target.GetPosition()) return;
        
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

    public override bool GeneratesOwnTasks() { return true; }
}

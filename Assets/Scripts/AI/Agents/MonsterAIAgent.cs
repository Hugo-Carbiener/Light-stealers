using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterAIAgent : AIAgent, ITaskAutoGeneration
{
    private IFightable target;
    private FightModule fightModule;

    private void Awake()
    {
        fightModule = unit.GetFightModule();
    }

    private void OnEnable()
    {
        DayNightCycleManager.OnCyclePhaseEnd.AddListener(SetupFleeBehavior);
    }

    private void OnDisable()
    {
        DayNightCycleManager.OnCyclePhaseEnd.RemoveListener(SetupFleeBehavior);

    }

    private void Update()
    {
        UpdateMovementDestination(assignedTask);
    }

    public Task GenerateTask(Unit unit)
    {
        target = GetTarget(unit.GetMovementModule());
        if (target == null) return null;

        Task existingTask = TaskManager.Instance.GetTask(target.GetPosition(), TaskType.Attack);
        return existingTask != null ? existingTask : new Task(target.GetPosition(), TaskManager.INFINITE_CAPACITY, TaskType.Attack);
    }

    private IFightable GetTarget(MovementModule movementModule)
    {
        IFightable closestTarget = null;
        int distanceToTarget = TilemapManager.Instance.GetTilemapColumns() * TilemapManager.Instance.GetTilemapRows();

        List<IFightable> buildingTargets = BuildingFactory.Instance.buildingsConstructed
            .Where(target => target.GetFightModule() != null)
            .Where(building => building.IsValidTargetForFight(fightModule.GetFaction()))
            .Select(building => (IFightable) building)
            .ToList();
        List <IFightable> unitTargets = UnitManager.Instance.GetAllActiveUnits()
            .Where(target => target.GetFightModule() != null)
            .Where(unit => unit.IsValidTargetForFight(fightModule.GetFaction()))
            .Select(unit => (IFightable)unit)
            .ToList();
        List<IFightable> targets = Enumerable.Concat<IFightable>(buildingTargets, unitTargets).ToList();
         
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

        behavior.StartBehavior(task, unit);
    }

    protected override void AssignNewBehavior()
    {
        if (assignedTask == null)
        {
            behavior = new IdleBehavior(this);
            return;
        }

        switch (assignedTask.type)
        {
            default:
                behavior = new AttackBehavior(this, fightModule, assignedTask.location);
                break;
            case TaskType.Attack:
                behavior = new AttackBehavior(this, fightModule, assignedTask.location);
                break;
            case TaskType.Defense:
                behavior = new AttackBehavior(this, fightModule, assignedTask.location);
                break;
            case TaskType.Flee:
                behavior = new FleeBehavior(this);
                break;
        }
        behavior.StartBehavior(assignedTask, unit);
    }

    /**
     * Removes fighter from fight, set the agent as idle and generate a new idle task that will be assigned by the task manager.
     */
    private void SetupFleeBehavior(DayNightCyclePhases phase)
    {
        if (phase != DayNightCyclePhases.Night) return;
        Fight fight = FightManager.Instance.GetFight(fightModule);
        if (fight != null)
        {
            fight.RemoveFighter(fightModule);
        }
        CellData destination = FractureManager.Instance.GetRandomFracture();
        TaskManager.Instance.RegisterNewTask(new Task(destination.coordinates, 1, TaskType.Flee));
        target = null;
        SetIdle();
    }

    public override bool GeneratesOwnTasks() { return true; }

}

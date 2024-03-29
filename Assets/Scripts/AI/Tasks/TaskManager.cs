using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Manager responsible for all the ordered tasks in the current game
 */
public class TaskManager : MonoBehaviour
{
    private static TaskManager _instance;
    public static TaskManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TaskManager>();
            }

            return _instance;
        }
    }

    public List<Task> tasks { get; private set; } = new List<Task>();

    private void Update()
    {
        PollUnits();
    }

    private void PollUnits()
    {
        if (ArmyManager.Instance.armyUnits == null || ArmyManager.Instance.armyUnits.Count == 0) return;

        foreach (Unit unit in ArmyManager.Instance.armyUnits)
        {
            if (unit == null) continue;
            BehaviorModule behaviorModule = unit.GetBehaviorModule();
            if (behaviorModule == null) continue;

            if (unit.GetBehaviorModule().IsIdle())
            {
                Task task = GetTaskForUnit(unit);
                if (task == null) continue;

                behaviorModule.AssignNewTask(task, unit.GetMovementModule());
            }
        }
    }
     
    private Task GetTaskForUnit(Unit unit)
    {
        BehaviorModule behavior = unit.GetBehaviorModule();

        if (behavior.GeneratesOwnTasks)
        {
            Task task = ((ITaskAutoGeneration)behavior).GenerateTask(unit);
            RegisterNewTask(task);
            return task;
        }

        foreach (Task task in tasks)
        {
            if (task == null || 
                task.status != Status.Pending || 
                !behavior.GetAcceptedTasks().Contains(task.type)) continue;

            return task;
        }

        return null;
    }

    public void RegisterNewTask(Task task)
    {
        tasks.Add(task);
    }
}

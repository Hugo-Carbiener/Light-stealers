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

    private void PollUnits()
    {
        foreach (Unit unit in ArmyManager.Instance.armyUnits)
        {
            if (unit == null) continue;
            if (unit.GetBehaviorModule().IsIdle())
            {
                Task task = GetTaskForUnit(unit.GetBehaviorModule());
                if (task == null) continue;
            }
        }
    }
     
    private Task GetTaskForUnit(BehaviorModule behavior)
    {
        if (behavior == null) return null;

        if (behavior impleme)
        foreach (Task task in tasks)
        {
            if (task == null || 
                task.status != Status.Pending || 
                !behavior.GetAcceptedTasks().Contains(task.type)) continue;


        }
    }
}

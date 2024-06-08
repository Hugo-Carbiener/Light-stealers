using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    public static readonly double INFINITE_CAPACITY = Double.MaxValue;

    public List<Task> tasks { get; private set; } = new List<Task>();
    public List<Task> assignedTasks { get; private set; } = new List<Task>();

    private void Update()
    {
        PollTasks();
        PollUnits();
    }

    /**
     * Goes over the tasks and remove obsolete ones
     */
    private void PollTasks()
    {
        assignedTasks = assignedTasks.Where(task => task != null && task.status != Status.Done).ToList();
    }

    /**
     * Goes over units and assign tasks to idle ones.
     */ 
    private void PollUnits()
    {
        List<Unit> units = UnitManager.Instance.GetAllActiveUnits();
        if (units == null || units.Count == 0) return;

        foreach (Unit unit in units)
        {
            if (unit == null) continue;
            AIAgent agent = unit.GetAgent();
            if (agent == null) continue;

            if (unit.GetAgent().IsIdle())
            {
                Task task = FindTaskForUnit(unit);
                if (task == null) continue;

                AssignNewTask(task, agent);
            }
        }
    }
     
    /**
     * Finds an appropriate task for an idle unit. Self generating units are required to generate one themselves.
     */
    private Task FindTaskForUnit(Unit unit)
    {
        AIAgent agent = unit.GetAgent();
        // sort task by priority
        List<Task> orderTasks = tasks.OrderBy(task => agent.GetTaskTypeWeight(task.type)).ToList();

        foreach (Task task in orderTasks)
        {
            if (!IsValidFor(task, agent)) continue;
            return task;
        }

        if (agent.GeneratesOwnTasks())
        {
            Task task = ((ITaskAutoGeneration)agent).GenerateTask(unit);
            RegisterNewTask(task);
            return task;
        }

        return null;
    }

    private void AssignNewTask(Task task, AIAgent agent)
    {
        task.capacity++;
        agent.AssignNewTask(task);
    }

    public Task GetTask(Vector2Int location, TaskType type)
    {
        return tasks.Find(task => task.location == location && task.type == type);
    }

    /**
     * Adds a new task to the pile
     */
    public void RegisterNewTask(Task task)
    {
        Debug.Log("Register new task " + task.type + ", capacity " + task.capacity + " at " + task.location);
        tasks.Add(task);
    }

    private bool IsValidFor(Task task, AIAgent agent)
    {
        return task != null
            && (task.status == Status.Pending || task.status == Status.InProgress) 
            && agent.GetAcceptedTasks().Contains(task.type);
    }

    public void OnTaskFullyAssigned(Task task)
    {
        tasks.Remove(task);
        assignedTasks.Add(task);
    }
}

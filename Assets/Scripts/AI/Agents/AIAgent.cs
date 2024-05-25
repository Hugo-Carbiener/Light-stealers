using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


/**
 * An AI Agent holds the logic to assign the right behavior on a unit.
 * Each agent accepts a certain types of task.
 */
public abstract class AIAgent : MonoBehaviour
{
    [SerializeField] protected Unit unit;
    [SerializeField] private SerializableDictionary<int, TaskType> weightedAcceptedTasks;

    protected Behavior behavior { get; set; }
    protected Task assignedTask { get; private set; }

    private void Awake()
    {
        Assert.IsTrue(weightedAcceptedTasks != null && weightedAcceptedTasks.Count() > 0);
        Assert.IsTrue(unit != null);
    }

    private void Update()
    {
        if (assignedTask == null)
        {
            AssignNewBehavior();
        }
    }

    public void AssignNewTask(Task task)
    {
        if (!weightedAcceptedTasks.ContainsValue(task.type))
        {
            Debug.LogError(string.Format($"Attempting to assign wrong task ({task.type}) to behavior {this.name}"));
            return;
        }
        task.status = Status.InProgress;
        assignedTask = task;
        AssignNewBehavior();
    }

    protected abstract void AssignNewBehavior();
    public Unit GetUnit() { return unit; }
    public bool IsIdle() { return assignedTask == null; }
    public List<TaskType> GetAcceptedTasks() { return weightedAcceptedTasks.GetValues(); }
    public virtual bool GeneratesOwnTasks() { return false; }
    public void EndTask()
    {
        assignedTask.Finish();
        assignedTask = null;
    }
}

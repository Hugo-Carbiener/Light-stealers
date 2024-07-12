using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Describes the objective of a unit
 */
public class Task
{
    public Vector2Int location { get; set; }
    public BoundCounter capacity { get; set; }
    public TaskType type { get; private set; }
    public Status status { get; set; } 

    public Task(Vector2Int location, double capacity, TaskType type)
    {
        this.location = location;
        this.capacity = new BoundCounter(capacity);
        this.type = type;
        status = Status.Pending;
        Init();
    }

    private void Init()
    {
        capacity.OnMaxValueReachedOrExceeded.AddListener(OnTaskFullyAssigned);
        capacity.OnMinValueReachedOrExceeded.AddListener(ThrowNegativeCapacityError);
    }

    public void Finish()
    {
        status = Status.Done;
    }

    private void OnTaskFullyAssigned()
    {
        TaskManager.Instance.OnTaskFullyAssigned(this);
    }

    private void ThrowNegativeCapacityError()
    {
        Debug.LogError(string.Format($"Task of type {type} on cell {location} reached a negative capaity {capacity}"));
    }


    public override string ToString()
    {
        return type + " at " + location + " for " + capacity;
    }
}

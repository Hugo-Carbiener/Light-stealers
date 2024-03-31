using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Describes the objective of a unit
 */
public class Task
{
    public Vector2Int location { get; set; }
    public TaskType type { get; private set; }
    public Status status { get; set; } 

    public Task(Vector2Int location, TaskType type)
    {
        this.location = location;
        this.type = type;
        status = Status.Pending;
    }

    public void Finish()
    {
        status = Status.Done;
    }
}

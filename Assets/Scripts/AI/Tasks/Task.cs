using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Describes the objective of a unit
 */
public class Task
{
    public CellData location { get; set; }
    public TaskType type { get; private set; }
    public Status status { get; private set; } 



    public void Finish()
    {
        status = Status.Done;
    }
}

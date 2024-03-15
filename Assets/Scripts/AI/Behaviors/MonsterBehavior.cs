using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : Behavior
{
    public override void Execute(Task task)
    {
        
    }

    protected override void SetTaskLocation(Task task)
    {
        // get closest attackable cell for now
        task.location = null;
    }
}

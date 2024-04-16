using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fight : MonoBehaviour
{
    public Dictionary<Factions, List<FightModule>> belligerents { get; private set; }

    public Status status { get; private set; }
    
    public Fight()
    {
        status = Status.Pending;
        Init();
    }

    public void Init()
    {
        StartCoroutine("SetupCoroutine");
    }

    private IEnumerator SetupCoroutine()
    {
        float timer = 0;
        float setupDuration = FightManager.Instance.getSetupDuration();
        while (timer < setupDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        status = Status.InProgress;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class DayNightCycleManager : MonoBehaviour
{
    private static DayNightCycleManager instance;

    public static DayNightCycleManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<DayNightCycleManager>();
            }
            return instance;
        }
    }

    [Header("Durations")]
    [SerializeField] private SerializableDictionary<DayNightCyclePhases, float> phasesDurations;

    private int day;
    private DayNightCyclePhases phase;

    public static event cyclePhaseHandler OnCyclePhaseStart;
    public static event cyclePhaseHandler OnCyclePhaseEnd;
    public delegate void cyclePhaseHandler(DayNightCyclePhases phase);


    private void Awake()
    {
        OnCyclePhaseStart += notifyPhaseStart;
        OnCyclePhaseEnd += notifyPhaseEnd;
        Assert.AreNotEqual(phasesDurations.Count(), 0);
    }

    private void Start()
    {

        initializeCycle();
    }

    private void notifyPhaseStart(DayNightCyclePhases phase)
    {
        Debug.Log("Phase " + phase.ToString() + " has started and will last for " + phasesDurations.At(phase));
    }

    private void notifyPhaseEnd(DayNightCyclePhases phase)
    {
        Debug.Log("Phase " + phase.ToString() + " has ended.");
    }

    private void initializeCycle()
    {
        day = 0;
        phase = DayNightCyclePhases.Day;

        start(DayNightCyclePhases.Day);
    }

    private void start(DayNightCyclePhases phase)
    {
        StartCoroutine(startCycleCoroutine(phase));
    }

    private IEnumerator startCycleCoroutine(DayNightCyclePhases phase)
    {
        // invoke phase start event
        OnCyclePhaseStart.Invoke(phase);

        if (phase == DayNightCyclePhases.Day)
        {
            day++;
        }

        float timer = 0;
        float phaseDuration = phasesDurations.At(phase);
        while (timer < phaseDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // invoke the phase end event
        OnCyclePhaseEnd.Invoke(phase);

        DayNightCyclePhases nextPhase = phase.Next();

        start(nextPhase);
    }
}

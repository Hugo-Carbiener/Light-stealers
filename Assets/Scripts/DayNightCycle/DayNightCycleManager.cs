using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

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
    [Header("Global lighting colors")]
    [SerializeField] private SerializableDictionary<DayNightCyclePhases, Gradient> phaseColorGradients;
    [Header("Global lighting intensities")]
    [SerializeField] private SerializableDictionary<DayNightCyclePhases, AnimationCurve> phaseIntensityGradients;
    [Header("Light")]
    [SerializeField] private Light2D globalLight;
    private int day;
    private DayNightCyclePhases phase;

    public static event cyclePhaseHandler OnCyclePhaseStart;
    public static event cyclePhaseHandler OnCyclePhaseEnd;
    public delegate void cyclePhaseHandler(DayNightCyclePhases phase);


    private void Awake()
    {
        if (!globalLight) globalLight = GetComponent<Light2D>();
        OnCyclePhaseStart += OnPhaseStart;
        OnCyclePhaseEnd += OnPhaseEnd;
        Assert.AreNotEqual(phasesDurations.Count(), 0);
    }

    private void Start()
    {

        InitializeCycle();
    }

    private void OnPhaseStart(DayNightCyclePhases phase)
    {
        Debug.Log("Phase " + phase.ToString() + " has started and will last for " + phasesDurations.At(phase));
    }

    private void OnPhaseEnd(DayNightCyclePhases phase)
    {
        Debug.Log("Phase " + phase.ToString() + " has ended.");
    }

    private void InitializeCycle()
    {
        day = 0;
        phase = DayNightCyclePhases.Day;

        Start(DayNightCyclePhases.Day);
    }

    private void Start(DayNightCyclePhases phase)
    {
        StartCoroutine(StartCycleCoroutine(phase));
    }

    private IEnumerator StartCycleCoroutine(DayNightCyclePhases phase)
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

            UpdateGlobalLightIntensity(phase, timer / phaseDuration);
            UpdateGlobalLightColor(phase, timer / phaseDuration);

            yield return null;
        }

        DayNightCyclePhases nextPhase = phase.Next();

        Start(nextPhase);
    }

    private void UpdateGlobalLightColor(DayNightCyclePhases currentPhase, float phaseProgress)
    {
        Gradient phaseColorGradient = phaseColorGradients.At(currentPhase);
        if (phaseColorGradient == null)
        {
            Debug.LogError("Error : the phase color gradient does not exist for phase " + currentPhase);
            return;
        }
        globalLight.color = phaseColorGradient.Evaluate(phaseProgress);
    }

    private void UpdateGlobalLightIntensity(DayNightCyclePhases currentPhase, float phaseProgress)
    {
        AnimationCurve phaseIntensityCurve = phaseIntensityGradients.At(currentPhase);
        if (phaseIntensityCurve == null)
        {
            Debug.LogError("Error : the phase intensity curve does not exist for phase " + currentPhase);
            return;
        }
        globalLight.intensity = phaseIntensityCurve.Evaluate(phaseProgress);
    }
}

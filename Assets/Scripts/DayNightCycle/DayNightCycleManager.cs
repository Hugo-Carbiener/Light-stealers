using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
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
    [Header("Lights")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D travellingLight;
    [Header("Travelling light positions")]
    [SerializeField] private Vector3 start;
    [SerializeField] private Vector3 destination;
    [SerializeField] private DayNightCyclePhases activePhase;
    [SerializeField] private float transitionDuration;
    private float travellingLightCruseIntensity;

    private int day;
    private DayNightCyclePhases phase;

    public static event cyclePhaseHandler OnCyclePhaseStart;
    public static event cyclePhaseHandler OnCyclePhaseEnd;
    public delegate void cyclePhaseHandler(DayNightCyclePhases phase);


    private void Awake()
    {
        if (!globalLight) globalLight = GetComponent<Light2D>();
        OnCyclePhaseStart += OnPhaseStart;
        OnCyclePhaseStart += InitTravellingLight;
        OnCyclePhaseEnd += OnPhaseEnd;
        OnCyclePhaseEnd += ResetTravellingLight;
        Assert.AreNotEqual(phasesDurations.Count(), 0);
    }

    private void Start()
    {
        travellingLightCruseIntensity = travellingLight.intensity;
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

            UpdateLightIntensity(globalLight, phase, timer / phaseDuration);
            UpdateLightColor(globalLight, phase, timer / phaseDuration);
            UpdateLightColor(travellingLight, phase, timer / phaseDuration);
            UpdateLightTravel(phase, timer / phaseDuration);
            yield return null;
        }

        OnCyclePhaseEnd.Invoke(phase);
        DayNightCyclePhases nextPhase = phase.Next();
        Start(nextPhase);
    }

    private void UpdateLightColor(Light2D light, DayNightCyclePhases currentPhase, float phaseProgress)
    {
        if (!light.enabled) return;

        Gradient phaseColorGradient = phaseColorGradients.At(currentPhase);
        if (phaseColorGradient == null)
        {
            Debug.LogError("Error : the phase color gradient does not exist for phase " + currentPhase);
            return;
        }
        light.color = phaseColorGradient.Evaluate(phaseProgress);
    }

    private void UpdateLightIntensity(Light2D light, DayNightCyclePhases currentPhase, float phaseProgress)
    {
        if (!light.enabled) return;

        AnimationCurve phaseIntensityCurve = phaseIntensityGradients.At(currentPhase);
        if (phaseIntensityCurve == null)
        {
            Debug.LogError("Error : the phase intensity curve does not exist for phase " + currentPhase);
            return;
        }
        light.intensity = phaseIntensityCurve.Evaluate(phaseProgress);
    }

    private void UpdateLightTravel(DayNightCyclePhases phase, float phaseProgress)
    {
        if (phase != DayNightCyclePhases.Day) return;

        travellingLight.transform.position = Vector3.Lerp(start, destination, phaseProgress);
    }

    private void InitTravellingLight(DayNightCyclePhases phase)
    {
        if (phase != DayNightCyclePhases.Day) return;
        travellingLight.transform.position = start;
        StartCoroutine(FadeLightIntensity(travellingLight, 0, travellingLightCruseIntensity, transitionDuration));
    }

    private void ResetTravellingLight(DayNightCyclePhases phase)
    {
        if (phase != DayNightCyclePhases.Day) return;
        StartCoroutine(FadeLightIntensity(travellingLight, travellingLightCruseIntensity, 0, transitionDuration));

    }

    private IEnumerator FadeLightIntensity(Light2D light, float from, float to, float duration)
    {
        if (from == 0) light.gameObject.SetActive(true);
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            float progress = counter / duration;
            float intensity = (from * (1 - progress)) + (to * progress);
            light.intensity = intensity;
            yield return null;
        }

        if (to == 0) light.gameObject.SetActive(false);
    }
}

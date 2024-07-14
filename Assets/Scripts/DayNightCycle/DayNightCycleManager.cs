using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
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

    [Header("General")]
    [SerializeField] private DayNightCyclePhases startingPhase;
    [Header("Durations")]
    [SerializeField] private SerializableDictionary<DayNightCyclePhases, float> phasesDurations;
    [Header("Global lighting colors")]
    [SerializeField] private SerializableDictionary<DayNightCyclePhases, Gradient> phaseColorGradients;
    [Header("Global lighting intensities")]
    [SerializeField] private SerializableDictionary<DayNightCyclePhases, AnimationCurve> phaseIntensityGradients;
    [Header("Lights")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private SerializableDictionary<DayNightCyclePhases, Light2D> travellingLights;
    [Header("Travelling path values")]
    [SerializeField] private int radius;
    [SerializeField] private float transitionDuration;
    [SerializeField] private Vector2Int centerTilePosition;
    private Vector3 easternPoint;
    private Vector3 westernPoint;
    private Dictionary<DayNightCyclePhases, float> travellingLightCruseIntensity;

    public int day { get; private set; }
    private DayNightCyclePhases phase;

    public static UnityEvent<DayNightCyclePhases> OnCyclePhaseStart { get; set; } = new UnityEvent<DayNightCyclePhases>();
    public static UnityEvent<DayNightCyclePhases> OnCyclePhaseEnd { get; set; } = new UnityEvent<DayNightCyclePhases>();

    private void Awake()
    {
        if (!globalLight) globalLight = GetComponent<Light2D>();
        OnCyclePhaseStart.AddListener(OnPhaseStart);
        OnCyclePhaseStart.AddListener(InitTravellingLight);
        OnCyclePhaseEnd.AddListener(OnPhaseEnd);
        OnCyclePhaseEnd.AddListener(ResetTravellingLight);
        travellingLightCruseIntensity = new Dictionary<DayNightCyclePhases, float>();
        Assert.AreNotEqual(phasesDurations.Count(), 0);
    }

    private void Start()
    {
        Vector3 centerWorldPosition = TilemapManager.Instance.groundTilemap.layoutGrid.CellToWorld((Vector3Int) centerTilePosition);
        easternPoint = new Vector3(centerWorldPosition.x + radius, centerWorldPosition.y);
        westernPoint = new Vector3(centerWorldPosition.x - radius, centerWorldPosition.y);
        foreach (DayNightCyclePhases phases in DayNightCyclePhases.GetValues(typeof(DayNightCyclePhases)))
        {
            travellingLightCruseIntensity[phases] = travellingLights[phases].intensity;
        }
        InitializeCycle();
    }

    private void OnPhaseStart(DayNightCyclePhases phase)
    {
        Debug.Log("Phase " + phase.ToString() + " has started and will last for " + phasesDurations[phase]);
    }

    private void OnPhaseEnd(DayNightCyclePhases phase)
    {
        Debug.Log("Phase " + phase.ToString() + " has ended.");
    }

    private void InitializeCycle()
    {
        day = 0;
        phase = startingPhase;

        StartNewPhaseCycle();
    }

    private void StartNewPhaseCycle()
    {
        StartCoroutine(StartCycleCoroutine());
    }

    private IEnumerator StartCycleCoroutine()
    {
        // invoke phase start event
        OnCyclePhaseStart.Invoke(phase);

        if (phase == DayNightCyclePhases.Day)
        {
            day++;
        }

        float timer = 0;
        float phaseDuration = phasesDurations[phase];
        while (timer < phaseDuration)
        {
            timer += Time.deltaTime;

            UpdateLights(timer / phaseDuration);
           
            yield return null;
        }

        OnCyclePhaseEnd.Invoke(phase);
        phase = phase.Next();
        StartNewPhaseCycle();
    }

    private void UpdateLights(float phaseProgress)   {
        // global light
        UpdateLightIntensity(globalLight, phaseProgress);
        UpdateLightColor(globalLight, phaseProgress);

        // travelling lights
        foreach (DayNightCyclePhases currentPhase in DayNightCyclePhases.GetValues(typeof(DayNightCyclePhases)))
        {
            UpdateLightTravel(phaseProgress);
        }
    }

    /**
     * Lerps the color of a given light.
     */
    private void UpdateLightColor(Light2D light, float phaseProgress)
    {
        if (!light.enabled) return;

        Gradient phaseColorGradient = phaseColorGradients[phase];
        if (phaseColorGradient == null)
        {
            Debug.LogError("Error : the phase color gradient does not exist for phase " + phase);
            return;
        }
        light.color = phaseColorGradient.Evaluate(phaseProgress);
    }

    /**
     * Lerps the intensity of a given light.
     */ 
    private void UpdateLightIntensity(Light2D light, float phaseProgress)
    {
        if (!light.enabled) return;

        AnimationCurve phaseIntensityCurve = phaseIntensityGradients[phase];
        if (phaseIntensityCurve == null)
        {
            Debug.LogError("Error : the phase intensity curve does not exist for phase " + phase);
            return;
        }
        light.intensity = phaseIntensityCurve.Evaluate(phaseProgress);
    }

    /**
     * Lerps the position of the active travelling light.
     */
    private void UpdateLightTravel(float phaseProgress)
    {
        Light2D light = travellingLights[phase];
        float x = Mathf.Lerp(easternPoint.x, westernPoint.x, SmoothLerpProgress(phaseProgress));
        float y = easternPoint.y + Mathf.Sin(phaseProgress * Mathf.PI) * radius;
        light.transform.position = new Vector3(x, y, 0);
        light.transform.rotation = Quaternion.Euler(0, 0, AngleProgress(phaseProgress));
    }
    
    private float SmoothLerpProgress(float progress)
    {
        return (Mathf.Sin((progress * Mathf.PI) - (Mathf.PI / 2)) + 1) / 2;
    }

    private float AngleProgress(float progress)
    {
        return 90 + 180 * progress;
    }

    /**
     * Called on phase start, init the active travelling light.
     */
    private void InitTravellingLight(DayNightCyclePhases eventPhase)
    {
        Light2D travellingLight = travellingLights[eventPhase];
        travellingLight.transform.position = easternPoint;
        StartCoroutine(FadeLightIntensity(travellingLight, 0, travellingLightCruseIntensity[eventPhase], transitionDuration));
    }

    /**
     * Called on phase end, fade out all inactive travelling lights.
     */
    private void ResetTravellingLight(DayNightCyclePhases eventPhase)
    {
        Light2D travellingLight = travellingLights[eventPhase];
        StartCoroutine(FadeLightIntensity(travellingLight, travellingLightCruseIntensity[eventPhase], 0, transitionDuration));
    }

    /**
     * Lerps light intensity and disables it if destination intensity is 0 or enables it if the start intensity is 0.
     */
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

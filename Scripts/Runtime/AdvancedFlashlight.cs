using UnityEngine;
using System.Collections;

public class AdvancedFlashlight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [SerializeField] private Light flashlightLight;
    [SerializeField] private AudioSource toggleSound;
    [SerializeField] private AudioSource flickerSound;
    [SerializeField] private bool handleInputInternally = true;
    [SerializeField] private KeyCode KeyPress = KeyCode.F;


    [Header("Flashlight Object")]
    [SerializeField] private GameObject flashlightObject;

    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyDrainRate = 1f;
    [SerializeField] private float lowEnergyThreshold = 20f;
    [SerializeField] private float criticalEnergyThreshold = 5f;
    [SerializeField] private bool maxEnergyAtFirst = true;
    [SerializeField] private bool infiniteEnergy = false;

    [Header("Flicker Settings")]
    [SerializeField] private float minFlickerDelay = 0.05f;
    [SerializeField] private float maxFlickerDelay = 0.2f;
    [SerializeField] private float minFlickerIntensity = 0.1f;
    [SerializeField] private float flickerDuration = 1.5f;

    [Space(15)]
    [SerializeField] private float currentEnergy;

    private bool isOn;
    private bool isFlickering;
    private float originalIntensity;
    private Coroutine flickerRoutine;

    private void Awake()
    {
        if (!flashlightLight)
            flashlightLight = GetComponentInChildren<Light>();

        originalIntensity = flashlightLight.intensity;
        if (maxEnergyAtFirst)
            currentEnergy = maxEnergy;
        SetFlashlight(false);
    }

    private void Update()
    {
        if (handleInputInternally && Input.GetKeyDown(KeyPress))
        {
            ToggleFlashlight();
        }

        if (isOn && !infiniteEnergy)
        {
            DrainEnergy();
            HandleFlickering();
        }
    }

    public void ToggleFlashlight()
    {
        if (currentEnergy <= 0 && !isOn) return;

        isOn = !isOn;
        SetFlashlight(isOn);

        if (toggleSound != null)
            toggleSound.Play();

        if (!isOn) StopFlicker();
    }

    private void SetFlashlight(bool state)
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = state;
            flashlightLight.intensity = state ? originalIntensity : 0f;
        }

        if (flashlightObject != null)
        {
            flashlightObject.SetActive(state);
        }
    }

    private void DrainEnergy()
    {
        currentEnergy -= energyDrainRate * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        if (currentEnergy < lowEnergyThreshold)
        {
            float energyPercent = currentEnergy / lowEnergyThreshold;
            flashlightLight.intensity = Mathf.Lerp(originalIntensity * 0.3f, originalIntensity, energyPercent);
        }

        if (currentEnergy <= 0f)
        {
            ForceTurnOff();
        }
    }

    private void HandleFlickering()
    {
        if (isFlickering) return;

        if (currentEnergy <= criticalEnergyThreshold)
        {
            StartFlicker(CriticalEnergyFlicker());
        }
        else if (currentEnergy <= lowEnergyThreshold && Random.value < 0.01f)
        {
            StartFlicker(EnergyWarningFlicker());
        }
    }

    private void StartFlicker(IEnumerator flickerCoroutine)
    {
        if (flickerRoutine != null) StopCoroutine(flickerRoutine);
        flickerRoutine = StartCoroutine(flickerCoroutine);
    }

    private void StopFlicker()
    {
        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }

        isFlickering = false;
    }

    private IEnumerator EnergyWarningFlicker()
    {
        isFlickering = true;
        if (flickerSound != null)
            flickerSound.Play();

        float elapsed = 0f;
        while (elapsed < flickerDuration / 2f)
        {
            flashlightLight.enabled = !flashlightLight.enabled;
            float delay = Random.Range(minFlickerDelay, maxFlickerDelay);
            yield return new WaitForSeconds(delay);
            elapsed += delay;
        }

        SetFlashlight(isOn);
        isFlickering = false;
    }

    private IEnumerator CriticalEnergyFlicker()
    {
        isFlickering = true;
        if (flickerSound != null)
            flickerSound.Play();

        float elapsed = 0f;
        while (elapsed < flickerDuration && currentEnergy > 0f)
        {
            flashlightLight.enabled = !flashlightLight.enabled;

            if (flashlightLight.enabled)
                flashlightLight.intensity = Random.Range(minFlickerIntensity, originalIntensity * 0.5f);

            float delay = Random.Range(minFlickerDelay / 2f, maxFlickerDelay / 2f);
            yield return new WaitForSeconds(delay);
            elapsed += delay;
        }

        SetFlashlight(currentEnergy > 0f && isOn);
        isFlickering = false;
    }

    private void ForceTurnOff()
    {
        isOn = false;
        SetFlashlight(false);
        StopFlicker();
        if (toggleSound != null)
            toggleSound.Play();
    }

    public void AddEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
    }

    public float GetMaxEnergy() => maxEnergy;
    public float GetCurrentEnergy() => currentEnergy;
    public float GetEnergyPercentage() => currentEnergy / maxEnergy;
    public bool IsOn() => isOn;
}
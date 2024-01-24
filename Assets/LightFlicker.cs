using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    Light lightComponent;
    bool isFlickering;


    [SerializeField] private float minFlickerTime;
    [SerializeField] private float maxFlickerTime;

    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;

    [SerializeField] private float defaultIntensity;
    [SerializeField] private float flickerIntensity;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
        StartCoroutine(StartFlicker());
    }

    private void Update()
    {
        if (!isFlickering)
        {
            StartCoroutine(FlickerLight());
        }
    }

    IEnumerator StartFlicker()
    {
        isFlickering = true;
        lightComponent.intensity = defaultIntensity;
        yield return new WaitForSeconds(Random.Range(0f, minWaitTime));
        lightComponent.intensity = flickerIntensity;
        yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));
        isFlickering = false;
    }

    IEnumerator FlickerLight()
    {
        isFlickering = true;
        lightComponent.intensity = defaultIntensity;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        lightComponent.intensity = flickerIntensity;
        yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));
        isFlickering = false;
    }
}

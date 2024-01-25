using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLightFlash : MonoBehaviour
{
    bool isFlashing;
    Light redLight;
    [SerializeField] private float flashSpeed;

    private void Start()
    {
        redLight = GetComponent<Light>();
    }

    private void Update()
    {
        if (!isFlashing)
        {
            StartCoroutine(LightFlashDelay());
        }
    }

    IEnumerator LightFlashDelay()
    {
        isFlashing = true;
        redLight.enabled = false;
        yield return new WaitForSeconds(flashSpeed);
        redLight.enabled = true;
        yield return new WaitForSeconds(flashSpeed);
        isFlashing = false;
    }
}

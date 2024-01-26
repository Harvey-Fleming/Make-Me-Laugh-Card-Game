using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightswitch : MonoBehaviour
{
    bool isLightsOn;
    [SerializeField] private GameObject[] lights;

    private void Start()
    {
        LightOnOff(false);
    }

    public void LightOnOff(bool lightBool)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(lightBool);
        }

        isLightsOn = !lightBool;
    }
}

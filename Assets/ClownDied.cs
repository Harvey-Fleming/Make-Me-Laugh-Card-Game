using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownDied : MonoBehaviour
{
    [SerializeField] private GameObject redLight;

    public void TurnLightOff()
    {
        redLight.SetActive(false);
    }
}

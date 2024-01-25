using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBalloonInactive : MonoBehaviour
{
    private void Start()
    {
        
    }

    public void SetInactive()
    {
        gameObject.SetActive(false);
    }

    public void BalloonPopSFX()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.balloonPop, transform.position);
    }
}

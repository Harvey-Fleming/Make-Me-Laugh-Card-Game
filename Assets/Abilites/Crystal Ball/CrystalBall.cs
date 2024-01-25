using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBall : Ability
{
    [SerializeField] private GameObject smokeFX;

    public override void Use()
    {
        smokeFX.SetActive(true);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.CrystalBall, transform.position);
        gameObject.SetActive(false);
        CardManager.Instance.StartFlipCards();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBall : Ability
{
    [SerializeField] private GameObject smokeFX;

    public override void Use()
    {
        smokeFX.SetActive(true);
        gameObject.SetActive(false);
    }
}

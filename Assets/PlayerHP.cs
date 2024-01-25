using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private int healthPoints;
    [SerializeField] private int maxHealthPoints;

    [SerializeField] private GameObject[] balloons;

    private void Start()
    {
        healthPoints = maxHealthPoints;
    }

    public void TakeDamage()
    {
        healthPoints--;

        Debug.Log("Lower HP");

        if(balloons[0].activeSelf == true)
        {
            balloons[0].GetComponent<Animator>().SetTrigger("Popped");
        }
        else
        {
            balloons[1].GetComponent<Animator>().SetTrigger("Popped");
        }
    }
}

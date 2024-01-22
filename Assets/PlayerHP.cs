using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private int healthPoints;
    [SerializeField] private int maxHealthPoints;

    [SerializeField] private GameObject[] balloons;
    [SerializeField] private GameObject[] poppedBalloons;

    private void Start()
    {
        healthPoints = maxHealthPoints;

        for (int i = 0; i < poppedBalloons.Length; i++)
        {
            poppedBalloons[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        healthPoints--;

        if(balloons[0].activeSelf == true)
        {
            balloons[0].SetActive(false);
            poppedBalloons[0].SetActive(true);
        }
        else
        {
            balloons[1].SetActive(false);
            poppedBalloons[1].SetActive(true);
        }
    }
}

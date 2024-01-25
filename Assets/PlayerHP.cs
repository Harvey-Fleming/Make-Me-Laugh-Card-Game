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

    public void TakeDamage()
    {
        healthPoints--;

        Debug.Log("Lower HP");

        if(balloons[0].activeSelf == true)
        {
            balloons[0].SetActive(false);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.balloonPop, balloons[0].transform.position);
            poppedBalloons[0].SetActive(true);
            Debug.Log("Change graphic");
        }
        else
        {
            balloons[1].SetActive(false);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.balloonPop, balloons[1].transform.position);
            poppedBalloons[1].SetActive(true);
            Debug.Log("Change graphic");
        }
    }
}

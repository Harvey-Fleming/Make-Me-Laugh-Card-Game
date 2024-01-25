using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaughOMeterUI : MonoBehaviour
{
    [SerializeField] private Vector3 minRotation;
    [SerializeField] private Vector3 maxRotation;
    [SerializeField] private float currentValue;
    [SerializeField] private float maxValue;

    [SerializeField] private Transform arrow;

    bool isMoving;
    float timer;

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if(timer < currentValue)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = currentValue;
            }
        }
        else
        {
            if(timer > 0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0f;
            }
        }

        arrow.localRotation = Quaternion.Euler(Vector3.Lerp(minRotation, maxRotation, timer / maxValue));
    }

    void SubmitScore(int score)
    {
        currentValue = score;
        isMoving = true;
    }

    void ResetScore()
    {
        isMoving = false;
        currentValue = 0f;
    }

    #region - Event Subscription
    private void OnEnable()
    {
        CardManager.SubmitScore += SubmitScore;
        CardManager.OnRoundStart += ResetScore;
    }

    private void OnDisable()
    {
        CardManager.SubmitScore -= SubmitScore;
        CardManager.OnRoundStart -= ResetScore;
    }
    #endregion
}

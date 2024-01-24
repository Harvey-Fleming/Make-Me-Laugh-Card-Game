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

    // Update is called once per frame
    void Update()
    {
        arrow.localRotation = Quaternion.Euler(Vector3.Lerp(minRotation, maxRotation, currentValue / maxValue));
    }

    void SubmitScore(int score)
    {
        currentValue = score;
    }

    void ResetScore()
    {
        currentValue = 0;
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

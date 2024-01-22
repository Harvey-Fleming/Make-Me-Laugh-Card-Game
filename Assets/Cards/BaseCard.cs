using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseCard : MonoBehaviour
{
    [SerializeField] private CardObject cardDetails;
    [SerializeField] private int scoreToAdd = 1;

    [Space]
    [SerializeField] private TMP_Text jokeText;

    [Space]
    [SerializeField] private GameObject dotLayoutParent;
    [SerializeField] private GameObject dotPrefab;

    public int ScoreToAdd { get => scoreToAdd; set => scoreToAdd = value; }
    public CardObject CardDetails { get => cardDetails; set => cardDetails = value; }

    // Start is called before the first frame update
    void Start()
    {
        jokeText.text = cardDetails.PartOfJoke;

        for(int i = 0; i < (int)cardDetails.CardType; i++)
        {
            Instantiate(dotPrefab, dotLayoutParent.transform);
        }
    }
}



public enum CardType
{
    Start,
    Middle,
    End,
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseCard : MonoBehaviour
{
    [SerializeField] private CardObject cardDetails;
    [SerializeField] private int scoreToAdd = 1;

    [Space]
    [SerializeField] private TMP_Text jokeText;

    [Space]
    [SerializeField] private Image FrontIcon;
    [SerializeField] private Image BackIcon;
    [Space]
    [SerializeField] private Sprite elipsesSprite;
    [SerializeField] private Sprite questionSprite;
    [SerializeField] private Sprite exclamationSprite;

    public int ScoreToAdd { get => scoreToAdd; set => scoreToAdd = value; }
    public CardObject CardDetails { get => cardDetails; set => cardDetails = value; }

    // Start is called before the first frame update
    void Start()
    {
        jokeText.text = cardDetails.PartOfJoke;

        switch (cardDetails.CardType)
        {
            case CardType.Start:
                FrontIcon.sprite = elipsesSprite;
                BackIcon.sprite = elipsesSprite;
                break;
            case CardType.Middle:
                FrontIcon.sprite = questionSprite;
                BackIcon.sprite = questionSprite;
                break;
            case CardType.End:
                FrontIcon.sprite = exclamationSprite;
                BackIcon.sprite = exclamationSprite;
                break;
        }

    }
}



public enum CardType
{
    Start,
    Middle,
    End,
}


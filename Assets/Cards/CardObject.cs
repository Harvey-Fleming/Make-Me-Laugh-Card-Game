using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New Card", menuName = "Card Game/Card")]
public class CardObject : ScriptableObject
{
    [SerializeField] private CardType cardType;

    [SerializeField] private string partOfJoke;

    [SerializeField] private CardObject[] synergies;

    public CardObject[] Synergies { get => synergies;}
    public string PartOfJoke { get => partOfJoke;}
    public CardType CardType { get => cardType;}
}

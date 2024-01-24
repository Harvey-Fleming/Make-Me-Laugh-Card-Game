using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New Card", menuName = "Card Game/Card")]
public class CardObject : ScriptableObject
{
    [SerializeField] private CardType cardType;

    [SerializeField] private string partOfJoke;

    [SerializeField] private CardObject[] weakSynergies;
    [SerializeField] private CardObject[] strongSynergies;

    public CardObject[] WeakSynergies { get => weakSynergies; }
    public CardObject[] StrongSynergies { get => strongSynergies; }
    public string PartOfJoke { get => partOfJoke;}
    public CardType CardType { get => cardType;}
}

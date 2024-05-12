using System;
using UnityEngine;

[Serializable]
public class RoundSettingsSerialized: ISettings, ISettingsStorage
{
    [SerializeField, Header("Cards amount to place")] private int _cardsAmount;
    [SerializeField, Header("If checked reveals cards on start of round")] private bool _showFrontOnStartOfRound;
    [SerializeField, Header("If checked hides cards upon cards match")] private bool _hideCardsOnMatch;
    [SerializeField, Header("Cards reveal duration at the start of round")] private float _roundStartRevealDuration;
    [SerializeField, Header("Cards reveal duration when player picked wrong cards")] private float _cardRevealOnPairFailDuration;
    [SerializeField, Header("Cards reveal duration on hint")] private float _hintRevealDuration;
    [SerializeField, Header("Minimal round duration")] private float _baseRoundDuration;
    [SerializeField, Header("Additional round duration for every used card")] private float _additionalRoundDurationPerCard;

    public int CardsAmount { get => _cardsAmount; set => _cardsAmount = value; }
    public bool ShowFrontOnStartOfRound { get => _showFrontOnStartOfRound; set => _showFrontOnStartOfRound = value; }
    public bool HideCardsOnMatch { get => _hideCardsOnMatch; set => _hideCardsOnMatch = value; }
    public float RoundStartRevealDuration { get => _roundStartRevealDuration; set => _roundStartRevealDuration = value; }
    public float HintRevealDuration { get => _hintRevealDuration; set => _hintRevealDuration = value; }
    public float CardRevealOnPairFailDuration { get => _cardRevealOnPairFailDuration; set => _cardRevealOnPairFailDuration = value; }
    public float BaseRoundDuration { get => _baseRoundDuration; set => _baseRoundDuration = value; }
    public float RoundDurationIncrementPerCard { get => _additionalRoundDurationPerCard; set => _additionalRoundDurationPerCard = value; }

    public float GetRoundDuration(int cardPairs) => _baseRoundDuration + _additionalRoundDurationPerCard * cardPairs;
}
using UnityEngine;

[CreateAssetMenu(fileName = "RoundSettings", menuName = "RoundSettings")]
public class RoundSettings : ScriptableObject
{
    [SerializeField, Header("Reveal cards on start of round")] bool _showFrontOnStartOfRound;
    [SerializeField, Header("Action performed with matched cards")] MatchAction _matchAction;
    [SerializeField, Header("Cards reveal duration at the start of round")] private float _roundStartRevealDuration;
    [SerializeField, Header("Cards reveal duration when player picked wrong cards")] private float _cardRevealOnPairFailDuration;
    [SerializeField, Header("Cards reveal duration on hint")] private float _hintRevealDuration;
    [SerializeField, Header("Minimal round duration")] private float _baseRoundDuration;
    [SerializeField, Header("Duration increment for every used card")] private float _roundDurationIncrement;

    public bool ShowFrontOnStartOfRound => _showFrontOnStartOfRound;
    public MatchAction MatchAction => _matchAction;
    public float RoundStartRevealDuration => _roundStartRevealDuration;
    public float HintRevealDuration => _hintRevealDuration;
    public float CardRevealOnPairFailDuration => _cardRevealOnPairFailDuration;

    public float GetRoundDuration(int cardPairs) => _baseRoundDuration + _roundDurationIncrement * cardPairs;
}
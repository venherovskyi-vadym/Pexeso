public interface ISettings
{
    int CardsAmount { get; }
    bool ShowFrontOnStartOfRound { get; }
    bool HideCardsOnMatch { get; }
    float RoundStartRevealDuration { get; }
    float HintRevealDuration { get; }
    float CardRevealOnPairFailDuration { get; }
    float GetRoundDuration(int cardPairs);
}
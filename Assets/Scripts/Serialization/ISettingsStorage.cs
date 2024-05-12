public interface ISettingsStorage
{
    int CardsAmount { get; set; }
    bool ShowFrontOnStartOfRound { get; set; }
    bool HideCardsOnMatch { get; set; }
    float RoundStartRevealDuration { get; set; }
    float HintRevealDuration { get; set; }
    float CardRevealOnPairFailDuration { get; set; }
    float BaseRoundDuration { get; set; }
    float RoundDurationIncrementPerCard { get; set; }    
}
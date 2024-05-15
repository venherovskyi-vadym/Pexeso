using System;
using UnityEngine;

public class PlayerPrefsStorage : ISettings, ISettingsStorage
{
    private const string _cardsAmount = "cardsAmount";
    private const string _showFrontOnStartOfRound = "showFrontOnStartOfRound";
    private const string _hideCardsOnMatch = "hideCardsOnMatch";
    private const string _roundStartRevealDuration = "roundStartRevealDuration";
    private const string _cardRevealOnPairFailDuration = "cardRevealOnPairFailDuration";
    private const string _hintRevealDuration = "hintRevealDuration";
    private const string _baseRoundDuration = "baseRoundDuration";
    private const string _additionalRoundDurationPerCard = "additionalRoundDurationPerCard";

    private string GetPrefsKey(string propertyName)
    {
        return $"{Application.productName}.{propertyName}";
    }

    public bool ShowFrontOnStartOfRound 
    { 
        get => PlayerPrefs.HasKey(GetPrefsKey(_showFrontOnStartOfRound));
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt(GetPrefsKey(_showFrontOnStartOfRound), 1);
            }
            else 
            {
                PlayerPrefs.DeleteKey(GetPrefsKey(_showFrontOnStartOfRound));
            }
            PlayerPrefs.Save();
        }
    }

    public bool HideCardsOnMatch
    {
        get => PlayerPrefs.HasKey(GetPrefsKey(_hideCardsOnMatch));
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt(GetPrefsKey(_hideCardsOnMatch), 1);
            }
            else
            {
                PlayerPrefs.DeleteKey(GetPrefsKey(_hideCardsOnMatch));
            }
            PlayerPrefs.Save();
        }
    }

    public float RoundStartRevealDuration
    {
        get => PlayerPrefs.GetFloat(GetPrefsKey(_roundStartRevealDuration), 10);
        set 
        { 
            PlayerPrefs.SetFloat(GetPrefsKey(_roundStartRevealDuration), Math.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public float HintRevealDuration
    {
        get => PlayerPrefs.GetFloat(GetPrefsKey(_hintRevealDuration), 10);
        set
        {
            PlayerPrefs.SetFloat(GetPrefsKey(_hintRevealDuration), Math.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public float CardRevealOnPairFailDuration
    {
        get => PlayerPrefs.GetFloat(GetPrefsKey(_cardRevealOnPairFailDuration), 0);
        set
        {
            PlayerPrefs.SetFloat(GetPrefsKey(_cardRevealOnPairFailDuration), Math.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public float BaseRoundDuration 
    {
        get => PlayerPrefs.GetFloat(GetPrefsKey(_baseRoundDuration), 30);
        set
        {
            PlayerPrefs.SetFloat(GetPrefsKey(_baseRoundDuration), Math.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public float RoundDurationIncrementPerCard 
    {
        get => PlayerPrefs.GetFloat(GetPrefsKey(_additionalRoundDurationPerCard), 10);
        set
        {
            PlayerPrefs.SetFloat(GetPrefsKey(_additionalRoundDurationPerCard), Math.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public int CardsAmount
    {
        get => PlayerPrefs.GetInt(GetPrefsKey(_cardsAmount), 30);
        set
        {
            PlayerPrefs.SetInt(GetPrefsKey(_cardsAmount), Math.Max(2, value));
            PlayerPrefs.Save();
        }
    }


    public float GetRoundDuration(int cardPairs) => BaseRoundDuration + RoundDurationIncrementPerCard * cardPairs;
}

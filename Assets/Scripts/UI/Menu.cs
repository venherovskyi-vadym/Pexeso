using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Toggle _showCardsOnRoundStart;
    [SerializeField] private Toggle _hideCardsOnMatch;
    [SerializeField] private TMP_InputField _cardsAmount;
    [SerializeField] private TMP_InputField _baseRoundDuration;
    [SerializeField] private TMP_InputField _additionalRoundDurationPerCard;
    [SerializeField] private TMP_InputField _cardRevealOnStartDuration;
    [SerializeField] private TMP_InputField _hintRevealDuration;
    [SerializeField] private TMP_InputField _cardRevealOnPairFailDuration;

    private ISettingsStorage _settingsStorage;
    public event Action OnStartRound;
    public event Action OnDestroyEvent;

    [Inject]
    public void PostConstruct(ISettingsStorage settingsStorage)
    {
        _settingsStorage = settingsStorage;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        _showCardsOnRoundStart.isOn = _settingsStorage.ShowFrontOnStartOfRound;
        _hideCardsOnMatch.isOn = _settingsStorage.HideCardsOnMatch;
        _cardsAmount.text = _settingsStorage.CardsAmount.ToString();
        _baseRoundDuration.text = _settingsStorage.BaseRoundDuration.ToString();
        _additionalRoundDurationPerCard.text = _settingsStorage.RoundDurationIncrementPerCard.ToString();
        _hintRevealDuration.text = _settingsStorage.HintRevealDuration.ToString();
        _cardRevealOnPairFailDuration.text = _settingsStorage.CardRevealOnPairFailDuration.ToString();
        _cardRevealOnStartDuration.text = _settingsStorage.RoundStartRevealDuration.ToString();

        _startButton.onClick.AddListener(StartRound);
        _showCardsOnRoundStart.onValueChanged.AddListener(SaveShowCardsOnRoundStart);
        _hideCardsOnMatch.onValueChanged.AddListener(SaveHideCardsOnMatch);
        _cardsAmount.onValueChanged.AddListener(SaveCardsAmount);
        _baseRoundDuration.onValueChanged.AddListener(SaveBaseRoundDuration);
        _additionalRoundDurationPerCard.onValueChanged.AddListener(SaveAdditionalRoundDurationPerCard);
        _hintRevealDuration.onValueChanged.AddListener(SaveHintRevealDuration);
        _cardRevealOnPairFailDuration.onValueChanged.AddListener(SaveCardRevealOnPairFailDuration);
        _cardRevealOnStartDuration.onValueChanged.AddListener(SaveCardRevealOnStartDuration);
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
        _startButton.onClick.RemoveListener(StartRound);
        _showCardsOnRoundStart.onValueChanged.RemoveListener(SaveShowCardsOnRoundStart);
        _hideCardsOnMatch.onValueChanged.RemoveListener(SaveHideCardsOnMatch);
        _cardsAmount.onValueChanged.RemoveListener(SaveCardsAmount);
        _baseRoundDuration.onValueChanged.RemoveListener(SaveBaseRoundDuration);
        _additionalRoundDurationPerCard.onValueChanged.RemoveListener(SaveAdditionalRoundDurationPerCard);
        _hintRevealDuration.onValueChanged.RemoveListener(SaveHintRevealDuration);
        _cardRevealOnPairFailDuration.onValueChanged.RemoveListener(SaveCardRevealOnPairFailDuration);
        _cardRevealOnStartDuration.onValueChanged.RemoveListener(SaveCardRevealOnStartDuration);
    }

    private void StartRound()
    {
        OnStartRound?.Invoke();
    }

    private void SaveCardRevealOnStartDuration(string arg0)
    {
        if (float.TryParse(arg0, out var cardRevealOnStartDuration))
        {
            _settingsStorage.RoundStartRevealDuration = Math.Max(0, cardRevealOnStartDuration);
        }
    }

    private void SaveCardRevealOnPairFailDuration(string arg0)
    {
        if (float.TryParse(arg0, out var cardRevealOnPairFailDuration))
        {
            _settingsStorage.CardRevealOnPairFailDuration = Math.Max(0, cardRevealOnPairFailDuration);
        }
    }

    private void SaveHintRevealDuration(string arg0)
    {
        if (float.TryParse(arg0, out var hintRevealDuration))
        {
            _settingsStorage.HintRevealDuration = Math.Max(0, hintRevealDuration);
        }
    }

    private void SaveAdditionalRoundDurationPerCard(string arg0)
    {
        if (float.TryParse(arg0, out var roundDurationIncrementPerCard))
        {
            _settingsStorage.RoundDurationIncrementPerCard = Math.Max(0, roundDurationIncrementPerCard);
        }
    }

    private void SaveBaseRoundDuration(string arg0)
    {
        if (float.TryParse(arg0, out var baseRoundDuration))
        {
            _settingsStorage.BaseRoundDuration = Math.Max(0, baseRoundDuration);
        }
    }

    private void SaveCardsAmount(string arg0)
    {
        if (int.TryParse(arg0, out var cardsAmount))
        {
            _settingsStorage.CardsAmount = Math.Max(0, cardsAmount);
        }
    }

    private void SaveHideCardsOnMatch(bool arg0)
    {
        _settingsStorage.HideCardsOnMatch = arg0;
    }

    private void SaveShowCardsOnRoundStart(bool arg0)
    {
        _settingsStorage.ShowFrontOnStartOfRound = arg0;
    }
}
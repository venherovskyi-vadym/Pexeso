using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private Button _backToMenu;
    [SerializeField] private Button _hint;
    [SerializeField] private TextMeshProUGUI _remainingRoundTime;
    [SerializeField] private TextMeshProUGUI _cardsCollected;
    [SerializeField] private AnimatorTrigger _winTrigger;
    [SerializeField] private AnimatorTrigger _lostTrigger;

    public event Action OnBackToMenu;
    public event Action OnHint;

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
        _backToMenu.onClick.AddListener(BackToMenu);
        _hint.onClick.AddListener(Hint);
    }

    private void OnDestroy()
    {
        _backToMenu.onClick.RemoveListener(BackToMenu);
        _hint.onClick.RemoveListener(Hint);
    }

    public void SetRoundResult(RoundResult result)
    {
        if (result == RoundResult.Win)
        {
            _winTrigger.Trigger();
        }

        if (result == RoundResult.Loose)
        {
            _lostTrigger.Trigger();
        }
    }

    public void UpdateRemainingTime(float remainingTime)
    {
        _remainingRoundTime.text = TimeSpan.FromSeconds(remainingTime).ToString(@"m\:ss");
    }
    public void UpdateMatchedCards(int matchedCards)
    {
        _cardsCollected.text = matchedCards.ToString();
    }

    private void BackToMenu()
    {
        OnBackToMenu?.Invoke();
    }

    private void Hint()
    {
        OnHint?.Invoke();
    }
}
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CardView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _cardFront;
    [SerializeField] private Image _cardBack;
    [SerializeField] private AnimatorTrigger _turnToFront;
    [SerializeField] private AnimatorTrigger _turnToBack;
    [SerializeField] private AnimatorTrigger _hide;

    private UniTaskCompletionSource _taskCompletionSource;

    public int CardId { get; private set; }
    public event Action<CardView> OnClick;

    private void Awake()
    {
        _button.onClick.AddListener(CardClicked);
    }

    private void OnDestroy()
    {
        _taskCompletionSource = null;
        _button.onClick.RemoveListener(CardClicked);
    }

    public void InitCardFront(Sprite cardSprite, int cardId)
    {
        _cardFront.sprite = cardSprite;
        CardId = cardId;
    }

    public void InitCardBack(Sprite cardBack)
    {
        _cardBack.sprite = cardBack;
    }

    public void TurnToFront()
    {
        _button.interactable = false;
        _turnToFront.Trigger();
        _taskCompletionSource = new UniTaskCompletionSource();
    }

    public void TurnToBack()
    {
        _button.interactable = false;
        _turnToBack.Trigger();
        _taskCompletionSource = new UniTaskCompletionSource();
    }

    public void SetInteractableButton(bool interactable)
    {
        _button.interactable = interactable;
    }

    public UniTask WaitTurnComplete()
    {
        if (_taskCompletionSource == null)
        {
            return UniTask.CompletedTask;
        }

        return _taskCompletionSource.Task;
    }

    public void Hide()
    {
        _hide.Trigger();
    }

    private void AnimationComplete()
    {
        _taskCompletionSource.TrySetResult();
        _button.interactable = true;
    }

    private void CardClicked()
    {
        OnClick?.Invoke(this);
    }

    public class Factory : PlaceholderFactory<CardView>
    {
    }
}
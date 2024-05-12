using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

public class GamePresenter
{
    private readonly CardPool _cardPool;
    private readonly ISettings _roundSettings;
    private readonly Menu _menu;
    private readonly RoundUI _roundUI;
    private readonly List<int> _cardIds = new List<int>();
    private readonly List<CardPresenter> _cards = new List<CardPresenter>();
    private readonly List<CardPresenter> _matchedCards = new List<CardPresenter>();
    private readonly List<UniTask> _turnTasks = new List<UniTask>();
    private CardPresenter _previousCardPresenter;
    private UniTaskCompletionSource<RoundResult> _winCompletionSource;
    private float _roundRemainingTime;

    public GamePresenter(CardPool cardPool, ISettings roundSettings, Menu menu, RoundUI roundUI)
    {
        _cardPool = cardPool;
        _roundSettings = roundSettings;
        _menu = menu;
        _roundUI = roundUI;
        _menu.Show();
        _menu.OnStartRound += StartRound;
        _menu.OnDestroyEvent += Dispose;
        _roundUI.OnBackToMenu += BackToMenu;
        _roundUI.OnHint += Hint;
        _roundUI.Hide();
    }

    public async void StartRound()
    {
        _roundUI.Show();
        _menu.Hide();
        await UniTask.DelayFrame(1);
        var cardsAmount = _roundSettings.CardsAmount;
        var trimmedFieldSize = Math.Min(cardsAmount, _cardPool.CardsVariantsCount * 2);
        var oddCount = trimmedFieldSize % 2 == 1;

        if (oddCount)
        {
            trimmedFieldSize--;
        }

        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].OnCardClicked -= OnCardClicked;
            _cardPool.ReturnCard(_cards[i]);
        }

        _cards.Clear();
        _matchedCards.Clear();

        FillCardIds(cardsAmount, trimmedFieldSize);

        for (int i = _cardIds.Count - 1; i >= 0; i--)
        {
            var index = UnityEngine.Random.Range(0, _cardIds.Count);
            var card = _cardPool.GetCard(_roundSettings.ShowFrontOnStartOfRound, _cardIds[index]);
            _cards.Add(card);
            card.DisableButton();
            _cardIds.RemoveAt(index);
        }

        _roundRemainingTime = (int)_roundSettings.GetRoundDuration(_cards.Count);
        _roundUI.UpdateRemainingTime(_roundRemainingTime);

        if (_roundSettings.ShowFrontOnStartOfRound)
        {
            await Task.Delay(TimeSpan.FromSeconds(_roundSettings.RoundStartRevealDuration));

            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].TurnToBack();
            }
        }

        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].EnableButton();
            _cards[i].SetInteractableButton(true);
            _cards[i].OnCardClicked += OnCardClicked;
        }

        if (_cards.Count == 0)
        {
            return;
        }

        _winCompletionSource = new UniTaskCompletionSource<RoundResult>();

        while (_roundRemainingTime > 0 && _winCompletionSource.Task.Status != UniTaskStatus.Succeeded)
        {
            if (_cards.Count == 0)
            {
                return;
            }
            await UniTask.Delay(1000);
            _roundRemainingTime--;
            _roundUI.UpdateRemainingTime(_roundRemainingTime);
        }

        _winCompletionSource.TrySetResult(RoundResult.Loose);
    }

    public UniTask<RoundResult> WaitForResult()
    {
        return _winCompletionSource.Task;
    }

    private void BackToMenu()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].OnCardClicked -= OnCardClicked;
            _cardPool.ReturnCard(_cards[i]);
        }

        _cards.Clear();
        _matchedCards.Clear();

        if (_winCompletionSource == null)
        {
            _winCompletionSource = new UniTaskCompletionSource<RoundResult>();
        }

        _winCompletionSource.TrySetResult(RoundResult.Quit);
        _roundUI.Hide();
        _menu.Show();
    }

    private async void Hint()
    {
        if (_previousCardPresenter != null && _previousCardPresenter.Model.IsOnFront)
        {
            await _previousCardPresenter.WaitTurnComplete();
            if (_previousCardPresenter.Model.IsOnFront)
            {
                _previousCardPresenter.TurnToBack();
                await _previousCardPresenter.WaitTurnComplete();
            }
        }

        var newCardsOrder = new List<CardPresenter>();

        for (int i = _cards.Count - 1; i >= 0; i--)
        {
            var randomIndex = UnityEngine.Random.Range(0, _cards.Count);
            newCardsOrder.Add(_cards[randomIndex]);
            _cards.RemoveAt(randomIndex);
        }

        _cards.Clear();
        _cards.AddRange(newCardsOrder);

        for (int i = _cards.Count - 1; i >= 0; i--)
        {
            _cards[i].View.transform.SetSiblingIndex(i);
        }

        _turnTasks.Clear();

        for (int i = 0; i < _cards.Count; i++)
        {
            var card = _cards[i];

            if (_matchedCards.Contains(card))
            {
                continue;
            }

            card.TurnToFront();
            _turnTasks.Add(card.WaitTurnComplete());
        }

        await UniTask.WhenAll(_turnTasks);
        await UniTask.Delay(TimeSpan.FromSeconds(_roundSettings.HintRevealDuration));
        _turnTasks.Clear();

        for (int i = 0; i < _cards.Count; i++)
        {
            var card = _cards[i];

            if (_matchedCards.Contains(card))
            {
                continue;
            }

            card.TurnToBack();
            _turnTasks.Add(card.WaitTurnComplete());
        }
    }

    private void OnCardClicked(CardPresenter presenter, bool wasOnFront, bool willBeOnFront)
    {
        if (wasOnFront || presenter.Model.IsMatched || _previousCardPresenter == presenter)
        {
            return;
        }

        if (_previousCardPresenter == null)
        {
            _previousCardPresenter = presenter;
            presenter.DisableButton();

            return;
        }

        if (_previousCardPresenter.Model.CardId == presenter.Model.CardId)
        {
            MatchBothCards(presenter, _previousCardPresenter);
            CheckWinRound();
            _previousCardPresenter = null;
        }
        else
        {
            TurnBothCardsOnBack(presenter, _previousCardPresenter);
            _previousCardPresenter = null;
        }
    }

    private void CheckWinRound()
    {
        if (_matchedCards.Count == _cards.Count)
        {
            _winCompletionSource.TrySetResult(RoundResult.Win);
        }
    }

    private async void TurnBothCardsOnBack(CardPresenter presenterForWaiting, CardPresenter otherPresenter)
    {
        await presenterForWaiting.WaitTurnComplete();
        await UniTask.Delay(TimeSpan.FromSeconds(_roundSettings.CardRevealOnPairFailDuration));
        presenterForWaiting.TurnToBack();
        otherPresenter.TurnToBack();
        await presenterForWaiting.WaitTurnComplete();
        presenterForWaiting.EnableButton();
        otherPresenter.EnableButton();
    }

    private async void MatchBothCards(CardPresenter presenterForWaiting, CardPresenter otherPresenter)
    {
        if (!_matchedCards.Contains(presenterForWaiting))
        {
            _matchedCards.Add(presenterForWaiting);
        }

        if (!_matchedCards.Contains(otherPresenter))
        {
            _matchedCards.Add(otherPresenter);
        }

        presenterForWaiting.Model.IsMatched = true;
        otherPresenter.Model.IsMatched = true;
        presenterForWaiting.DisableButton();
        otherPresenter.DisableButton();
        await presenterForWaiting.WaitTurnComplete();

        if (_roundSettings.HideCardsOnMatch)
        {
            presenterForWaiting.Hide();
            otherPresenter.Hide();
        }

        _roundUI.UpdateMatchedCards(_matchedCards.Count);
    }

    private void FillCardIds(int initialFieldSize, int fieldSize)
    {
        //collection of card Ids forming for randomization purposes
        _cardIds.Capacity = Math.Max(initialFieldSize, _cardIds.Capacity);
        _cardIds.Capacity = Math.Max(_cardPool.CardsVariantsCount * 2, _cardIds.Capacity);
        _cardIds.Clear();

        for (int i = 0; i < _cardPool.CardsVariantsCount; i++)
        {
            _cardIds.Add(i);
        }

        for (int i = _cardPool.CardsVariantsCount - 1; i >= (fieldSize / 2); i--)
        {
            _cardIds.RemoveAt(UnityEngine.Random.Range(0, _cardIds.Count));
        }

        for (int i = 0; i < fieldSize / 2; i++)
        {
            _cardIds.Add(_cardIds[i]);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].OnCardClicked -= OnCardClicked;
            _cardPool.ReturnCard(_cards[i]);
        }

        _cards.Clear();
        _cardPool.Dispose();
        _menu.OnDestroyEvent -= Dispose;
        _menu.OnStartRound -= StartRound;
        _roundUI.OnBackToMenu -= BackToMenu;
        _roundUI.OnHint -= Hint;
    }
}
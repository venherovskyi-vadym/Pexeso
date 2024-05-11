using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

public class GameController
{
    private readonly CardPool _cardPool;
    private readonly RoundSettings _roundSettings;
    private readonly List<int> _cardIds = new List<int>();
    private readonly List<CardPresenter> _cards = new List<CardPresenter>();
    private readonly List<CardPresenter> _matchedCards = new List<CardPresenter>();
    private CardPresenter _previousCardPresenter;
    private UniTaskCompletionSource _winCompletionSource;

    public GameController(CardPool cardPool, RoundSettings roundSettings)
    {
        _cardPool = cardPool;
        _roundSettings = roundSettings;
        StartRound(50);
    }

    public async UniTask StartRound(int height, int width)
    {
        await StartRound(height, width);
    }

    public async UniTask StartRound(int fieldSize)
    {
        var trimmedfieldSize = Math.Min(fieldSize, _cardPool.CardsVariantsCount * 2);
        var oddCount = trimmedfieldSize % 2 == 1;

        if (oddCount)
        {
            trimmedfieldSize--;
        }

        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].OnCardClicked -= OnCardClicked;
            _cardPool.ReturnCard(_cards[i]);
        }

        _cards.Clear();
        _matchedCards.Clear();

        FillCardIds(fieldSize, trimmedfieldSize);

        for (int i = _cardIds.Count - 1; i >= 0; i--)
        {
            var index = UnityEngine.Random.Range(0, _cardIds.Count);
            var card = _cardPool.GetCard(_roundSettings.ShowFrontOnStartOfRound, _cardIds[index]);
            _cards.Add(card);
            card.DisableButton();
            _cardIds.RemoveAt(index);
        }

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
            _cards[i].OnCardClicked += OnCardClicked;
        }

        _winCompletionSource = new UniTaskCompletionSource();
    }

    public async UniTask<RoundResult> WaitForResult(Action<int> timerUpdate)
    {
        var remainingRoundTime = (int)_roundSettings.GetRoundDuration(_cards.Count);
        timerUpdate?.Invoke(remainingRoundTime);
        while (remainingRoundTime > 0 && _winCompletionSource.Task.Status != UniTaskStatus.Succeeded)
        {
            await UniTask.Delay(1000);
            remainingRoundTime--;
            timerUpdate?.Invoke(remainingRoundTime);
        }

        if (_winCompletionSource.Task.Status == UniTaskStatus.Succeeded)
        {
            return RoundResult.Win;
        }

        return RoundResult.Loose;
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
            _winCompletionSource.TrySetResult();
        }
    }

    private async void TurnBothCardsOnBack(CardPresenter presenterForWaiting, CardPresenter otherPresenter)
    {
        await presenterForWaiting.WaitTurnComplete();
        presenterForWaiting.TurnToBack();
        otherPresenter.TurnToBack();
        await UniTask.Delay(TimeSpan.FromSeconds(_roundSettings.CardRevealOnPairFailDuration));
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

        if (_roundSettings.MatchAction == MatchAction.Hide)
        {
            presenterForWaiting.Hide();
            otherPresenter.Hide();
        }
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
    }
}
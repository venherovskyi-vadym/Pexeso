using UnityEngine;
using System.Collections.Generic;

public class CardPool
{
    private readonly CardView.Factory _cardViewFactory;
    private readonly CardModel.Factory _cardModelFactory;
    private readonly CardCollection _cardCollection;
    private readonly CardsHolders _cardsParent;
    private readonly Stack<CardPresenter> _cardStack = new Stack<CardPresenter>();

    public int CardsVariantsCount => _cardCollection.CardsCount;

    public CardPool(CardView.Factory cardViewFactory, CardModel.Factory cardModelFactory, CardCollection cardCollection, CardsHolders cardsParent)
    {
        _cardViewFactory = cardViewFactory;
        _cardModelFactory = cardModelFactory;
        _cardCollection = cardCollection;
        _cardsParent = cardsParent;
    }

    public CardPresenter GetCard(bool isOnFront, int cardId) 
    {
        if (_cardStack.Count > 0)
        {
            var presenter = _cardStack.Pop();
            presenter.Model.IsOnFront = isOnFront;
            presenter.Model.CardId = cardId;
            ResetView(presenter.View);
            presenter.UpdateFromModel();
            return presenter;
        }

        return Create(isOnFront, cardId);
    }

    public void ReturnCard(CardPresenter cardPresenter)
    {
        cardPresenter.Model.IsMatched = false;

        if (cardPresenter.View == null)
        {
            return;
        }

        cardPresenter.View.gameObject.SetActive(false);
        cardPresenter.View.transform.SetParent(_cardsParent.Pool);
        _cardStack.Push(cardPresenter);
    }

    public void Dispose()
    {
        while (_cardStack.Count > 0)
        {
            var cardPresenter = _cardStack.Pop();
            cardPresenter.Dispose();
        }
    }

    private CardPresenter Create(bool isOnFront, int cardId)
    {
        var cardModel = _cardModelFactory.Create();
        cardModel.IsOnFront = isOnFront;
        cardModel.CardId = cardId;
        var view = _cardViewFactory.Create();
        var cardPresenter = new CardPresenter(cardModel, view, _cardCollection);
        ResetView(view);
        return cardPresenter;
    }

    private void ResetView(CardView cardView)
    {
        cardView.gameObject.SetActive(true);
        cardView.transform.SetParent(_cardsParent.Grid);
        cardView.transform.localScale = Vector3.one;
        cardView.transform.localPosition = Vector3.zero;
    }
}

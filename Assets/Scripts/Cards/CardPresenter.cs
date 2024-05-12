using Cysharp.Threading.Tasks;
using System;

public class CardPresenter
{
    private CardModel _model;
    private CardView _view;
    private CardCollection _cardCollection;
    private bool _blockCardTurning;

    public CardModel Model => _model;
    public CardView View => _view;
    public event Action<CardPresenter, bool, bool> OnCardClicked;

    public CardPresenter(CardModel model, CardView view, CardCollection cardCollection)
    {
        _model = model;
        _view = view;
        _cardCollection = cardCollection;
        _view.OnClick += CardClicked;
        UpdateFromModel(); 
    }

    public void UpdateFromModel()
    {
        _view.InitCardBack(_cardCollection.GetCardBack());
        _view.InitCardFront(_cardCollection.GetCardSprite(_model.CardId), _model.CardId);

        if (_model.IsOnFront)
        {
            _view.TurnToFront();
        }
        else
        {
            _view.TurnToBack();
        }
    }

    public void TurnToBack()
    {
        _view.TurnToBack();
        _model.IsOnFront = false;
    }

    public void TurnToFront()
    {
        _view.TurnToFront();
        _model.IsOnFront = true;
    }

    public void DisableButton()
    {
        _blockCardTurning = true;
    }

    public void EnableButton()
    {
        _blockCardTurning = false;
    }

    public void SetInteractableButton(bool interactable)
    {
        View.SetInteractableButton(interactable);
    }

    public UniTask WaitTurnComplete()
    {
        return _view.WaitTurnComplete();
    }

    public void Dispose()
    {
        _view.OnClick -= CardClicked;
        _model = null;
        _view = null;
    }

    public void Hide()
    {
        _view.Hide();
    }

    private void CardClicked(CardView view)
    {
        if (view != _view || _blockCardTurning)
        {
            return;
        }

        if (_model.IsOnFront)
        {
            _view.TurnToBack();
        }
        else
        {
            _view.TurnToFront();
        }

        OnCardClicked?.Invoke(this, _model.IsOnFront, !_model.IsOnFront);
        _model.IsOnFront = !_model.IsOnFront;
    }
}
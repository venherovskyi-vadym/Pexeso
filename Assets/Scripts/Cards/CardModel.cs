using Zenject;

public class CardModel
{
    public int CardId;
    public bool IsMatched;
    public bool IsOnFront;
    public class Factory : PlaceholderFactory<CardModel>
    {
    }
}
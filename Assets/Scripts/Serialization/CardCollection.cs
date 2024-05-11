using UnityEngine;

[CreateAssetMenu(fileName = "CardCollection", menuName = "CardCollection")]
public class CardCollection : ScriptableObject
{
    [SerializeField] private Sprite[] _cardSprites;
    [SerializeField] private Sprite _cardBack;

    public int CardsCount => _cardSprites.Length;

    public Sprite GetCardSprite(int cardIndex)
    {
        if (_cardSprites == null || cardIndex >= _cardSprites.Length)
        {
            return null;
        }

        return _cardSprites[cardIndex];
    }

    public Sprite GetCardBack() => _cardBack;
}
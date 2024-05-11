using System;
using UnityEngine;
using UnityEngine.UI;

public class CardGrid : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField, Range(0, 1)] private float _widthHeightRatio = 0.5f;

    private Vector2 _oldSize;
    private int _gridChildCount;

    public void Awake()
    {
        if (_rectTransform == null)
        {
            return;
        }

        _oldSize = _rectTransform.rect.size;
        _gridChildCount = _rectTransform.childCount;
        ResizeGrid(_rectTransform.rect.size.y, _rectTransform.rect.size.x, _rectTransform.childCount);
    }

    private void Update()
    {
        if (_rectTransform == null || _gridLayout == null || _rectTransform.childCount == 0)
        {
            return;
        }

        if (_oldSize != _rectTransform.rect.size || _gridChildCount != _rectTransform.childCount)
        {
            ResizeGrid(_rectTransform.rect.size.y, _rectTransform.rect.size.x, _rectTransform.childCount);
        }

        _oldSize = _rectTransform.rect.size;
        _gridChildCount = _rectTransform.childCount;
    }

    private void OnValidate()
    {
        if (_rectTransform == null || _gridLayout == null || _rectTransform.childCount == 0)
        {
            return;
        }

        ResizeGrid(_rectTransform.rect.size.y, _rectTransform.rect.size.x, _rectTransform.childCount);
    }

    private void ResizeGrid(float height, float width, int childCount)
    {
        var gridArea = width * height;
        var gridItemSquareSide = Math.Sqrt(gridArea / childCount);
        var ratioSqrt = Math.Sqrt(_widthHeightRatio);
        var gridItemHeight = gridItemSquareSide / ratioSqrt;
        var gridItemWidth = gridItemSquareSide * ratioSqrt;
        var rowsCount = (float)(height / gridItemHeight);
        var columnCount = (float)(width / gridItemWidth);
        var rowsCountInt = Mathf.FloorToInt(rowsCount);
        var columnCountInt = Mathf.FloorToInt(columnCount);
        var insufficientCount = childCount - rowsCountInt * columnCountInt;

        if (insufficientCount > 0)
        {
            if (insufficientCount <= rowsCountInt)
            {
                gridItemWidth = width / Mathf.CeilToInt(columnCount);
                gridItemHeight = gridItemWidth / _widthHeightRatio;
            }
            else if(insufficientCount <= columnCountInt)
            {
                gridItemHeight = height / Mathf.CeilToInt(rowsCount);
                gridItemWidth = gridItemHeight * _widthHeightRatio;
            }
            else 
            {
                gridItemWidth = width / Mathf.CeilToInt(columnCount);
                gridItemHeight = height / Mathf.CeilToInt(rowsCount);
            }
        }

        _gridLayout.cellSize = new Vector2((float)gridItemWidth, (float)gridItemHeight);
    }
}
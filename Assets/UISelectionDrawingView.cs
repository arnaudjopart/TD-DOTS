using System;
using UnityEngine;

internal class UISelectionDrawingView : MonoBehaviour
{
    [SerializeField] private RectTransform _selectionImage;


    public void Draw(Vector2 startMousePosition, Vector2 currentMousePosition)
    {
        _selectionImage.gameObject.SetActive(true);

        var bottomLeftCorner = new Vector2(Mathf.Min(startMousePosition.x, currentMousePosition.x), Mathf.Min(startMousePosition.y, currentMousePosition.y));
        var width = Mathf.Abs(startMousePosition.x- currentMousePosition.x);
        var height = Mathf.Abs(startMousePosition.y- currentMousePosition.y);

        _selectionImage.anchoredPosition = bottomLeftCorner;
        _selectionImage.sizeDelta = new Vector2(width, height);

    }

    internal void EndDraw()
    {
        _selectionImage.gameObject.SetActive(false);
    }
}
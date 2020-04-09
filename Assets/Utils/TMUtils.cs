using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IMonoBehaviour
{
    Transform transform { get; }
    GameObject gameObject { get; }
    T GetComponent<T>();
}

public interface ITMLayoutable : IMonoBehaviour
{
    Vector2 GetSize();
}


public enum TMEffectAreaType
{
    SingleSquare,
    Row,
    Column,
    
    // Neighbors including target square
    FiveSquares,
    NineSquares,
    
    // Neighbors not including target square
    FourSquares,
    EightSquares
}

public class TMUtils
{
    // Returns size of the layouted area
    public static Vector2 HorizontalLayoutLocal<T> 
        (ICollection<T> toLayout, Vector3 center, float padding) where T : ITMLayoutable
    {
        int numItems = toLayout.Count;
        float maxHeight = 0.0f;
        float totalWidth = 0.0f;
        foreach (T item in toLayout)
        {
            Vector2 size = item.GetSize();
            totalWidth += size.x;
            if (size.y > maxHeight)
            {
                maxHeight = size.y;
            }
        }
        totalWidth += (numItems - 1) * padding;

        Vector3 cursor = center;
        cursor.x = center.x - totalWidth * 0.5f - padding;

        foreach (T currentItem in toLayout)
        {
            Vector2 currentSize = currentItem.GetSize();
            cursor.x += padding;
            cursor.x += currentSize.x * 0.5f;
            currentItem.transform.localPosition = cursor;
            cursor.x += currentSize.x * 0.5f;
        }

        return new Vector2(totalWidth, maxHeight);
    }
    
    public static Vector2 VerticalLayoutLocal<T> 
        (ICollection<T> toLayout, Vector3 center, float padding) where T : ITMLayoutable
    {
        int numItems = toLayout.Count;
        float maxWidth = 0.0f;
        float totalHeight = 0.0f;
        foreach (T item in toLayout)
        {
            Vector2 size = item.GetSize();
            totalHeight += size.y;
            if (size.x > maxWidth)
            {
                maxWidth = size.x;
            }
        }
        totalHeight += (numItems - 1) * padding;

        Vector3 cursor = center;
        cursor.y = center.y + totalHeight * 0.5f + padding;

        foreach (T currentItem in toLayout)
        {
            Vector2 currentSize = currentItem.GetSize();
            cursor.y -= padding;
            cursor.y -= currentSize.y * 0.5f;
            currentItem.transform.localPosition = cursor;
            cursor.y -= currentSize.y * 0.5f;
        }

        return new Vector2(maxWidth, totalHeight);
    }

    public static Rect PlaceItemAtTop(ITMLayoutable layoutable, Rect container, float topPadding, float sidePadding)
    {
        Vector2 size = layoutable.GetSize();

        float scale = 1.0f;
        if (size.x > container.width - 2.0f * sidePadding)
        {
            scale = (container.width - 2.0f * sidePadding) / size.x;
        }

        size *= scale;
        layoutable.transform.localScale = new Vector3(scale, scale, 1.0f);

        float x = (container.xMin + container.xMax) * 0.5f;
        float y = container.yMax - topPadding - size.y * 0.5f;
        
        layoutable.transform.position = new Vector3(x, y, layoutable.transform.position.z);

        Rect ret = Rect.MinMaxRect(container.xMin, container.yMin, container.xMax, container.yMax - size.y - topPadding);

        return ret;
    }

}

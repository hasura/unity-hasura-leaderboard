using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardTitleGroup : MonoBehaviour, ITMLayoutable
{
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private TextMeshPro titleLabel;
    [SerializeField] private TextMeshPro extraDataLabel;
    
    public Vector2 GetSize()
    {
        return boxCollider.size;
    }

    public void SetSubtitle(string subtitle)
    {
        extraDataLabel.text = subtitle;
    }
}

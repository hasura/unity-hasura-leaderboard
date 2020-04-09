using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class LeaderBoardEntry : MonoBehaviour, ITMLayoutable
{
    [SerializeField] private BoxCollider2D boxCollider;

    [SerializeField] private TextMeshPro rankLabel, userNameLabel, scoreLabel;
    
    public Vector2 GetSize()
    {
        return boxCollider.size;
    }

    public void Load(ScoreEntry scoreEntry)
    {
        rankLabel.text = scoreEntry.rank + ".";
        userNameLabel.text = scoreEntry.userName;
        scoreLabel.text = scoreEntry.score.ToString();

        if (scoreEntry.isLocalPlayer)
        {
            Color highlightColor = Color.yellow;
            rankLabel.color = highlightColor;
            userNameLabel.color = highlightColor;
            scoreLabel.color = highlightColor;
        }
    }
}

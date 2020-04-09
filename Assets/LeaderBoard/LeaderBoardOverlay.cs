using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreEntry
{
    public string userName;
    public int score;
    public int rank;
    public bool isLocalPlayer;
}

public class LeaderBoardOverlay : MonoBehaviour
{
    [SerializeField] private LeaderBoardEntry leaderBoardEntryPrefab;
    [SerializeField] private LeaderBoardTitleGroup titleGroup;
    
    private GameObject entriesParent;
    
    private List<LeaderBoardEntry> entries = new List<LeaderBoardEntry>();

    public void DisplayScores(List<ScoreEntry> scoreEntries)
    {
        Device.InitDeviceData();
        
        Rect screenRect = Device.orthoScreenRect;

        Rect remainingRect = TMUtils.PlaceItemAtTop(titleGroup, screenRect, 0.6f, 0.1f);
        
        if (entriesParent != null)
        {
            Destroy(entriesParent);
            entries.Clear();
        }
        entriesParent = new GameObject();
        entriesParent.transform.SetParent(transform);
        entriesParent.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);
        entriesParent.name = "Entries";
        
        foreach (ScoreEntry scoreEntry in scoreEntries)
        {
            LeaderBoardEntry newEntry = Instantiate(leaderBoardEntryPrefab.gameObject).GetComponent<LeaderBoardEntry>();
            newEntry.transform.SetParent(entriesParent.transform);
            newEntry.Load(scoreEntry);
            entries.Add(newEntry);
        }
        
        TMUtils.VerticalLayoutLocal(entries, Vector3.zero, 0.025f);

        Vector3 entriesPos = entriesParent.transform.position;
        entriesPos.y = (remainingRect.yMin + remainingRect.yMax) * 0.5f;

        entriesParent.transform.position = entriesPos;
    }
    
    
}

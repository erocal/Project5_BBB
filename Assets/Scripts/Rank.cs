using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rank : MonoBehaviour
{

    public class RankData
    {
        public string name;
        public int score;

        public RankData(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }

    private List<RankData> rankList = new List<RankData>();

    // update
    public void UpdateRank(string name, int score)
    {
        RankData newData = new RankData(name, score);

        rankList.Add(newData);

        rankList.Sort((a, b) => b.score.CompareTo(a.score));


        if (rankList.Count > 10)
        {
            rankList.RemoveAt(rankList.Count - 1);
        }
    }

    // output
    public void PrintRank()
    {
        Debug.Log("===== ±Æ¦æº] Top 10 =====");

        for (int i = 0; i < rankList.Count; i++)
        {
            Debug.Log((i + 1) + ". " + rankList[i].name + " : " + rankList[i].score);
        }
    }
}
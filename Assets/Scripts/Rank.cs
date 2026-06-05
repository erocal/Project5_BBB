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

    private readonly List<RankData> rankList = new List<RankData>();

    private const string DefaultArcadeName = "AAA";

    public void UpdateRank(string name, int score)
    {
        name = GetValidPlayerName(name);

        RankData newData = new RankData(name, score);

        rankList.Add(newData);

        rankList.Sort((a, b) => b.score.CompareTo(a.score));

        if (rankList.Count > 10)
        {
            rankList.RemoveAt(rankList.Count - 1);
        }
    }

    private string GetValidPlayerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DefaultArcadeName;
        }

        return name.Trim().ToUpper();
    }

    public void PrintRank()
    {
        Debug.Log("===== ±Æ¦æº] Top 10 =====");

        for (int i = 0; i < rankList.Count; i++)
        {
            Debug.Log($"{i + 1}. {rankList[i].name} : {rankList[i].score}");
        }
    }
}
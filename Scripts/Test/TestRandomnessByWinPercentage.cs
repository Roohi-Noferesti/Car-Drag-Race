using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestRandomnessByWinPercentage : MonoBehaviour 
{
    public AnimationCurve ProbabilityCurve;

    public int ConsecutiveWins;
    public int ConsecutiveLoses;

    public float LoseWinBoost = 0.05F;

    public int PPDifThreshold;

    public int TestRepeat;

    void Awake()
    {
        string numbers = "";
        List<int> values = new List<int>();
        for (int i = 0; i < TestRepeat; i++)
        {
            float randomValue = ProbabilityCurve.Evaluate(Random.value + ConsecutiveWins / 10F - ConsecutiveLoses/10F);
            var round = Mathf.RoundToInt(randomValue);
            values.Add(round);
            numbers += round + ",";
        }

        //int countEasy = 0;
        //int countHard = 0;
        //int countZero = 0;
        Dictionary<int,int> numdDict = new Dictionary<int, int>();
        foreach (var value in values)
        {
            if (!numdDict.ContainsKey(value))
            {
                numdDict.Add(value,0);
            }
            numdDict[value]++;
        }

        Debug.Log(numbers);

        string percentages = "";
        foreach (var i in numdDict.OrderBy(v=>v.Key))
        {
            percentages += string.Format("Percentage of {0} is : {1} ",i.Key,(i.Value*100/values.Count) + " %")+"\n";
        }
        Debug.Log(percentages);
        //Debug.Log(string.Format("EasyPercentage : {0}  , HardPercentage : {1},ZeroPercentage : {2}", (countEasy * 100 / values.Count) + " %", (countHard * 100 / values.Count) + " %",
        //    (countZero * 100 / values.Count) + " %"));
    }
}

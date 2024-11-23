using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelIncreament : MonoBehaviour
{
    [SerializeField]
    private int totalLevel = 100;
    [SerializeField] private int levelUpBaseXP = 10;
    private List<int> _xpTotalAtEndOfLevel;
    [SerializeField] private int levelUpIncrementPercent = 10;
    private List<int> _xpPerLevel;

    // Use this for initialization
	IEnumerator Start ()
	{
	    _xpTotalAtEndOfLevel = new List<int>();
        _xpTotalAtEndOfLevel.Add(0);
	    _xpPerLevel = new List<int>();
        int num = this._xpTotalAtEndOfLevel[0];
        for (int i = 1; i <= totalLevel; i++)
        {
            int num2 = levelUpBaseXP;
            int num3 = levelUpBaseXP*(i - 1)*levelUpIncrementPercent/100;
            num2 += num3;
            this._xpPerLevel.Add(num2);
            num += num2;
            this._xpTotalAtEndOfLevel.Add(num);

            Debug.Log(string.Format("Level {0} : Percent={1},Xp={2},XpAtEnd={3}",i,num3,num2,num));
            yield return 0;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

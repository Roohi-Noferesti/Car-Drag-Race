using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LeaguesRewardEditor : EditorWindow
{
    public Object source;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/Update Leagues Reward ")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LeaguesRewardEditor window = (LeaguesRewardEditor)EditorWindow.GetWindow(typeof(LeaguesRewardEditor));
        window.Show();
    }

    void OnGUI()
    {
        source = EditorGUILayout.ObjectField(source, typeof(CurrenciesConfiguration), true);
        if (GUILayout.Button("update leagues rewards"))
        {
           CurrenciesConfiguration config = source as CurrenciesConfiguration;
            //UnityEngine.Debug.Log(config.LeagueRewardData.Count);

            //foreach (var item in config.LeagueRewardData)
            //{
            //    item.WeeklyRewards = config.WeeklyRewardData.WeeklyRewards;
                
            //}

            //for (int i = 0; i < config.LeagueRewardData.Count; i++)
            //{
            //    config.LeagueRewardData[i].LeagueName = LeaguesHelper.Leagues[i].ID;
            //}

            //AssetDatabase.Refresh();
            //EditorUtility.SetDirty(config);
            //AssetDatabase.SaveAssets();

        }

    }

}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NarrativeSceneConfiguration
{
	public List<string> NarrativeScenes;

#if UNITY_EDITOR
    //Just For serialization in editor
    public List<NarrativeScene> NarrativeScenesObjects;
#endif

    private Dictionary<string, string> NarrativeScenesMap = new Dictionary<string, string>();

	public void RegisterScene(TextAsset sceneData)
	{
		string name = sceneData.name;
		if (this.NarrativeScenesMap.ContainsKey(name))
		{
			return;
		}
		this.NarrativeScenesMap.Add(name, sceneData.text);
	}

	public bool GetScene(string sceneID, out NarrativeScene scene)
	{
		string text;
		if (!this.NarrativeScenesMap.TryGetValue(sceneID, out text))
		{
		    if (!Application.isPlaying)
		    {
			    GTDebug.Log(GTLogChannel.Narrative,"SceneID : " + sceneID + " not found.Ignore it");
		        scene = null;
		        return false;
		    }
            string path = TierXManager.Instance.NarrativeScenesResourcesPath + sceneID;
			TextAsset textAsset = (TextAsset)Resources.Load(path);
			if ((textAsset == null))
			{
				scene = null;
				return false;
			}
			text = textAsset.text;
		}
		scene = JsonConverter.DeserializeObject<NarrativeScene>(text);
		scene.Initialise();
		return true;
	}

#if UNITY_EDITOR
    //Just in editor for serialization
    public void DoSerializatuionStaff()
    {
        NarrativeScenesObjects = new List<NarrativeScene>();
        foreach (var narrativeSceneID in NarrativeScenes)
        {
            NarrativeScene narrativeScene;
            if (GetScene(narrativeSceneID, out narrativeScene))
            {
                NarrativeScenesObjects.Add(narrativeScene);
            }
        }
    }
#endif
}

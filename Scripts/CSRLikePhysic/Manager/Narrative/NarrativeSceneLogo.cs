using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NarrativeSceneLogo
{
    [Serializable]
	public class NarrativeSceneLogoDetails
	{
		public string[] StringValues;
	}

	private delegate INarrativeSceneLogo CreateLogoPrefab(GameObject parent, NarrativeSceneLogo.NarrativeSceneLogoDetails details);

	public string Type = "StandardTextureAndText";

	public NarrativeSceneLogo.NarrativeSceneLogoDetails Details = new NarrativeSceneLogo.NarrativeSceneLogoDetails
	{
		StringValues = new string[]
		{
			string.Empty
		}
	};

	private static readonly Dictionary<string, NarrativeSceneLogo.CreateLogoPrefab> LogoCreators = new Dictionary<string, NarrativeSceneLogo.CreateLogoPrefab>
	{
		{
			"StandardTextureAndText",
			new NarrativeSceneLogo.CreateLogoPrefab(NarrativeSceneLogo.CreateStandardTextureAndTextLogo)
		},
		{
			"WorldTourTextureAndText",
			new NarrativeSceneLogo.CreateLogoPrefab(NarrativeSceneLogo.CreateWTTextureAndTextLogo)
		},
		{
			"PrefabLogo",
			new NarrativeSceneLogo.CreateLogoPrefab(NarrativeSceneLogo.CreatePrefabLogo)
		}
	};

	public INarrativeSceneLogo CreateLogo(GameObject parent)
	{
		if (!NarrativeSceneLogo.LogoCreators.ContainsKey(this.Type))
		{
			return null;
		}
		return NarrativeSceneLogo.LogoCreators[this.Type](parent, this.Details);
	}

	private static INarrativeSceneLogo CreateStandardTextureAndTextLogo(GameObject parent, NarrativeSceneLogo.NarrativeSceneLogoDetails details)
	{
		if (details.StringValues.Length == 0)
		{
			return null;
		}
		string path = details.StringValues[0];
		Texture2D texture2D = Resources.Load(path) as Texture2D;
		if (texture2D == null)
		{
			return null;
		}
		return NarrativeSceneLogo.InstantiateTextureAndTextLogoWithTexture(parent, texture2D);
	}

	private static INarrativeSceneLogo CreateWTTextureAndTextLogo(GameObject parent, NarrativeSceneLogo.NarrativeSceneLogoDetails details)
	{
		if (details.StringValues.Length == 0)
		{
			return null;
		}
		string key = details.StringValues[0];
		if (!TierXManager.Instance.CrewLogos.ContainsKey(key))
		{
			return null;
		}
		Texture2D texture = TierXManager.Instance.CrewLogos[key];
		return NarrativeSceneLogo.InstantiateTextureAndTextLogoWithTexture(parent, texture);
	}

	private static INarrativeSceneLogo InstantiateTextureAndTextLogoWithTexture(GameObject parent, Texture2D texture)
	{
        GameObject prefab = Resources.Load("CharacterCards/Crew/NarrativeSceneLogos/StandardLogo") as GameObject;
        NarrativeSceneStandardLogo narrativeSceneStandardLogo = GameObjectHelper.InstantiatePrefab<NarrativeSceneStandardLogo>(prefab, parent);
	    narrativeSceneStandardLogo.Logo.texture = texture;
	    //float num = 200f;
        //narrativeSceneStandardLogo.Logo.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        //narrativeSceneStandardLogo.Logo.Setup((float)texture.width / num / 2f, (float)texture.height / num / 2f, new Vector2(0f, (float)(texture.height - 1)), new Vector2((float)texture.width, (float)texture.height));
        return narrativeSceneStandardLogo;
    }

	private static INarrativeSceneLogo CreatePrefabLogo(GameObject parent, NarrativeSceneLogo.NarrativeSceneLogoDetails details)
	{
		if (details.StringValues.Length == 0)
		{
			return null;
		}
		GameObject gameObject = Resources.Load(details.StringValues[0]) as GameObject;
		if (gameObject == null)
		{
			return null;
		}
        //return GameObjectHelper.InstantiatePrefab<NarrativeScenePrefabLogo>(gameObject, parent);
	    return null;
	}
}

using System;
using System.Collections.Generic;

[Serializable]
public class NarrativeSceneCharactersGroup
{
	public NarrativeSceneLogo Logo = new NarrativeSceneLogo();

	public int GroupOrCrewIndex = -1;

	public int CrewDefeatedCount;

	public bool UseDefaultCrews = true;

	public bool SetStrikeFramesActive;

	public bool HideMembers;

	public NarrativeSceneCharacter Leader = new NarrativeSceneCharacter
	{
		Name = "Dummy Member",
		CardTextureName = "card_lastella"
	};

	public List<NarrativeSceneCharacter> Members = new List<NarrativeSceneCharacter>
	{
		new NarrativeSceneCharacter(),
		new NarrativeSceneCharacter(),
		new NarrativeSceneCharacter(),
		new NarrativeSceneCharacter()
	};

	public bool UseWorldTourLogo
	{
		get
		{
			return false;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public string LogoTextureName
	{
		get
		{
            if (Logo != null && Logo.Details != null && Logo.Details.StringValues.Length > 0)
            {
                return Logo.Details.StringValues[0];
            }
			return string.Empty;
		}
		set
		{
			throw new NotImplementedException();
		}
	}
}

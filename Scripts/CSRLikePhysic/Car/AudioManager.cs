using Fabric;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	public Fabric.Component Music;

	public Fabric.Component SFX;

	public Transform EnginesRootTransform_Human;

	public Transform EnginesRootTransform_AI;

	public RaceCarAudio DefaultEngine;

	public float HumanEngineVolume = 1f;

	public bool HumanEngineMute;

	public eAudioEngineType HumanEngineOverride;

	public float MaxNitrousVolume = 0.15f;

	public float MaxTransmissionVolume = 0.15f;

	public float MaxTurboVolume = 0.15f;

	public float AIEngineVolume = 0.8f;

	public bool AIEngineMute;

	public eAudioEngineType AIEngineOverride;

	public bool EnableGears = true;

	public AnimationCurve TensionLoopDistanceCurve;

	public AnimationCurve TensionLoopVolumeCurve;

	public float RaceStartMusicFadeDuration;

	public float RaceQuitMusicFadeDuration;

	public float RaceResultsMusicFadeLevel;

	public float RaceResultsMusicFadeDuration;

	public float RaceResultsMusicFadeDelay;

	public float LevelUpMusicFadeLevel;

	public float LevelUpMusicFadeDuration;

	public float LevelUpMusicFadeDelay;

	public float TierUnlockedMusicFadeLevel;

	public float TierUnlockedMusicFadeDuration;

	public float TierUnlockedMusicFadeDelay;

	public float LogoAnimationSoundDelay;

	private static FabricManager _fabricManager = null;

	private static EventManager _fabricEventManager = null;

	private bool musicResultsActive;

	private AudioComponent musicResultsLoseAudioComponent;

	private AudioComponent musicResultsWinAudioComponent;

	private Fabric.Component _carsComponent_Human;

	private Fabric.Component _carsComponent_AI;

	private Fabric.Component _carsEnginesComponent_Human;

	private Fabric.Component _carsEnginesComponent_AI;

	private AudioFader _musicFader;

	private bool _preventMusicStarting = true;

	private bool _isPlayingDeviceMusic;

	private bool AppHasAudioFocus;

	public static string EventNameSuffixHuman = "_Hum";

	public static string EventNameSuffixAI = "_AI";

	public static string[] EventEngineNames = new string[]
	{
		string.Empty,
		"Engines/4_Cylinder",
		"Engines/6_Cylinder",
		"Engines/8_Cylinder_A",
		"Engines/8_Cylinder_B",
		"Engines/V10_A",
		"Engines/V12_A",
		"Engines/W16"
	};

	public static string GetEventNameWithHumanAISuffix(string eventName, bool isHuman)
	{
		return eventName + ((!isHuman) ? AudioManager.EventNameSuffixAI : AudioManager.EventNameSuffixHuman);
	}

	public static string GetEventNameEngine(eAudioEngineType audioEngine, int qualityLevel, bool isHuman)
	{
		return AudioManager.GetEventNameWithHumanAISuffix(AudioManager.EventEngineNames[(int)audioEngine], isHuman);
	}

	public static string GetEventNameUpgrade(eUpgradeType upgradeType, CarUpgradeSetup upgradeSetup, bool isHuman)
	{
		string text = string.Empty;
		int levelFitted = upgradeSetup.UpgradeStatus[upgradeType].levelFitted;
		switch (upgradeType)
		{
		case eUpgradeType.TURBO:
			if (levelFitted == 0)
			{
				return text;
			}
			text = AudioEvent.CarEffects_TurboLoop_Base;
			break;
		case eUpgradeType.NITROUS:
			text = AudioEvent.CarEffects_NitrousLoop_Base;
			break;
		case eUpgradeType.TRANSMISSION:
			text = AudioEvent.CarEffects_Transmission_Base;
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			switch (levelFitted)
			{
			case 0:
			case 1:
			case 2:
				text += "Low";
				break;
			case 3:
			case 4:
				text += "Medium";
				break;
			case 5:
				text += "High";
				break;
			case 6:
				text += "Ultra";
				break;
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			text = AudioManager.GetEventNameWithHumanAISuffix(text, isHuman);
		}
		return text;
	}

	private void Start()
	{
		this.Music.Mute = true;
	}

	public override void Awake()
	{
		base.Awake();
	    this.AppHasAudioFocus = true;//BasePlatform.ActivePlatform.AppHasAudioFocus();
		this.Music = base.transform.Find("Music").GetComponent<GroupComponent>();
		this.SFX = base.transform.Find("SFXAudio").GetComponent<GroupComponent>();
		this.musicResultsLoseAudioComponent = this.Music.transform.FindChildRecursively("Mus_Results_Lose").GetComponent<AudioComponent>();
		this.musicResultsWinAudioComponent = this.Music.transform.FindChildRecursively("Mus_Results_Win").GetComponent<AudioComponent>();
		AudioManager._fabricManager = FabricManager.Instance;
		if (AudioManager._fabricManager == null)
		{
		}
		AudioManager._fabricEventManager = EventManager.Instance;
		this._carsComponent_Human = this.GetComponentByName("Audio_SFXAudio_Cars_Human");
		this._carsComponent_AI = this.GetComponentByName("Audio_SFXAudio_Cars_AI");
		this._carsEnginesComponent_Human = this.GetComponentByName("Audio_SFXAudio_Cars_Human_Engines");
		this._carsEnginesComponent_AI = this.GetComponentByName("Audio_SFXAudio_Cars_AI_Engines");
		PlayerProfileManager instance = PlayerProfileManager.Instance;
		if (instance != null && instance.ActiveProfile != null)
		{
			this.OnProfileChanged();
		}
		Transform transform = base.transform.Find("AudioSourcePool");
		if (transform != null)
		{
			int num = 0;
			foreach (Transform transform2 in transform)
			{
				transform2.name = "AudioSource" + num++;
			}
		}
        //DeviceMusicHandler.RegisterUnityIPodCallbackListener(base.gameObject, "OnDeviceMusicEvent");
	    this._isPlayingDeviceMusic = false;//DeviceMusicHandler.IsDeviceMusicPlaying;
	}

	private void OnDeviceMusicEvent(string message)
	{
	    this._isPlayingDeviceMusic = false;//DeviceMusicHandler.IsDeviceMusicPlaying;
		this.SetMusicMuted(this._isPlayingDeviceMusic || PlayerProfileManager.Instance.ActiveProfile.OptionMusicMute);
		if (this._preventMusicStarting)
		{
			this.AllowMusicToPlay();
		}
	}

	public void OnProfileChanged()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile != null)
		{
			bool optionMusicMute = activeProfile.OptionMusicMute;
			this.SetMusicMuted(optionMusicMute);
			bool optionSoundMute = activeProfile.OptionSoundMute;
		    this.SFX.Mute = optionSoundMute;
		}
	}

	private void ApplicationDidBecomeActive()
	{
	}

	public void SetVolume(float volume)
	{
		this.SFX.SetVolume(volume);
	}

	private void Update()
	{
		if (this._musicFader != null && this._musicFader.Update(ref this.Music))
		{
			this._musicFader = null;
			if (this.Music.Volume == 0f)
			{
				this.StopSound(AudioEvent.MainMusic, null);
				this.StopSound(AudioEvent.CrewMusic, null);
			}
		}
		if (this.musicResultsActive && !this.musicResultsLoseAudioComponent.IsPlaying() && !this.musicResultsWinAudioComponent.IsPlaying())
		{
			this.musicResultsActive = false;
			if (true)//TutorialQuery.IsObjectiveComplete("IntroduceShaxGarage"))
			{
				this.StartMainMusic();
			}
		}
	}

	public void FadeMusicToCrew()
	{
		this.PlaySound(AudioEvent.CrewMusic, null);
		this.StopSound(AudioEvent.MainMusic, null);
	}

    public void SetMusicMuted(bool mute)
	{
		if (this.Music.Mute != mute)
		{
			this.Music.Mute = mute;
			if (mute)
			{
				this.StopSound(AudioEvent.Music_MainMusic_CSR2, null);
			}
			else if (!this._preventMusicStarting)
			{
				this.StartMainMusic();
			}
		}
	}

	public void AllowMusicToPlay()
	{
		if (this._preventMusicStarting && !this._isPlayingDeviceMusic)
		{
			if (!this.Music.Mute)
			{
				this.StartMainMusic();
			}
			this._preventMusicStarting = false;
		}
	}

	public void SetAppAudioFocus(bool app_has_audio_focus)
	{
		this.AppHasAudioFocus = app_has_audio_focus;
	}

	public void AudioFocusChange(bool app_has_audio_focus)
	{
		this.AppHasAudioFocus = app_has_audio_focus;
		if (app_has_audio_focus)
		{
			if (!RaceController.RaceIsRunning())// && ScreenManager.Active.CurrentScreen != ScreenID.MiniGame)
			{
				this.StartMainMusic();
			}
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			if (activeProfile != null)
			{
				this.SFX.Mute = activeProfile.OptionSoundMute;
			}
			else
			{
				this.SFX.Mute = false;
			}
		}
		else
		{
			this.TurnOffMusic();
			this.SFX.Mute = true;
		}
	}

	public void TurnOffMusic()
	{
		this.StopSound(AudioEvent.Music_MainMusic_CSR2, null);
	}

	public void TurnOnMusic()
	{
		AudioManager._fabricManager.enabled = true;
	}

	public void FadeMusicForSFX(float target, float time, float delay = 0f)
	{
		if (!this.SFX.Mute)
		{
			this.FadeMusic(target, time, delay);
		}
	}

	public void FadeMusic(float target, float time, float delay = 0f)
	{
		if (this.Music.Volume != target || this._musicFader != null)
		{
			if (this._musicFader != null)
			{
				this._musicFader = null;
			}
			this._musicFader = new AudioFader(target, time, delay);
		}
	}

	public void FadeOutAndPauseSounds(float duration, ref List<AudioComponent> fadeOutAudioComponents)
	{
		AudioComponent[] componentsInChildren = base.GetComponentsInChildren<AudioComponent>();
		AudioComponent[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			AudioComponent audioComponent = array[i];
			if (audioComponent.IsPlaying() && audioComponent.CurrentState != AudioComponentState.Paused)
			{
				AudioComponentFader audioComponentFader = audioComponent.gameObject.AddMissingComponent<AudioComponentFader>();
				audioComponentFader.Fade(0f, duration, 0f, null, new AudioComponentFader.OnFadeState(AudioComponentFader.OnFadeStatePause));
				if (fadeOutAudioComponents != null)
				{
					fadeOutAudioComponents.Add(audioComponent);
				}
			}
		}
	}

	public void FadeInAndUnpauseSounds(List<AudioComponent> audioComponents, float duration)
	{
		if (audioComponents != null)
		{
			foreach (AudioComponent current in audioComponents)
			{
				AudioComponentFader component = current.gameObject.GetComponent<AudioComponentFader>();
				if (component != null)
				{
					component.Fade(component.startVolume, duration, 0f, new AudioComponentFader.OnFadeState(AudioComponentFader.OnFadeStateUnpause), null);
				}
			}
		}
	}

	private bool PostEvent(string eventName, EventAction eventAction, object parameter, GameObject parentGameObject, InitialiseParameters initialiseParamaters = null)
	{
		
		bool result;
		if (initialiseParamaters != null)
		{
			result = AudioManager._fabricEventManager.PostEvent(eventName, eventAction, parameter, parentGameObject, initialiseParamaters);
		}
		else
		{
			result = AudioManager._fabricEventManager.PostEvent(eventName, eventAction, parameter, parentGameObject);
		}
		return result;
	}

	public void StartMainMusic()
	{
	    this.AppHasAudioFocus = true;//BasePlatform.ActivePlatform.AppHasAudioFocus();
		if (this.AppHasAudioFocus && !this.Music.Mute)
		{
			this.PlaySound(AudioEvent.Music_MainMusic_CSR2, null);
		}
	}

	public void StartWinLoseMusic(bool won)
	{
		if (this.AppHasAudioFocus)
		{
			if (!this.Music.Mute)
			{
				this.PlaySound((!won) ? AudioEvent.Music_LoseMusic_CSR2 : AudioEvent.Music_WinMusic_CSR2, null);
				this.musicResultsActive = true;
			}
			this.PlaySound((!won) ? AudioEvent.HUD_LoseRace : AudioEvent.HUD_WinRace, null);
		}
	}

	public void StopMusic()
	{
		this.StopSound(AudioEvent.Music_LoseMusic_CSR2, null);
		this.StopSound(AudioEvent.Music_MainMusic_CSR2, null);
		this.StopSound(AudioEvent.Music_WinMusic_CSR2, null);
		this.musicResultsActive = false;
	}

	public void SetVolume(string eventName, float volume, GameObject gameObject = null)
	{
		this.PostEvent(eventName, EventAction.SetVolume, volume, gameObject, null);
	}

	public void SetPitch(string eventName, float pitch, GameObject gameObject = null)
	{
		this.PostEvent(eventName, EventAction.SetPitch, pitch, gameObject, null);
	}

	public void SetParameter(string eventName, string parameterName, float value, GameObject parentGameObject = null)
	{
		AudioManager._fabricEventManager.SetParameter(eventName, parameterName, value, parentGameObject);
	}

	private void DoPlaySound(string eventName, GameObject gameObject = null, InitialiseParameters initialiseParameters = null)
	{
		Debug.Log(eventName+"   azaaz");
		this.PostEvent(eventName, EventAction.PlaySound, null, gameObject, null);
	}
	
		
	public void PlaySound(string eventName, GameObject gameObject = null)
	{
		this.DoPlaySound(eventName, gameObject, null);
	}

	public void PlaySound(string eventName, float volume, GameObject gameObject = null)
	{
		this.DoPlaySound(eventName, gameObject, null);
		this.SetVolume(eventName, volume, gameObject);
	}

	public void PlaySound(string eventName, float volume, float pitch, GameObject gameObject = null)
	{
		this.DoPlaySound(eventName, gameObject, null);
		this.SetVolume(eventName, volume, gameObject);
		this.SetPitch(eventName, pitch, gameObject);
	}

	public void PlaySoundSilentLoop(string eventName, GameObject gameObject = null)
	{
		this.DoPlaySound(eventName, gameObject, null);
		this.SetVolume(eventName, 0f, gameObject);
	}

	public void PauseSound(string eventName, GameObject gameObject = null)
	{
		this.PostEvent(eventName, EventAction.PauseSound, null, gameObject, null);
	}

	public void PauseSounds(ref List<AudioComponent> pausedAudioComponents)
	{
		AudioComponent[] componentsInChildren = base.GetComponentsInChildren<AudioComponent>();
		AudioComponent[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			AudioComponent audioComponent = array[i];
			if (audioComponent.IsPlaying() && audioComponent.CurrentState != AudioComponentState.Paused)
			{
				audioComponent.Pause(true);
				if (pausedAudioComponents != null)
				{
					pausedAudioComponents.Add(audioComponent);
				}
			}
		}
	}

	public void UnpauseSound(string eventName, GameObject gameObject = null)
	{
		this.PostEvent(eventName, EventAction.UnpauseSound, null, gameObject, null);
	}

	public void UnpauseSounds(List<AudioComponent> audioComponents)
	{
		if (audioComponents != null)
		{
			foreach (AudioComponent current in audioComponents)
			{
				current.Pause(false);
			}
		}
	}

	public void StopSound(string eventName, GameObject gameObject = null)
	{
		this.PostEvent(eventName, EventAction.StopSound, null, gameObject, null);
	}

	public void FadeInComponent(string destinationComponent, float targetMS, float curve)
	{
		AudioManager._fabricManager.FadeInComponent(destinationComponent, targetMS, curve);
	}

	public void FadeOutComponent(string destinationComponent, float targetMS, float curve)
	{
		AudioManager._fabricManager.FadeOutComponent(destinationComponent, targetMS, curve);
	}

	public Fabric.Component GetComponentByName(string destinationComponent)
	{
		return AudioManager._fabricManager.GetComponentByName(destinationComponent);
	}

	public void AdjustEngineVolume(float amount)
	{
		if (this._carsEnginesComponent_Human != null)
		{
			this._carsEnginesComponent_Human.Volume += amount;
		}
		if (this._carsEnginesComponent_AI != null)
		{
			this._carsEnginesComponent_AI.Volume += amount;
		}
	}

	public void SetCarsVolume(float value)
	{
		if (this._carsComponent_Human != null)
		{
			this._carsComponent_Human.Volume = value;
		}
		if (this._carsComponent_AI != null)
		{
			this._carsComponent_AI.Volume = value;
		}
	}

    public void SetMute(string component, bool value)
    {
        FabricManager.Instance.GetComponentByName(component).Mute = value;
    }
}

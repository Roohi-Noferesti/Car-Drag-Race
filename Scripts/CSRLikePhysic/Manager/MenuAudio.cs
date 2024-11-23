using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    [SerializeField] private AudioSource m_current;
    [SerializeField] private AudioSource m_previous;
	private const float MaxVol = 0.86f;

	public static MenuAudio Instance;

	private bool muteMusic = true;

	private MusicLooper musicSource = new MusicLooper();

	private MusicState ourMusicState;

	private CrewMusicState ourCrewMusicState = CrewMusicState.Stopped;

	public AudioClip RaceWon;

	public AudioClip RaceLost;

	public AnimationCurve FadeCurve;

	public AudioClip[] musicElems;

	public AudioClip CrewProgressionMusic;

    private int currentMusicId = -1;

	private int nextMusicId;

    private bool ascending = true;

    private int sameLoop;

	private float TargetFadeTime;

	private float CurrentFadeTime;

	private float StartFadeVol;

	private float TargetFadeVol;

	private bool WeAreFading;

	public void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
		this.musicSource = new MusicLooper();
	    this.musicSource.Initialise(m_current, m_previous);
	    nextMusicId = Random.Range(0, musicElems.Length - 1);
	    //OSXEvents.ApplicationDidBecomeKeyEvent += new OSXEvents_Delegate(this.ApplicationGainedFocus);
	    //OSXEvents.ApplicationDidResignKeyEvent += new OSXEvents_Delegate(this.ApplicationLostFocus);
	}

	private void ApplicationLostFocus()
	{
		this.MuteAudio();
	}

	private void ApplicationGainedFocus()
	{
		this.UnMuteAudio();
	}

	public void MuteAudio()
	{
		Instance.setMuteMusic(true);
        //AudioManager.Instance.SetMute("Audio_SFXAudio", true);
        AudioManager.Instance.SetMusicMuted(true);
	}

	public void UnMuteAudio()
	{
		Instance.setMuteMusic(PlayerProfileManager.Instance.ActiveProfile.OptionMusicMute);
        //AudioManager.Instance.SetMute("Audio_SFXAudio", PlayerProfileManager.Instance.ActiveProfile.OptionSoundMute);
        AudioManager.Instance.SetMusicMuted(PlayerProfileManager.Instance.ActiveProfile.OptionSoundMute);
	}

	public void OnProfileChanged()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.OptionMusicMute != this.muteMusic)
		{
			this.muteMusic = PlayerProfileManager.Instance.ActiveProfile.OptionMusicMute;
			this.setMuteMusic(this.muteMusic);
		}
	}

	public void OnDestroy()
	{
		Instance = null;
	}

	public void OnActivate()
	{
	    this.nextMusicId = this.musicElems.Length - 3;
	}

	public void Update()
	{
		if (this.muteMusic)
		{
			return;
		}
		if (this.ourCrewMusicState != CrewMusicState.Stopped)
		{
			this.musicSource.Update(this.CrewProgressionMusic);
		}
		else if (this.ourMusicState != MusicState.Stopped && this.musicSource.Update(this.musicElems[this.nextMusicId]))
		{
            this.currentMusicId = this.nextMusicId;
			this.FindNextMusicId();
		}
		if (this.WeAreFading)
		{
			this.CurrentFadeTime += Time.deltaTime;
			float num = Mathf.Min(this.CurrentFadeTime / this.TargetFadeTime, 1f);
			this.musicSource.volume = this.TargetFadeVol * num + (1f - num) * this.StartFadeVol;
			if (num >= 1f)
			{
				this.WeAreFading = false;
			}
		}
		if (this.ourMusicState == MusicState.FadeOut)
		{
			if (!this.muteMusic)
			{
				this.musicSource.volume = this.musicSource.volume * (0.9f - Time.deltaTime);
			}
			if ((double)this.musicSource.volume < 0.1)
			{
				this.musicSource.volume = 0f;
				this.ourMusicState = MusicState.Stopped;
			}
		}
		if (this.ourMusicState == MusicState.FadeIn)
		{
			if (!this.muteMusic)
			{
				this.musicSource.volume = this.musicSource.volume * (1.1f + Time.deltaTime);
			}
			if ((double)this.musicSource.volume > 0.8100000143051147)
			{
				this.musicSource.volume = 0.86f;
				this.ourMusicState = MusicState.Playing;
			}
		}
	}

	public void FadeMusicToCrewProgression()
	{
		this.ourCrewMusicState = CrewMusicState.Playing;
	}

	public void FadeMusicFromCrewProgression()
	{
		this.ourCrewMusicState = CrewMusicState.Stopped;
	}

	public void setMuteMusic(bool mute)
	{
		this.muteMusic = mute;
		if (this.muteMusic)
		{
			this.ourMusicState = MusicState.Stopped;
			this.musicSource.volume = 0f;
			BasePlatform.ActivePlatform.SetMusicMode(BasePlatform.eAudioMode.iPodMusic);
		}
		else
		{
			this.ourMusicState = MusicState.Playing;
			this.musicSource.volume = 1f;
			BasePlatform.ActivePlatform.SetMusicMode(BasePlatform.eAudioMode.gameMusic);
		}
	}

    public void stopMusicPlaying()
    {
        this.musicSource.volume = 1f;
        this.ourMusicState = MusicState.Stopped;
    }

    public void setMusicPlaying(bool play)
	{
        //Debug.Log(play + "   " + ourMusicState);
		if (!play)
		{
			if (this.ourMusicState != MusicState.Playing)
			{
				return;
			}
			this.ourMusicState = MusicState.FadeOut;
		}
		else if (this.ourMusicState != MusicState.Playing)
		{
		    this.nextMusicId = Random.Range(0, musicElems.Length - 1);
            this.sameLoop = 0;
			this.ourMusicState = MusicState.Playing;
			if (!this.muteMusic)
			{
				this.musicSource.volume = 1f;
			}
		}
	}

	public void fadeMusic(float target, float time)
	{
		if (this.muteMusic)
		{
			return;
		}
		this.TargetFadeTime = time;
		this.CurrentFadeTime = 0f;
		this.StartFadeVol = this.musicSource.volume;
		this.TargetFadeVol = target;
		this.WeAreFading = true;
	}

	public void FindNextMusicId()
	{
        //nextMusicId = 0;
        int num = Random.Range(0, 15);
        if (this.sameLoop % 2 == 1)
        {
            if (this.nextMusicId == this.musicElems.Length - 1)
            {
                this.ascending = false;
            }
            else if (this.nextMusicId == 0)
            {
                this.ascending = true;
            }
            else if (Random.Range(0, 12) < 2)
            {
                this.ascending = !this.ascending;
            }
            if (num < 2)
            {
                this.nextMusicId += ((!this.ascending) ? -2 : 2);
                if (this.nextMusicId < 0)
                {
                    this.nextMusicId = Random.Range(0, musicElems.Length - 1);
                }
                if (this.nextMusicId >= this.musicElems.Length)
                {
                    this.nextMusicId = this.musicElems.Length - 1;
                }
            }
            else
            {
                this.nextMusicId += ((!this.ascending) ? -1 : 1);
            }
        }
        if (this.currentMusicId == this.nextMusicId)
        {
            this.sameLoop++;
        }
        else
        {
            this.sameLoop = 0;
        }
	}

	public void playSound(AudioSfx bleep)
	{
		if (bleep == AudioSfx.None)
		{
			return;
		}
		AudioClip nextClip = null;
		if (bleep == AudioSfx.RaceLost || bleep == AudioSfx.RaceWon)
		{
			if (this.muteMusic)
			{
				return;
			}
			if (bleep != AudioSfx.RaceWon)
			{
				if (bleep == AudioSfx.RaceLost)
				{
					nextClip = this.RaceLost;
				}
			}
			else
			{
				nextClip = this.RaceWon;
			}
            this.nextMusicId = Random.Range(0, musicElems.Length - 1);
            this.currentMusicId = this.nextMusicId;
			this.musicSource.Interrupt(nextClip);
			this.setMusicPlaying(true);
		}
		else
		{
			string text = string.Empty;
		    switch (bleep)
		    {
		        case AudioSfx.MenuClickBeep:
		            text = "Frontend/Forward";
		            break;
		        case AudioSfx.MenuClickForward:
                    text = "Frontend/Forward";
		            break;
		        case AudioSfx.MenuClickBack:
                    text = "Frontend/Back";
		            break;
		        case AudioSfx.MenuClickFail:
		            text = "MenuFail";
		            break;
		        case AudioSfx.Purchase:
		            text = AudioEvent.Frontend_Purchase;//"MenuPurchase";
		            break;
		        case AudioSfx.PaintChange:
		            text = AudioEvent.PaintChange;//"PaintChange";
		            break;
                case AudioSfx.SpoilerChange:
		            text = AudioEvent.Frontend_SpoilerChanged;//"SpoilerChange";
                    break;
		        case AudioSfx.UpgradeArrived:
		            text = AudioEvent.Frontend_UpgradeArrived;//"MenuUpgradeArrive";
		            break;
		        case AudioSfx.CarArrived:
		            text = AudioEvent.Frontend_CarArrived;//"MenuCarArrive";
		            break;
		        case AudioSfx.StartTheRace:
                    text = "Frontend/StartRace";
		            break;
                case AudioSfx.Popup:
		            text = AudioEvent.Frontend_DialoguePopUp_First;//"Popup";
                    break;
		    }
		    if (text.Length > 0)
		    {
		        if (Camera.main != null)
		            AudioManager.Instance.PlaySound(text, Camera.main.gameObject);
		    }
		}
	}
}

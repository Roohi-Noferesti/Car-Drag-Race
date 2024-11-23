using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/SequenceManager")]
public class SequenceManager : MonoBehaviour
{
	public delegate void SequenceChangeDelegate(string zSequenceName);

	public delegate void ShotEndDelegate();

	private ShotSequence activeSequence;

	private List<ShotSequence> sequences = new List<ShotSequence>();

	private static SequenceManager instance;

	private bool loopCurrentSequence;

	private ShotSequence currentSequence;

	private CameraState lastCameraState;

	private bool isTransitioning;

	private float transitionDuration;

	private float transitionTimer;

	private AnimationCurve transitionCurve;

	private CameraState transitionStartState;

    public event SequenceChangeDelegate OnSequenceEnd;

    public event SequenceChangeDelegate OnSequenceStart;

    public event ShotEndDelegate OnShotEnd;

	public static SequenceManager Instance
	{
		get
		{
			return instance;
		}
	}

	public ShotSequence ActiveSequence
	{
		get
		{
			return activeSequence;
		}
	}

	private void Awake()
	{
		if (transform.position != Vector3.zero)
		{
		}
		if (instance != null)
		{
			return;
		}
		instance = this;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Sequence");
		GameObject[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject = array2[i];
			sequences.Add(gameObject.GetComponent<ShotSequence>());
		}
	}

	public ShotSequence GetSequence(string sequenceName)
	{
		return sequences.Find(q => q.name == sequenceName);
	}

	public void PlaySequence(ShotSequence zSequence)
	{
		PlaySequence(zSequence, false);
	}

	public void PlaySequence(ShotSequence zSequence, bool zLoop)
	{
		zSequence.Activate();
		activeSequence = zSequence;
		loopCurrentSequence = zLoop;
		currentSequence = zSequence;
		Update();
		if (OnSequenceStart != null)
		{
			OnSequenceStart(activeSequence.name);
		}
	}

	public void StopSequence()
	{
		activeSequence = null;
		currentSequence = null;
	}

	public void PlaySequence(string zName)
	{
		PlaySequence(zName, false);
	}

	public void PlaySequence(string zName, bool zLoop)
	{
		ShotSequence sequence = GetSequence(zName);
		if (sequence == null)
		{
			return;
		}
		PlaySequence(sequence, zLoop);
	}

	public void CallShotEnd()
	{
		if (OnShotEnd != null)
		{
			OnShotEnd();
		}
	}

	public void TransitionToSequence(string name, float duration)
	{
		TransitionToSequence(name, duration, null);
	}

	public void TransitionToSequence(string name, float duration, AnimationCurve curve)
	{
		transitionStartState = lastCameraState;
		isTransitioning = true;
		transitionDuration = duration;
		transitionTimer = 0f;
		transitionCurve = curve;
		PlaySequence(name);
	}

	public void TriggerSequenceEndCall()
	{
		if (OnSequenceEnd != null)
		{
			OnSequenceEnd(activeSequence.name);
		}
	}

	private void Update()
	{
		if (activeSequence != null)
		{
			bool flag = activeSequence.OnUpdate(out lastCameraState);
			if (isTransitioning)
			{
				transitionTimer += Time.deltaTime;
				if (transitionTimer >= transitionDuration)
				{
					isTransitioning = false;
				}
				else
				{
					float num = transitionTimer / transitionDuration;
					if (transitionCurve != null)
					{
						num = transitionCurve.Evaluate(num);
					}
					lastCameraState = CameraState.Lerp(transitionStartState, lastCameraState, num);
				}
			}
		    var mainCamera = Camera.main;
		    if (mainCamera != null)
		    {
		        Camera.main.transform.position = lastCameraState.position;
		        Camera.main.transform.rotation = lastCameraState.rotation;
		        Camera.main.fieldOfView = lastCameraState.fov;
		    }
		    if (!flag)
			{
				if (OnSequenceEnd != null)
				{
					OnSequenceEnd(activeSequence.name);
				}
				if (loopCurrentSequence)
				{
					PlaySequence(currentSequence, true);
				}
			}
		}
	}
}

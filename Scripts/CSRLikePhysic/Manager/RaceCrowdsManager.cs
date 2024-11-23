using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceCrowdsManager : MonoBehaviour
{
	public const int maxFlashes = 25;

	public const float flashTimer = 0.015f;

	public const float flashVar = 0.005f;

	private const float startFlashesZPre = 10f;

	public static RaceCrowdsManager Instance;

	public List<Vector3> crowdBoundsPositions;

	public List<Vector3> crowdBoundsSizes;

	public List<GameObject> cameraFlashes;

	private List<Bounds> _crowdBillboardBounds;

	private Queue<GameObject> _cameraFlashesQueue;

	private float nextFlashTimer;

	private float startFlashesZ;

	private Transform cameraTrans;

	private void Awake()
	{
		Instance = this;
		this._crowdBillboardBounds = new List<Bounds>(this.crowdBoundsPositions.Count);
		for (int i = 0; i < this.crowdBoundsPositions.Count; i++)
		{
			this._crowdBillboardBounds.Add(new Bounds(this.crowdBoundsPositions[i], this.crowdBoundsSizes[i]));
		}
		this.startFlashesZ = this._crowdBillboardBounds.Min((Bounds q) => q.min.z);
		this._cameraFlashesQueue = new Queue<GameObject>(this.cameraFlashes.Count);
		foreach (GameObject current in this.cameraFlashes)
		{
			this._cameraFlashesQueue.Enqueue(current);
		}
		this.cameraTrans = Camera.main.transform;
	}

	public void StartFlashes()
	{
		base.gameObject.SetActive(true);
	}

	public void StopFlashes()
	{
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (this.cameraTrans.position.z < this.startFlashesZ)
		{
			return;
		}
		this.nextFlashTimer -= Time.deltaTime;
		while (this.nextFlashTimer < 0f)
		{
			this.nextFlashTimer += Random.Range(0.01f, 0.02f);
			this.StartFlash();
		}
	}

	private void StartFlash()
	{
		if (this._cameraFlashesQueue.Count == 0)
		{
			return;
		}
		GameObject gameObject = this._cameraFlashesQueue.Peek();
		if (gameObject.activeInHierarchy)
		{
			return;
		}
		this._cameraFlashesQueue.Dequeue();
		Bounds bounds = this._crowdBillboardBounds[Random.Range(0, this._crowdBillboardBounds.Count)];
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		min.y = (min.y + max.y) * 0.5f;
		float value = Random.value;
		float value2 = Random.value;
		float x = Mathf.Max(min.x, max.x);
		Vector3 position = new Vector3(x, min.y * (1f - value) + max.y * value, min.z * (1f - value2) + max.z * value2);
		gameObject.transform.position = position;
		gameObject.SetActive(true);
		this._cameraFlashesQueue.Enqueue(gameObject);
	}
}

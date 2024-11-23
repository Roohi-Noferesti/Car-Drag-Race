using System;
using UnityEngine;

public class AdvancedLaser : MonoBehaviour
{
	private GameObject _laser;

	private GameObject _lightFlare;

	private static Color _beamColour = new Color(0.121f, 0.447f, 0.639f, 1f);

	private static Color _flareColour = new Color(0.121f, 0.447f, 0.639f, 1f);

	public static bool ColourChanged;

	public static bool FlickerEnabled;

	private Vector3 originalPos;

	private float originalWidth;

	private Vector3 hidePosition = new Vector3(0f, 0f, -10000f);

	private bool phase;

	private static uint laserCount = 0u;

	private void Awake()
	{
		foreach (Transform transform in base.transform)
		{
			if (transform.name.ToLower() == "laser")
			{
				this._laser = transform.gameObject;
			}
			else if (transform.name.ToLower() == "light")
			{
				this._lightFlare = transform.gameObject;
			}
		}
		AdvancedLaser.laserCount += 1u;
		if (AdvancedLaser.laserCount % 2u == 0u)
		{
			this.phase = true;
		}
		else
		{
			this.phase = false;
		}
	}

	private void Update()
	{
		float num = 1f;
		if (this.phase)
		{
			num = -1f;
		}
		if (AdvancedLaser.FlickerEnabled)
		{
			float x = Mathf.Lerp(this._laser.transform.localScale.x, this.originalWidth * 2f, Time.deltaTime);
			if (Mathf.Sin(Time.timeSinceLevelLoad * 2f * 3.14159274f * 12f) * num > 0f)
			{
				this._laser.transform.position = this.hidePosition;
				this._lightFlare.transform.position = this.hidePosition;
			}
			else
			{
				this._laser.transform.localScale = new Vector3(x, this._laser.transform.localScale.y, this._laser.transform.localScale.z);
				this._laser.transform.position = this.originalPos;
				this._lightFlare.transform.position = this.originalPos;
			}
		}
		else
		{
			float x2 = Mathf.Lerp(this._laser.transform.localScale.x, this.originalWidth, Time.deltaTime * 6f);
			this._laser.transform.localScale = new Vector3(x2, this._laser.transform.localScale.y, this._laser.transform.localScale.z);
			this._laser.transform.position = this.originalPos;
			this._lightFlare.transform.position = this.originalPos;
		}
	}

	public void UpdateColours()
	{
		this._laser.GetComponent<Renderer>().material.SetColor("_color", AdvancedLaser._beamColour);
		this._lightFlare.GetComponent<Renderer>().material.SetColor("_color", AdvancedLaser._flareColour);
	}

	public static void SetColours(Color beamColour, Color flareColour)
	{
		AdvancedLaser._beamColour = new Color(beamColour.r, beamColour.g, beamColour.b, AdvancedLaser._beamColour.a);
		AdvancedLaser._flareColour = new Color(flareColour.r, flareColour.g, flareColour.b, AdvancedLaser._flareColour.a);
		AdvancedLaser.ColourChanged = true;
	}

	public static void SetBeamColour(Color beamColour)
	{
		AdvancedLaser._beamColour = new Color(beamColour.r, beamColour.g, beamColour.b, AdvancedLaser._beamColour.a);
		AdvancedLaser.ColourChanged = true;
	}

	public static void SetFlareColour(Color flareColour)
	{
		AdvancedLaser._flareColour = new Color(flareColour.r, flareColour.g, flareColour.b, AdvancedLaser._flareColour.a);
		AdvancedLaser.ColourChanged = true;
	}

	public static void SetAlpha(float alpha)
	{
		AdvancedLaser._flareColour.a = alpha;
		AdvancedLaser._beamColour.a = alpha;
		AdvancedLaser.ColourChanged = true;
	}

	public static void GetColours(ref Color beamColour, ref Color flareColour)
	{
		beamColour = AdvancedLaser._beamColour;
		flareColour = AdvancedLaser._flareColour;
	}

	public void LockPosition()
	{
		this.originalPos = base.transform.position;
		this.originalWidth = 0.7f;
	}
}

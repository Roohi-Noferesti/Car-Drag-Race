using System;
using UnityEngine;

public class FlareManager : MonoBehaviour
{
	private const float ScreenConversion = 200f;

	private FlareGroup[] Groups;

	private Vector2 Line;

	public Vector2 CentrePosition = Vector2.zero;

	public Vector2 Position = new Vector2(1f, 0f);

	public float Length = 1f;

	private void Awake()
	{
		this.Groups = base.GetComponentsInChildren<FlareGroup>();
		this.SaveAlphas();
	}

	private void Start()
	{
		this.UpdateLine();
		this.UpdateColors();
		this.InitialisePositions();
	}

	private void Update()
	{
	}

	public void InitialiseAnimation(float startingX, float y)
	{
		Vector2 position = new Vector2(startingX, y);
		this.Position = position;
		this.InitialisePositions();
		this.InitialiseColors();
		this.UpdateColors();
	}

	public void SaveAlphas()
	{
		FlareGroup[] groups = this.Groups;
		for (int i = 0; i < groups.Length; i++)
		{
			FlareGroup flareGroup = groups[i];
			flareGroup.SaveAlpha();
		}
	}

	public void InitialiseColors()
	{
		FlareGroup[] groups = this.Groups;
		for (int i = 0; i < groups.Length; i++)
		{
			FlareGroup flareGroup = groups[i];
			flareGroup.InitialiseColor();
		}
	}

	public void UpdateAnimation(float x)
	{
		this.Position.x = x;
		this.UpdatePositions(false);
		this.UpdateLine();
	}

	public void UpdateAnimation(float startingX, float y)
	{
		Vector2 position = new Vector2(startingX, y);
		this.Position = position;
		this.UpdatePositions(true);
		this.UpdateLine();
	}

	public void UpdatePositions(bool ProgressBarFinished)
	{
		Vector2 a = Vector2.zero;
		FlareGroup[] groups = this.Groups;
		for (int i = 0; i < groups.Length; i++)
		{
			FlareGroup flareGroup = groups[i];
			FlareElement[] elements = flareGroup.Elements;
			for (int j = 0; j < elements.Length; j++)
			{
				FlareElement flareElement = elements[j];
				a = this.Line;
				float elementPosition = flareElement.ElementPosition;
				a *= elementPosition;
				if (ProgressBarFinished)
				{
					flareElement.gameObject.transform.localPosition = new Vector3(a.x, a.y, 0f);
				}
				else
				{
					flareElement.gameObject.transform.localPosition = new Vector3(a.x, flareElement.GetYPosition(), 0f);
				}
			}
		}
	}

	public void UpdateColors()
	{
		FlareGroup[] groups = this.Groups;
		for (int i = 0; i < groups.Length; i++)
		{
			FlareGroup flareGroup = groups[i];
			flareGroup.UpdateColor();
		}
	}

	public void FadeColorsDelta(float deltaAlpha)
	{
		FlareGroup[] groups = this.Groups;
		for (int i = 0; i < groups.Length; i++)
		{
			FlareGroup flareGroup = groups[i];
			flareGroup.FadeColorDelta(deltaAlpha);
		}
	}

	public void FadeColorsMultiply(float alpha)
	{
		FlareGroup[] groups = this.Groups;
		for (int i = 0; i < groups.Length; i++)
		{
			FlareGroup flareGroup = groups[i];
			flareGroup.FadeColorMultiply(alpha);
		}
	}

	private void UpdateLine()
	{
		Vector2 position = this.Position;
		this.Line = this.CentrePosition - position;
		if (this.Line.magnitude < 0.01f)
		{
			this.Line = new Vector2(1f, 0f);
		}
        //this.Line = this.Line.normalized * GUICamera.Instance.ScreenWidth * this.Length;
		Debug.DrawLine(this.CentrePosition, position);
	}

	public void InitialisePositions()
	{
		Vector2 a = Vector2.zero;
		FlareGroup[] groups = this.Groups;
		for (int i = 0; i < groups.Length; i++)
		{
			FlareGroup flareGroup = groups[i];
			FlareElement[] elements = flareGroup.Elements;
			for (int j = 0; j < elements.Length; j++)
			{
				FlareElement flareElement = elements[j];
				a = this.Line;
				float elementPosition = flareElement.ElementPosition;
				a *= elementPosition;
				flareElement.gameObject.transform.localPosition = new Vector3(a.x, a.y, 0f);
				flareElement.SetYPosition(a.y);
			}
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}
}

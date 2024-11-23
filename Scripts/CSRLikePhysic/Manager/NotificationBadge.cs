using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBadge : MonoBehaviour
{
	public Image badgeIconSprite;

	public TextMeshProUGUI valueLabel;

	private int m_currentValue = -1;

	private bool cached_isEnabled;

	private string cached_message = string.Empty;

	private int cached_value = -1;

	public int CurrentValue
	{
		get
		{
			return this.m_currentValue;
		}
	}

	private void Awake()
	{
		this.cached_isEnabled = true;
		this.SetNotification(false, -1);
	}

	public void SetNotification(bool isEnabled, string message, int value, Color32? color = null)
	{
		if (this.cached_isEnabled != isEnabled || this.cached_message != message || value != this.cached_value)
		{
			this.cached_isEnabled = isEnabled;
			this.cached_message = message;
			value = this.cached_value;
			this.m_currentValue = value;
			if (this.valueLabel != null)
			{
				this.valueLabel.text = message;
				this.valueLabel.gameObject.SetActive(isEnabled);
			}
			if (this.badgeIconSprite != null)
			{
				if (color.HasValue)
				{
					this.badgeIconSprite.color = color.Value;
				}
				this.badgeIconSprite.gameObject.SetActive(isEnabled);
			}
		}
	}

	public void SetNotification(bool isEnabled, int value = -1)
	{
		if (this.cached_isEnabled != isEnabled || value != this.cached_value)
		{
			string message = (0 > value) ? string.Empty : value.ToString();
			this.SetNotification(isEnabled, message, value, null);
		}
	}

	public void SetNotification(int value)
	{
		if (this.cached_value != value)
		{
			this.SetNotification(true, value);
		}
	}
}

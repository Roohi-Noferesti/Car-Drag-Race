using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using I2.Loc;
using UnityEngine;

public class KeyMappings : MonoBehaviour
{
	public enum Action
	{
		THROTTLE,
		NITRO,
		GEAR_UP,
		GEAR_DOWN,
		PAUSE,
		COUNT
	}

	private static KeyCode[] mDisallowedKeys = new KeyCode[]
	{
		KeyCode.F1,
		KeyCode.F2,
		KeyCode.F3,
		KeyCode.F4,
		KeyCode.F5,
		KeyCode.F6,
		KeyCode.F7,
		KeyCode.F8,
		KeyCode.F9,
		KeyCode.F10,
		KeyCode.F11,
		KeyCode.F12,
		KeyCode.F13,
		KeyCode.F14,
		KeyCode.F15,
		KeyCode.Numlock,
		KeyCode.CapsLock,
		KeyCode.ScrollLock,
		KeyCode.Pause,
		KeyCode.AltGr,
		KeyCode.SysReq,
		KeyCode.Break,
		KeyCode.Menu,
		KeyCode.Help,
		KeyCode.Print,
		KeyCode.LeftWindows,
		KeyCode.RightWindows,
		(KeyCode)160
	};

    private static KeyCode[] mLocalisedKeys = new KeyCode[]
    {
        KeyCode.Backspace,
        KeyCode.Delete,
        KeyCode.Tab,
        KeyCode.Clear,
        KeyCode.Return,
        KeyCode.Escape,
        KeyCode.Space,
        KeyCode.KeypadEnter,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.Insert,
        KeyCode.Home,
        KeyCode.End,
        KeyCode.PageUp,
        KeyCode.PageDown,
        KeyCode.RightShift,
        KeyCode.LeftShift,
        KeyCode.RightControl,
        KeyCode.LeftControl,
        KeyCode.RightAlt,
        KeyCode.LeftAlt,
        KeyCode.LeftCommand,
        KeyCode.RightCommand
    };

    private static KeyCode[] mModifierKeys = new KeyCode[]
	{
		KeyCode.RightShift,
		KeyCode.LeftShift,
		KeyCode.RightControl,
		KeyCode.LeftControl,
		KeyCode.RightAlt,
		KeyCode.LeftAlt,
		KeyCode.LeftCommand,
		KeyCode.RightCommand
	};

    private static Dictionary<KeyCode, string> mKeyTranslations = new Dictionary<KeyCode, string>
    {
        {
            KeyCode.Alpha0,
            "0"
        },
        {
            KeyCode.Alpha1,
            "1"
        },
        {
            KeyCode.Alpha2,
            "2"
        },
        {
            KeyCode.Alpha3,
            "3"
        },
        {
            KeyCode.Alpha4,
            "4"
        },
        {
            KeyCode.Alpha5,
            "5"
        },
        {
            KeyCode.Alpha6,
            "6"
        },
        {
            KeyCode.Alpha7,
            "7"
        },
        {
            KeyCode.Alpha8,
            "8"
        },
        {
            KeyCode.Alpha9,
            "9"
        },
        {
            KeyCode.Exclaim,
            "!"
        },
        {
            KeyCode.DoubleQuote,
            "\""
        },
        {
            KeyCode.Hash,
            "#"
        },
        {
            KeyCode.Dollar,
            "$"
        },
        {
            KeyCode.Ampersand,
            "&"
        },
        {
            KeyCode.Quote,
            "'"
        },
        {
            KeyCode.LeftParen,
            "("
        },
        {
            KeyCode.RightParen,
            ")"
        },
        {
            KeyCode.Asterisk,
            "*"
        },
        {
            KeyCode.Plus,
            "+"
        },
        {
            KeyCode.Comma,
            ","
        },
        {
            KeyCode.Minus,
            "-"
        },
        {
            KeyCode.Period,
            "."
        },
        {
            KeyCode.Slash,
            "/"
        },
        {
            KeyCode.Colon,
            ":"
        },
        {
            KeyCode.Semicolon,
            ";"
        },
        {
            KeyCode.Less,
            "<"
        },
        {
            KeyCode.Equals,
            "="
        },
        {
            KeyCode.Greater,
            ">"
        },
        {
            KeyCode.Question,
            "?"
        },
        {
            KeyCode.At,
            "@"
        },
        {
            KeyCode.LeftBracket,
            "["
        },
        {
            KeyCode.Backslash,
            "\\"
        },
        {
            KeyCode.RightBracket,
            "]"
        },
        {
            KeyCode.Caret,
            "^"
        },
        {
            KeyCode.Underscore,
            "_"
        },
        {
            KeyCode.BackQuote,
            "`"
        },
        {
            KeyCode.Keypad0,
            "0"
        },
        {
            KeyCode.Keypad1,
            "1"
        },
        {
            KeyCode.Keypad2,
            "2"
        },
        {
            KeyCode.Keypad3,
            "3"
        },
        {
            KeyCode.Keypad4,
            "4"
        },
        {
            KeyCode.Keypad5,
            "5"
        },
        {
            KeyCode.Keypad6,
            "6"
        },
        {
            KeyCode.Keypad7,
            "7"
        },
        {
            KeyCode.Keypad8,
            "8"
        },
        {
            KeyCode.Keypad9,
            "9"
        },
        {
            KeyCode.KeypadPeriod,
            "."
        },
        {
            KeyCode.KeypadDivide,
            "/"
        },
        {
            KeyCode.KeypadMultiply,
            "*"
        },
        {
            KeyCode.KeypadMinus,
            "-"
        },
        {
            KeyCode.KeypadPlus,
            "+"
        },
        {
            KeyCode.KeypadEquals,
            "="
        }
    };

    public static KeyMappings Instance;

	private List<KeyCode> mMappings;

	private volatile KeyCode mLastKeyDown;

	private void Awake()
	{
		Instance = this;
		this.ReadMappings();
		base.gameObject.SetActive(false);
	}

	public void ReadMappings()
	{
		this.mMappings = new List<KeyCode>(5);
		this.mMappings.Add((KeyCode)PlayerPrefs.GetInt("km_throttle", (int) KeyCode.UpArrow));
		this.mMappings.Add((KeyCode)PlayerPrefs.GetInt("km_nitrous", (int) KeyCode.DownArrow));
		this.mMappings.Add((KeyCode)PlayerPrefs.GetInt("km_gear_up", (int) KeyCode.RightArrow));
		this.mMappings.Add((KeyCode)PlayerPrefs.GetInt("km_gear_down", (int) KeyCode.LeftArrow));
		this.mMappings.Add((KeyCode)PlayerPrefs.GetInt("km_pause", (int) KeyCode.Escape));
	}

	public void WriteMappings()
	{
		PlayerPrefs.SetInt("km_throttle", (int)this.mMappings[0]);
		PlayerPrefs.SetInt("km_nitrous", (int)this.mMappings[1]);
		PlayerPrefs.SetInt("km_gear_up", (int)this.mMappings[2]);
		PlayerPrefs.SetInt("km_gear_down", (int)this.mMappings[3]);
		PlayerPrefs.SetInt("km_pause", (int)this.mMappings[4]);
		PlayerPrefs.Save();
	}

	public IEnumerator RemapAction(Action action)
	{
        mLastKeyDown = KeyCode.None;
        while (mLastKeyDown == KeyCode.None)
        {
            yield return null;
        }
        mMappings[(int)action] = mLastKeyDown;
        mLastKeyDown = KeyCode.None;
    }

	private void OnGUI()
	{
		if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None && Array.IndexOf<KeyCode>(mDisallowedKeys, Event.current.keyCode) == -1)
		{
			this.mLastKeyDown = Event.current.keyCode;
		}
		if (this.mLastKeyDown == KeyCode.None)
		{
			KeyCode[] array = mModifierKeys;
			for (int i = 0; i < array.Length; i++)
			{
				KeyCode key = array[i];
				if (Input.GetKey(key))
				{
					this.mLastKeyDown = key;
					break;
				}
			}
		}
	}

	public KeyCode GetMapping(Action action)
	{
		return this.mMappings[(int)action];
	}

	public bool GetKey(Action action)
	{
		return Input.GetKey(this.mMappings[(int)action]);
	}

	public bool GetKeyDown(Action action)
	{
		return Input.GetKeyDown(this.mMappings[(int)action]);
	}

	public bool GetKeyUp(Action action)
	{
		return Input.GetKeyUp(this.mMappings[(int)action]);
	}

	public string GetMappingString(Action action)
	{
        KeyCode keyCode = mMappings[(int)action];
        if (mKeyTranslations.ContainsKey(keyCode))
        {
            return mKeyTranslations[keyCode];
        }
        if (Array.IndexOf(mLocalisedKeys, keyCode) != -1)
        {
            return LocalizationManager.GetTranslation("TEXT_KEY_" + keyCode.ToString().ToUpper());
        }
        return keyCode.ToString();
    }
}

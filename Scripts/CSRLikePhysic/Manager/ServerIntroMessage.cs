using System;
using System.Text;
using UnityEngine;

public static class ServerIntroMessage
{
    private const string idTag = "id";

    private const string timesToDisplayTag = "max_times";

    private const string messageTag = "text";

    private const string linkTag = "link";

    public static bool TryShowMessage()
    {
        string messageFromServer = GetMessageFromServer();
        if (string.IsNullOrEmpty(messageFromServer))
        {
            return false;
        }
        JsonDict jsonDictFromMessage = GetJsonDictFromMessage(messageFromServer);
        if (jsonDictFromMessage == null)
        {
            return false;
        }
        if (!ShouldShowMessage(jsonDictFromMessage))
        {
            return false;
        }
        string bodyStringFromMessage = GetBodyStringFromMessage(jsonDictFromMessage);
        if (bodyStringFromMessage.Length <= 0)
        {
            return false;
        }
        string body = string.Empty;
        try
        {
            byte[] array = Convert.FromBase64String(bodyStringFromMessage);
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            body = uTF8Encoding.GetString(array, 0, array.Length);
        }
        catch (Exception var_6_6D)
        {
            return false;
        }
        string text = GetLinkStringFromMessage(jsonDictFromMessage);
        if (text.Length <= 0)
        {
            text = null;
        }
        ShowMessage(body, text);
        return true;
    }

    private static JsonDict GetJsonDictFromMessage(string message)
    {
        JsonDict jsonDict = new JsonDict();
        jsonDict.Read(message);
        return jsonDict;
    }

    private static string GetMessageFromServer()
    {
        return UserManager.Instance.currentAccount.IntroMessageJson;
    }

    private static bool ShouldShowMessage(JsonDict dict)
    {
        if (!dict.ContainsKey(idTag))
        {
            return false;
        }
        int @int = dict.GetInt(idTag);
        int num = 1;
        if (dict.ContainsKey(timesToDisplayTag))
        {
            num = dict.GetInt(timesToDisplayTag);
        }
        if (num == 0)
        {
            return false;
        }
        if (PlayerProfileManager.Instance.ActiveProfile.LastServerMessageDisplayedID == @int)
        {
            if (num > 0 && PlayerProfileManager.Instance.ActiveProfile.LastServerMessageDisplayedCount >= num)
            {
                return false;
            }
        }
        else
        {
            PlayerProfileManager.Instance.ActiveProfile.LastServerMessageDisplayedCount = 0;
        }
        PlayerProfileManager.Instance.ActiveProfile.LastServerMessageDisplayedID = @int;
        PlayerProfileManager.Instance.ActiveProfile.LastServerMessageDisplayedCount++;
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        return true;
    }

    private static void ShowMessage(string body, string link = null)
    {
        PopUp popup = (link != null) ? new PopUp
        {
            Title = "TEXT_POPUPS_SERVERMESSAGE_TITLE",
            BodyText = body,
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmAction = delegate
            {
                Application.OpenURL(link);
            },
            CancelText = "TEXT_BUTTON_NO",
            ConfirmText = "TEXT_BUTTON_GO",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        } : new PopUp
        {
            Title = "TEXT_POPUPS_SERVERMESSAGE_TITLE",
            BodyText = body,
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
    }

    private static string GetBodyStringFromMessage(JsonDict dict)
    {
        if (dict.ContainsKey(messageTag))
        {
            return dict.GetString(messageTag);
        }
        return string.Empty;
    }

    private static string GetLinkStringFromMessage(JsonDict dict)
    {
        if (dict.ContainsKey(linkTag))
        {
            return dict.GetString(linkTag);
        }
        return string.Empty;
    }
}

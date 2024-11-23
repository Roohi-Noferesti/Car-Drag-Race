using System.Collections.Generic;

public static class StaticMessages
{
    public enum MessageKey
    {
        Disconnected
    }

    private static Dictionary<MessageKey, string> _messages = new Dictionary<MessageKey, string>();

    static StaticMessages()
    {
        _messages.Add(MessageKey.Disconnected, "؟ﯽﻠﺻﻭ ﺖﻧﺮﺘﻨﯾﺍ ﻪﺑ ﻪﮐ ﯽﻨﺌﻤﻄﻣ");
    }

    public static string DISCONNECTED
    {
        get { return GetMessage(MessageKey.Disconnected); }
    }

    public static string GetMessage(MessageKey key)
    {
        return _messages[key];
    }
}

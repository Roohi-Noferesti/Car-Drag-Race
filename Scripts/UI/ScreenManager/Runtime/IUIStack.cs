

using System;

namespace KingKodeStudio
{
    public interface IUIStack
    {
        ScreenID TopScreen { get; }
        ScreenID TopNextScreen { get; }
        int Count { get; }
        void Push(ScreenID screenID);
        ScreenID Pop();

        bool PopToScreen(ScreenID screenID);
        void Pop(int count, params Type[] types);
        void clear();
        bool Remove(ScreenID screenID);
        bool IsScreenOnStack(ScreenID screenID);
    }
}

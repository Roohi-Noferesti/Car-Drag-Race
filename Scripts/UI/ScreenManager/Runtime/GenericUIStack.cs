using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace KingKodeStudio
{
    public class GenericUIStack : IUIStack
    {
        private Stack<ScreenID> m_stack;

        public GenericUIStack(IEnumerable<ScreenID> screens)
        {
            m_stack = new Stack<ScreenID>();
        }

        public ScreenID TopScreen
        {
            get
            {
                if (m_stack.Count > 0)
                {
                    return m_stack.Peek();
                }
                return ScreenID.Invalid;
            }
        }

        public ScreenID TopNextScreen
        {
            get
            {
                if (m_stack.Count > 1)
                {
                    var popScreen = m_stack.Pop();
                    var result = m_stack.Peek();
                    m_stack.Push(popScreen);
                    return result;
                }
                return ScreenID.Invalid;
            }
        }

        public int Count
        {
            get { return m_stack.Count; }
        }

        public void Push(ScreenID screenId)
        {
            m_stack.Push(screenId);
            //hudScreen.transform.SetAsLastSibling();
        }

        public ScreenID Pop()
        {
            if (m_stack.Count > 0)
                return m_stack.Pop();
            return ScreenID.Invalid;
        }

        public bool PopToScreen(ScreenID screenID)
        {
            if (m_stack.Contains(screenID))
            {
                while (m_stack.Count > 0)
                {
                    var topScreen = m_stack.Peek();

                    if (topScreen == screenID)
                    {
                        return true;
                    }
                    m_stack.Pop();
                }
            }
            return false;
        }

        public void Pop(int count, params Type[] types)
        {
            for (int j = 0; j < count; j++)
            {
                if (m_stack.Count > 0)
                {
                    var screen = m_stack.Peek();
                    if (types != null && types.Any(t => t == screen.GetType()))
                    {
                        m_stack.Pop();
                        //screen.Visible = false;
                        //screen.transform.SetAsFirstSibling();
                    }
                }
            }
        }

        public void clear()
        {
            //if (m_stack.Count > 0)
            //{
            //    var screen = m_stack.Pop();
            //    while (screen!=null)
            //    {
            //        //Object.Destroy(screen.gameObject);
            //        screen = null;
            //        if (m_stack.Count > 0)
            //            screen = m_stack.Pop();
            //    }
            //}
            m_stack.Clear();
        }

        public bool Remove(ScreenID screenID)
        {
            bool found = false;
            if (screenID!=ScreenID.Invalid)
            {
                Stack<ScreenID> tempStack = new Stack<ScreenID>();
                while (m_stack.Count > 0)
                {
                    var topScreen = m_stack.Pop();

                    if (topScreen != screenID)
                    {
                        tempStack.Push(topScreen);
                    }
                    else
                    {
                        found = true;
                        break;
                    }
                }

                while (tempStack.Count > 0)
                {
                    var tempScreen = tempStack.Pop();
                    if (tempScreen != ScreenID.Invalid)
                        m_stack.Push(tempScreen);
                }
            }

            return found;
        }

        public bool IsScreenOnStack(ScreenID screenID)
        {
            return m_stack.Contains(screenID);
        }
    }

}
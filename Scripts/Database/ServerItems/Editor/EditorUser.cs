using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EditorUser
{
    public string username;
    public string displayName;
    public long userid;
}

[Serializable]
public class EditorUsers
{
    public List<EditorUser> Users = new List<EditorUser>();

    public int Count
    {
        get { return Users.Count; }
    }

    public EditorUser this[int i]
    {
        get { return Users[i]; }
    }

    public void Add(EditorUser editorUser)
    {
        Users.Add(editorUser);
    }

    public bool Has(long userID)
    {
        return Users.Any(editorUser => editorUser.userid == userID);
    }

    public void Delete(long userid)
    {
        var user = Users.FirstOrDefault(u => u.userid == userid);

        if (user != null)
        {
            Users.Remove(user);
        }
    }
}

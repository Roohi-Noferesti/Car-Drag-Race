using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class AvatarIDs
{
    private static string[] m_avatarIDs;

    private static string[] avatarIdsToExcludeInsideCountry = new string[] {
        "avatar_33",
        "avatar_44",
        "avatar_49",
        "avatar_51",
        "avatar_52",
        "avatar_53",
        "avatar_54"
    };

    private static bool filterAvatars;

    static AvatarIDs()
    {
        filterAvatars = !BasePlatform.ActivePlatform.ShouldShowFemaleHair;

        m_avatarIDs = new string[filterAvatars ? 54 - avatarIdsToExcludeInsideCountry.Length : 54];
        int arrayIndex = 0;
        for (int i = 0; i < 54; i++)
        {
            string id = "avatar_" + (i + 1).ToString();
            if (!filterAvatars || (filterAvatars && !avatarIdsToExcludeInsideCountry.Contains(id)))
            {
                m_avatarIDs[arrayIndex] = id;
                arrayIndex++;
            }
        }
    }

    public static string[] IDs
    {
        get { 
            if(filterAvatars)
            {
                return m_avatarIDs.Where(i => !avatarIdsToExcludeInsideCountry.Contains(i)).ToArray();
            } else {
                return m_avatarIDs;
            }
        }
    }
}

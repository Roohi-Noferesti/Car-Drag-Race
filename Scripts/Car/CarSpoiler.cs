using System;
using UnityEngine;

[Serializable]
public class CarSpoiler 
{
    [SerializeField]
    private string m_spoilerID;

    [SerializeField]
    private Vector3 m_position;

    [SerializeField]
    private Vector3 m_scale;

    [SerializeField]
    private Quaternion m_rotation;

    public string SpoilerID
    {
        get { return m_spoilerID; }
    }

    public Vector3 Position
    {
        get { return m_position; }
    }

    public Vector3 Scale
    {
        get { return m_scale; }
    }

    public Quaternion Rotation
    {
        get { return m_rotation; }
    }
}

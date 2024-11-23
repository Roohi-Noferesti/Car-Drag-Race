using UnityEngine;
using System.Collections;

public interface IMapPin
{
    Vector3 position { get; }

    ProgressionMapPinEventType type { get; }

    eCarTier tier { get; }
    bool interactable { get; set; }
    string Name { get; set; }

    void SetHightlight(bool value);
}

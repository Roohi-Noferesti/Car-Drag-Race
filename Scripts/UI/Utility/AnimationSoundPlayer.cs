using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundPlayer : MonoBehaviour 
{
    public void PlaySound(string soundName)
    {
        AudioManager.Instance.PlaySound(soundName, Camera.main.gameObject);
    }
}

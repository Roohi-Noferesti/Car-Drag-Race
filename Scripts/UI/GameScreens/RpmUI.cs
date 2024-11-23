using System;
using UnityEngine;

[Serializable]
public class RpmUI
{
    [SerializeField] private Transform m_needle;
    [SerializeField] private float _minDegree = -1.636322F;
    [SerializeField] private float _maxDegree = -147.2975F;
    [HideInInspector]
    public float minRpm;
    [HideInInspector]
    public float maxRpm;
    //[SerializeField]
    //private float _smoothness = 4;

    //void Start()
    //{
    //    LevelController.gameStateChanged += GameController_GameStateChanged;
    //    enabled = false;
    //}

    //void OnDestroy()
    //{
    //    LevelController.gameStateChanged -= GameController_GameStateChanged;
    //}

    //void GameController_GameStateChanged(LevelController.GameStates obj)
    //{
    //    switch (obj)
    //    {
    //        case LevelController.GameStates.Ready:
    //            _engine = ObjectLocator.getObject<CarEngine>(ObjectLocator.PlayerCarEngine);
    //            MinRpm = _engine.Specification.MinEngineRpm;
    //            MaxRpm = _engine.Specification.MaxEngineRpm;
    //            enabled = true;
    //            break;
    //    }
    //}


    public void updateUI(float rpm)
    {
        //if (jitter)
        //{
        //    var t = Mathf.InverseLerp(minRpm, maxRpm, rpm);
        //    var angle = Mathf.Lerp(_minDegree, _maxDegree, t) + Mathf.Sin((Time.time*gear1JitterSpeed)%10)*gear1JitterSize;
        //    m_needle.localRotation = Quaternion.Lerp(m_needle.localRotation, Quaternion.Euler(0, 0, angle),
        //        Time.deltaTime * _gear1JitterlerpSpeed);
        //}
        //else
        //{
            var t = Mathf.InverseLerp(minRpm, maxRpm, rpm);
            var angle = Mathf.Lerp(_minDegree, _maxDegree, t);
            m_needle.localRotation = Quaternion.Euler(0, 0, angle);//Quaternion.Lerp(m_needle.localRotation, Quaternion.Euler(0, 0, angle),
                //Time.deltaTime * _smoothness);
        //}

    }
}

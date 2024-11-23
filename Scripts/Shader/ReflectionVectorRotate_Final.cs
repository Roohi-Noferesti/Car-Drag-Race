using System.Collections.Generic;
using UnityEngine;

public class ReflectionVectorRotate_Final : MonoBehaviour
{


    [SerializeField] private float _multiplier = 2.5f;

    private CarPhysics m_carEngine;
    private CarVisuals m_carVisuals;

    private Dictionary<Material, float> m_materialDirections = new Dictionary<Material, float>();
    private Dictionary<Material, float> m_matRotX = new Dictionary<Material, float>();
    private Dictionary<Material, float> m_matRotY = new Dictionary<Material, float>();

    public void Init(CarPhysics carPhysics)
    {
        m_materialDirections.Clear();
        m_matRotX.Clear();
        m_matRotY.Clear();
        m_carEngine = carPhysics;
        m_carVisuals = GetComponent<CarVisuals>();
        enabled = m_carEngine != null && m_carVisuals != null;
    }

    public void AddMaterial(Material material, float speed)
    {
        if (material == null)
        {
            Debug.LogError("Can't add material to reflection rotation.Because material is null for engine : " +
                           m_carEngine.gameObject.name);
            return;
        }
        m_materialDirections.Add(material, speed);
        m_matRotX.Add(material, 0);
        m_matRotY.Add(material, 0);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (PauseGame.isGamePaused)
	        return;
	    foreach (var material in m_materialDirections)
	    {
	        m_matRotX[material.Key] -= Time.deltaTime*m_carEngine.SpeedMS*_multiplier*material.Value;
            m_matRotY[material.Key] += Time.deltaTime * m_carEngine.SpeedMS * _multiplier * material.Value;
            Matrix4x4 m1 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(m_matRotX[material.Key], 0, 0), Vector3.one);
            Matrix4x4 m2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, m_matRotY[material.Key], 0), Vector3.one);

	        material.Key.SetMatrix("_myMatrix_up", m1);
	        material.Key.SetMatrix("_myMatrix_side", m2);
	    }
	}
}

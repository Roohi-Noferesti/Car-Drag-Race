using UnityEngine;
using System.Collections;

public class AB_Quality_vs_Performance : MonoBehaviour {


	

	public void AA (int Q) 
    {
        QualitySettings.antiAliasing = Q;
	}


    public void Tex_Full(int Q)
    {
        QualitySettings.masterTextureLimit = Q;
    }

}

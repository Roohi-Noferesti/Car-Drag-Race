using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class cam_effect : MonoBehaviour {

    public float bias = 0.81f;
	public float smoothness = 0.98f;
    public Color col;
	private Material material;

	// Creates a private material used to the effect
	void Awake ()
	{
        //material = new Material(Shader.Find("Custom/AB_Vignette"));

        //enabled = EnvQualitySettings.UseVignette;

    }

	// Postprocess the image
    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination);

    //    material.SetFloat("_bias", bias);
    //    material.SetFloat("_smoothness", smoothness);
    //    material.SetColor("_Col", col);

    //    material.SetInt("_X", Screen.width);
    //    material.SetInt("_Y", Screen.height);

    //    Graphics.Blit(source, destination, material);
    //}
}
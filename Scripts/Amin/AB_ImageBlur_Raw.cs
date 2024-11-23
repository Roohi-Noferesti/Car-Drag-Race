using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AB_ImageBlur_Raw : MonoBehaviour
{

	public int Pass = 5;
	public float Amount = 0.0034f;
    public float Mult = 1f;
	public Texture VigTex;

    

	private Material material;

	// Creates a private material used to the effect
	void Awake ()
	{
		//material = new Material( Shader.Find("Custom/AB_ImageBlur") );

	 //   enabled = EnvQualitySettings.UseMotionBlur;
	}

	// Postprocess the image
	//void OnRenderImage (RenderTexture source, RenderTexture destination)
	//{

 //       Graphics.Blit (source, destination);


	//	material.SetInt("_pass", Pass);
	//	material.SetFloat("_amount", Amount);
	//	material.SetFloat("_mult", Mult);
	//	material.SetTexture ("_vigTex", VigTex);


	//	Graphics.Blit (source, destination, material);
	//}
}
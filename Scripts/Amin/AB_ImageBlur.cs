using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AB_ImageBlur : MonoBehaviour {

	public int Pass = 3;
	public float Amount = 0.01f;
    public float Mult = 1f;
    public int DownSample = 1;
	public Texture VigTex;

    

	private Material material;

	// Creates a private material used to the effect
	void Awake ()
	{
		material = new Material( Shader.Find("Custom/AB_ImageBlur") );
	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{

        source.filterMode = FilterMode.Bilinear;

        // downsample
        int rtW = source.width / DownSample;
        int rtH = source.height / DownSample;


        RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
        rt.filterMode = FilterMode.Bilinear;

        Graphics.Blit(source, rt);



        material.SetTexture("_BlurTex", rt);
		material.SetFloat("_pass", Pass);
		material.SetFloat("_amount", Amount);
		material.SetFloat("_mult", Mult);
		material.SetTexture ("_vigTex", VigTex);


		Graphics.Blit (source, destination, material);
        RenderTexture.ReleaseTemporary(rt);
	}
}
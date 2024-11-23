using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AB_Horizontal_Blur : MonoBehaviour {

	public float Amount = 1;
		public Texture VigTex;


		private Material material;

		// Creates a private material used to the effect
		void Awake ()
		{
		material = new Material( Shader.Find("Custom/AB_Horizontal_Blur") );
		}

		// Postprocess the image
		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit (source, destination);
			material.SetFloat("_amount", Amount);
			material.SetTexture ("_vigTex", VigTex);

			Graphics.Blit (source, destination, material);
		}
	}
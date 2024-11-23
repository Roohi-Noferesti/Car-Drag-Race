using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CarLightFlare : MonoBehaviour
{
	public float AngleLimit = 75f;

    public float Size_x_mult = 1f;
    public float Size_y_mult = 1f;


	private float maxdot;

	public AnimationCurve ScaleCurve;

	private float original_scale;

	private Vector3 original_carspace_forward;

	private List<Collider> raycastWith = new List<Collider>();

	private bool isSetup;

    //private int WorkshopLayer;

    private int DefaultLayer;

	private void Awake()
	{
        //WorkshopLayer = LayerMask.NameToLayer("TransparentFX");
        DefaultLayer = LayerMask.NameToLayer("Default");
		this.DoSetupRepeatable();
	}

	private void DoSetupRepeatable()
	{
		this.original_scale = base.transform.localScale.x;
	}

	private void Start()
	{
		this.DoSetup();
	}

	private void DoSetup()
	{
		if (this.isSetup)
		{
			return;
		}
		this.original_carspace_forward = base.transform.parent.InverseTransformDirection(base.transform.forward);
		if (this.ScaleCurve == null || this.ScaleCurve.length == 0)
		{
			this.ScaleCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0.002219083f, 1.001775f, -0.02535195f, -0.02535195f),
				new Keyframe(0.6521954f, 0.8256066f, -0.719081f, -0.719081f),
				new Keyframe(0.9965186f, 0.0004976988f, -4.298107f, -4.298107f)
			});
		}
		this.ScaleCurve.preWrapMode = WrapMode.Once;
		this.ScaleCurve.postWrapMode = WrapMode.Once;
		this.SetAngleLimit(this.AngleLimit);
		this.DoSetupRepeatable();
		if (false)//ScreenManager.Instance.ActiveCSRScreen.ID == ScreenID.Workshop)
		{
			//base.gameObject.layer = this.WorkshopLayer;
		}
		else
		{
			base.gameObject.layer = this.DefaultLayer;
		}
		this.isSetup = true;

	    AddColliders(transform.parent.GetComponentInChildren<Collider>());
	}

	public void SetAngleLimit(float limit)
	{
		this.AngleLimit = limit;
		this.maxdot = (90f - this.AngleLimit) / 90f;
	}

	public void AddColliders(params Collider[] colliders)
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			Collider collider = colliders[i];
			if (!(collider == null))
			{
				this.raycastWith.Add(collider);
			}
		}
	}

	public void ClearColliders()
	{
		this.raycastWith.Clear();
	}

	private void OnWillRenderObject()
	{
	    this.UpdateToCamera(Camera.current);
	}

	public void UpdateToCamera(Camera camera)
	{
		this.DoSetup();
		Vector3 lhs = base.transform.parent.TransformDirection(this.original_carspace_forward);
		float num = -Vector3.Dot(lhs, camera.transform.forward);
		float num2 = 1f - (num - this.maxdot) / (1f - this.maxdot);
		if (num2 > 1f)
		{
			base.transform.localScale = Vector3.zero;
			return;
		}
		if (this.raycastWith.Count > 0)
		{
			Vector3 direction = camera.transform.position - base.transform.position;
			direction.Normalize();
			Ray ray = new Ray(base.transform.position, direction);
			foreach (var current in this.raycastWith)
			{
				RaycastHit raycastHit;
				if (current.Raycast(ray, out raycastHit, 30))
				{
					base.transform.localScale = Vector3.zero;
					return;
				}
			}
		}
		float num3 = this.ScaleCurve.Evaluate(num2);
		base.transform.LookAt(camera.transform);
		base.transform.localScale = new Vector3(num3* Size_x_mult, num3 * Size_y_mult, num3) * this.original_scale;

		//GetComponent<Renderer>().material.SetFloat ("_Brightness",  num3);


	}

	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmos()
	{
		if (!base.GetComponent<Renderer>().enabled)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * 0.3f);
	}
}

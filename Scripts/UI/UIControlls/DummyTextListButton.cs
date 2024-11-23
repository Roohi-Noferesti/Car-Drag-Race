using System;
using System.Collections.Generic;
using UnityEngine;

public class DummyTextListButton : BaseDummyControl
{
    private float Width = 1.28f;

    public List<string> ListValues = new List<string>();

	public MonoBehaviour ScriptWithMethodToInvoke;

	public AudioSfx onClickSound = AudioSfx.MenuClickForward;

	public string IndexChangedMethodToInvoke = string.Empty;

	//private GameObject runtimeTextButtonGO;

    public RuntimeTextListButton runtimeButtonBehaviour;

	public override void ForceAwake()
	{
		//base.ForceAwake();
		//if (this.runtimeTextButtonGO != null)
		//{
		//	return;
		//}
		//UnityEngine.Object original = Resources.Load("RuntimeButtons/TextListButton");
		//this.runtimeTextButtonGO = (UnityEngine.Object.Instantiate(original) as GameObject);
		//this.runtimeTextButtonGO.transform.parent = base.transform;
		//this.runtimeTextButtonGO.transform.position = base.transform.position;
		//this.runtimeTextButtonGO.transform.rotation = base.transform.rotation;
		//this.runtimeButtonBehaviour = this.runtimeTextButtonGO.GetComponent<RuntimeTextListButton>();
		this.runtimeButtonBehaviour.LeftButton.AddValueChangedDelegate(this.onPlayButtonSound);
		this.runtimeButtonBehaviour.RightButton.AddValueChangedDelegate(this.onPlayButtonSound);
		GameObjectHelper.MakeLocalPositionPixelPerfect(base.gameObject);
		this.runtimeButtonBehaviour.Init(this.ScriptWithMethodToInvoke, this.IndexChangedMethodToInvoke, this.Width, this.ListValues);
	}

	public override GameObject GetControl()
    {
        return runtimeButtonBehaviour.gameObject;//this.runtimeTextButtonGO;
	}

    public void onPlayButtonSound()
    {
        MenuAudio.Instance.playSound(this.onClickSound);
    }
}

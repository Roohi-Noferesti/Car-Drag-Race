using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostContainerTitleOnly : MonoBehaviour
{
	public int CharacterLimit;

	public float DefaultCharacterSize;

	public TextMeshProUGUI TitleText;

	public void SetText(string title)
	{
        //this.TitleText.characterSize = this.DefaultCharacterSize * (float)this.CharacterLimit / (float)Math.Max(this.CharacterLimit, title.Length);
		this.TitleText.text = title;
	}
}

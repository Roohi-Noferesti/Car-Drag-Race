using UnityEngine;

public class NarrativePopup : PopUpDialogue
{
    private int m_dialogIndex=-1;
    private string m_animationName;

    public Animator Animator;

    public string[] Dialogs;

    public string[] AnimationNames;

    public string SoundName;

    protected override void Start()
    {
        base.Start();
        if (!string.IsNullOrEmpty(SoundName))
        {
            AudioManager.Instance.PlaySound(SoundName, Camera.main.gameObject);
        }
    }

    public void OnNextDialog()
    {
        AudioManager.Instance.StopSound(SoundName, Camera.main.gameObject);
        m_dialogIndex++;
        if (m_dialogIndex > -1 && m_dialogIndex < Dialogs.Length)
        {
            BodyText.text = Dialogs[m_dialogIndex];
            PlayAnimationIfExist();
        }
        else
        {
            this.DoButtonAction(this.popup.ConfirmAction, true);
        }
    }

    private void PlayAnimationIfExist()
    {
        if (Animator == null)
            return;
        if (m_dialogIndex > -1 && m_dialogIndex < AnimationNames.Length )
        {
            if (AnimationNames[m_dialogIndex] != m_animationName)
            {
                m_animationName = AnimationNames[m_dialogIndex];
                Animator.Play(m_animationName);
            }
        }
    }
}

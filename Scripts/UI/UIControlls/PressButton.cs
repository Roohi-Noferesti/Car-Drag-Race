using UnityEngine.EventSystems;
using UnityEngine.UI;

//This button Act as pointer down instead off pointer click
public class PressButton : Button
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        //Do nothing
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!this.IsActive() || !this.IsInteractable())
            return;
        this.onClick.Invoke();
    }
}

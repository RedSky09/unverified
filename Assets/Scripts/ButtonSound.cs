using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MenuAudio.Instance != null)
            MenuAudio.Instance.OnButtonHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (MenuAudio.Instance != null)
            MenuAudio.Instance.OnButtonClick();
    }
}
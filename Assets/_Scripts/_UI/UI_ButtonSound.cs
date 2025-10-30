using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ButtonSound : MonoBehaviour, IPointerClickHandler
{
    private string clickSoundName;

    public void Start()
    {
        clickSoundName = "ButtonClick2";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFX(clickSoundName);
    }
}

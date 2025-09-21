using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject GameStartCanvas;
    public GameObject CharacterSelectCanvas;

    public void OnClickAgain()
    {
        if (GameStartCanvas == null || CharacterSelectCanvas == null) return;

        GameStartCanvas.SetActive(false);
        CharacterSelectCanvas.SetActive(true);
    }

    public void OnClickContinue()
    {
        Debug.Log("계속 하기");
    }

    public void OnClickGoBack()
    {
        if (GameStartCanvas == null || MainMenuCanvas == null) return;

        GameStartCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
}

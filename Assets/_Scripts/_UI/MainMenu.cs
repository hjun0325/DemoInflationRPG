using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject GameStartCanvas;

    public void OnClickGameStart()
    {
        if (GameStartCanvas == null || MainMenuCanvas == null) return;

        MainMenuCanvas.SetActive(false);
        GameStartCanvas.SetActive(true);
    }

    public void OnClickGameInformation()
    {
        Debug.Log("���� ����");
    }

    public void OnClickSetting()
    {
        Debug.Log("����");
    }

    public void OnClickEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

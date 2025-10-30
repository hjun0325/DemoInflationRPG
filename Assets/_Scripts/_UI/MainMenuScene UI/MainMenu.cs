using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject GameStartCanvas;
    public GameObject optionsPanel;
    public GameObject blockerPanel;

    private void Start()
    {
        Time.timeScale = 1f;
        SoundManager.Instance.PlayBGM("Main");
    }

    public void OnClickGameStart()
    {
        if (GameStartCanvas == null || MainMenuCanvas == null) return;

        MainMenuCanvas.SetActive(false);
        GameStartCanvas.SetActive(true);
    }
    public void OnClickSetting()
    {
        Debug.Log("¼³Á¤");

        blockerPanel.SetActive(true);
        optionsPanel.SetActive(true);
    }

    public void OnClickEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickBack()
    {
        blockerPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }
}

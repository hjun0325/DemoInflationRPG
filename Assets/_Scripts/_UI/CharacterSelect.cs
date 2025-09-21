using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public GameObject GameStartCanvas;
    public GameObject CharacterSelectCanvas;

    public void OnClickPlayerSelect()
    {
        if (CharacterSelectCanvas == null) return;

        CharacterSelectCanvas.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickGoBack()
    {
        if (GameStartCanvas == null || CharacterSelectCanvas == null) return;

        CharacterSelectCanvas.SetActive(false);
        GameStartCanvas.SetActive(true);
    }
}

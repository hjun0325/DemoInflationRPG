using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject GameStartCanvas;
    public GameObject CharacterSelectCanvas;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        // DataManager�� �����ϰ�, ��ư�� ����Ǿ����� Ȯ��
        if (DataManager.Instance != null && continueButton != null)
        {
            // DataManager�� ����� ���� �����Ͱ� �ִ��� Ȯ��
            if (DataManager.Instance.saveData.currentSessionData != null)
            {
                Debug.Log("123");
                // ���� �����Ͱ� ������ '�̾��ϱ�' ��ư Ȱ��ȭ
                continueButton.interactable = true;
            }
            else
            {
                // ������ ��Ȱ��ȭ (���� �� ���� ȸ������ ����)
                continueButton.interactable = false;
            }
        }
    }

    public void OnClickAgain()
    {
        if (GameStartCanvas == null || CharacterSelectCanvas == null) return;

        GameStartCanvas.SetActive(false);
        CharacterSelectCanvas.SetActive(true);
    }

    public void OnClickContinue()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickGoBack()
    {
        if (GameStartCanvas == null || MainMenuCanvas == null) return;

        GameStartCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
}

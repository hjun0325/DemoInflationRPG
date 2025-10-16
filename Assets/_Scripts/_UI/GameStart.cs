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
        // DataManager가 존재하고, 버튼이 연결되었는지 확인
        if (DataManager.Instance != null && continueButton != null)
        {
            // DataManager에 저장된 세션 데이터가 있는지 확인
            if (DataManager.Instance.saveData.currentSessionData != null)
            {
                Debug.Log("123");
                // 세션 데이터가 있으면 '이어하기' 버튼 활성화
                continueButton.interactable = true;
            }
            else
            {
                // 없으면 비활성화 (누를 수 없게 회색으로 변함)
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

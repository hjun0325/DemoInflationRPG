using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class FieldBoss : MonoBehaviour
{
    [SerializeField] private MonsterData bossData;

    private void Start()
    {
        CheckIfDefeated();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBossClear += OnBossClear;
        }
    }
    
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBossClear -= OnBossClear;
        }
    }

    private void OnBossClear()
    {
        // 전투에서 승리해서 이 신호가 오면, 즉시 사라집니다.
        gameObject.SetActive(false);
    }

    // 이미 잡은 보스라면 맵에서 보이지 않게 비활성화
    private void CheckIfDefeated()
    {
        var session = DataManager.Instance.saveData.currentSessionData;
        
        // 세션 데이터가 있고, isBossDefeated가 true라면
        if (session != null && session.isBossDefeated)
        {
            gameObject.SetActive(false); // 보스 오브젝트 끄기
        }
    }

    // 2D 충돌 감지
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log($"[{bossData.monsterName}] 보스와 조우했습니다!");

            BattleManager.Instance.StartBattle(bossData);
            GameManager.Instance.ChangeGameState(GameState.Battle);
        }
    }
}

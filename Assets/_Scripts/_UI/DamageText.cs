using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DamageText : MonoBehaviour
{
    private TMP_Text text;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show(int damage, Vector3 position)
    {
        // 초기 상태 설정
        text.text =  damage.ToString();
        transform.position = position;
        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one;

        // DOTween 시퀀스 생성
        Sequence sequence = DOTween.Sequence();
        // 애니메이션 (0.3초 동안 위로 50픽셀 이동)
        sequence.Append(
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 50f, 0.3f).SetEase(Ease.OutQuad));
        // 0.2초 동안 살짝 커졌다가 돌아오기
        sequence.Join(rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f));
        // 0.5초의 딜레이 후, 0.3초 동안 사라지게(Fade Out) 함
        sequence.AppendInterval(0.1f);
        sequence.Append(canvasGroup.DOFade(0f, 0.1f));

        // Time.timeScale=0 (전투 중)에도 작동하도록 설정
        sequence.SetUpdate(true);

        // 애니메이션이 모두 끝나면, 풀(Pool)에 반환
        sequence.OnComplete(() =>
        {
            // EffectManager에 자신을 반환해달라고 요청
            EffectManager.Instance.ReturnToPool(gameObject, "DamageText");
        });
    }
}

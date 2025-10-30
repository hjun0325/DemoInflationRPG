using UnityEngine;

public class EffectAutoReturn : MonoBehaviour
{
    private string effectName;

    public void Setup(string name)
    {
        effectName = name;
    }

    public void OnAnimationFinished()
    {
        // 이름표가 정상적으로 설정되었다면
        if (!string.IsNullOrEmpty(effectName))
        {
            // EffectManager에게 나(gameObject)를 effectName 풀에 반환해달라고 요청
            EffectManager.Instance.ReturnToPool(gameObject, effectName);
        }
        else
        {
            // 혹시라도 이름표 설정이 누락된 경우, 풀링을 포기하고 그냥 파괴한다. (안전장치)
            Debug.LogWarning("이펙트 이름이 설정되지 않아 풀에 반환할 수 없습니다. 오브젝트를 파괴합니다.");
            Destroy(gameObject);
        }
    }
}

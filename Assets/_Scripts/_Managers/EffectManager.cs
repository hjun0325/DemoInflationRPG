using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    [SerializeField] private EffectDatabase effectDB;

    private Dictionary<string, Queue<GameObject>> effectPool 
        = new Dictionary<string, Queue<GameObject>>();
    private Transform poolParent;

    private void Awake()
    {
        Instance = this;
        poolParent = new GameObject("EffectPool").transform;
        poolParent.SetParent(this.transform);
        effectPool["DamageText"] = new Queue<GameObject>();
    }

    // 지정된 위치에 이펙트를 재생
    public void PlayEffect(string name, Vector3 position, Transform parent)
    {
        // 풀에서 사용가능한 이펙트를 가져온다.
        GameObject effect = GetFromPool(name);
        if (effect == null) return;

        effect.transform.SetParent(parent);

        RectTransform effectRect = effect.GetComponent<RectTransform>();
        effectRect.localScale = Vector3.one;
        effectRect.localRotation = Quaternion.identity;
        effectRect.anchorMin = new Vector2(0, 0);
        effectRect.anchorMax = new Vector2(1, 1);
        effectRect.pivot = new Vector2(0.5f, 0.5f);
        effectRect.offsetMin = Vector2.zero; // left, bottom
        effectRect.offsetMax = Vector2.zero; // right, top
        effect.SetActive(true);

        EffectAutoReturn autoReturn = effect.GetComponent<EffectAutoReturn>();
        if (autoReturn != null)
        {
            autoReturn.Setup(name);
        }
        else
        {
            Debug.LogWarning($"이펙트 '{name}' 프리팹에 EffectAutoReturn 스크립트가 없습니다!");
            // 비상시 3초 뒤에 강제 파괴 (풀링 실패)
            Destroy(effect, 3f);
        }
    }

    public void ShowDamageText(string effectName, int damage, Transform parent)
    {
        // "DamageText"라는 이름으로 DB에 등록된 프리팹을 풀에서 가져온다.
        GameObject textGO = GetFromPool(effectName);
        if (textGO == null) return;

        textGO.transform.SetParent(parent);
        textGO.transform.localScale = Vector3.one;
        textGO.SetActive(true);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchoredPosition = Vector2.zero; // 부모 패널의 (0,0) 위치에 고정
        }

        // DamageText 스크립트를 찾아 Show 함수를 호출
        DamageText textScript = textGO.GetComponent<DamageText>();
        if (textScript != null)
        {
            textScript.Show(damage, effectName);
        }
        else
        {
            // DamageText 스크립트가 프리팹에 없는 경우,
            // 즉시 풀에 반환하여 무한 생성을 방지
            ReturnToPool(textGO, effectName);
        }
    }

    // 풀에서 이펙트 오브젝트 가져오기
    private GameObject GetFromPool(string name)
    {
        if (!effectPool.ContainsKey(name))
        {
            effectPool[name] = new Queue<GameObject>();
        }
        Queue<GameObject> pool = effectPool[name];
        GameObject effectToSpawn;

        if (pool.Count > 0 )
        {
            // 풀에 여분이 있으면, 비활성화된 오브젝트를 꺼내서 반환
            return pool.Dequeue();
        }
        else
        {
            GameObject prefab = effectDB.GetEffect(name);
            if (prefab != null)
            {
                effectToSpawn = Instantiate(prefab);
            }
            else
            {
                return null;
            }
        }

        effectToSpawn.transform.SetParent(null); // 부모를 초기화하고 반환
        return effectToSpawn;
    }

    // 이펙트를 풀에 반환하는 함수
    public void ReturnToPool(GameObject effect, string name)
    {
        if (effect == null || string.IsNullOrEmpty(name) || !effectPool.ContainsKey(name))
        {
            if (effect != null) Destroy(effect);
            return;
        }

        effect.SetActive(false);
        effect.transform.SetParent(poolParent); // 풀로 다시 이동
        effectPool[name].Enqueue(effect);
    }
}

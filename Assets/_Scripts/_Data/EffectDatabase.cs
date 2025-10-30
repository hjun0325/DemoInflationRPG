using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EffectClip
{
    public string name;
    public GameObject prefab; // 이펙트 프리팹
}

[CreateAssetMenu(fileName = "EffectDatabase", menuName = "ScriptableObjects/EffectDatabase")]
public class EffectDatabase : ScriptableObject
{
    public List<EffectClip> effects;

    public GameObject GetEffect(string name)
    {
        EffectClip clip = effects.Find(e => e.name == name);
        if (clip == null)
        {
            Debug.LogWarning($"EffectDatabase에서 {name} 이펙트를 찾을 수 없습니다.");
            return null;
        }
        return clip.prefab;
    }
}

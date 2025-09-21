using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ¾ÀÀÌ ¹Ù²î¾îµµ ÆÄ±«µÇÁö ¾ÊÀ½.
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

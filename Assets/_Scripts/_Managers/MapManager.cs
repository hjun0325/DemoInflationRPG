using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� ����.
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

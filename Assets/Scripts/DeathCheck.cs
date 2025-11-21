using UnityEngine;

public class DeathCheck : MonoBehaviour
{
    public static DeathCheck Instance;

    public Transform checkPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (checkPoint == null)
            {
                collision.transform.position = Vector3.zero;
            }
            else
            {
                // 체크포인트가 있으면 그 위치로 이동
                collision.transform.position = checkPoint.position;
            }
        }
    }
}

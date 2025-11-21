using UnityEngine;

public class Bubble : MonoBehaviour
{
    [Header("Bubble")]
    [SerializeField] float bubbleDetroyTime = 5.0f;
    [SerializeField] float explosionRadius = 2.0f;   // 넉백이 닿는 반경
    [SerializeField] float explosionForce = 10.0f;   // 밀어낼 힘

    float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer>bubbleDetroyTime)
        {
           Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Untagged")) Destroy(gameObject);

        if (collision.CompareTag("Niddle")) Explode();
    }

    

    void Explode()
    {
        // 1. 버블 중심 기준으로 플레이어 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player"))
                continue;

            Rigidbody2D rb = hit.attachedRigidbody;

            if (rb != null)
            {
                // 방향 계산
                Vector2 dir = (rb.transform.position - transform.position).normalized;

                // 넉백 (공기 폭발 느낌)
                rb.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

}

 
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [Header("Bubble")]
    [SerializeField] float bubbleDetroyTime = 5.0f;
    [SerializeField] float explosionRadius = 2.0f;   // 넉백이 닿는 반경
    [SerializeField] float explosionForce = 10.0f;   // 밀어낼 힘
    bool isMonsterIn;

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

        if (collision.CompareTag("Untagged"))
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if(collision.CompareTag("Monster"))
        {
            //버블 멈추고
            var bubbleRb = GetComponent<Rigidbody2D>();
            bubbleRb.linearVelocity = Vector2.zero;
            bubbleRb.angularVelocity = 0f;
            bubbleRb.constraints = RigidbodyConstraints2D.FreezeAll;

            BubbleMonster(collision.transform);
            collision.GetComponent<MonsterBase>().TrappedInBubble();
        }

        if (collision.CompareTag("Niddle")) Explode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Untagged"))
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
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

    void BubbleMonster(Transform Monster)
    {
        var rb = Monster.GetComponent<Rigidbody2D>();
        var col = Monster.GetComponent<Collider2D>();

        // 물리 영향 제거
        rb.gravityScale = 0;

        // 충돌 영향 제거
        col.isTrigger = true;

        // 1) 부모 설정 전 "월드 스케일" 저장
        Vector3 worldScale = Monster.lossyScale;

        // 2) 부모 설정 (월드좌표 유지 X)
        Monster.SetParent(transform, false);

        Monster.localPosition = Vector3.zero;

        // 3) 월드 스케일이 그대로 보이도록 localScale 보정
        Vector3 parentScale = transform.lossyScale;
        Monster.localScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );

        var monsterSR = Monster.GetComponent<SpriteRenderer>();
        var bubbleSR = GetComponent<SpriteRenderer>();

        if (monsterSR != null && bubbleSR != null)
        {
            // 몬스터를 버블보다 한 단계 뒤로
            monsterSR.sortingLayerID = bubbleSR.sortingLayerID;
            monsterSR.sortingOrder = bubbleSR.sortingOrder - 1;
        }

        isMonsterIn = true;
    }
}

 
using UnityEngine;

public class NormalMonster : MonsterBase
{
    [Header("Patrol Settings")]
    [SerializeField] float moveDistance = 3f;    // 좌우 이동 범위
    [SerializeField] float waitTime = 0.5f;      // 끝점에서 멈추는 시간

    private Vector2 startPos;
    private int direction = 1;                   // 1 = 오른쪽, -1 = 왼쪽
    private float waitTimer = 0f;

    private void Awake()
    {
        base.Awake();

        startPos = transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MonsterMove();
    }

    protected override void MonsterMove()
    {
        // 끝점에서 잠시 멈추기
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 이동
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // 이동 범위 체크
        float distanceFromStart = transform.position.x - startPos.x;

        if (direction == 1 && distanceFromStart >= moveDistance)
        {
            direction = -1;
            waitTimer = waitTime;
        }
        else if (direction == -1 && distanceFromStart <= -moveDistance)
        {
            direction = 1;
            waitTimer = waitTime;
        }
    }
}

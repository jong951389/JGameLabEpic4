using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] GameObject bubblePrefab;
    [SerializeField] GameObject niddlePrefab;
    [SerializeField] float bubbleSpeed = 300f;
    [SerializeField] float bubbleCoolTIme = 1.0f;
    [SerializeField] float bubbleCost = 25f;
    [SerializeField] float bubbleFootSpawnOffset = 0.8f;
    [SerializeField] float bubbleMaxGuage = 100.0f;
    [SerializeField] float bubbleCurrentGuage = 100.0f;
    [SerializeField] GameObject bubbleGuageImage;
    [SerializeField] float niddleCoolTime = 0.5f;
    [SerializeField] float niddleSpeed = 1000.0f;

    [Header("Bubble Gun")]
    [SerializeField] Transform bubbleGun;   // 플레이어 주변 원을 따라 도는 버블건

    [SerializeField] float bubbleGunRadius = 1.0f;

    [SerializeField] float jumpDoubleTapTime = 0.25f;

    PlayerInputActions actions;
    GameObject bubble;
    Rigidbody2D bubbleRb;

    float lastBubbleTime = -999f;
    float lastNiddleTime = -999f;

    float lastJumpTapTime = -999f; // 마지막 점프 입력 시간

    private void Awake()
    {
        actions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        actions.Enable();
        actions.AttackActions.Bubble.performed += FireBubble;
        actions.AttackActions.Bubble.canceled += StopBubble;
        actions.AttackActions.Niddle.performed += FireNiddle;

        // 🔥 점프(스페이스) 입력 감지 - 더블탭용
        actions.MoveActions.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        actions.AttackActions.Bubble.performed -= FireBubble;
        actions.AttackActions.Bubble.canceled -= StopBubble;
        actions.AttackActions.Niddle.performed -= FireNiddle;

        actions.MoveActions.Jump.performed -= OnJumpPerformed;
        actions.Disable();
    }

    private void Update()
    {
       
        if (bubbleCurrentGuage < bubbleMaxGuage)
        {
            bubbleCurrentGuage += 10 * Time.deltaTime;
        }

        float ratio = bubbleCurrentGuage / bubbleMaxGuage;
        float snapped = Mathf.Floor(ratio * 4) / 4f;
        bubbleGuageImage.GetComponent<Image>().fillAmount = snapped;

       
        if (bubbleGun != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;

            Vector2 dir = (mousePos - transform.position).normalized;

            // 원 테두리 위치로 이동
            bubbleGun.position = transform.position + (Vector3)(dir * bubbleGunRadius);

            // 총구가 마우스를 바라보게 회전(2D 기준)
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bubbleGun.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

   
    void FireBubble(InputAction.CallbackContext ctx)
    {
        if (Time.time < lastBubbleTime + bubbleCoolTIme)
            return;

        if (bubbleCurrentGuage < bubbleCost)
            return;

        bubbleCurrentGuage -= bubbleCost;
        lastBubbleTime = Time.time;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        Vector2 dir = (mousePos - transform.position).normalized;

        Vector3 spawnPos =
            bubbleGun != null ? bubbleGun.position :
            transform.position;

        SpawnBubble(spawnPos, dir, false);
    }

    void StopBubble(InputAction.CallbackContext ctx)
    {
        if (bubbleRb == null || bubble == null)
            return;
        if (!bubbleRb) return;

        bubbleRb.linearVelocity = Vector2.zero;
        bubbleRb.constraints = RigidbodyConstraints2D.FreezeAll;

        var col = bubble.GetComponent<CircleCollider2D>();
        if (col != null)
            col.isTrigger = false;

        bubbleRb = null;
        bubble = null;
    }


    void FireNiddle(InputAction.CallbackContext ctx)
    {
        if (Time.time < lastNiddleTime + niddleCoolTime)
            return;

        lastNiddleTime = Time.time;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        Vector2 dir = (mousePos - transform.position).normalized;

        // 🔥 버블건 기준 스폰 위치
        Vector3 spawnPos =
            bubbleGun != null ? bubbleGun.position :
            transform.position;

        // 니들 생성 (버블건 위치에서)
        GameObject niddle = Instantiate(niddlePrefab, spawnPos, Quaternion.identity);

        // 니들이 마우스를 바라보게 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        niddle.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 발사
        Rigidbody2D rb = niddle.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(dir * niddleSpeed);
    }


    void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        float now = Time.time;

        if (now - lastJumpTapTime <= jumpDoubleTapTime)
        {
            FireFootBubble();
            lastJumpTapTime = -999f;
        }
        else
        {
            lastJumpTapTime = now;
        }
    }

    void FireFootBubble()
    {
        if (Time.time < lastBubbleTime + bubbleCoolTIme)
            return;

        if (bubbleCurrentGuage < bubbleCost)
            return;

        bubbleCurrentGuage -= bubbleCost;
        lastBubbleTime = Time.time;

        Vector2 dir = Vector2.down;

        // 발 아래 원 테두리 기준 위치
        Vector3 spawnPos = transform.position + (Vector3)(dir * bubbleGunRadius);

        SpawnBubble(spawnPos, dir, true);
    }

  
    void SpawnBubble(Vector3 spawnPos, Vector2 dir, bool isFootBubble)
    {
        // 발 아래 버블은 살짝 더 아래로 보정
        Vector2 finalSpawnPos = isFootBubble
            ? new Vector2(spawnPos.x, spawnPos.y - bubbleFootSpawnOffset)
            : (Vector2)spawnPos;

        bubble = Instantiate(bubblePrefab, finalSpawnPos, Quaternion.identity);
        bubbleRb = bubble.GetComponent<Rigidbody2D>();

        if (bubbleRb == null) return;

        bubbleRb.gravityScale = 0f;

        if (!isFootBubble)
        {
            bubbleRb.linearVelocity = dir * bubbleSpeed;
        }

        if (isFootBubble)
        {
            bubbleRb.constraints = RigidbodyConstraints2D.FreezeAll;
            var col = bubble.GetComponent<CircleCollider2D>();
            if (col != null) col.isTrigger = false;
        }
    }
}

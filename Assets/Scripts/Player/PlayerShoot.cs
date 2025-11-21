using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] GameObject bubblePrefab;
    [SerializeField] GameObject niddlePrefab;
    [SerializeField] float bubbleCoolTIme = 1.0f;
    [SerializeField] float bubbleSpeed = 300.0f;
    [SerializeField] float bubbleMaxGuage = 100.0f;
    [SerializeField] float bubbleCurrentGuage = 100.0f;
    [SerializeField] GameObject bubbleGuageImage;
    [SerializeField] float niddleCoolTime = 0.5f;
    [SerializeField] float niddleSpeed = 1000.0f;

    // 🔥 플레이어 주변 원 반지름
    [SerializeField] float bubbleSpawnRadius = 1.0f;

    PlayerInputActions actions;
    GameObject bubble;
    Rigidbody2D bubbleRb;

    float lastBubbleTime = -999f;
    float lastNiddleTime = -999f;

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
    }

    private void OnDisable()
    {
        actions.AttackActions.Bubble.performed -= FireBubble;
        actions.AttackActions.Bubble.canceled -= StopBubble;
        actions.AttackActions.Niddle.performed -= FireNiddle;
        actions.Disable();
    }

    private void Update()
    {
        // 게이지 회복
        if (bubbleCurrentGuage < bubbleMaxGuage)
        {
            bubbleCurrentGuage += 10 * Time.deltaTime;
        }

        // 4단계 스냅 게이지
        float ratio = bubbleCurrentGuage / bubbleMaxGuage;
        float snapped = Mathf.Floor(ratio * 4) / 4f;
        bubbleGuageImage.GetComponent<Image>().fillAmount = snapped;
    }


    void FireBubble(InputAction.CallbackContext ctx)
    {
        if (Time.time < lastBubbleTime + bubbleCoolTIme)
            return;

        if (bubbleCurrentGuage < 25)
            return;

        bubbleCurrentGuage -= 25;
        lastBubbleTime = Time.time;

        // 마우스 좌표
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        // 플레이어 → 마우스 방향
        Vector2 dir = (mousePos - transform.position).normalized;

        Vector3 spawnPos = transform.position + (Vector3)(dir * bubbleSpawnRadius);

        // 버블 생성
        bubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
        bubbleRb = bubble.GetComponent<Rigidbody2D>();

        if (bubbleRb == null) return;

        bubbleRb.gravityScale = 0f;
        bubbleRb.linearVelocity = dir * bubbleSpeed;
    }


    void StopBubble(InputAction.CallbackContext ctx)
    {
        // 버블이 존재할 때만 동작
        if (bubbleRb == null || bubble == null)
            return;
        if (!bubbleRb) return;

        bubbleRb.linearVelocity = Vector2.zero;
        bubbleRb.constraints = RigidbodyConstraints2D.FreezeAll;

        var col = bubble.GetComponent<CircleCollider2D>();
        if (col != null)
            col.isTrigger = false;

        // 참조 정리
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

        GameObject niddle = Instantiate(niddlePrefab, transform.position, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        niddle.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = niddle.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(dir * niddleSpeed);
    }
}

using UnityEngine;

public abstract class MonsterBase : MonoBehaviour, IMonster
{
    [Header("MonsterStatus")]
    [SerializeField] protected int maxHP;
    [SerializeField] protected float moveSpeed;

    protected float currentHp;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        currentHp = maxHP;
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected abstract void MonsterMove();

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0) MonsterDeath();
    }

    public virtual void MonsterDeath()
    {
        //몬스터 죽음 처리
        Destroy(gameObject);
    }

   public virtual void TrappedInBubble()
    {
        //몬스터 버블 갇힘 처리
        moveSpeed = 0;
    }

}

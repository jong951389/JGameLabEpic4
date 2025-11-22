using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance;

    [Header("PlayerStatus")]
    public int maxHP = 3;
    public int currentHp;
    public int Atk = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentHp = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0) PlayerDeath();
    }

    public void PlayerDeath()
    {
        //플레이어 죽음 처리
    }
}

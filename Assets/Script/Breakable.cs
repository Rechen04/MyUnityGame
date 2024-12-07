using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float health = 100f; // 生命值
    private PlayerStatus playerStatus; // 引用 PlayerStatus

    private void Start()
    {
        playerStatus = FindObjectOfType<PlayerStatus>(); // 查找 PlayerStatus 实例
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount; // 减少生命值

        if (health <= 0)
        {
            Break(); // 处理破坏逻辑
        }
    }

    private void Break()
    {
        playerStatus?.OnBreakableDestroyed(); // 通知 PlayerStatus 增加矿石
        Destroy(gameObject); // 销毁当前对象
    }
}
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float health = 100f; // ����ֵ
    private PlayerStatus playerStatus; // ���� PlayerStatus

    private void Start()
    {
        playerStatus = FindObjectOfType<PlayerStatus>(); // ���� PlayerStatus ʵ��
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount; // ��������ֵ

        if (health <= 0)
        {
            Break(); // �����ƻ��߼�
        }
    }

    private void Break()
    {
        playerStatus?.OnBreakableDestroyed(); // ֪ͨ PlayerStatus ���ӿ�ʯ
        Destroy(gameObject); // ���ٵ�ǰ����
    }
}
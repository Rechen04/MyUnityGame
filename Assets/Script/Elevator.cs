using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float upHeight = 5f; // �߶������ƶ�
    public float downHeight = 0f; // �߶������ƶ�
    public float speed = 2f; // �����ٶ�

    private bool isDown = true; // ���ݳ�ʼ״̬
    private bool isMoving = false; // �����Ƿ������ƶ�
    private Transform player; // ��Ҷ��������

    private void Update()
    {
        if (isMoving || player == null) return; // �ƶ��л�û�����ʱ�˳�

        // �ƶ�����
        if (isDown)
        {
            StartCoroutine(MoveElevator(upHeight));
        }
        else
        {
            StartCoroutine(MoveElevator(downHeight));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �������Ƿ����������
        Debug.Log("enter!");
        if (other.CompareTag("Player") && player == null)
        {
            player = other.transform; // �����������
            player.SetParent(transform); // ���������Ϊ���ݵ�������
            Debug.Log("player entered!");
            Update(); // ���������ƶ�
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �������Ƿ��˳��������
        Debug.Log("exit!");
        if (other.CompareTag("Player") && player != null)
        {
            player.SetParent(null); // ȡ����ҵ�������״̬
            player = null; // �Ƴ��������
            Debug.Log("player exit!");
        }
    }

    private System.Collections.IEnumerator MoveElevator(float targetHeight)
    {
        isMoving = true;
        Vector3 targetPosition = new Vector3(transform.position.x, targetHeight, transform.position.z);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; // �ȴ���һ֡
        }

        transform.position = targetPosition; // ȷ������λ��׼ȷ

        isDown = !isDown; // �л�����״̬
        if(player != null)
        {
            player.SetParent(null);
            player = null;
        }
        isMoving = false; // �ƶ����
    }

    public void ReceiveMessage(string message)
    {
        if (!isMoving)
        {
            if (message == "Up" && isDown)
            {
                StartCoroutine(MoveElevator(upHeight));
            }
            else if (message == "Down" && !isDown)
            {
                StartCoroutine(MoveElevator(downHeight));
            }
        }
    }

}
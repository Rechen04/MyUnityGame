using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float upHeight = 5f; // 高度向上移动
    public float downHeight = 0f; // 高度向下移动
    public float speed = 2f; // 电梯速度

    private bool isDown = true; // 电梯初始状态
    private bool isMoving = false; // 电梯是否正在移动
    private Transform player; // 玩家对象的引用

    private void Update()
    {
        if (isMoving || player == null) return; // 移动中或没有玩家时退出

        // 移动电梯
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
        // 检查玩家是否进入检测区域
        Debug.Log("enter!");
        if (other.CompareTag("Player") && player == null)
        {
            player = other.transform; // 设置玩家引用
            player.SetParent(transform); // 将玩家设置为电梯的子物体
            Debug.Log("player entered!");
            Update(); // 触发电梯移动
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 检查玩家是否退出检测区域
        Debug.Log("exit!");
        if (other.CompareTag("Player") && player != null)
        {
            player.SetParent(null); // 取消玩家的子物体状态
            player = null; // 移除玩家引用
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
            yield return null; // 等待下一帧
        }

        transform.position = targetPosition; // 确保最终位置准确

        isDown = !isDown; // 切换电梯状态
        if(player != null)
        {
            player.SetParent(null);
            player = null;
        }
        isMoving = false; // 移动完成
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
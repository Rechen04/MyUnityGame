using System;
using UnityEngine;

public class AttackDetection : MonoBehaviour
{
    private void Start()
    {
        this.anim = base.GetComponent<Animator>();
    }

    private void Update()
    {
    }

    public void AttackPauseFrame()
    {
        this.anim.speed = 0f;
        this.isHaveAttackTarget = true;
        base.Invoke("RestoreAnimPlaySpeed", this.pauseTime);
    }

    public void RestoreAnimPlaySpeed()
    {
        this.anim.speed = 1f;
    }

    public void FanAttackDetection(string param)
    {
        /*        if (this.isShockscreen)
                {
                    Shockscreen.instance.ShockscreenMethod();
                }*/
        //Debug.Log("Atk!");
        string[] array = param.Split('#', StringSplitOptions.None);
        this.attackRadius = float.Parse(array[0]);
        this.attackAngle = float.Parse(array[1]);
        foreach (Collider collider in Physics.OverlapSphere(base.transform.position, this.attackRadius, this.targetLayer))
        {
            if (Vector3.Angle(collider.transform.position - base.transform.position, base.transform.forward) < this.attackAngle * 0.5f)
            {
                Debug.Log("攻击目标是：" + collider.gameObject.name);
                this.AttackPauseFrame();
                collider.gameObject.GetComponent<CharacterBasicSystem>().BeAttack(this.attackValue, base.transform);
            }
        }
    }

    public void BoxAttackDetection(string param)
    {
/*        if (this.isShockscreen)
        {
            Shockscreen.instance.ShockscreenMethod();
        }*/
        string[] array = param.Split('#', StringSplitOptions.None);
        float x = float.Parse(array[0]);
        float y = float.Parse(array[1]);
        float z = float.Parse(array[2]);
        this.boxParam = new Vector3(x, y, z);
        foreach (Collider collider in Physics.OverlapBox(base.transform.position + base.transform.rotation * Vector3.forward * this.boxParam.z * 0.5f, this.boxParam / 2f, base.transform.rotation, this.targetLayer))
        {
            Debug.Log("攻击目标是：" + collider.gameObject.name);
            this.AttackPauseFrame();
            collider.gameObject.GetComponent<CharacterBasicSystem>().BeAttack(10f, base.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (this.isShowBox)
        {
            Vector3 position = base.transform.position;
            position.z += this.boxParam.z / 2f;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(position, this.boxParam);
        }
        if (this.isShowFan)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(base.transform.position, this.attackRadius);
            Quaternion rotation = Quaternion.AngleAxis(-this.attackAngle * 0.5f, base.transform.up);
            Quaternion rotation2 = Quaternion.AngleAxis(this.attackAngle * 0.5f, base.transform.up);
            Vector3 a = rotation * base.transform.forward;
            Vector3 a2 = rotation2 * base.transform.forward;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(base.transform.position, base.transform.position + a * this.attackRadius);
            Gizmos.DrawLine(base.transform.position, base.transform.position + a2 * this.attackRadius);
        }
    }

    public float attackRadius = 5f;
    public float attackAngle = 45f;
    public Vector3 boxParam;
    public bool isShowBox;
    public bool isShowFan;

    [Header("攻击检测层级")]
    public LayerMask targetLayer;

    private Animator anim;

    [Header("攻击定帧时间")]
    public float pauseTime = 0.1f;

    [Space(20f)]
    public bool isShockscreen = true;

    [Header("攻击力")]
    public float attackValue = 10f;

    [Header("是否有攻击目标")]
    public bool isHaveAttackTarget;
}
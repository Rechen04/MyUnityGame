using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponController : MonoBehaviour
{
    public WeaponType weapontype;
    public GameObject VfxPrefab;
    public Transform VfxPosition;
    public float attackRadius = 5f;
    public float attackAngle = 45f;
    public LayerMask targetLayer;
    public float attackValue = 10f;
    public bool isShowBox;
    public bool isShowFan;

    private void OnCollisionEnter(Collision collision)
    {
        if (this.weapontype == WeaponType.Sword && collision.gameObject.CompareTag("Enemy"))
        {// 当首次碰撞时调用
            Debug.Log("Collided with: " + collision.gameObject.name);
            collision.gameObject.GetComponent<CharacterBasicSystem>().BeAttack(this.attackValue, base.transform);
        }
    }



    public void Shoot()
    {
        this.FanAttackDetection();
        this.VfxGenerator();
    }

    public void FanAttackDetection()
    {
        foreach (Collider collider in Physics.OverlapSphere(base.transform.position, this.attackRadius, this.targetLayer))
        {
            if (Vector3.Angle(collider.transform.position - base.transform.position, base.transform.forward) < this.attackAngle * 0.5f)
            {
                Debug.Log("攻击目标是：" + collider.gameObject.name);
                collider.gameObject.GetComponent<CharacterBasicSystem>().BeAttack(this.attackValue, base.transform);
            }
        }
    }

    public void VfxGenerator()
    {
        if (VfxPrefab != null && VfxPosition != null)
        {
            GameObject VfxInstance = Instantiate(VfxPrefab, VfxPosition.position, VfxPosition.rotation);
            Destroy(VfxInstance, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
/*        if (this.isShowBox)
        {
            Vector3 position = base.transform.position;
            position.z += this.boxParam.z / 2f;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(position, this.boxParam);
        }*/
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
}

public enum WeaponType
{
    Gun,
    Sword
}
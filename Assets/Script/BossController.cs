using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    private void Start()
    {
        this._audio = base.GetComponent<AudioSource>();
        this._navMeshAgent = base.GetComponent<NavMeshAgent>();
        this._navMeshAgent.speed = this._moveSpeed;
        this._anim = base.GetComponent<Animator>();
        this._CBY = base.GetComponent<CharacterBasicSystem>();
        this._collider = base.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        this._animInfo = this._anim.GetCurrentAnimatorStateInfo(0);
        if (this._CBY.PercentageHealth() <= 0f)
        {
            this._currentState = BossController.BossState.Dead;
        }
        if (!this._animInfo.IsName("Intialize"))
        {
            this.BasicState();
        }
    }

    public void BasicState()
    {
            switch (this._currentState)
            {
                case BossController.BossState.Idle:
                    this.IdleState();
                    return;
                case BossController.BossState.Attack:
                    this.AttackState();
                    return;
			    case BossController.BossState.Standoff:
				    this.StandoffState();
                    return;
                case BossController.BossState.Chase:
                    this.ChaseState();
                    return;
                case BossController.BossState.Dead:
                    this.DeadState();
                    break;
                default:
                    return;
            }
    }

    public void IdleState()
    {
        if (!this._animInfo.IsName("Idle Walk Run Blend"))
        {
            this._anim.Play("Idle Walk Run Blend", 0 , 1f);
        }
        if (this.DistanceFromPlayer() <= this._searchRadius && this._playerPos != null)
        {
            if (this.DistanceFromPlayer() <= 8f)
            {
                Debug.Log("Distance < 8f");
                int num = UnityEngine.Random.Range(0, 2);
                if(num == 0)
                {
                    this._currentState = BossController.BossState.Attack;
                    return;
                }
                if(num == 1)
                {
                    this._currentState = BossController.BossState.Standoff;
                    return;
                }
            }
            if (this.DistanceFromPlayer() > 8f)
            {
                Debug.Log("Distance > 8f");
                this._currentState = BossController.BossState.Chase;
            }
        }
    }

    public void AttackState()
    {
        if (this._animInfo.IsName("Idle Walk Run Blend"))
        {
            this._isAttacking = true;
            if (this.DistanceFromPlayer() < 4f)
            {
                int num = UnityEngine.Random.Range(0, 3);
                if (num == 0)
                {
                    this._anim.CrossFade(this._attack01, 0f);
                    //this.AttackSound(0);
                }
                else if (num == 1)
                {
                    this._anim.CrossFade(this._attack02, 0f);
                    //this.AttackSound(1);
                }
                else if (num == 2)
                {
                    this._anim.CrossFade(this._attack03, 0f);
                    //this.AttackSound(2);
                }
            }
            else if (this.DistanceFromPlayer() >= 4f)
            {
                int num1 = UnityEngine.Random.Range(0, 4);
                if (num1 == 0)
                {
                    this._anim.CrossFade(this._skill01, 0f);
                    //this.AttackSound();
                }
                else if (num1 == 1)
                {
                    this._anim.CrossFade(this._skill02, 0f);
                    //this.AttackSound();
                }
                else if (num1 == 2)
                {
                    this._anim.CrossFade(this._skill03, 0f);
                    //this.AttackSound();
                }
                else if (num1 == 3)
                {
                    this._anim.CrossFade(this._skill04, 0f);
                    //this.AttackSound();
                }
            }
        }
        if (!this._isAttacking)
        {
            this._currentState = BossController.BossState.Idle;
        }
    }

    public void StandoffState()
    {
        if (!this._animInfo.IsName("Walk_L90") && !this._animInfo.IsName("Walk_R90"))
        {
            int direction = UnityEngine.Random.Range(0, 2);
            float stanoffTime = (float)UnityEngine.Random.Range(2, 4);
            base.StartCoroutine(this.Standoff(direction, stanoffTime));
        }
        if (this.DistanceFromPlayer() < 5f)
        {
            this._isInStandoff = false;
            base.StopAllCoroutines();
        }
        if (!this._isInStandoff)
        {
            this._anim.CrossFade("Idle Walk Run Blend", 0.1f);
            this._currentState = BossController.BossState.Idle;
        }
    }

    public void ChaseState()
    {
        this._navMeshAgent.isStopped = false;
        this._navMeshAgent.SetDestination(this._playerPos.position);
        this._anim.SetFloat("MoveSpeed", this._navMeshAgent.velocity.magnitude);
        Debug.Log("BOSS剩余距离" + this._navMeshAgent.remainingDistance.ToString());
        if (!this._navMeshAgent.pathPending && this._navMeshAgent.remainingDistance < this._navMeshAgent.stoppingDistance && this._navMeshAgent.velocity.magnitude == 0f)
        {
            this._navMeshAgent.isStopped = true;
            this._currentState = BossController.BossState.Idle;
        }
    }

    public void DeadState()
    {
        if (!this._animInfo.IsName("Death"))
        {
            this.BGM.SetActive(false);
            this._collider.enabled = false;
            base.StopAllCoroutines();
            this._anim.CrossFade("Death", 0f);
        }
    }


    public float DistanceFromPlayer()
    {
        return Vector3.Distance(base.transform.position, this._playerPos.position);
    }

    public void AttackStateCancel()
    {
        this._isAttacking = false;
    }

    public void AttackMove(float movetime)
    {
        // 如果已经在移动，先停止之前的协程
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 启动新的移动协程
        moveCoroutine = StartCoroutine(MoveDuringAttack(movetime));
    }

    private IEnumerator MoveDuringAttack(float movetime)
    {
        float elapsedTime = 0f;
        Vector3 moveDirection = (movetime > 0) ? Vector3.forward : Vector3.back;
        float absoluteMovetime = Mathf.Abs(movetime);
        while (elapsedTime < absoluteMovetime)
        {
            // 计算每帧的移动距离
            float moveDistance = _moveSpeed * Time.deltaTime;
            transform.Translate(moveDirection * moveDistance);

            elapsedTime += Time.deltaTime; // 增加已用时间
            yield return null; // 等待下一帧
        }

        // 移动结束后的逻辑（如果需要）
        moveCoroutine = null; // 清空协程引用
    }

    public void AttackSound(int index)
    {
        this._audio.PlayOneShot(this._attackClip[index]);
    }

    public void TowardsTarget()
    {
        Vector3 worldPosition = new Vector3(this._playerPos.position.x, base.transform.position.y, this._playerPos.position.z);
        base.transform.LookAt(worldPosition);
    }

    private IEnumerator Standoff(int direction, float stanoffTime)
    {
        this._isInStandoff = true;
        if (direction == 0)
        {
            this._anim.CrossFade("Walk_L90", 0f);
        }
        else if (direction == 1)
        {
            this._anim.CrossFade("Walk_R90", 0f);
        }
        Vector3 moveDirection = (direction == 0) ? Vector3.left : Vector3.right;
        float onTime = Time.time;
        while (Time.time - onTime < stanoffTime)
        {
            this.TowardsTarget();
            base.transform.Translate(moveDirection * 1f * Time.deltaTime);
            yield return null;
        }
        this._isInStandoff = false;
        yield break;
    }

    public BossController.BossState _currentState;

    [Header("招式技能")]
    public string _attack01;
    public string _attack02;
    public string _attack03;
    public string _skill01;
    public string _skill02;
    public string _skill03;
    public string _skill04;

    [Header("配置")]
    public float _moveSpeed = 6f;
    public float _searchRadius = 100f;


    [Header("狂暴特效")]
    public GameObject _effect;

    [Header("BGM")]
    public GameObject BGM;
    public GameObject otherBGM;

    [Header("攻击音频")]
    public AudioClip[] _attackClip;


    [Space(20f)]
    public Transform _playerPos;
    public GameObject[] _weaponObj;

    private Animator _anim;
    private AnimatorStateInfo _animInfo;
    private AudioSource _audio;
    private NavMeshAgent _navMeshAgent;
    private CharacterBasicSystem _CBY;
    private CapsuleCollider _collider;
    public bool _isAttacking;
    private bool _isInStandoff;

    [HideInInspector]
    private Coroutine moveCoroutine;

    public enum BossState
    {
        Idle,
        Standoff,
        Attack,
        Chase,
        Dead
    }

}
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterBasicSystem : MonoBehaviour
{
    private void Start()
    {
        this.anim = base.GetComponent<Animator>();
        if (this.audio == null)
        {
            this.audio = base.GetComponent<AudioSource>();
        }
        this.selfCollider = base.GetComponent<CapsuleCollider>();
        this.maxHealth = this.healthValue;
        if (this.isShowHealthSlider)
        {
            this.UpdateHealthUIInfo();
        }
    }

    private void Update()
    {
    }

    public float PercentageHealth()
    {
        return this.healthValue / this.maxHealth;
    }

    public void BeAttack(float damage, Transform player)
    {
        if (!this.isDeath)
        {
            this.BeHit(player);
            this.DamageCalculation(damage);
            this.HealthValueDetection();
            if (this.isShowHealthSlider)
            {
                this.UpdateHealthUIInfo();
            }
        }
    }

    public void UpdateHealthUIInfo()
    {
        this.healthSlider.value = this.healthValue / this.maxHealth;
        if (this.healthText != null)
        {
            if (this.healthValue > 0f)
            {
                this.healthText.text = ((int)this.healthValue).ToString() + "/" + this.maxHealth.ToString();
                return;
            }
            this.healthText.text = "0/" + this.maxHealth.ToString();
        }
    }

    private void DamageCalculation(float damage)
    {
        float num = 1f - 1f / (this.defenseValue * 0.05f + 1f);
        num = Mathf.Clamp(num, 0f, 0.9f);
        this.healthValue -= damage * (1f - num);
        Debug.Log(base.gameObject.name + "剩余生命值：" + this.healthValue.ToString());
    }

    private void HealthValueDetection()
    {
        if (this.healthValue <= 0f)
        {
            if (this.isHaveDeathAction)
            {
                this.anim.CrossFade(this.death, 0.1f);
            }
            this.isDeath = true;
            if (this.selfCollider != null)
            {
                this.selfCollider.enabled = false;
            }
            base.Invoke("DestroySelf", this.delayDestroyTime);
        }
    }

    private void DestroySelf()
    {
        UnityEngine.Object.Destroy(gameObject);
    }

    private void BeHit(Transform player)
    {
        Debug.Log(base.gameObject.name + "受击");
        this.isHit = true;
        this.hitEff.Play();
        int num = UnityEngine.Random.Range(0, this.hitClip.Length);
        this.audio.PlayOneShot(this.hitClip[num]);
    }

    private Animator anim;
    public AudioSource audio;
    [Header("UI")]
    public bool isShowHealthSlider;
    public Slider healthSlider;
    public TMP_Text healthText;
    private float maxHealth;
    [Header("是否有死亡动作")]
    public bool isHaveDeathAction = true;
    [Header("受击动作")]
    public string death;
    public AudioClip[] hitClip;
    [Header("受击特效")]
    public ParticleSystem hitEff;
    [Space(10f)]
    [Header("配置")]
    public float healthValue = 100f;
    public float defenseValue = 10f;
    public float delayDestroyTime = 5f;
    [HideInInspector]
    public bool isDeath;
    private CapsuleCollider selfCollider;
    public bool isHit;
}
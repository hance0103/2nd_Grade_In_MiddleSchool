using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObject", menuName = "Scriptable Object/ProjectileScriptableObject", order = int.MaxValue)]

public class ProjectileScriptableObject : ScriptableObject
{
    [Header("투사체 관련 설정")]
    // 데미지
    [SerializeField] private float damage;
    public float Damage { get { return damage; } }
    
    //투사체속도
    [SerializeField] private float projectileSpeed;
    public float ProjectileSpeed { get { return projectileSpeed; } }
    
    //투사체 개수
    [SerializeField] private float projectileCount;
    public float ProjectileCount { get { return projectileCount; } }

    // 발사체 크기
    [SerializeField] private Vector3 projectileScale;
    public Vector3 ProjectileScale { get { return projectileScale; } }

    // 수직 간격
    [SerializeField] private float verticalSpacing = 1f;
    public float VerticalSpacing { get { return verticalSpacing; } }


    [Header("시간 관련 설정")]
    // 발사 속도 (초당 발사체 수)
    [SerializeField] private float fireRate;
    public float FireRate { get { return fireRate; } }

    //// 지속시간
    //[SerializeField] private float patternDuration;
    //public float PatternDuration { get { return patternDuration; } }

    //// 패턴 반복 간격
    //[SerializeField] private float patternRepeatDelay = 0.5f;
    //public float PatternRepeatDelay { get { return patternRepeatDelay; } }

    // 투사체가 화면을 완전히 벗어날 때까지의 예상 시간
    [SerializeField] private float afterFireDelay = 0.5f;
    public float AfterFireDelay { get { return afterFireDelay; } }

    public BossProjectileEffect effect;
}

[Serializable]
public class BossProjectileEffect
{
    public bool effectActive;
    public float time;
    public float maxSpeed;
    public float accel;

    public BossProjectileEffect(float time, float maxSpeed, float accel, bool effectActive)
    {
        this.time = time;
        this.maxSpeed = maxSpeed;
        this.accel = accel;
        this.effectActive = effectActive;
    }
}
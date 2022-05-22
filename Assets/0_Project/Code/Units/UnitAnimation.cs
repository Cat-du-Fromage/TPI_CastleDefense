using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;

public class UnitAnimation : MonoBehaviour
{
    public bool Test = false;
    
    //[SerializeField] private VisualEffect MuzzleFlash;
    [SerializeField] private ParticleSystem MuzzleFlash;
    private Animator animator;

    public float animationsSpeed;
    public float speedIdle;
    public float speed;
    
    public bool shoot;
    public bool aim;

    //trigger
    private int animTriggerIDDeath;

    //int
    private int animIDDeathIndex;
    
    //float
    private int animIDAnimationsSpeed;
    private int animIDSpeed;
    private int animIDIdleSpeed;
    //bool
    private int animIDIsAiming;
    private int animIDIsShooting;

    private void Awake()
    {
        MuzzleFlash = MuzzleFlash == null ? GetComponentInChildren<ParticleSystem>() : MuzzleFlash;
        animator = GetComponent<Animator>();
        AssignAnimationIDs();
        
        if (Test) InitIdleRandom(1);
    }

    public void SetSpeed(float value)
    {
        animator.SetFloat(animIDSpeed, value);
    }

    private void AssignAnimationIDs()
    {
        animTriggerIDDeath = Animator.StringToHash("Death");
        
        animIDDeathIndex = Animator.StringToHash("DeathIndex");
        
        animIDAnimationsSpeed = Animator.StringToHash("AnimationsSpeed");
        animIDSpeed = Animator.StringToHash("Speed");
        
        animIDIsAiming = Animator.StringToHash("IsAiming");
        animIDIsShooting = Animator.StringToHash("IsShooting");
        animIDIdleSpeed = Animator.StringToHash("IdleSpeed");
    }
    public void InitIdleRandom(int index)
    {
        Random rand = Random.CreateFromIndex(min((uint)index,uint.MaxValue-1));
        
        speedIdle = rand.NextFloat(4, 10) / 10f;
        animator.SetFloat(animIDIdleSpeed, speedIdle);
        
        animationsSpeed = rand.NextFloat(6, 10) / 10f;
        animator.SetFloat(animIDAnimationsSpeed, animationsSpeed);
    }
    
    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            
            aim = !aim;
            animator.SetBool(animIDIsAiming, aim);
            
            shoot = !shoot;
            animator.SetBool(animIDIsShooting, shoot);
        }
    }

    private void PlayMuzzleFlash() => MuzzleFlash.Play();
}

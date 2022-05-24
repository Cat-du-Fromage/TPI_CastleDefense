using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KaizerWald
{
    public partial class ShootManager : MonoBehaviour
    {
        public bool debug;
        
        private ShootSystem shootSystem;
        private GameObject bulletPrefab;

        [field:SerializeField] private Regiment regiment;
        [field:SerializeField] public Regiment TargetRegiment { get; private set; }
        [field:SerializeField] public LayerMask HitMask { get; private set; }
        [field:SerializeField] public bool HasTarget { get; private set; }
        [field:SerializeField] public bool IsFiring { get; private set; }

        public int NumShooter;
        
        private JobHandle targetHandle;
        private NativeArray<RaycastHit> results;
        private NativeArray<SpherecastCommand> commands;
        
        [field:SerializeField] public AudioSource ShootSound{ get; private set; }
        private void Awake()
        {
            regiment = GetComponent<Regiment>();
            shootSystem = FindObjectOfType<ShootSystem>();
            bulletPrefab = shootSystem.bullet;

            ShootSound = gameObject.AddComponent<AudioSource>();
            ShootSound.spatialBlend = 1f;
        }

        private void Start()
        {
            HitMask = regiment.IsPlayer ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Player");
        }

        private void Update()
        {
            if (regiment.IsMoving)
            {
                ShootTarget(false);
                HasTarget = false;
            }
            else if (!HasTarget)
            {
                if(IsFiring) ShootTarget(false);
                GetTarget();
            }
            else if(HasTarget)
            {
                if (IsFiring && NumShooter == regiment.CurrentLineFormation) return;
                ShootTarget(true);
            }
        }

        private void LateUpdate()
        {
            if (!results.IsCreated && !commands.IsCreated) return;
            targetHandle.Complete();

            TargetRegiment = IsTargetAcquire();
            
            results.Dispose();
            commands.Dispose();
        }

        public void ShootBullet(Unit unit)
        {
            Transform unitTransform = unit.transform;
            Vector3 unitPosition = unitTransform.position;
            
            int2 xTarget = unit.IndexInRegiment.GetXY2(TargetRegiment.CurrentLineFormation);

            Vector3 unitTargetPosition = TargetRegiment.Units[xTarget.x].transform.position + new Vector3(0, 0.5f, 0);
            
            Vector3 dir = (unitTargetPosition - unitPosition).Flat().normalized;
            dir += Random.insideUnitSphere * 0.1f;
            
            Vector3 pos = unitPosition + Vector3.up + unitTransform.forward;
            
#if UNITY_EDITOR
            if (debug) Debug.DrawRay(pos, dir * 20f, Color.magenta, 3f);
#endif     
            
            GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
            bullet.GetComponent<BulletComponent>().Shoot(pos,dir, HitMask);

            if (ShootSound.isPlaying) return;
            if(transform.position.DistanceTo(regiment.Units[regiment.Units.Length / 2].transform.position) > 5)
                transform.position = regiment.Units[regiment.Units.Length / 2].transform.position;
            ShootSound.PlayOneShot(shootSystem.Audio);
        }

        private void ShootTarget(bool state)
        {
            //int numTarget = TargetRegiment.Units.Length;

            NumShooter = regiment.CurrentLineFormation;
            for (int i = 0; i < regiment.CurrentLineFormation; i++)
            {
                regiment.Units[i].Animation.SetFire(state);
            }
            IsFiring = state;
        }

        private Regiment IsTargetAcquire()
        {
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].transform == null) continue;
                if (!results[i].transform.TryGetComponent(out Unit unit)) continue;
                HasTarget = true;
                return unit.RegimentAttach;
            }
            HasTarget = false;
            return null;
        }

        private void GetTarget()
        {
            results = new (regiment.CurrentLineFormation, Allocator.TempJob);
            commands = new (regiment.CurrentLineFormation, Allocator.TempJob);

            Vector3 offset = new Vector3(0, 0.5f, 0);
            
            for (int i = 0; i < regiment.CurrentLineFormation; i++)
            {
                Vector3 origin = regiment.UnitsTransform[i].position + offset + regiment.UnitsTransform[i].forward;
                
                Vector3 direction = regiment.UnitsTransform[i].forward;
                
#if UNITY_EDITOR
                if (debug) Debug.DrawRay(origin, direction * 20f);
#endif
                

                commands[i] = new SpherecastCommand(origin, 2f,direction, 20f,HitMask);
            }
            targetHandle = SpherecastCommand.ScheduleBatch(commands, results, regiment.CurrentLineFormation);
        }

        
    }
}

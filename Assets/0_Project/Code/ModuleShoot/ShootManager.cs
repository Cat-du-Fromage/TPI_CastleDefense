using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

using static Unity.Mathematics.math;

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

            if (!HasTarget)
            {
                if (IsTargetAcquire())
                {
                    //AssignTargets();
                    ShootTarget(true);
                };
            }
            
            results.Dispose();
            commands.Dispose();
        }
/*
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
        */
//======================================================================================================================
// NEW SHOOT

        public void ShootBullet(Unit unit)
        {
            if (unit == null) return;
            if (unit.Target == null || unit.Target.IsDead)
            {
                /*
#if UNITY_EDITOR
                if (debug && !regiment.IsPlayer) Debug.Log($"unit?:t: {unit}; index = {unit.IndexInRegiment}; Target {TargetRegiment}");
#endif   
*/
                Unit currentUnit = unit;
                //Debug.Log(unit);
                Unit findTarget = GetTargetUnit(currentUnit.IndexInRegiment, TargetRegiment.CurrentLineFormation);
                if(findTarget == null)
                {
                    SetNoTarget();
                    return;
                }
                unit.Target = findTarget;
            }
            
            Transform unitTransform = unit.transform;
            Vector3 unitPosition = unitTransform.position;

            Vector3 unitTargetPosition = TargetRegiment.Units[unit.Target.IndexInRegiment].transform.position + new Vector3(0, 0.5f, 0);
            
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
        
        private void AssignTargets()
        {
            for (int i = 0; i < regiment.Units.Length; i++)
            {
                //int2 xTarget = regiment.Units[i].IndexInRegiment.GetXY2(TargetRegiment.CurrentLineFormation);
                Unit findTarget = GetTargetUnit(regiment.Units[i].IndexInRegiment, TargetRegiment.CurrentLineFormation);
                if (findTarget == null)
                {
                    ShootTarget(false);
                    //Debug.Log("Target NOT Assigned");
                    return;
                }
                
                regiment.Units[i].Target = findTarget;
                break;
            }
            ShootTarget(true);
        }
        
        public void SetNoTarget()
        {
            ShootTarget(false);
            HasTarget = false;
            TargetRegiment = null;
        }

//======================================================================================================================
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

        private bool IsTargetAcquire()
        {
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].transform == null) continue;
                if (!results[i].transform.TryGetComponent(out Unit unit)) continue;
                if (unit.IsDead) continue;
                HasTarget = true;
                TargetRegiment = unit.RegimentAttach;
                AssignTargets();
                return true;
            }
            HasTarget = false;
            TargetRegiment = null;
            return false;
        }

        private Unit GetTargetUnit(int indexInRegiment, int targetLineFormation)
        {
            //int currentIndex = indexInRegiment;
            int targetRowLength = targetLineFormation;
            
            (int x, _) = indexInRegiment.GetXY(targetLineFormation);

            //===============================================================================
            //CHECK IF LINE BIGGER THAN OPPOSITE
            int divisor = 1;
            if (regiment.CurrentLineFormation > TargetRegiment.CurrentLineFormation)
            {
                divisor = regiment.CurrentLineFormation - TargetRegiment.CurrentLineFormation;
                if(x > TargetRegiment.CurrentLineFormation)
                    x /= divisor;
            }
            //===============================================================================
            int xCheck = x;
            int yCheck = 0;
            
            int maxX = targetRowLength;
            
            //CAREFULL: Need to adapt to CURRENT num Units!
            int maxY = Mathf.CeilToInt(TargetRegiment.RegimentClass.BaseNumUnits / (float)targetRowLength) - 1;
/*      
#if UNITY_EDITOR
            Debug.Log($"max Y : {maxY} maxX : {maxX}");
#endif
*/           
            int indexLoop = 0;
            bool leftReach = xCheck <= 0;
            bool rightReach = xCheck >= maxX - 1;
            //bool2 leftRightReach = new bool2(false);
            
            //Internal Method
            int GetIndexTarget() => yCheck * targetRowLength + xCheck; //int2(xCheck, yCheck).GetIndex(maxX);
            bool CheckUnitNotNull()
            {
                //Debug.Log($"CHECK ZERO (x:{xCheck}y:{yCheck}){leftReach} {rightReach} maxIndex = {TargetRegiment.RegimentClass.BaseNumUnits}; currentIndex = {GetIndexTarget()};");
                return TargetRegiment.Units[GetIndexTarget()] != null;
            }
            

            bool exit = false;
            bool Exit() => leftReach == true && rightReach == true && yCheck == maxY;
            while (true)
            {
                exit = Exit();
                
                if (leftReach == true && rightReach == true)
                {
                    if (yCheck == maxY) return null;
                    if (maxY == 0) return null; //if there is only 1 line 
                    
                    yCheck += 1;
                    //Debug.Log($"Double egalité l:{leftReach} r:{rightReach} NEW Y: {yCheck}");
                    if (yCheck == maxY)
                    {
                        //left = false;
                        //right = false;
                        maxX = GetTargetLastRowSize();
                        
                        //ATTENTION
                        
                        //===============================================================================
                        //CHECK IF LINE BIGGER THAN OPPOSITE
                        int divisor1 = 1;
                        if (regiment.CurrentLineFormation > maxX)
                        {
                            //divisor1 = regiment.CurrentLineFormation - maxX;
                            (x,_) = indexInRegiment.GetXY(maxX);
                            //if(x > maxX) x = 0;
                        }
                        //===============================================================================
                        
                        //Debug.Log($"LastRow: {maxX}");
                    }
                    indexLoop = 0;
                }
                
                if (indexLoop == 0)
                {
                    xCheck = x;
                    leftReach = xCheck <= 0;
                    rightReach = xCheck >= maxX - 1;
                    //Debug.Log($"xCheck: {xCheck}; yCheck: {yCheck}");
                    if (CheckUnitNotNull() && !TargetRegiment.Units[GetIndexTarget()].IsDead)
                    {
                        //Debug.DrawLine(TargetRegiment.Units[GetIndexTarget()].transform.position,regiment.Units[indexInRegiment].transform.position,Color.yellow, 3f);
                        
                        //Debug.Log($"CHECK ZERO (x:{xCheck}y:{yCheck}){leftReach} {rightReach} maxIndex = {TargetRegiment.RegimentClass.BaseNumUnits}; currentIndex = {GetIndexTarget()};");
                        return TargetRegiment.Units[GetIndexTarget()];
                        //return true;
                    }

                    indexLoop += 1;
                }
                
                if (rightReach == false)
                {
                    //offset = 1;
                    xCheck = x + indexLoop * 1;
                    if (CheckUnitNotNull() && !TargetRegiment.Units[GetIndexTarget()].IsDead)
                    {
                        //Debug.Log($"CHECK RIGHT (x:{xCheck}y:{yCheck}){leftReach} {rightReach} maxIndex = {TargetRegiment.RegimentClass.BaseNumUnits}; currentIndex = {GetIndexTarget()};");
                        return TargetRegiment.Units[GetIndexTarget()];
                    }

                    if (xCheck >= maxX - 1) rightReach = true;
                }

                if (leftReach == false)
                {
                    //offset = -1;
                    xCheck = x + indexLoop * -1;
                    /*
                    if (GetIndexTarget() == -1)
                    {
                        Debug.Log($"x:{x}; left:{leftReach} right:{rightReach} xCheck: {xCheck}; yCheck: {yCheck}; indexLoop: {indexLoop}");
                    }
                    */
                    if (CheckUnitNotNull() && !TargetRegiment.Units[GetIndexTarget()].IsDead)
                    {
                        //Debug.Log($" CHECK LEFT (x:{xCheck}y:{yCheck}){leftReach} {rightReach} maxIndex = {TargetRegiment.RegimentClass.BaseNumUnits}; currentIndex = {GetIndexTarget()};");
                        return TargetRegiment.Units[GetIndexTarget()];
                    }

                    if (xCheck <= 0) leftReach = true;
                }

                indexLoop += 1;
                
            } 
            
            //Debug.Log("return null");
            //return null;
        }
        
        private int GetTargetLastRowSize()
        {
            float formationNumLine = TargetRegiment.RegimentClass.BaseNumUnits / (float)TargetRegiment.CurrentLineFormation;
            int numCompleteLine = Mathf.FloorToInt(formationNumLine);
            //======================================================================================
            int lastLineNumUnit = TargetRegiment.RegimentClass.BaseNumUnits - (numCompleteLine * TargetRegiment.CurrentLineFormation);
            return lastLineNumUnit;
        }

        private void GetTarget()
        {
            //results = new (regiment.CurrentLineFormation, Allocator.TempJob);
            //commands = new (regiment.CurrentLineFormation, Allocator.TempJob);

            Vector3 offset = new Vector3(0, 0.5f, 0);

            List<int> searcher = new List<int>(regiment.CurrentLineFormation);
            for (int i = 0; i < regiment.CurrentLineFormation; i++)
            {
                if (regiment.Units[i].IsDead) continue;
                searcher.Add(i);
            }
            
            results = new (searcher.Count, Allocator.TempJob);
            commands = new (searcher.Count, Allocator.TempJob);

            for (int i = 0; i < searcher.Count; i++)
            {
                int unitIndex = searcher[i];
                Vector3 direction = regiment.UnitsTransform[unitIndex].forward;
                Vector3 origin = regiment.UnitsTransform[unitIndex].position + offset + direction;
#if UNITY_EDITOR
                if (debug) Debug.DrawRay(origin, direction * 20f);
#endif
                commands[i] = new SpherecastCommand(origin, 2f,direction, 20f,HitMask);
            }
            
            /*
            for (int i = 0; i < regiment.CurrentLineFormation; i++)
            {
                Vector3 origin = regiment.UnitsTransform[i].position + offset + regiment.UnitsTransform[i].forward;
                
                Vector3 direction = regiment.UnitsTransform[i].forward;
                
#if UNITY_EDITOR
                if (debug) Debug.DrawRay(origin, direction * 20f);
#endif
                

                commands[i] = new SpherecastCommand(origin, 2f,direction, 20f,HitMask);
            }
            */
            targetHandle = SpherecastCommand.ScheduleBatch(commands, results, regiment.CurrentLineFormation);
        }

        
    }
    
    /*
     
     private bool GetTargetUnit(int targetIndexInRegiment, int rowWidth, out Unit unit)
        {
            unit = null;
            int currentIndex = targetIndexInRegiment;
            (int x, int y) = currentIndex.GetXY(rowWidth);

            int offset = 0;

            int xCheck = x;
            int yCheck = 0;
            bool2 leftRightReach = new bool2(false);
            //CAREFULL: Need to adapt to CURRENT num Units!
            int maxY = (TargetRegiment.RegimentClass.BaseNumUnits / TargetRegiment.CurrentLineFormation) - 1;
            
#if UNITY_EDITOR
            if (debug) Debug.Log($"max Y : {maxY}");
#endif
            
            int indexLoop = 0;
            while (!all(leftRightReach) && yCheck == maxY)
            {
                int GetUpdatedX() => offset * indexLoop + xCheck;
                bool CheckValidUnit(int y, int xOffseted)
                {
                    int index = mad(y, TargetRegiment.CurrentLineFormation, xOffseted);
                    //Debug.Log(index);
                    Debug.Log($"xOffseted: {xOffseted} get at: {index.GetXY2(TargetRegiment.CurrentLineFormation)}");
                    return !TargetRegiment.Units[index].IsDead;
                }
                //CHECK WHEN 0
                if (indexLoop == 0)
                {
                    if (CheckValidUnit(yCheck, xCheck))
                    {
                        unit = TargetRegiment.Units[mad(yCheck, TargetRegiment.CurrentLineFormation, xCheck)];
                        return true;
                    }
                    indexLoop++;
                }
                
                //======================================================
                // Did we hit border (left or right)
                if (any(leftRightReach))
                {
                    offset = select(-1,1, leftRightReach.x);
                    if (CheckValidUnit(yCheck, GetUpdatedX()))
                    {
                        unit = TargetRegiment.Units[mad(yCheck, TargetRegiment.CurrentLineFormation, GetUpdatedX())];
                        return true;
                    }
                }
                else
                {
                    offset = -1;
                    if (CheckValidUnit(yCheck, GetUpdatedX()))
                    {
                        unit = TargetRegiment.Units[mad(yCheck, TargetRegiment.CurrentLineFormation, GetUpdatedX())];
                        return true;
                    }
                    offset = 1;
                    if (CheckValidUnit(yCheck, GetUpdatedX()))
                    {
                        unit = TargetRegiment.Units[mad(yCheck, TargetRegiment.CurrentLineFormation, GetUpdatedX())];
                        return true;
                    }
                }
                //======================================================


                //Update left right information
                if (xCheck == 0) leftRightReach.x = true;
                if (xCheck == TargetRegiment.CurrentLineFormation - 1) leftRightReach.y = true;

                //Go to next Line
                if (all(leftRightReach))
                {
                    leftRightReach = new bool2(false);
                    yCheck = min(maxY, yCheck+1);
                    indexLoop = 0;// reset
                    continue;
                }
                
                indexLoop++;
            }
            return false;
        }
    */
     
}

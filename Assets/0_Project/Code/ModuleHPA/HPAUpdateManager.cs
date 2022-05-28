using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using KWUtils;
using Unity.Mathematics;
using UnityEngine.Jobs;
using static KWUtils.NativeCollectionExt;
using static UnityEngine.Mathf;
using static UnityEngine.Physics;
using static UnityEngine.Vector3;
using static Unity.Mathematics.math;
using static Unity.Mathematics.quaternion;


namespace KaizerWald
{
    public class HPAUpdateManager : MonoBehaviour
    {
        private IHighlightCoordinator coordinator;
        private RegimentManager regimentManager;

        private NativeArray<JobHandle> jobHandles;
        
        private void Awake()
        {
            regimentManager = GetComponent<RegimentManager>();
            IHighlightCoordinator coord = (IHighlightCoordinator)regimentManager;
            coordinator = coord;
        }

        // Update is called once per frame
        private void Update()
        {
            if (coordinator.MovingRegiments.Count == 0) return;
            
            jobHandles = AllocNtvAry<JobHandle>(coordinator.MovingRegiments.Count, Allocator.Temp);

            for (int i = 0; i < coordinator.MovingRegiments.Count; i++)
            {
                
                
                MoveRegiments(coordinator.MovingRegiments[i]);
                PlaceRegimentUnits(coordinator.MovingRegiments[i]);
            }
        }
        
        private void LateUpdate()
        {
            if (coordinator.MovingRegiments.Count == 0) return;
            CheckReachGoal();
        }
        
        private void CheckReachGoal()
        {
            for (int i = coordinator.MovingRegiments.Count - 1; i > -1; i--)
            {
                TransformAccessArray goalAccessArray = coordinator.PlacementAccessArrays[coordinator.MovingRegiments[i].RegimentID];
                TransformAccessArray unitAccessArray = coordinator.MovingRegiments[i].UnitsTransformAccessArray;

                int numUnitsReach = 0;
                for (int j = 0; j < coordinator.MovingRegiments[i].UnitsTransform.Length; j++)
                {
                    float distance = goalAccessArray[j].position.Flat().DistanceTo(unitAccessArray[j].position.Flat());
                    distance = abs(distance);
                    
                    if (distance < 1f)
                    {
                        //=============================================================
                        //TEMPORARY
                        coordinator.MovingRegiments[i].Units[j].Animation.SetSpeed(0);
                        //TEMPORARY
                        //=============================================================
                    }
                    
                    if(distance > 0.1f) continue;
                    numUnitsReach += 1;
                }
                
                if (numUnitsReach != coordinator.MovingRegiments[i].UnitsTransform.Length) continue;
                coordinator.MovingRegiments[i].SetMoving(false);
                coordinator.MovingRegiments.Remove(coordinator.MovingRegiments[i]);
            }
        }

        private void MoveRegiments(Regiment regiment)
        {
            int regimentId = regiment.RegimentID;
            int lastPointIndex = regimentManager.hpaPathfinder.Waypoints[regimentId].Count - 1;
            int currentWaypointIndex = regimentManager.hpaPathfinder.currentTargetWaypoint[regimentId];
            Vector3 regimentPosition = regiment.transform.position;
            Vector3 waypoint = regimentManager.hpaPathfinder.Waypoints[regimentId][currentWaypointIndex];

            if (regimentPosition == waypoint) //Update if reached
            {
                if (currentWaypointIndex == lastPointIndex) return;
                regimentManager.hpaPathfinder.currentTargetWaypoint[regimentId] += 1;
                waypoint = regimentManager.hpaPathfinder.Waypoints[regimentId][currentWaypointIndex];
            }

            float speed = regiment.RegimentClass.Speed * 4;
            Vector3 newPosition = MoveTowards(regimentPosition, waypoint, speed * Time.deltaTime);
            Quaternion targetRotation = LookRotationSafe(regiment.transform.position.DirectionTo(waypoint), up());

            if (currentWaypointIndex == lastPointIndex)
            {
                targetRotation = regimentManager.hpaPathfinder.DestinationRegiment[regimentId].Item2;
            }
            
            quaternion newRotation = slerp(regiment.transform.rotation, targetRotation, Time.deltaTime * speed);

            regiment.transform.SetPositionAndRotation(newPosition, newRotation);
        }
        
        
        private void PlaceRegimentUnits(Regiment regiment)
        {
            Vector3 regimentForwardDir = regiment.transform.forward;
            int lineFormation = regiment.CurrentLineFormation;
            int currentNumUnits = regiment.Units.Length;
            float unitSpaceSize = regiment.RegimentClass.SpaceSizeBetweenUnit;
            float midSizeRegiment = regiment.RegimentClass.SpaceSizeBetweenUnit * (lineFormation - 1) / 2f;
            
            Vector3 leftRegiment = Cross(regimentForwardDir, Vector3.up).normalized;
            Vector3 lineDirection = -leftRegiment;
            Vector3 startPosition = regiment.transform.position + (leftRegiment * midSizeRegiment);
            
            //Directions to follow
            //Vector3 lineDirection = (mouseEnd - mouseStart).normalized;
            
            
            Vector3 columnDirection = -regimentForwardDir;

            //======================================================================================
            //Lines information
            float formationNumLine = currentNumUnits / (float)lineFormation;
            int totalLine = CeilToInt(formationNumLine);
            int numCompleteLine = FloorToInt(formationNumLine);
            //======================================================================================
                
            int lastLineNumUnit = currentNumUnits - (numCompleteLine * lineFormation);
            float offset = select(0, (lineFormation - lastLineNumUnit) / 2f,totalLine != numCompleteLine);
            Vector3 offsetPosition = (lineDirection * unitSpaceSize) * offset;
            
            Vector3[] formationPositions = new Vector3[currentNumUnits];
            for (int j = 0; j < currentNumUnits; j++)
            {
                (int x, int y) = j.GetXY(lineFormation);

                Vector3 linePosition = startPosition + lineDirection * (unitSpaceSize * x);

                bool isLastRow = y == totalLine - 1 && offsetPosition != zero;
                linePosition += (Vector3)select(zero, offsetPosition, isLastRow);

                Vector3 columnPosition = columnDirection * (unitSpaceSize * y);

                //Position Here!
                formationPositions[j] = linePosition + columnPosition;
            }

            for (int j = 0; j < formationPositions.Length; j++)
            {
                Vector3 position = new(formationPositions[j].x, 0, formationPositions[j].z);

                //===================================================
                //NEW
                Transform unitTransform = regiment.Units[j].transform;
                //===================================================
                Vector3 positionToGo = MoveTowards(regiment.Units[j].transform.position, position, Time.deltaTime * 4);
                //Transform highlightTransform = register.Records[coordinator.SelectedRegiments[i].RegimentID][j].HighlightTransform;
                unitTransform.SetPositionAndRotation(positionToGo, regiment.transform.rotation);
            }
        
        }
        private float MinSizeFormation()
        {
            float min = 0;
            for (int i = 0; i < coordinator.SelectedRegiments.Count; i++)
            {
                RegimentClass regimentClass = coordinator.SelectedRegiments[i].RegimentClass;
                min += regimentClass.SpaceSizeBetweenUnit * regimentClass.MinRow;

                if (i == 0) continue;
                float distanceBetweenUnit = coordinator.SelectedRegiments[i - 1].RegimentClass.SpaceSizeBetweenUnit;
                float spaceBetweenRegiment = distanceBetweenUnit + regimentClass.SpaceSizeBetweenUnit / 2f;
                min += spaceBetweenRegiment;
            }

            return min;
        }
        
    }
}

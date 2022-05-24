using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using static UnityEngine.Mathf;
using static UnityEngine.Physics;
using static UnityEngine.Vector3;
using static Unity.Mathematics.math;
using static Unity.Mathematics.quaternion;
using static PlayerControls;

namespace KaizerWald
{
    public class PlacementController : IPlacementActions
    {
        private readonly IHighlightCoordinator coordinator;
        private readonly PlacementRegister register;
        private readonly Camera playerCamera;

        private bool placementsVisible;
        
        private bool mouseStartValid;
        private Vector3 mouseStart;
        private Vector3 mouseEnd;

        private float mouseDistance;
        public PlacementController(IHighlightCoordinator highlightCoordinator, PlacementRegister placementRegister)
        {
            playerCamera = Camera.main;
            register = placementRegister;
            coordinator = highlightCoordinator;
            highlightCoordinator.Controls.Placement.Enable();
            highlightCoordinator.Controls.Placement.SetCallbacks(this);
        }

        public void OnRightMouseClickAndMove(InputAction.CallbackContext context)
        {
            //Guard Clause: need regiments selected
            if (coordinator.SelectedRegiments.Count == 0) return;

            switch (context.phase)
            {
                case InputActionPhase.Started:
                    mouseStartValid = GetMouseStart(context.ReadValue<Vector2>());
                    break;
                case InputActionPhase.Performed:
                    if (!mouseStartValid || !GetMouseEnd(context.ReadValue<Vector2>())) return;
                    mouseDistance = mouseStart.DistanceTo(mouseEnd);
                    PlaceRegiments();
                    break;
                case InputActionPhase.Canceled:
                    if (placementsVisible == false) return; //Means Left Click is pressed
                    coordinator.DispatchEvent(register);
                    OnMouseReleased();
                    break;
            }
        }

        private void OnMouseReleased()
        {
            for (int i = 0; i < coordinator.SelectedRegiments.Count; i++)
            {
                coordinator.SelectedRegiments[i].SetMoving(true);
                if (register.MovingRegiments.Contains(coordinator.SelectedRegiments[i])) continue;
                register.MovingRegiments.Add(coordinator.SelectedRegiments[i]);
            }
            
            mouseDistance = 0;
            register.OnClearHighlight();
            placementsVisible = false;
        }

        public void OnSpaceKey(InputAction.CallbackContext context)
        {
            if (context.performed) return;
            
            if (context.started)
            {
                register.OnEnableAll();
            }
            else if (context.canceled)
            {
                register.OnClearHighlight();
            }
        }

        public void OnLeftMouseCancel(InputAction.CallbackContext context)
        {
            if (!placementsVisible && !context.performed) return;
            register.OnClearHighlight();
            placementsVisible = false;
        }

        //private float 

        private void PlaceRegiments()
        {
            //First Guard Clause : mouse goes far enough
            if (!placementsVisible)
            {
                if (mouseDistance < coordinator.SelectedRegiments[0].RegimentClass.SpaceSizeBetweenUnit) return;
                register.EnableAllSelected();
                placementsVisible = true;
            }
            
            float minRegimentsFormationSize = MinSizeFormation();

            //Second Guard Clause : Check if we add something (no return because we want to rotate)
            int numUnitToAdd = (int)(mouseDistance - minRegimentsFormationSize) / coordinator.SelectedRegiments.Count;
            numUnitToAdd = Max(0, numUnitToAdd);
            
            //Directions to follow
            Vector3 lineDirection = (mouseEnd - mouseStart).normalized;
            Vector3 columnDirection = Cross(lineDirection, Vector3.down).normalized;
            
            float offsetRegiment = 0;
            for (int i = 0; i < coordinator.SelectedRegiments.Count; i++)
            {
                //Gather regimentData
                RegimentClass regimentClass = coordinator.SelectedRegiments[i].RegimentClass;
                int currentNumUnits = coordinator.SelectedRegiments[i].UnitsTransform.Length;
                float unitSpaceSize = regimentClass.SpaceSizeBetweenUnit;
                
                //OFFSET REGIMENT?
                if (i != 0)
                {
                    float totalSize = coordinator.SelectedRegiments[i - 1].RegimentClass.SpaceSizeBetweenUnit;
                    offsetRegiment += (totalSize + unitSpaceSize) / 2f;
                }
                
                //what we actually update
                int numUnitPerLine = Min(regimentClass.MaxRow,regimentClass.MinRow + numUnitToAdd);
                
                //======================================================================================
                //Lines information
                float formationNumLine = currentNumUnits / (float)numUnitPerLine;
                int totalLine = CeilToInt(formationNumLine);
                int numCompleteLine = FloorToInt(formationNumLine);
                //======================================================================================
                
                int lastLineNumUnit = currentNumUnits - (numCompleteLine * numUnitPerLine);
                float offset = select(0, (numUnitPerLine - lastLineNumUnit) / 2f,totalLine != numCompleteLine);
                Vector3 offsetPosition = (lineDirection * unitSpaceSize) * offset;

                Vector3[] formationPositions = new Vector3[currentNumUnits];
                for (int j = 0; j < currentNumUnits; j++)
                {
                    (int x, int y) = j.GetXY(numUnitPerLine);

                    Vector3 linePosition = mouseStart + lineDirection * (unitSpaceSize * x);
                    linePosition += lineDirection * offsetRegiment;

                    bool isLastRow = y == totalLine - 1 && offsetPosition != zero;
                    linePosition += (Vector3)select(zero, offsetPosition, isLastRow);

                    Vector3 columnPosition = columnDirection * (unitSpaceSize * y);
                    
                    //Position Here!
                    formationPositions[j] = linePosition + columnPosition;
                }

                for (int j = 0; j < formationPositions.Length; j++)
                {
                    Vector3 position = new (formationPositions[j].x, 0.05f, formationPositions[j].z);
                    Transform highlightTransform = register.Records[coordinator.SelectedRegiments[i].RegimentID][j].HighlightTransform;
                    highlightTransform.SetPositionAndRotation(position,LookRotationSafe(-columnDirection, up()));
                }

                offsetRegiment += (numUnitPerLine * unitSpaceSize);
                coordinator.SelectedRegiments[i].SetCurrentLineFormation(numUnitPerLine);
            }
        }

        //Maybe a good idea to Cache the result on Selection Change
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
        
        //==============================================================================================================
        //Mouses Positions
        private bool GetMouseStart(in Vector2 mouseInput)
        {
            Ray singleRay = playerCamera.ScreenPointToRay(mouseInput);
            bool hit = Raycast(singleRay, out RaycastHit singleHit, Mathf.Infinity, 1 << 8);
            mouseStart = select(mouseStart, singleHit.point, hit);
            return hit;
        }
        
        private bool GetMouseEnd(in Vector2 mouseInput)
        {
            Ray singleRay = playerCamera.ScreenPointToRay(mouseInput);
            bool hit = Raycast(singleRay, out RaycastHit singleHit, Mathf.Infinity, 1 << 8);
            mouseEnd = select(mouseEnd, singleHit.point, hit);
            return hit;
        }
        //==============================================================================================================
    }
}

/*
        private void SetFormation()
        {
            RegimentClass regimentClass = coordinator.SelectedRegiments[0].RegimentClass;
            int currentNumUnits = coordinator.SelectedRegiments[0].Units.Length;
            float unitSpaceSize = regimentClass.SpaceSizeBetweenUnit;
            
            Vector3[] formationPositions = new Vector3[currentNumUnits];

            int numUnitPerRow = Mathf.Max(Mathf.FloorToInt(mouseDistance / regimentClass.SpaceSizeBetweenUnit) + 1, regimentClass.MinRow);
            numUnitPerRow = Mathf.Min(regimentClass.MaxRow, numUnitPerRow);
            
            float formationNumRow = (float)currentNumUnits / (float)numUnitPerRow;
            int formationTotalRow = Mathf.CeilToInt(formationNumRow);
            int formationNumCompleteRow = Mathf.FloorToInt(formationNumRow);

            int lastRowNumUnit = currentNumUnits - (formationNumCompleteRow * numUnitPerRow);
            
            float offset = select(0, (numUnitPerRow - lastRowNumUnit) / 2f,formationTotalRow != formationNumCompleteRow);
            
            //Better make 2 different function (1 if last row is complete and 2) if not)?
            Vector3 lineDirection = (mouseEnd - mouseStart).normalized;
            Vector3 offsetPosition = (lineDirection * unitSpaceSize) * offset;
            
            Vector3 columnDirection = Vector3.Cross(lineDirection, Vector3.down).normalized;
            
            for (int i = 0; i < currentNumUnits; i++)
            {
                int y = i / numUnitPerRow;
                int x = i - (y * numUnitPerRow);
                //Debug.Log($"TotalRow: {formationTotalRow} CompleteRow: {formationNumCompleteRow}; y value: {y}");
                Vector3 linePosition = mouseStart + lineDirection * (unitSpaceSize * x);
                
                if (y == formationTotalRow-1 && offsetPosition != Vector3.zero)
                {
                    linePosition += offsetPosition;
                }

                Vector3 columnPosition = linePosition + columnDirection * (unitSpaceSize * y);

                formationPositions[i] = columnPosition;
            }

            for (int i = 0; i < formationPositions.Length; i++)
            {
                Vector3 position = new Vector3(formationPositions[i].x, 0.05f, formationPositions[i].z);
                register.Records[coordinator.SelectedRegiments[0].RegimentID][i]
                    .HighlightTransform.SetPositionAndRotation(position, Quaternion.LookRotation(-columnDirection, Vector3.up));
            }
        }
*/
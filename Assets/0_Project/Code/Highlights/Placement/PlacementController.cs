using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using static UnityEngine.Physics;
using static Unity.Mathematics.math;
using static PlayerControls;

namespace KaizerWald
{
    public class PlacementController : IPlacementActions
    {
        private Camera playerCamera;
        
        private IHighlightCoordinator coordinator;
        private PlacementRegister register;

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
            //if (coordinator.SelectedRegiments.Count != 1) return;//On Utilise que 1 régiment pour le moment
            if (context.started)
            {
                mouseStart = mouseEnd = GetMousePositionOnTerrain(context.ReadValue<Vector2>());
            }
            else if (context.performed)
            {
                if (mouseStart == Vector3.negativeInfinity) return;
                mouseEnd = GetMousePositionOnTerrain(context.ReadValue<Vector2>());
                
                //Guard Clause: drag mouse long enough
                mouseDistance = mouseStart.DistanceTo(mouseEnd); //Vector3.Distance(mouseEnd, mouseStart);
                //Debug.Log($"pass guard 2: {mouseDistance}");
                
                if (mouseDistance < coordinator.SelectedRegiments[0].RegimentClass.SpaceSizeBetweenUnit) return;
                //Debug.Log("pass guard 3");
                for (int i = 0; i < coordinator.SelectedRegiments.Count; i++)
                {
                    register.OnEnableHighlight(coordinator.SelectedRegiments[i]);
                }
                //register.OnEnableHighlight(coordinator.SelectedRegiments[0]);
                PlaceRegiments();
                //SetFormation();
            }
            else
            {
                mouseDistance = 0;
                //Clear All renderer In final version
            }
        }

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

        private Vector3 GetMousePositionOnTerrain(Vector2 mouseInput)
        {
            Ray singleRay = playerCamera.ScreenPointToRay(mouseInput);
            bool hit = Raycast(singleRay, out RaycastHit singleHit, Mathf.Infinity, 1 << 8);
            
            return hit ? singleHit.point : Vector3.negativeInfinity;
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

        public void PlaceRegiments()
        {
            float minRegimentsFormationSize = MinSizeFormation();
            //First Guard Clause : mouse goes far enough
            if (mouseDistance <= minRegimentsFormationSize) return;
            //Second Guard Clause : Check if we add something
            int numUnitToAdd = (int)(mouseDistance - minRegimentsFormationSize) / coordinator.SelectedRegiments.Count;
            if (numUnitToAdd == 0) return;
            
            float offsetRegiment = 0;
            for (int i = 0; i < coordinator.SelectedRegiments.Count; i++)
            {
                //Gather regimentData
                RegimentClass regimentClass = coordinator.SelectedRegiments[i].RegimentClass;
                int currentNumUnits = coordinator.SelectedRegiments[0].Units.Length;
                float unitSpaceSize = regimentClass.SpaceSizeBetweenUnit;
                
                //OFFSET REGIMENT?
                if (i != 0)
                {
                    offsetRegiment += (coordinator.SelectedRegiments[i - 1].RegimentClass.SpaceSizeBetweenUnit +
                                       unitSpaceSize) / 2f;
                }

                //what we actually update
                int numUnitPerLine = Mathf.Min(regimentClass.MaxRow,regimentClass.MinRow + numUnitToAdd);
                
                //Lines information
                float formationNumLine = (float)currentNumUnits / (float)numUnitPerLine;
                int formationTotalLine = Mathf.CeilToInt(formationNumLine);
                int formationNumCompleteLine = Mathf.FloorToInt(formationNumLine);
                
                int lastLineNumUnit = currentNumUnits - (formationNumCompleteLine * numUnitPerLine);
                
                float offset = select(0, (numUnitPerLine - lastLineNumUnit) / 2f,formationTotalLine != formationNumCompleteLine);
                
                Vector3 lineDirection = (mouseEnd - mouseStart).normalized;
                Vector3 offsetPosition = (lineDirection * unitSpaceSize) * offset;
            
                Vector3 columnDirection = Vector3.Cross(lineDirection, Vector3.down).normalized;
                
                Vector3[] formationPositions = new Vector3[currentNumUnits];
                for (int j = 0; j < currentNumUnits; j++)
                {
                    int y = j / numUnitPerLine;
                    int x = j - (y * numUnitPerLine);
                    //Debug.Log($"TotalRow: {formationTotalRow} CompleteRow: {formationNumCompleteRow}; y value: {y}");
                    Vector3 linePosition = mouseStart + lineDirection * (unitSpaceSize * x);
                    linePosition += lineDirection * offsetRegiment;
                    
                    if (y == formationTotalLine-1 && offsetPosition != Vector3.zero)
                    {
                        linePosition += offsetPosition;
                    }

                    Vector3 columnPosition = linePosition + columnDirection * (unitSpaceSize * y);

                    formationPositions[j] = columnPosition;
                }

                for (int j = 0; j < formationPositions.Length; j++)
                {
                    Vector3 position = new Vector3(formationPositions[j].x, 0.05f, formationPositions[j].z);
                    register.Records[coordinator.SelectedRegiments[0].RegimentID][j]
                        .HighlightTransform.SetPositionAndRotation(position, Quaternion.LookRotation(-columnDirection, Vector3.up));
                }

                offsetRegiment += (numUnitPerLine * unitSpaceSize);
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
                float spaceBetweenRegiment =
                    coordinator.SelectedRegiments[i - 1].RegimentClass.SpaceSizeBetweenUnit
                    + regimentClass.SpaceSizeBetweenUnit / 2f;
                min += spaceBetweenRegiment;
            }

            return min;
        }
    }

    public struct JPlacements : IJobFor
    {
        
        public void Execute(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}

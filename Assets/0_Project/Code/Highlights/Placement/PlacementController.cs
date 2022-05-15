using System.Collections;
using System.Collections.Generic;
using KWUtils;
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
            if (coordinator.SelectedRegiments.Count != 1) return;//On Utilise que 1 régiment pour le moment
            //Debug.Log("pass guard 1");
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
                register.OnEnableHighlight(coordinator.SelectedRegiments[0]);
                SetFormation();
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
            //Debug.Log($"formationTotalRow: {formationTotalRow} formationNumCompleteRow: {formationNumCompleteRow}");

            int lastRowNumUnit = currentNumUnits - (formationNumCompleteRow * numUnitPerRow);

            //float offset = formationTotalRow == formationNumCompleteRow ? 0 : (numUnitPerRow-lastRowNumUnit)/2f;
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
                //register.Records[coordinator.SelectedRegiments[0].RegimentID][i].HighlightTransform.position = formationPositions[i];
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
    }
}

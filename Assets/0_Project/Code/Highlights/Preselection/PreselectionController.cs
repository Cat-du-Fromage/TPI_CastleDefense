using System;
using KWUtils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Jobs;
using static UnityEngine.Physics;
using static Unity.Mathematics.math;
using static KWUtils.UGuiUtils;

using static PlayerControls;

using static Unity.Jobs.LowLevel.Unsafe.JobsUtility;

namespace KaizerWald
{
    public class PreselectionController : IPreselectionActions
    {
        private readonly IHighlightCoordinator coordinator;
        private readonly PreselectionRegister register;
        private readonly Camera playerCamera;

        private readonly LayerMask selectionLayer = 1 << 7 | 1 << 10 | 1 << 11;

        public bool ClickDragPerformed{ get; private set; }
        public Vector2 StartLMouse{ get; private set; }
        public Vector2 EndLMouse{ get; private set; }
        
        //Raycast
        private Regiment preselectionCandidate;
        private readonly RaycastHit[] Hits = new RaycastHit[4];

        public PreselectionController(IHighlightCoordinator highlightCoordinator, PreselectionRegister preselectionRegister)
        {
            playerCamera = Camera.main;
            register = preselectionRegister;
            coordinator = highlightCoordinator;
            highlightCoordinator.Controls.Preselection.Enable();
            highlightCoordinator.Controls.Preselection.SetCallbacks(this);
        }
        
        //------------------------------------------------------------------------------------------------------------------
        //Single Unit Preselection
        public void OnMouseMove(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            EndLMouse = context.ReadValue<Vector2>();
            
            if (ClickDragPerformed) return;
            CheckMouseHoverUnit();
        }

        //------------------------------------------------------------------------------------------------------------------
        //Mouse Hover Check
        private void CheckMouseHoverUnit()
        {
            Ray singleRay = playerCamera.ScreenPointToRay(EndLMouse);
            int numHits = SphereCastNonAlloc(singleRay, 0.5f, Hits,INFINITY,selectionLayer);

            if (NoHits(numHits)) return;
            MouseHoverSingleEntity(singleRay, numHits);
        }
        
        //Guard Clause : Num Hits
        private bool NoHits(int numHits)
        {
            if (numHits == 0)
            {
                ClearPreSelections();
                return true;
            }
            return false;
        }
        
        //Mouse Hover : No Drag
        private void MouseHoverSingleEntity(in Ray singleRay, int numHits)
        {
            preselectionCandidate = Hits[0].transform.GetComponent<IUnit>().RegimentAttach;
            if (numHits > 1)
            {
                if (!AreUnitsFromSameRegiment(Hits[0].transform.GetComponent<IUnit>().RegimentAttach.RegimentID, numHits))
                {
                    bool hit = Raycast(singleRay, out RaycastHit unitHit, INFINITY, selectionLayer);
                    preselectionCandidate = hit == false
                        ? preselectionCandidate
                        : unitHit.transform.GetComponent<IUnit>().RegimentAttach;
                }
            }
            if (preselectionCandidate.IsPreselected) return;
            ClearPreSelections();
            AddPreselection(preselectionCandidate);
            
            bool AreUnitsFromSameRegiment(int firstHitRegimentIndex, int numHits)
            {
                for (int i = 1; i < numHits; i++)
                {
                    int regimentId = Hits[i].transform.GetComponent<Unit>().RegimentAttach.RegimentID;
                    if (firstHitRegimentIndex != regimentId) return false;
                }
                return true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------
        //Multiple Unit Preselection
        private bool IsDragSelection() => lengthsq(EndLMouse - StartLMouse) >= 128;
        public void OnLeftMouseClickAndMove(InputAction.CallbackContext context)
        {
            if (context.canceled) return;

            if (context.started)
            {
                StartLMouse = EndLMouse = context.ReadValue<Vector2>();
                ClickDragPerformed = false;
            }
            else
            {
                ClickDragPerformed = IsDragSelection();
                if (!ClickDragPerformed) return;
                PreselectionMethodChoice();
            }
        }

        //Event Dispatched by SystemController
        public void OnLeftMouseReleased()
        {
            ClearPreSelections();
            CheckMouseHoverUnit();
            ClickDragPerformed = false;
        }

        //------------------------------------------------------------------------------------------------------------------
        //Preselection

        private void PreselectionMethodChoice()
        {
            if (coordinator.Regiments.Count < 8) //Iteration Check
                PreselectRegiments();
            else 
                PreselectRegiments_JobSystem(); //Parallel Check
        }
        
        private void PreselectRegiments()
        {
            int numRegiment = coordinator.Regiments.Count;
            Bounds selectionBounds = playerCamera.GetViewportBounds(StartLMouse, EndLMouse);
            CameraProject cameraData = new CameraProject(playerCamera);

            for (int i = 0; i < numRegiment; i++)
            {
                Regiment regiment = coordinator.Regiments[i];
                if (coordinator.Regiments[i] == null) continue;
                
                bool isInSelectionRectangle = CheckUnitsInRectangleBounds(regiment);
                if(!regiment.IsPreselected && isInSelectionRectangle) 
                    AddPreselection(regiment);
                else if(regiment.IsPreselected && !isInSelectionRectangle) 
                    RemovePreselection(regiment);
            }

            bool CheckUnitsInRectangleBounds(Regiment regiment)
            {
                int numUnits = regiment.UnitsTransform.Length;
                for (int index = 0; index < numUnits; index++)
                {
                    if (regiment.UnitsTransform[index] == null) continue;
                    float3 unitPosition = regiment.UnitsTransform[index].transform.position;
                    float3 unitPositionInRect = unitPosition.WorldToViewportPoint(cameraData);
                    if (!selectionBounds.Contains(unitPositionInRect)) continue;
                    return true;
                }
                return false;
            }
        }
        
        private void PreselectRegiments_JobSystem()
        {
            Bounds selectionBounds = playerCamera.GetViewportBounds(StartLMouse, EndLMouse);
            CameraProject cameraData = new CameraProject(playerCamera);

            int numRegiment = coordinator.Regiments.Count;
            NativeList<JobHandle> jobHandles = new(numRegiment, Allocator.Temp);
            NativeHashSet<int> regimentToPreselect = new (numRegiment, Allocator.TempJob);
            
            for (int i = 0; i < numRegiment; i++)
            {
                if (coordinator.Regiments[i] == null) continue;
                JPreselectionHover job = new JPreselectionHover
                {
                    RegimentIndex = coordinator.Regiments[i].RegimentID,
                    SelectionBounds = selectionBounds,
                    CameraData = cameraData,
                    RegimentToPreselect = regimentToPreselect.AsParallelWriter()
                };
                //JobHandle dependency;
                JobHandle dependency = i == 0 ? default : jobHandles[i - 1];
                jobHandles.Add(job.ScheduleReadOnly(coordinator.Regiments[i].UnitsTransformAccessArray, JobWorkerCount - 1, dependency));
            }
            jobHandles[^1].Complete();
            jobHandles.Dispose();
            
            for (int i = 0; i < numRegiment; i++)
            {
                Regiment regiment = coordinator.Regiments[i];
                if(regimentToPreselect.Contains(regiment.RegimentID))
                    AddPreselection(regiment);
                else
                    RemovePreselection(regiment);
            }

            regimentToPreselect.Dispose();
        }

        //==================================================================================================================
        //Methods: Preselection
        //==================================================================================================================
        private void AddPreselection(Regiment regiment) => register.OnEnableHighlight(regiment);
        private void RemovePreselection(Regiment regiment) => register.OnDisableHighlight(regiment);
        private void ClearPreSelections() => register.OnClearHighlight();
        
        
        [BurstCompile(CompileSynchronously = true)]
        private struct JPreselectionHover : IJobParallelForTransform
        {
            [ReadOnly] public int RegimentIndex;
            [ReadOnly] public Bounds SelectionBounds;
            [ReadOnly] public CameraProject CameraData;
        
            [NativeDisableParallelForRestriction]
            [WriteOnly]public NativeHashSet<int>.ParallelWriter RegimentToPreselect;
            public void Execute(int index, TransformAccess transform)
            {
                if (!transform.isValid) return;
                float3 unitPosition = transform.position;
                float3 unitPositionInRect = unitPosition.WorldToViewportPoint(CameraData);
                if (!SelectionBounds.Contains(unitPositionInRect)) return;
                RegimentToPreselect.Add(RegimentIndex);
            }
        }
    }
    
}
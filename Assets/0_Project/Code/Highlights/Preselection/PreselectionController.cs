using System;
using KWUtils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
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
    /*
    //[RequireComponent(typeof(PreselectionResponse))]
    public class PreselectionController : MonoBehaviour, IPreselectionActions
    {
        private IHighlightRegister preselectionRegister;
        
        [SerializeField] private RegimentManager RegimentManager;
        [SerializeField] private Camera PlayerCamera;

        private PlayerControls controls;

        public bool ClickDragPerformed{ get; private set; }
        private Vector2 startLMouse;
        private Vector2 endLMouse;
        
        //Raycast
        private IRegiment preselectionCandidate;
        private readonly RaycastHit[] Hits = new RaycastHit[4];

        public event Action<bool> OnLeftClickReleased;

        private void Awake()
        {
            preselectionRegister = GetComponent<IHighlightRegister>();
            RegimentManager ??= GetComponent<RegimentManager>();
            
            PlayerCamera = PlayerCamera == null ? Camera.main : PlayerCamera;
        }
        
        private void Start()
        {
            controls ??= new PlayerControls();
            controls.Preselection.Enable();
            controls.Preselection.SetCallbacks(this);
        }
        //------------------------------------------------------------------------------------------------------------------
        //Single Unit Preselection
        public void OnMouseMove(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            endLMouse = context.ReadValue<Vector2>();
            
            if (ClickDragPerformed) return;
            CheckMouseHoverUnit();
        }

        //------------------------------------------------------------------------------------------------------------------
        //Mouse Hover Check
        private void CheckMouseHoverUnit()
        {
            Ray singleRay = PlayerCamera.ScreenPointToRay(endLMouse);
            int numHits = SphereCastNonAlloc(singleRay, 0.5f, Hits,INFINITY,1 << 7);

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
            preselectionCandidate = Hits[0].transform.GetComponent<Unit>().RegimentAttach;
            if (numHits > 1)
            {
                if (!AreUnitsFromSameRegiment(preselectionCandidate.RegimentID, numHits))
                {
                    bool hit = Raycast(singleRay, out RaycastHit unitHit, INFINITY, 1 << 7);
                    preselectionCandidate = hit == false
                        ? preselectionCandidate
                        : unitHit.transform.GetComponent<Unit>().RegimentAttach;
                }
            }
            if (preselectionCandidate.IsPreSelected) return;
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
        private bool IsDragSelection() => lengthsq(endLMouse - startLMouse) >= 128;
        public void OnLeftMouseClickAndMove(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    startLMouse = endLMouse = context.ReadValue<Vector2>();
                    ClickDragPerformed = false;
                    break;
                case InputActionPhase.Performed:
                    ClickDragPerformed = IsDragSelection();
                    if (!ClickDragPerformed) return;
                    PreselectionMethodChoice();
                    break;
                case InputActionPhase.Canceled:
                    OnLeftClickReleased?.Invoke(ClickDragPerformed);
                    OnLeftMouseReleased();
                    break;
            }
        }

        private void OnLeftMouseReleased()
        {
            if (!ClickDragPerformed) return;
            ClearPreSelections();
            CheckMouseHoverUnit();
            ClickDragPerformed = false;
        }

        //------------------------------------------------------------------------------------------------------------------
        //Preselection

        private void PreselectionMethodChoice()
        {
            if (RegimentManager.Regiments.Count < 8) //Iteration Check
                PreselectRegiments();
            else 
                PreselectRegiments_JobSystem(); //Parallel Check
        }
        
        private void PreselectRegiments()
        {
            int numRegiment = RegimentManager.Regiments.Count;
            Bounds selectionBounds = PlayerCamera.GetViewportBounds(startLMouse, endLMouse);
            CameraProject cameraData = new CameraProject(PlayerCamera);

            for (int i = 0; i < numRegiment; i++)
            {
                IRegiment regiment = RegimentManager.Regiments[i];
                if (RegimentManager.Regiments[i] == null) continue;
                
                bool isInSelectionRectangle = CheckUnitsInRectangleBounds(regiment);
                if(!regiment.IsPreSelected && isInSelectionRectangle) 
                    AddPreselection(regiment);
                else if(regiment.IsPreSelected && !isInSelectionRectangle) 
                    RemovePreselection(regiment);
            }

            bool CheckUnitsInRectangleBounds(Regiment regiment)
            {
                int numUnits = regiment.Units.Count;
                for (int index = 0; index < numUnits; index++)
                {
                    if (regiment.Units[index] == null) continue;
                    float3 unitPosition = regiment.Units[index].transform.position;
                    float3 unitPositionInRect = unitPosition.WorldToViewportPoint(cameraData);
                    if (!selectionBounds.Contains(unitPositionInRect)) continue;
                    return true;
                }
                return false;
            }
        }
        
        private void PreselectRegiments_JobSystem()
        {
            Bounds selectionBounds = PlayerCamera.GetViewportBounds(startLMouse, endLMouse);
            CameraProject cameraData = new CameraProject(PlayerCamera);

            int numRegiment = RegimentManager.Regiments.Count;
            NativeList<JobHandle> jobHandles = new(numRegiment, Allocator.Temp);
            NativeHashSet<int> regimentToPreselect = new (numRegiment, Allocator.TempJob);
            
            for (int i = 0; i < numRegiment; i++)
            {
                if (RegimentManager.Regiments[i] == null) continue;
                JPreselectionHover job = new JPreselectionHover
                {
                    RegimentIndex = RegimentManager.Regiments[i].Index,
                    SelectionBounds = selectionBounds,
                    CameraData = cameraData,
                    RegimentToPreselect = regimentToPreselect.AsParallelWriter()
                };
                //JobHandle dependency;
                JobHandle dependency = i == 0 ? default : jobHandles[i - 1];
                jobHandles.Add(job.ScheduleReadOnly(RegimentManager.Regiments[i].UnitsTransformAccessArray, JobWorkerCount - 1, dependency));
            }
            jobHandles[^1].Complete();
            jobHandles.Dispose();
            
            for (int i = 0; i < numRegiment; i++)
            {
                IRegiment regiment = RegimentManager.Regiments[i];
                if(regimentToPreselect.Contains(regiment.RegimentID))
                    AddPreselection(regiment);
                else
                    RemovePreselection(regiment);
            }

            regimentToPreselect.Dispose();
        }

        //==================================================================================================================
        //Rectangle OnScreen
        //==================================================================================================================
        private void OnGUI()
        {
            if (!ClickDragPerformed) return;
            // Create a rect from both mouse positions
            Rect rect = GetScreenRect(startLMouse, endLMouse);
            DrawScreenRect(rect);
            DrawScreenRectBorder(rect, 1);
        }
        
        //==================================================================================================================
        //Methods: Preselection
        //==================================================================================================================
        private void AddPreselection(IRegiment regiment) => preselectionRegister.OnEnableHighlight(regiment);
        private void RemovePreselection(IRegiment regiment) => preselectionRegister.OnDisableHighlight(regiment);
        private void ClearPreSelections() => preselectionRegister.OnClearHighlight();
        
        
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
    */
}
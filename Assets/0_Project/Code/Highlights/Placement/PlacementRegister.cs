using System.Collections;
using System.Collections.Generic;
using KWUtils;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    public sealed class PlacementRegister : HighlightBehaviour
    {
        private readonly Vector3 yOffset = new Vector3(0, 0.05f, 0);

        private readonly IHighlightSystem highlightSystem;
        
        public List<Regiment> MovingRegiments { get; private set; } = new (2);
        
    //###############################################################################################################
        // NEW : HPA PATHFINDING (Go to: RegimentManager.cs) 
        public HPAPathfinder hpaPathfinder { get; private set; }
    //###############################################################################################################
        
    //===============================================================================================================
    // NEW : Placement Destination
        public Dictionary<int, IHighlightable[]> DynamicPlacements { get; private set; }
        public Dictionary<int, Vector3> LeadersDestination { get; private set; }
    //===============================================================================================================

        public PlacementRegister(IHighlightSystem system, GameObject prefab, List<Regiment> regiments)
        {
            highlightSystem = system;
            //Fixe Points
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);

            IHighlightRegister registerInterface = this;
            HighlightRegister = registerInterface;
            
        //###############################################################################################################
        // NEW : HPA PATHFINDING (Go to: RegimentManager.cs) 
            hpaPathfinder = ((RegimentManager)highlightSystem.Coordinator).hpaPathfinder;
        //###############################################################################################################
            
        //===============================================================================================================
        // NEW : Placement Destination
            DynamicPlacements = new (regiments.Count);
            LeadersDestination = new (regiments.Count);
        //===============================================================================================================
            foreach (Regiment regiment in regiments)
            {
                registerInterface.RegisterNewRegiment<Placement>(regiment, regiment.RegimentClass.PrefabPlacement);
        //===============================================================================================================
        // NEW : Placement Destination
                CreateDuplicate(regiment);
                LeadersDestination.TryAdd(regiment.RegimentID, new Vector3());
        //===============================================================================================================
            }
            SetUpTransformAccess();
        }
        


    //===============================================================================================================
    // NEW : Placement Destination
        private void CreateDuplicate(Regiment regiment)
        {
            int regimentID = regiment.RegimentID;
            GameObject prefabUsed = regiment.RegimentClass.PrefabPlacement;

            DynamicPlacements.TryAdd(regimentID, new IHighlightable[regiment.UnitsTransform.Length]);
            for (int i = 0; i < DynamicPlacements[regimentID].Length; i++)
            {
                Vector3 unitPosition = regiment.UnitsTransform[i].position;

                GameObject high = GameObject.Instantiate(prefabUsed, unitPosition + Vector3.up * 0.05f, Quaternion.identity);
                IHighlightable highlight = high.GetComponent<IHighlightable>();

                DynamicPlacements[regimentID][i] = highlight;
            }
        }
        
        public void EnableAllDynamicSelected()
        {
            for (int i = 0; i < highlightSystem.Coordinator.SelectedRegiments.Count; i++)
            {
                OnEnableDynamicHighlight(highlightSystem.Coordinator.SelectedRegiments[i]);
            }
        }
        
        public void OnEnableDynamicHighlight(Regiment regiment)
        {
            if (regiment == null) return;
            if (!DynamicPlacements.TryGetValue(regiment.RegimentID, out IHighlightable[] highlights)) return;
            for (int i = 0; i < highlights.Length; i++)
            {
                highlights[i].HighlightRenderer.enabled = true;
            }
        }
        
        public void OnClearDynamicHighlight()
        {
            foreach ((_, IHighlightable[] highlights) in DynamicPlacements)
            {
                for (int i = 0; i < highlights.Length; i++)
                {
                    highlights[i].HighlightRenderer.enabled = false;
                }
            }
        }

        public void SwapDynamicToStatic()
        {
            foreach (Regiment regiment in highlightSystem.Coordinator.SelectedRegiments)
            {
                int numSelected = highlightSystem.Coordinator.SelectedRegiments.Count;
                for (int i = 0; i < numSelected; i++)
                {
                    int id = regiment.RegimentID;
                    for (int j = 0; j < DynamicPlacements[id].Length; j++)
                    {
                        Vector3 dynamicPosition = DynamicPlacements[id][j].HighlightTransform.position;
                        Quaternion dynamicRotation = DynamicPlacements[id][j].HighlightTransform.rotation;
                        Records[id][j].HighlightTransform.SetPositionAndRotation(dynamicPosition, dynamicRotation);
                    }
                }
            }
        }
    //===============================================================================================================
        public void AddMovingRegiment(Regiment regiment)
        {
            regiment.SetMoving(true);
            if (MovingRegiments.Contains(regiment)) return;
            MovingRegiments.Add(regiment);
        }
        
        public void RemoveMovingRegiment(Regiment regiment)
        {
            regiment.SetMoving(false);
            MovingRegiments.Remove(regiment);
        }

        public override void OnEnableHighlight(Regiment regiment)
        {
            //Static Token
            if (!regiment.IsMoving)
            {
                base.OnEnableHighlight(regiment);
                return;
            }
            
            //If Moving we dont reposition token!
            if (regiment == null) return;
            if (!Records.TryGetValue(regiment.RegimentID, out IHighlightable[] highlights)) return;
            for (int i = 0; i < highlights.Length; i++)
            {
                highlights[i].HighlightRenderer.enabled = true;
            }
        }

        public void OnEnableAll()
        {
            foreach (Regiment r in highlightSystem.Coordinator.Regiments)
            {
                OnEnableHighlight(r);
            }
        }

        public void EnableAllSelected()
        {
            for (int i = 0; i < highlightSystem.Coordinator.SelectedRegiments.Count; i++)
            {
                OnEnableHighlight(highlightSystem.Coordinator.SelectedRegiments[i]);
            }
        }
    }
}

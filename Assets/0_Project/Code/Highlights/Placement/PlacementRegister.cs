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
        
        public List<Regiment> MovingRegiments { get; private set; } = new List<Regiment>(2);

        public PlacementRegister(IHighlightSystem system, GameObject prefab, List<Regiment> regiments)
        {
            highlightSystem = system;
            
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);
            
            IHighlightRegister registerInterface = this;
            HighlightRegister = registerInterface;

            foreach (Regiment regiment in regiments)
            {
                registerInterface.RegisterNewRegiment<Placement>(regiment, regiment.RegimentClass.PrefabPlacement);
            }
            SetUpTransformAccess();
        }

        public void AddMovingRegiment(Regiment regiment)
        {
            regiment.SetMoving(true);
            MovingRegiments.Add(regiment);
        }
        
        public void RemoveMovingRegiment(Regiment regiment)
        {
            regiment.SetMoving(false);
            MovingRegiments.Remove(regiment);
        }

        public override void OnEnableHighlight(Regiment regiment)
        {
            if (!regiment.IsMoving)
            {
                base.OnEnableHighlight(regiment);
                return;
            }
            
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
    }
}

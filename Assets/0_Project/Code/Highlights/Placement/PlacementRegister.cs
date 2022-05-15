using System.Collections;
using System.Collections.Generic;
using KWUtils;
using UnityEngine;

namespace KaizerWald
{
    public sealed class PlacementRegister : HighlightBehaviour, IHighlightRegister
    {
        private readonly Vector3 yOffset = new Vector3(0, 0.05f, 0);

        private IHighlightSystem system;
        
        public GameObject Prefab { get; set; }
        public Dictionary<int, IHighlightable[]> Records { get; set; }

        public PlacementRegister(IHighlightSystem system, GameObject prefab, List<Regiment> regiments)
        {
            this.system = system;

            //Prefab = prefab;
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);
            
            HighlightRegister = this;
            IHighlightRegister registerInterface = this;
            
            foreach (Regiment regiment in regiments)
            {
                registerInterface.RegisterNewRegiment<Placement>(regiment, regiment.RegimentClass.PrefabPlacement);
            }
        }

        public void OnEnableAll()
        {
            foreach (Regiment r in system.Coordinator.Regiments)
            {
                OnEnableHighlight(r);
            }
            /*
            int index = 0;
            foreach ((_, IHighlightable[] placements) in Records)
            {
                for (int i = 0; i < placements.Length; i++)
                {
                    Transform unitTransform = coordinator.Regiments[index].Units[i];
                    Vector3 unitPosition = unitTransform.position + yOffset;
                
                    if (unitTransform.position.xz() != placements[i].HighlightTransform.position.xz())
                    {
                        placements[i].HighlightTransform.position = unitPosition;
                    }
                    placements[i].HighlightRenderer.enabled = true;
                }
                index++;
            }
            */
        }
    }
}

using System;
using System.Collections.Generic;
using KWUtils;
using UnityEngine;

namespace KaizerWald
{
    
    public abstract class HighlightBehaviour
    {
        private readonly Vector3 yOffset = new Vector3(0, 0.05f, 0); 
        
        protected IHighlightRegister HighlightRegister;
        private Dictionary<int, IHighlightable[]> Highlights => HighlightRegister.Records;

        public virtual void OnEnableHighlight(Regiment regiment)
        {
            if (regiment == null) return;
            
            if (!Highlights.TryGetValue(regiment.RegimentID, out IHighlightable[] highlights)) return;
            
            for (int i = 0; i < highlights.Length; i++)
            {
                Vector3 unitPosition = regiment.Units[i].position + yOffset;
                
                if (regiment.Units[i].position.xz() != highlights[i].HighlightTransform.position.xz())
                {
                    highlights[i].HighlightTransform.position = unitPosition;
                }
                
                highlights[i].HighlightRenderer.enabled = true;
            }
        }

        public virtual void OnDisableHighlight(Regiment regiment)
        {
            if (regiment == null) return;
            if (!Highlights.TryGetValue(regiment.RegimentID, out IHighlightable[] highlights)) return;
            
            for (int i = 0; i < highlights.Length; i++)
            {
                highlights[i].HighlightRenderer.enabled = false;
            }
        }

        public virtual void OnClearHighlight()
        {
            foreach ((_, IHighlightable[] highlights) in Highlights)
            {
                for (int i = 0; i < highlights.Length; i++)
                {
                    highlights[i].HighlightRenderer.enabled = false;
                }
            }
        }
/*
        protected virtual void OnRegimentDestroyed(UnitComponent unit)
        {
            Highlights.Remove(unit.Regiment);
        }

        protected void OnUnitDestroyed(UnitComponent unit)
        {
            for (int i = Highlights[unit.Regiment].Count - 1; i > -1; i--)
            {
                if (Highlights[unit.Regiment][i].Unit != unit) continue;
                Highlights[unit.Regiment].Remove(Highlights[unit.Regiment][i]);
                break;
            }
        }
        */
    }
    
}
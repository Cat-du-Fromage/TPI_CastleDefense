using System;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    
    public abstract class HighlightBehaviour : MonoBehaviour
    {
        //protected abstract Dictionary<Regiment, List<T>> Highlights { get; set; }
        protected IHighlightRegister HighlightRegister;
        private Dictionary<int, IHighlightable[]> Highlights => HighlightRegister.Records;
        
        public virtual void OnEnableHighlight(Regiment regiment)
        {
            if (regiment == null) return;
            if (!Highlights.TryGetValue(regiment.RegimentID, out IHighlightable[] highlights)) return;

            for (int i = 0; i < highlights.Length; i++)
            {
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
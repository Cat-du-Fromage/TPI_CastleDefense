using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    
    public abstract class HighlightBehaviour : IHighlightRegister
    {
        //Interface
        public GameObject Prefab { get; protected set; }
        public Dictionary<int, IHighlightable[]> Records { get; protected set; }
        
        private readonly Vector3 yOffset = new Vector3(0, 0.05f, 0); 
        
        protected IHighlightRegister HighlightRegister;
        
        public Dictionary<int, TransformAccessArray> TransformAccessArrays { get; protected set; }

        public virtual void SetUpTransformAccess()
        {
            TransformAccessArrays = new Dictionary<int, TransformAccessArray>(Records.Count);
            foreach ((int regimentIndex, IHighlightable[] highlightables) in Records)
            {
                TransformAccessArrays.Add(regimentIndex, new TransformAccessArray(highlightables.Length));
                Transform[] transforms = new Transform[highlightables.Length];
                for (int i = 0; i < transforms.Length; i++)
                {
                    transforms[i] = highlightables[i].HighlightTransform;
                }
                TransformAccessArrays[regimentIndex].SetTransforms(transforms);
            }
        }

        public void DisposeAllTransformAccessArrays()
        {
            if (TransformAccessArrays == null) return;
            if (TransformAccessArrays.Count == 0) return;
            foreach ((_, TransformAccessArray transformAccessArray) in TransformAccessArrays)
            {
                if (!transformAccessArray.isCreated) continue;
                transformAccessArray.Dispose();
            }
        }

        public virtual void OnEnableHighlight(Regiment regiment)
        {
            if (regiment == null) return;
            
            if (!Records.TryGetValue(regiment.RegimentID, out IHighlightable[] highlights)) return;
            
            for (int i = 0; i < highlights.Length; i++)
            {
                Vector3 unitPosition = regiment.UnitsTransform[i].position + yOffset;
                
                if (regiment.UnitsTransform[i].position.xz() != highlights[i].HighlightTransform.position.xz())
                {
                    highlights[i].HighlightTransform.position = unitPosition;
                }
                
                highlights[i].HighlightRenderer.enabled = true;
            }
        }

        public virtual void OnDisableHighlight(Regiment regiment)
        {
            if (regiment == null) return;
            if (!Records.TryGetValue(regiment.RegimentID, out IHighlightable[] highlights)) return;
            
            for (int i = 0; i < highlights.Length; i++)
            {
                highlights[i].HighlightRenderer.enabled = false;
            }
        }

        public virtual void OnClearHighlight()
        {
            foreach ((_, IHighlightable[] highlights) in Records)
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
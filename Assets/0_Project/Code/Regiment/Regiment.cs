using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    public class Regiment : MonoBehaviour, ISelectable
    {
        //Interface
        public bool IsPreselected { get; set; }
        public bool IsSelected { get; set; }
        
        //Properties
        public int RegimentID { get; set; }
        public IHighlightCoordinator HighlightCoordinator { get; set; }

        public Unit[] Units { get; private set; }
        public Transform[] UnitsTransform { get; set; }
        public TransformAccessArray UnitsTransformAccessArray { get; private set; }
        
        public int CurrentLineFormation { get; private set; }
        public RegimentClass RegimentClass { get; private set; }
        
        //Mouvement
        public bool IsMoving { get; private set; }

        //Shoot behaviour
        public bool IsPlayer { get; set; }
        public float regimentShootRange = 10f;
        public Regiment currentTarget;
        
        //==============================================================================================================
        // Private Setters
        //==============================================================================================================
        public void SetMoving(bool state)
        {
            IsMoving = state;
            float speed = state ? 6 : 0;
            for (int i = 0; i < Units.Length; i++)
            {
                Units[i].Animation.SetSpeed(speed);
            }
        }

        public void SetRegimentClass(RegimentClass regimentClass)
        {
            RegimentClass = regimentClass;
            CurrentLineFormation = RegimentClass.MinRow;
        }
        public void SetCurrentLineFormation(int unitPerLine) => CurrentLineFormation = unitPerLine;

        //==============================================================================================================
        // Unity Events
        //==============================================================================================================
        
        private void Start()
        {
            UnitsTransformAccessArray = new TransformAccessArray(UnitsTransform, UnitsTransform.Length);
            SetUpUnitsComponent();
        }
        
        private void OnDestroy()
        {
            if(UnitsTransformAccessArray.isCreated) UnitsTransformAccessArray.Dispose();
        }

        //==============================================================================================================

        private void SetUpUnitsComponent()
        {
            Units = new Unit[UnitsTransform.Length];
            for (int i = 0; i < UnitsTransform.Length; i++)
            {
                Units[i] = UnitsTransform[i].GetComponent<Unit>();
            }
        }
        
        //UNIT REARRANGE HERE!
        public void OnUnitKilled(int unitIndexInRegiment)
        {
            //Units.Rearrange(unitIndexInRegiment);
            HighlightCoordinator.OnUnitKilled(RegimentID, unitIndexInRegiment);
            //Faire le ménage ICI
            
            //Placer l'algorithme ici
            
            //Envoyer le message au IScrivener
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class Unit : MonoBehaviour, IUnit
    {
        public Regiment RegimentAttach { get; set; }
        public int IndexInRegiment { get; set; }
        //public void SetRegiment(IRegiment assignedRegiment) => Regiment = assignedRegiment;
        public UnitAnimation Animation { get; private set; }

        private void Awake()
        {
            Animation = GetComponent<UnitAnimation>();
        }

        public void OnDeath()
        {
            
        }
    }
}
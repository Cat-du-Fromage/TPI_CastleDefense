using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    public class Regiment : RegimentBehaviour//  MonoBehaviour, IRegiment
    {
        private void Start()
        {
            UnitsTransformAccessArray = new TransformAccessArray(Units, Units.Length);
        }

        private void OnDestroy()
        {
            if(UnitsTransformAccessArray.isCreated) UnitsTransformAccessArray.Dispose();
        }
    }
}

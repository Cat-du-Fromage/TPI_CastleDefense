using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class Regiment : MonoBehaviour, IRegiment
    {
        public Transform[] Units { get; set; }
        public IGeneral General { get; set; }
        public int RegimentID { get; set; }
        
        public RegimentClass RegimentClass { get; private set; }
        public void SetRegimentClass(RegimentClass regimentClass) => RegimentClass = regimentClass;
    }
}

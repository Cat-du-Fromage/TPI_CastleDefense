using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class RegimentManager : MonoBehaviour, ICoordinator
    {
        [SerializeField] private GameObject prefabPreselection;
        [SerializeField] private GameObject prefabSelection;
        public List<Regiment> Regiments { get; set; }
        public IHighlightRegister[] Registers { get; set; }

        private PreselectionRegister preselectionRegister;
        private SelectionRegister selectionRegister;

        private void Awake()
        {
            Regiments = GetComponent<RegimentFactory>().CreateRegiments();
            
            /*
            List<Regiment> regimentsNoInterface = GetComponent<RegimentFactory>().CreateRegiments();
            Regiments = new List<IRegiment>(regimentsNoInterface.Count);

            foreach (Regiment regiment in regimentsNoInterface)
            {
                IRegiment iRegiment = regiment;
                Regiments.Add(iRegiment);
            }
*/
            preselectionRegister = new PreselectionRegister(prefabPreselection, Regiments);
            selectionRegister = new SelectionRegister(prefabSelection, Regiments);
        }

        private void Start()
        {
            Registers = new IHighlightRegister[] {preselectionRegister, selectionRegister};
        }
    }
}

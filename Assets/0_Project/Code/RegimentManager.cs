using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public interface IUnit
    {
        IRegiment RegimentAttach { get; set; }
        public int IndexInRegiment { get; set; }

        public void OnKilled() => RegimentAttach.OnUnitKilled(IndexInRegiment);
    }
    
    public interface ICoordinator
    {
        public List<IRegiment> Regiments { get; set; }
        IHighlightRegister[] Registers { get; set; }

        public void OnRegimentUpdate(IRegiment regiment)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].Records.Remove(regiment.RegimentID);
            }
            Regiments.Remove(regiment);
        }
        
        public void OnUnitUpdate(int regimentID, int unitIndexInRegiment)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].OnUnitUpdate(regimentID, unitIndexInRegiment);
            }
        }
    }
    
    //Sert a enregistrer les composantes des unités des régiments
    //T peut être "IHighlightable"
    //Donc si Highlight => Dictionary<int, IHighlightable[]>


    public class RegimentManager : MonoBehaviour, ICoordinator
    {
        [SerializeField] private GameObject prefabPreselection;
        [SerializeField] private GameObject prefabSelection;
        public List<IRegiment> Regiments { get; set; }
        public List<Regiment> RegimentsNoInterface { get; set; }
        public IHighlightRegister[] Registers { get; set; }

        private PreselectionRegister preselectionRegister;
        private SelectionRegister selectionRegister;

        private void Awake()
        {
            preselectionRegister = GetComponent<PreselectionRegister>();
            selectionRegister = GetComponent<SelectionRegister>();
                
            RegimentsNoInterface = GetComponent<RegimentFactory>().CreateRegiments();
            Regiments = new List<IRegiment>(RegimentsNoInterface.Count);

            foreach (Regiment regiment in RegimentsNoInterface)
            {
                IRegiment iRegiment = regiment;
                //if (regiment is IRegiment iRegiment)
                //{
                Regiments.Add(iRegiment);
                //}
            }

            preselectionRegister.InitializeRegister(prefabPreselection, Regiments);
            selectionRegister.InitializeRegister(prefabSelection, Regiments);
        }

        private void Start()
        {
            Registers = new IHighlightRegister[2];
            Registers[0] = preselectionRegister.transform.GetComponent<IHighlightRegister>();
            Registers[1] = selectionRegister.transform.GetComponent<IHighlightRegister>();

            for (int i = 0; i < Registers.Length; i++)
            {
                Debug.Log($"{Registers[i].Records.Count}");
            }
        }
    }
}

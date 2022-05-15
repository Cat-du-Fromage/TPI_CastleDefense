using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public interface IHighlightCoordinator
    {
        public List<Regiment> Regiments { get; set; }
        IHighlightRegister[] Registers { get; set; }
        
        public PreselectionRegister PreselectionRegister { get; }
        public SelectionSystem SelectionSystem { get; }
        public PlacementSystem PlacementSystem { get; }
        public PlayerControls Controls { get; }
        public List<Regiment> PreselectedRegiments => PreselectionRegister.PreselectedRegiments;
        public List<Regiment> SelectedRegiments => SelectionSystem.Register.SelectedRegiments;
        

        public void OnRegimentKilled(Regiment regiment)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].Records.Remove(regiment.RegimentID);
            }
            Regiments.Remove(regiment);
        }
        
        public void OnUnitKilled(int regimentID, int unitIndexInRegiment)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].OnUnitUpdate(regimentID, unitIndexInRegiment);
            }
        }

        public void DispatchEvent(HighlightBehaviour sender)
        {
            if (sender is SelectionRegister)
            {
                PreselectionRegister.OnClearHighlight();
            }
        }
    }
}
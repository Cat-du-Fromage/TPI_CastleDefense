using System.Collections.Generic;

namespace KaizerWald
{
    public interface ICoordinator
    {
        public List<Regiment> Regiments { get; set; }
        IHighlightRegister[] Registers { get; set; }
        
        public PreselectionRegister PreselectionRegister { get; }
        public SelectionRegister SelectionRegister { get; }

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
    }
}
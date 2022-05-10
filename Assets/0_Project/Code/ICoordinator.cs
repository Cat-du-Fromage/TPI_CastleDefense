using System.Collections.Generic;

namespace KaizerWald
{
    public interface ICoordinator
    {
        public List<Regiment> Regiments { get; set; }
        IHighlightRegister[] Registers { get; set; }

        public void OnRegimentUpdate(Regiment regiment)
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
}
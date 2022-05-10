namespace KaizerWald
{
    public interface IUnit
    {
        IRegiment RegimentAttach { get; set; }
        public int IndexInRegiment { get; set; }

        public void OnKilled() => RegimentAttach.OnUnitKilled(IndexInRegiment);
    }
}
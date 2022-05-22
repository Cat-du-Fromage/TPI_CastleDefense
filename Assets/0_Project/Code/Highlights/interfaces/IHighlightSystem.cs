namespace KaizerWald
{
    public interface IHighlightSystem
    {
        public IHighlightRegister Register { get; }
        
        public IHighlightCoordinator Coordinator { get; }
    }
}
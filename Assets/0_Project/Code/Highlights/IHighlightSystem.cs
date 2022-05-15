namespace KaizerWald
{
    public interface IHighlightSystem
    {
        IHighlightCoordinator Coordinator { get; }
        IHighlightRegister Register { get; }
    }
}
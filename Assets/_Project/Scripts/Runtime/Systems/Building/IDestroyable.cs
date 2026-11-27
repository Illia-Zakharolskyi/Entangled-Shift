namespace EntangledShift.Systems.Building
{
    public interface IDestroyable
    {
        string Name { get; }
        void Destroy();
    }
}

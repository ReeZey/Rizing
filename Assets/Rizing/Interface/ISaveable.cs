namespace Rizing.Interface
{
    public interface ISaveable
    {
        object SaveState();
        void LoadState(object state);
    }
}
public interface ISaveable
{
    object SaveState();
    void LoadState(object state);
}
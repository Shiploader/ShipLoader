namespace ShipLoader.API
{
    public interface IQueueObject
    {
        bool Init();
        bool Conflicts(IQueueObject obj);
    }
}

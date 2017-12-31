namespace BluePhyre.Core.Interfaces
{
    public interface IHasStatus
    {
        bool Active { get; }
    }

    public interface IListItem
    {
        long? Id { get; set; }
        string Name { get; set; }
        bool Active { get; set; }
    }
}
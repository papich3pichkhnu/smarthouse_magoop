namespace SmartHome
{
    public interface IVisitor
    {
        void Visit(Room room);
        void Visit(Device device);

    }
}

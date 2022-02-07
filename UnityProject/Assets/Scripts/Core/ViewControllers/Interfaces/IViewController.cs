namespace Core.Controllers
{
    public interface IViewController
    {
        void Enable(object view);
        void Disable();
    }

    public interface IViewControllerData
    {
        void Set(object data);
    }
}

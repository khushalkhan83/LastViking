namespace Game.Views
{
    public interface INotification
    {
        void PlayShowTop();
        void PlayHideTop();
        void PlayShow();

        void SetAsLast();
        void SetAsFirst();
    }
}

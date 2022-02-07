
namespace Core.Extensions
{
    public static class CoreExtensions
    {
        public static T Inject<T>(this T target, InjectionSystem injectionSystem)
        {
            injectionSystem.Inject(target);
            return target;
        }

    }
}
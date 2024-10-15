using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.Services.WorldEvents
{
    public interface IWorldEventSubscriptionControlService
    {
        public void SubscribeToWorldEvents();
        public void UnsubscribeFromWorldEvents();
    }
}

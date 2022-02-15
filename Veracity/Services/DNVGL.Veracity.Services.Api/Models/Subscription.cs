namespace DNVGL.Veracity.Services.Api.Models
{
    public class Subscription
    {
        public ServiceReference Service { get; set; }
        public UserReference User { get; set; }
        public RoleReference Role { get; set; }
        public SubscriptionState SubscriptionState { get; set; }
    }

    public class SubscriptionState
    {
        public SubscriptionStates State { get; set; }
    }

    public enum SubscriptionStates
    {
        Subscribing
    }
}

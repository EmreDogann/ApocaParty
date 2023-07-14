using GuestRequests.Jobs;

namespace GuestRequests.Requests
{
    public class FoodRequest : Request
    {
        public FoodRequest()
        {
            _jobs.Add(new CookJob("Cooking 1"));
            _jobs.Add(new CookJob("Cooking 2"));
        }
    }
}
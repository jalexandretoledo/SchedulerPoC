using Akka.Actor;

namespace SchedulerPoC
{
    public class SchedActorSystem
    {

        private ActorSystem system;

        public SchedActorSystem()
        {
            system = ActorSystem.Create("SchedulerPoCActorSystem");
        }

        public async void Stop()
        {
            await system.Terminate();
        }

    }
}
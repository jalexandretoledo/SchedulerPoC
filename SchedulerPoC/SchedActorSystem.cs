using Akka.Actor;
using SchedulerPoC.Actors;

namespace SchedulerPoC
{
    public class SchedActorSystem
    {

        public ActorSystem System { get; }
        public IActorRef Coordinator { get; } 

        public SchedActorSystem()
        {
            System = ActorSystem.Create("SchedulerPoCActorSystem");

            Coordinator = System.ActorOf(Props.Create(() => new SchedulerCoordinator(System, null)), "coordinator");
        }

        public async void Stop()
        {
            await System.Terminate();
        }

    }
}
using Akka.Actor;
using SchedulerPoC.GUI.Actors;
using SchedulerPoC.GUI.Messages;
using SchedulerPoC.Tasks;

namespace SchedulerPoC.GUI
{
    public partial class Form1 : Form
    {
        public bool AlreadyLoaded { get; private set; }
        private IActorRef guiActor;


        public Form1()
        {
            InitializeComponent();
            guiActor = Program.SchedulerActorSystem.System.ActorOf(Props.Create(() => new GUIActor(Program.SchedulerActorSystem.Coordinator, gridTasks)), "guiActor");
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (AlreadyLoaded)
            {
                MessageBox.Show(this, "Task definitions already loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dlgOpen.ShowDialog(this) == DialogResult.OK)
            {
                guiActor.Tell(new LoadFromFile(dlgOpen.FileName));
                AlreadyLoaded = true;
            };
        }

        public class GridRowWrapper
        {
            public ScheduledTaskStatus Task { get; }
            private string _time { get; }

            public GridRowWrapper(ScheduledTaskStatus st)
            {
                Task = st;
                _time = Task.ScheduledTask.Schedule.ToString("HH:mm");
            }

            public string TaskId => Task.ScheduledTask.TaskId;
            public string Time => _time;
            public string Description => Task.ScheduledTask.Task.Description;
            public string Status => Task.Status.ToString();
        }

    }
}
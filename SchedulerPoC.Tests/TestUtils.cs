using NUnit.Framework;
using NUnit.Framework.Internal;
using SchedulerPoC.Tasks;

namespace SchedulerPoC.Tests
{

    [TestFixture]
    public class TestUtils
    {

        [SetUp]
        public void SetUp()
        {

        }

        [TestCase( 3, 45, "Python Task Test", @"C:\some path\some script.py", "env_name")]
        [TestCase(13,  5, "Python Task Test", @"C:\some path\some script.py", "env_name")]
        [TestCase(22, 25, "Python Task Test", @"C:\some path\some script.py", "env_name")]
        [TestCase(18, 35, "Python Task Test", @"C:\some path\some script.py", "env_name", "param1")]
        [TestCase(23, 55, "Python Task Test", @"C:\some path\some script.py", "env_name", "param1", "param2")]
        public void TestPyTaskInterpreter(int hour, int minute, string description, string script, string env, params string[] parameters)
        {
            var paramsAsStr = String.Join(", ", parameters);
            var src = $"{hour,2}:{minute:00} PY({description}, {script}, [{paramsAsStr}], {env})";
            Console.WriteLine("Testing '{0}'", src);

            src.AsTask().Match(
                error => Assert.Fail(error),
                sched =>
                {
                    Assert.AreEqual(hour, sched.Schedule.Hour, "Horário de agendamento: {0}", src);
                    Assert.AreEqual(minute, sched.Schedule.Minute, "Horário de agendamento: {0}", src);

                    var task = sched.Task;
                    if (task is PythonTask pyTask)
                    {
                        Assert.AreEqual(description, pyTask.Description, "Description");
                        Assert.AreEqual(script, pyTask.PythonScript, "Script");
                        Assert.AreEqual(env, pyTask.VirtualEnv, "VirtualEnv");
                        Assert.AreEqual(parameters.Length, pyTask.Parameters.Length, "# of Task parameters => [{0}]", String.Join(", ", pyTask.Parameters.Select(r => $"'{r}'")));

                        for (var i = 0; i < parameters.Length; i++)
                        {
                            Assert.AreEqual(parameters[i], pyTask.Parameters[i], "Task parameter #{0}", i);
                        }
                    }
                    else
                    {
                        Assert.Fail("Task não é uma PythonTask: {0} => {1}", src, task);
                    }
                });
        }




    }
}
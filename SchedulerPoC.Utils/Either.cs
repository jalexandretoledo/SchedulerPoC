namespace SchedulerPoC.Utils
{

    /// <summary>
    ///     Source: https://blog.ploeh.dk/2018/06/11/church-encoded-either/
    /// </summary>
    /// <typeparam name="L"></typeparam>
    /// <typeparam name="R"></typeparam>
    public interface IEither<L, R>
    {
        T Match<T>(Func<L, T> onLeft, Func<R, T> onRight);

        void Match(Action<L> onLeft, Action<R> onRight);
    }


    public class Left<L, R> : IEither<L, R>
    {
        private readonly L left;

        public Left(L left)
        {
            this.left = left;
        }

        public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
        {
            return onLeft(left);
        }

        public void Match(Action<L> onLeft, Action<R> onRight)
        {
            onLeft(left);
        }
    }

    public class Right<L, R> : IEither<L, R>
    {
        private readonly R right;

        public Right(R right)
        {
            this.right = right;
        }

        public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
        {
            return onRight(right);
        }

        public void Match(Action<L> onLeft, Action<R> onRight)
        {
            onRight(right);
        }
    }

}
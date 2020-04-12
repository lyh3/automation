namespace McAfeeLabs.Engineering.Automation.Monads
{
    public interface IMaybe<T>{}
    public class Nothing<T> : IMaybe<T>
    {
        public override string ToString()
        {
            return @"Nothing";
        }
    }

    public class Just<T> : IMaybe<T>
    {
        public T Value { get; private set; }
        public Just(T value)
        {
            Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    // The simplest possible Monad, Identity
    // it does nothing more than contain a value of the given the type
    public class Identity<T>
    {
        public T Value { get; private set; }

        public Identity(T value)
        {
            Value = value;
        }
    }
}

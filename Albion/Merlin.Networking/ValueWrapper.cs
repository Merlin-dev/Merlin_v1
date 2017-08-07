namespace Merlin.Networking
{
    public class ValueWrapper<T> where T : struct
    {
        public T Value { get; set; }

        public ValueWrapper(T value)
        {
            Value = value;
        }
    }
}
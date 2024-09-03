namespace MQOD
{
    public abstract class _Feature
    {
        protected bool _initialized;

        public bool initialized
        {
            get => _initialized;
            protected set => _initialized = value;
        }
    }
}
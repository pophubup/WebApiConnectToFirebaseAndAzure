namespace WebApplication1.Utility
{
    public class QueryCondition<T>
    {
        public QueryCondition(QueryComparsion comparsion, T value)
        {
            this.Comparsion = comparsion;
            this.Value = value;
        }

        public QueryComparsion Comparsion { get; private set; }

        public T Value { get; private set; }
    }
}

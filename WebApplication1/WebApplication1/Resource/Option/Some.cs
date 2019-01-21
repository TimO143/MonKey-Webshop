namespace WebApplication1.Resource.Option
{
    public class Some<T> : Option<T>
    {
        public bool empty { get; }
        public T data { get; set; }

        public Some(T data)
        {
            empty = false;
            this.data = data;
        }
        public Some()
        {
            empty = false;
        }
    }
}
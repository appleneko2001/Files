namespace Files.Tasks
{
    public class TaskResultModel<T>
    {
        public bool IsSuccessful;
        public T? Result;
    }
}
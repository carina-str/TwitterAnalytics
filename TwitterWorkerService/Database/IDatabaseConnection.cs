namespace TwitterWorkerService
{
    interface IDatabaseConnection<T>
    {
        public bool Exists(string fieldDefinition, string value);
        public void Insert(T value);
        public void Update(T value);
        public void Delete(T value);
    }
}

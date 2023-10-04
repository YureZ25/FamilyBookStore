namespace Services.Exeptions
{
    public class EntityNotFoundExeption : ApplicationException
    {
        public string EntityName { get; set; }

        public object EntityKey { get; set; }

        public EntityNotFoundExeption(string entityName, object entityKey) : this(entityName, entityKey, null)
        {
            
        }

        public EntityNotFoundExeption(string entityName, object entityKey, Exception innerException) : base($"Объект \"{entityName}\" с идентификатором {entityKey} не найден", innerException)
        {
            EntityName = entityName;
            EntityKey = entityKey;
        }
    }
}

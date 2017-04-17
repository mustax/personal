namespace Data.Entities.Builders
{
    public interface ICallLogBuilder
    {
        CallLog Build(string logEntry);
    }
}
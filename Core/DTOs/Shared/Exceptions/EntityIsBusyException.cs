using System.Globalization;


namespace DTOs.Shared.Exceptions
{
    public class EntityIsBusyException : Exception
    {
        public EntityIsBusyException() : base() { }

        public EntityIsBusyException(string message) : base(message) { }

        public EntityIsBusyException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public EntityIsBusyException(string name, object key)
            : base($"Entity \"{name}\" ({key}) Is Busy and Can't be deleted.")
        {
        }
    }
}

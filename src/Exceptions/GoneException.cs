using Danorjuela.Utils.ResponseProcessor.Interface;

namespace Danorjuela.Utils.ResponseProcessor.Exceptions
{
    /// <summary>
    /// Se utiliza cuando el recurso existio pero ya no esta disponible
    /// de forma permanente
    /// </summary>
    public class GoneException<T> : Exception, IGoneException
    {
        public T ObjectData { get; set; }

        public GoneException(string message, T data)
            : base(message)
        {
            ObjectData = data;
        }
    }


    public class GoneException : Exception, IGoneException
    {
        public GoneException(string message)
            : base(message)
        { }
    }
}

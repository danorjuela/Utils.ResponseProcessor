using Danorjuela.Utils.ResponseProcessor.Interface;

namespace Danorjuela.Utils.ResponseProcessor.Exceptions
{
    /// <summary>
    /// Se utiliza cuando se supera el limite de solicitudes permitidas
    /// </summary>
    public class TooManyRequestsException<T> : Exception, ITooManyRequestsException
    {
        public T ObjectData { get; set; }

        public TooManyRequestsException(string message, T data)
            : base(message)
        {
            ObjectData = data;
        }
    }


    public class TooManyRequestsException : Exception, ITooManyRequestsException
    {
        public TooManyRequestsException(string message)
            : base(message)
        { }
    }
}

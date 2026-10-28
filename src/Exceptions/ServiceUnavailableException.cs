using Danorjuela.Utils.ResponseProcessor.Interface;

namespace Danorjuela.Utils.ResponseProcessor.Exceptions
{
    /// <summary>
    /// Se utiliza cuando el servicio o una dependencia externa
    /// no esta disponible temporalmente
    /// </summary>
    public class ServiceUnavailableException<T> : Exception, IServiceUnavailableException
    {
        public T ObjectData { get; set; }

        public ServiceUnavailableException(string message, T data)
            : base(message)
        {
            ObjectData = data;
        }
    }


    public class ServiceUnavailableException : Exception, IServiceUnavailableException
    {
        public ServiceUnavailableException(string message)
            : base(message)
        { }
    }
}

using Danorjuela.Utils.ResponseProcessor.Interface;

namespace Danorjuela.Utils.ResponseProcessor.Exceptions
{
    /// <summary>
    /// Se utiliza cuando no se cumple una precondicion de la solicitud
    /// (por ejemplo una version o ETag desactualizada)
    /// </summary>
    public class PreconditionFailedException<T> : Exception, IPreconditionFailedException
    {
        public T ObjectData { get; set; }

        public PreconditionFailedException(string message, T data)
            : base(message)
        {
            ObjectData = data;
        }
    }


    public class PreconditionFailedException : Exception, IPreconditionFailedException
    {
        public PreconditionFailedException(string message)
            : base(message)
        { }
    }
}

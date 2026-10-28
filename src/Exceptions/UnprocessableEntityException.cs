using Danorjuela.Utils.ResponseProcessor.Interface;

namespace Danorjuela.Utils.ResponseProcessor.Exceptions
{
    /// <summary>
    /// Se utiliza cuando la solicitud esta bien formada pero
    /// contiene errores de validacion
    /// </summary>
    public class UnprocessableEntityException<T> : Exception, IUnprocessableEntityException
    {
        public T ObjectData { get; set; }

        public UnprocessableEntityException(string message, T data)
            : base(message)
        {
            ObjectData = data;
        }
    }


    public class UnprocessableEntityException : Exception, IUnprocessableEntityException
    {
        public UnprocessableEntityException(string message)
            : base(message)
        { }
    }
}

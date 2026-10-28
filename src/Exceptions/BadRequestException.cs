using Danorjuela.Utils.ResponseProcessor.Interface;

namespace Danorjuela.Utils.ResponseProcessor.Exceptions
{
    /// <summary>
    /// Se utiliza cuando la solicitud es invalida
    /// (parametros incorrectos o mal formados)
    /// </summary>
    public class BadRequestException<T> : Exception, IBadRequestException
    {
        public T ObjectData { get; set; }

        public BadRequestException(string message, T data)
            : base(message)
        {
            ObjectData = data;
        }
    }


    public class BadRequestException : Exception, IBadRequestException
    {
        public BadRequestException(string message)
            : base(message)
        { }
    }
}

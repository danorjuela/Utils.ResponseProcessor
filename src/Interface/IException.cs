using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danorjuela.Utils.ResponseProcessor.Interface
{
    public interface INotFoundException
    {
    }
    public interface IBusinessException
    {
    }        
    
    public interface IConflicException
    {
    }

    public interface IForbiddenException
    {
    }
    
    public interface INoDataException
    {
    }    
    
    public interface INotAutorizationException
    {
    }

    public interface IBadRequestException
    {
    }

    public interface IGoneException
    {
    }

    public interface IPreconditionFailedException
    {
    }

    public interface IUnprocessableEntityException
    {
    }

    public interface ITooManyRequestsException
    {
    }

    public interface IServiceUnavailableException
    {
    }


}

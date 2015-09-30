using System;
using System.Threading.Tasks;

namespace CTime2.Services.ExceptionHandler
{
    public interface IExceptionHandler
    {
        Task HandleAsync(Exception exception);
    }
}
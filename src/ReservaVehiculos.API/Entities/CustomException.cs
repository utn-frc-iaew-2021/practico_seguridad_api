
using System;
namespace ReservaVehiculos.API.Entities
{
    public class CustomException : Exception
    {
        private readonly int _errorCode;

        public int ErrorCode
        {
            get { return _errorCode; }
        }

        public CustomException(int errorCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            _errorCode = errorCode;
        }

    }
}

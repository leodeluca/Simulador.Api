using System.Runtime.Serialization;

namespace Simulador.Api.Logic.Exceptions
{
    [Serializable]
    public class SimuladorValidationException : Exception
    {
        public SimuladorValidationException(){}

        public SimuladorValidationException(string? message) : base(message) {}

        public SimuladorValidationException(string? message, Exception? innerException) 
            : base(message, innerException) {}

        protected SimuladorValidationException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        internal static void ThrowWhenIsDifferent<T>(T first, T second, string message)
        {
            if (first?.ToString() != second?.ToString())
                throw new SimuladorValidationException(message);
        }
    }
}

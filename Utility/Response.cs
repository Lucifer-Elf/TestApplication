using Microsoft.AspNetCore.Http;

namespace Servize.Utility
{

    public class Response
    {

        public int StatusCode { get; private set; }

        public string Message { get; private set; }

        public bool IsSuccessStatusCode()
        {
            return (StatusCode == StatusCodes.Status200OK);
        }
        public Response(string message, int statusCode)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }

    public class Response<T> : Response
    {
       
        public T Resource { get; private set; }

     
        public Response(T resource, int statusCode) : base("", statusCode)
        {
            Resource = resource;
        }

        public Response(string message, int statusCode) : base(message, statusCode)
        {
            Resource = default;
        }
    }
}


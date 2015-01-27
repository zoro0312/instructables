using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Instructables.DataServices;

namespace Instructables.DataModel
{
    public interface IRequestError
    {
        string toString();
    }

    public class RequestError : IRequestError
    {
        public string error;
        public string toString()
        {
            return error;
        }
    }

    public class RequestResult<T, S>
        where T : class, IRequestError
        where S : class, IRequestError
    {
        private HttpResponseMessage _response;

        public RequestResult(HttpResponseMessage response)
        {
            this._response = response;
        }

        public HttpResponseMessage response
        {
            get
            {
                return _response;
            }
        }

        public bool isSucceeded
        {
            get
            {
                return _response.IsSuccessStatusCode;
            }
        }

        private T _error;
        public T error
        {
            get
            {
                if (_error == null) {
                    Task<string> result =  _response.Content.ReadAsStringAsync();
                    result.Wait();
                    T msg = SerializationHelper.Deserialize<T>(result.Result);
                    _error = msg;
                }
                return _error;
            }
        }

        private S _message;
        public S message
        {
            get
            {
                if (_message == null)
                {
                    Task<string> result = _response.Content.ReadAsStringAsync();
                    result.Wait();
                    S msg = SerializationHelper.Deserialize<S>(result.Result);
                    _message = msg;

                }
                return _message;
            }
        }
    }

    public class RequestResult<T> where T : class, IRequestError
    {
        private HttpResponseMessage _response;

        public RequestResult(HttpResponseMessage response)
        {
            this._response = response;
        }

        public HttpResponseMessage response
        {
            get
            {
                return _response;
            }
        }

        public bool isSucceeded
        {
            get
            {
                return _response.IsSuccessStatusCode;
            }
        }

        private T _error;
        public T error
        {
            get
            {
                if (_error == null)
                {
                    Task<string> result = _response.Content.ReadAsStringAsync();
                    result.Wait();
                    T msg = SerializationHelper.Deserialize<T>(result.Result);
                    _error = msg;
                }
                return _error;
            }
        }
    }


}

using System;
using System.Threading.Tasks;
using Telegram.Td;
using Telegram.Td.Api;

namespace AutoReplyUserBot
{
    public static class TDLibHelper
    {
        public static Task<BaseObject> SendAsyncHelper(this Client client, Function function)
        {
            TdCompletionSource tcs = new TdCompletionSource();
            client.Send(function, tcs);
            return tcs.Task;
        }

        public static async Task<BaseObject> SendAsync(this Client client, Function function)
        {
            BaseObject result = await client.SendAsyncHelper(function);
            if (result is Error error)
            {
                throw new TDLibException(error.Message, error.Code);
            }
            return result;
        }

        /// <summary>
        /// This is used to send async/await requests via TDLib
        /// </summary>
        private class TdCompletionSource : TaskCompletionSource<BaseObject>, ClientResultHandler
        {
            public void OnResult(BaseObject result)
            {
                SetResult(result);
            }
        }
    }


    [Serializable]
    public class TDLibException : Exception
    {
        public int Code { get; private set; }

        public Error _error;

        public TDLibException(Error error) : base(error.Message)
        {
            _error = error;
            Code = _error.Code;
        }
        public TDLibException(string message, int code) : base(message) { Code = code; }
        public TDLibException(string message) : base(message) { }
        public TDLibException(string message, Exception inner) : base(message, inner) { }
        protected TDLibException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

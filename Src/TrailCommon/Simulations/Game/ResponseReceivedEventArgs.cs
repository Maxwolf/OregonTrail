using System;

namespace TrailCommon
{
    /// <summary>
    ///     Awesome IPC using pipes.
    /// </summary>
    /// <remarks>http://stackoverflow.com/a/10434708</remarks>
    public sealed class ResponseReceivedEventArgs : EventArgs
    {
        public ResponseReceivedEventArgs(string id, string response)
        {
            Id = id;
            Response = response;
        }

        public string Id { private set; get; }

        public string Response { private set; get; }
    }

    public delegate void ResponseReceived(object sender, ResponseReceivedEventArgs e);
}
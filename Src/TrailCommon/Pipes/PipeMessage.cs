using System;

namespace TrailCommon
{
    [Serializable]
    public class PipeMessage
    {
        public int Id;
        public string Text;

        public override string ToString()
        {
            return $"\"{Text}\" (message ID = {Id})";
        }
    }
}
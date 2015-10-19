using System;

namespace TrailCommon
{
    [Serializable]
    public class PipeMessage
    {
        public int ID;
        public string Text;

        public override string ToString()
        {
            return $"\"{Text}\" (message ID = {ID})";
        }
    }
}
namespace OregonTrailDotNet.Bot
{
    /// <summary>
    ///     What a control-panel form is asking <see cref="Program" /> to do after it tears the UI down.
    /// </summary>
    public enum BotRequestKindEnum
    {
        None,
        Train,
        Watch,
        AutoTest,
        Benchmark,
        Quit
    }
}

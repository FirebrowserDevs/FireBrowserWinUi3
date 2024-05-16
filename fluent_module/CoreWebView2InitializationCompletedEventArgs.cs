namespace fluent_module
{
    internal class CoreWebView2InitializationCompletedEventArgs
    {
        public object InitializationException { get; internal set; }
        public bool IsSuccess { get; internal set; }
    }
}
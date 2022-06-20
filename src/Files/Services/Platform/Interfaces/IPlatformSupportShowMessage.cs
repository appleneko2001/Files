namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportShowMessage
    {
        /// <summary>
        /// Show a native message window with specified title and content.
        /// </summary>
        /// <param name="title">title of window</param>
        /// <param name="content">message text of window</param>
        void PopupMessageWindow(string title, string content);
    }
}
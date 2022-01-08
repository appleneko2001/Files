// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Files.Windows.Services.Native
{
    /// <summary>
    /// <para>Windows Native API access. Credit by <a href="http://pinvoke.net">pinvoke.net :)</a></para>
    /// <para>To use globally, add below code:
    ///     <code>using static Files.Windows.Services.Native.NativeApi</code>
    /// </para>
    /// </summary>
    public static partial class NativeApi
    {
        /// <summary>
        /// Dll file entry: user32.dll
        /// </summary>
        public const string WinUser = "user32.dll";
        
        /// <summary>
        /// Dll file entry: kernel32.dll
        /// </summary>
        public const string WinKernel = "Kernel32.dll";
        
        /// <summary>
        /// Dll file entry: shell32.dll
        /// </summary>
        public const string WinShell = "Shell32.dll";
    }
}
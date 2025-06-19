using System.Runtime.InteropServices;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Rubedo.Lib;

/// <summary>
/// External SDL2 functions. Be very careful with these!
/// </summary>
public static class SDL2Extern
{
    #region SDL2 Variables
    private const string nativeLibName = "SDL2";
    #endregion

    /* window refers to an SDL_Window* */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowMinimumSize(
        IntPtr window,
        int min_w,
        int min_h
    );
}
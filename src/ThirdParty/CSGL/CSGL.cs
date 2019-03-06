#region License
/*
    This is free and unencumbered software released into the public domain.

    Anyone is free to copy, modify, publish, use, compile, sell, or
    distribute this software, either in source code form or as a compiled
    binary, for any purpose, commercial or non-commercial, and by any
    means.

    In jurisdictions that recognize copyright laws, the author or authors
    of this software dedicate any and all copyright interest in the
    software to the public domain. We make this dedication for the benefit
    of the public at large and to the detriment of our heirs and
    successors. We intend this dedication to be an overt act of
    relinquishment in perpetuity of all present and future rights to this
    software under copyright law.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.

    For more information, please refer to <http://unlicense.org>
*/
#endregion

#region Include
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

#region Loader
namespace CSGL
{
    public static class DLL
    {
        #region DllImport
        [DllImport( "kernel32.dll" )]
        private static extern IntPtr LoadLibrary( string filename );

        [DllImport( "kernel32.dll" )]
        private static extern IntPtr GetProcAddress( IntPtr hModule, string procname );

        [DllImport( "kernel32.dll" )]
        private static extern void CopyMemory( IntPtr dest, IntPtr src, uint count );

        [DllImport( "libdl.so" )]
        private static extern IntPtr dlopen( string filename, int flags );

        [DllImport( "libdl.so" )]
        private static extern IntPtr dlsym( IntPtr handle, string symbol );

        [DllImport( "libc.so.6" )]
        private static extern void memcpy( IntPtr dest, IntPtr src, uint n );

        const int RTLD_NOW = 2;
        #endregion

        #region Abstracted
        private static bool _linuxset = false;
        private static bool _linux = false;

        public static bool __linux__
        {
            get
            {
                if ( !_linuxset )
                {
                    int p = (int)Environment.OSVersion.Platform;
                    _linux = ( p == 4 ) || ( p == 6 ) || ( p == 128 );
                    _linuxset = true;
                }

                return _linux;
            }
        }
        #endregion

        #region Fields
        private static Type _delegateType = typeof( MulticastDelegate );
        #endregion

        #region Methods
        public static IntPtr csglDllLoad( string filename )
        {
            IntPtr mHnd;

            if ( __linux__ )
                mHnd = dlopen( filename, RTLD_NOW );
            else
                mHnd = LoadLibrary( filename );

            if ( mHnd != IntPtr.Zero )
                Console.WriteLine( "Linked '{0}' -> '0x{1}'", filename, mHnd.ToString( "X" ) );
            else
                Console.WriteLine( "Failed to link '{0}'", filename );

            return mHnd;
        }

        public static IntPtr csglDllSymbol( IntPtr mHnd, string symbol )
        {
            IntPtr symPtr;

            if ( __linux__ )
                symPtr = dlsym( mHnd, symbol );
            else
                symPtr = GetProcAddress( mHnd, symbol );

            return symPtr;
        }

        public static Delegate csglDllDelegate( Type delegateType, IntPtr mHnd, string symbol )
        {
            IntPtr ptrSym = csglDllSymbol( mHnd, symbol );
            return Marshal.GetDelegateForFunctionPointer( ptrSym, delegateType );
        }

        public static void csglDllLinkAllDelegates( Type ofType, IntPtr mHnd )
        {
            FieldInfo[] fields = ofType.GetFields( BindingFlags.Public | BindingFlags.Static );

            foreach ( FieldInfo fi in fields )
            {
                if ( fi.FieldType.BaseType == _delegateType )
                {
                    IntPtr ptr = csglDllSymbol( mHnd, fi.Name );

                    if ( ptr != IntPtr.Zero )
                        fi.SetValue( null, Marshal.GetDelegateForFunctionPointer( ptr, fi.FieldType ) );
                    else
                        Console.WriteLine( "Could not resolve '{0}' in loaded assembly '0x{1}'.", fi.Name, mHnd.ToString( "X" ) );
                }
            }
        }

        public static void csglMemcpy( IntPtr dest, IntPtr source, uint count )
        {
            if ( __linux__ )
                memcpy( dest, source, count );
            else
                CopyMemory( dest, source, count );
        }
        #endregion
    }
}
#endregion

#region Implementation
namespace CSGL
{
    using static DLL;

    public static class Glfw3
    {
        public static string GLFWWindows = "glfw3.dll";
        public static string GLFWLinux = "./libglfw.so";

        private static IntPtr _glfwHnd;

        #region Constants
        public const int GLFW_VERSION_MAJOR = 3;
        public const int GLFW_VERSION_MINOR = 2;
        public const int GLFW_VERSION_REVISION = 1;
        public const int GLFW_TRUE = 1;
        public const int GLFW_FALSE = 0;
        public const int GLFW_RELEASE = 0;
        public const int GLFW_PRESS = 1;
        public const int GLFW_REPEAT = 2;
        public const int GLFW_KEY_UNKNOWN = -1;
        public const int GLFW_KEY_SPACE = 32;
        public const int GLFW_KEY_APOSTROPHE = 39;
        public const int GLFW_KEY_COMMA = 44;
        public const int GLFW_KEY_MINUS = 45;
        public const int GLFW_KEY_PERIOD = 46;
        public const int GLFW_KEY_SLASH = 47;
        public const int GLFW_KEY_0 = 48;
        public const int GLFW_KEY_1 = 49;
        public const int GLFW_KEY_2 = 50;
        public const int GLFW_KEY_3 = 51;
        public const int GLFW_KEY_4 = 52;
        public const int GLFW_KEY_5 = 53;
        public const int GLFW_KEY_6 = 54;
        public const int GLFW_KEY_7 = 55;
        public const int GLFW_KEY_8 = 56;
        public const int GLFW_KEY_9 = 57;
        public const int GLFW_KEY_SEMICOLON = 59;
        public const int GLFW_KEY_EQUAL = 61;
        public const int GLFW_KEY_A = 65;
        public const int GLFW_KEY_B = 66;
        public const int GLFW_KEY_C = 67;
        public const int GLFW_KEY_D = 68;
        public const int GLFW_KEY_E = 69;
        public const int GLFW_KEY_F = 70;
        public const int GLFW_KEY_G = 71;
        public const int GLFW_KEY_H = 72;
        public const int GLFW_KEY_I = 73;
        public const int GLFW_KEY_J = 74;
        public const int GLFW_KEY_K = 75;
        public const int GLFW_KEY_L = 76;
        public const int GLFW_KEY_M = 77;
        public const int GLFW_KEY_N = 78;
        public const int GLFW_KEY_O = 79;
        public const int GLFW_KEY_P = 80;
        public const int GLFW_KEY_Q = 81;
        public const int GLFW_KEY_R = 82;
        public const int GLFW_KEY_S = 83;
        public const int GLFW_KEY_T = 84;
        public const int GLFW_KEY_U = 85;
        public const int GLFW_KEY_V = 86;
        public const int GLFW_KEY_W = 87;
        public const int GLFW_KEY_X = 88;
        public const int GLFW_KEY_Y = 89;
        public const int GLFW_KEY_Z = 90;
        public const int GLFW_KEY_LEFT_BRACKET = 91;
        public const int GLFW_KEY_BACKSLASH = 92;
        public const int GLFW_KEY_RIGHT_BRACKET = 93;
        public const int GLFW_KEY_GRAVE_ACCENT = 96;
        public const int GLFW_KEY_WORLD_1 = 161;
        public const int GLFW_KEY_WORLD_2 = 162;
        public const int GLFW_KEY_ESCAPE = 256;
        public const int GLFW_KEY_ENTER = 257;
        public const int GLFW_KEY_TAB = 258;
        public const int GLFW_KEY_BACKSPACE = 259;
        public const int GLFW_KEY_INSERT = 260;
        public const int GLFW_KEY_DELETE = 261;
        public const int GLFW_KEY_RIGHT = 262;
        public const int GLFW_KEY_LEFT = 263;
        public const int GLFW_KEY_DOWN = 264;
        public const int GLFW_KEY_UP = 265;
        public const int GLFW_KEY_PAGE_UP = 266;
        public const int GLFW_KEY_PAGE_DOWN = 267;
        public const int GLFW_KEY_HOME = 268;
        public const int GLFW_KEY_END = 269;
        public const int GLFW_KEY_CAPS_LOCK = 280;
        public const int GLFW_KEY_SCROLL_LOCK = 281;
        public const int GLFW_KEY_NUM_LOCK = 282;
        public const int GLFW_KEY_PRINT_SCREEN = 283;
        public const int GLFW_KEY_PAUSE = 284;
        public const int GLFW_KEY_F1 = 290;
        public const int GLFW_KEY_F2 = 291;
        public const int GLFW_KEY_F3 = 292;
        public const int GLFW_KEY_F4 = 293;
        public const int GLFW_KEY_F5 = 294;
        public const int GLFW_KEY_F6 = 295;
        public const int GLFW_KEY_F7 = 296;
        public const int GLFW_KEY_F8 = 297;
        public const int GLFW_KEY_F9 = 298;
        public const int GLFW_KEY_F10 = 299;
        public const int GLFW_KEY_F11 = 300;
        public const int GLFW_KEY_F12 = 301;
        public const int GLFW_KEY_F13 = 302;
        public const int GLFW_KEY_F14 = 303;
        public const int GLFW_KEY_F15 = 304;
        public const int GLFW_KEY_F16 = 305;
        public const int GLFW_KEY_F17 = 306;
        public const int GLFW_KEY_F18 = 307;
        public const int GLFW_KEY_F19 = 308;
        public const int GLFW_KEY_F20 = 309;
        public const int GLFW_KEY_F21 = 310;
        public const int GLFW_KEY_F22 = 311;
        public const int GLFW_KEY_F23 = 312;
        public const int GLFW_KEY_F24 = 313;
        public const int GLFW_KEY_F25 = 314;
        public const int GLFW_KEY_KP_0 = 320;
        public const int GLFW_KEY_KP_1 = 321;
        public const int GLFW_KEY_KP_2 = 322;
        public const int GLFW_KEY_KP_3 = 323;
        public const int GLFW_KEY_KP_4 = 324;
        public const int GLFW_KEY_KP_5 = 325;
        public const int GLFW_KEY_KP_6 = 326;
        public const int GLFW_KEY_KP_7 = 327;
        public const int GLFW_KEY_KP_8 = 328;
        public const int GLFW_KEY_KP_9 = 329;
        public const int GLFW_KEY_KP_DECIMAL = 330;
        public const int GLFW_KEY_KP_DIVIDE = 331;
        public const int GLFW_KEY_KP_MULTIPLY = 332;
        public const int GLFW_KEY_KP_SUBTRACT = 333;
        public const int GLFW_KEY_KP_ADD = 334;
        public const int GLFW_KEY_KP_ENTER = 335;
        public const int GLFW_KEY_KP_EQUAL = 336;
        public const int GLFW_KEY_LEFT_SHIFT = 340;
        public const int GLFW_KEY_LEFT_CONTROL = 341;
        public const int GLFW_KEY_LEFT_ALT = 342;
        public const int GLFW_KEY_LEFT_SUPER = 343;
        public const int GLFW_KEY_RIGHT_SHIFT = 344;
        public const int GLFW_KEY_RIGHT_CONTROL = 345;
        public const int GLFW_KEY_RIGHT_ALT = 346;
        public const int GLFW_KEY_RIGHT_SUPER = 347;
        public const int GLFW_KEY_MENU = 348;
        public const int GLFW_KEY_LAST = GLFW_KEY_MENU;
        public const int GLFW_MOD_SHIFT = 1;
        public const int GLFW_MOD_CONTROL = 2;
        public const int GLFW_MOD_ALT = 4;
        public const int GLFW_MOD_SUPER = 8;
        public const int GLFW_MOUSE_BUTTON_1 = 0;
        public const int GLFW_MOUSE_BUTTON_2 = 1;
        public const int GLFW_MOUSE_BUTTON_3 = 2;
        public const int GLFW_MOUSE_BUTTON_4 = 3;
        public const int GLFW_MOUSE_BUTTON_5 = 4;
        public const int GLFW_MOUSE_BUTTON_6 = 5;
        public const int GLFW_MOUSE_BUTTON_7 = 6;
        public const int GLFW_MOUSE_BUTTON_8 = 7;
        public const int GLFW_MOUSE_BUTTON_LAST = GLFW_MOUSE_BUTTON_8;
        public const int GLFW_MOUSE_BUTTON_LEFT = GLFW_MOUSE_BUTTON_1;
        public const int GLFW_MOUSE_BUTTON_RIGHT = GLFW_MOUSE_BUTTON_2;
        public const int GLFW_MOUSE_BUTTON_MIDDLE = GLFW_MOUSE_BUTTON_3;
        public const int GLFW_JOYSTICK_1 = 0;
        public const int GLFW_JOYSTICK_2 = 1;
        public const int GLFW_JOYSTICK_3 = 2;
        public const int GLFW_JOYSTICK_4 = 3;
        public const int GLFW_JOYSTICK_5 = 4;
        public const int GLFW_JOYSTICK_6 = 5;
        public const int GLFW_JOYSTICK_7 = 6;
        public const int GLFW_JOYSTICK_8 = 7;
        public const int GLFW_JOYSTICK_9 = 8;
        public const int GLFW_JOYSTICK_10 = 9;
        public const int GLFW_JOYSTICK_11 = 10;
        public const int GLFW_JOYSTICK_12 = 11;
        public const int GLFW_JOYSTICK_13 = 12;
        public const int GLFW_JOYSTICK_14 = 13;
        public const int GLFW_JOYSTICK_15 = 14;
        public const int GLFW_JOYSTICK_16 = 15;
        public const int GLFW_JOYSTICK_LAST = GLFW_JOYSTICK_16;
        public const int GLFW_NOT_INITIALIZED = 65537;
        public const int GLFW_NO_CURRENT_CONTEXT = 65538;
        public const int GLFW_INVALID_ENUM = 65539;
        public const int GLFW_INVALID_VALUE = 65540;
        public const int GLFW_OUT_OF_MEMORY = 65541;
        public const int GLFW_API_UNAVAILABLE = 65542;
        public const int GLFW_VERSION_UNAVAILABLE = 65543;
        public const int GLFW_PLATFORM_ERROR = 65544;
        public const int GLFW_FORMAT_UNAVAILABLE = 65545;
        public const int GLFW_NO_WINDOW_CONTEXT = 65546;
        public const int GLFW_FOCUSED = 131073;
        public const int GLFW_ICONIFIED = 131074;
        public const int GLFW_RESIZABLE = 131075;
        public const int GLFW_VISIBLE = 131076;
        public const int GLFW_DECORATED = 131077;
        public const int GLFW_AUTO_ICONIFY = 131078;
        public const int GLFW_FLOATING = 131079;
        public const int GLFW_MAXIMIZED = 131080;
        public const int GLFW_RED_BITS = 135169;
        public const int GLFW_GREEN_BITS = 135170;
        public const int GLFW_BLUE_BITS = 135171;
        public const int GLFW_ALPHA_BITS = 135172;
        public const int GLFW_DEPTH_BITS = 135173;
        public const int GLFW_STENCIL_BITS = 135174;
        public const int GLFW_ACCUM_RED_BITS = 135175;
        public const int GLFW_ACCUM_GREEN_BITS = 135176;
        public const int GLFW_ACCUM_BLUE_BITS = 135177;
        public const int GLFW_ACCUM_ALPHA_BITS = 135178;
        public const int GLFW_AUX_BUFFERS = 135179;
        public const int GLFW_STEREO = 135180;
        public const int GLFW_SAMPLES = 135181;
        public const int GLFW_SRGB_CAPABLE = 135182;
        public const int GLFW_REFRESH_RATE = 135183;
        public const int GLFW_DOUBLEBUFFER = 135184;
        public const int GLFW_CLIENT_API = 139265;
        public const int GLFW_CONTEXT_VERSION_MAJOR = 139266;
        public const int GLFW_CONTEXT_VERSION_MINOR = 139267;
        public const int GLFW_CONTEXT_REVISION = 139268;
        public const int GLFW_CONTEXT_ROBUSTNESS = 139269;
        public const int GLFW_OPENGL_FORWARD_COMPAT = 139270;
        public const int GLFW_OPENGL_DEBUG_CONTEXT = 139271;
        public const int GLFW_OPENGL_PROFILE = 139272;
        public const int GLFW_CONTEXT_RELEASE_BEHAVIOR = 139273;
        public const int GLFW_CONTEXT_NO_ERROR = 139274;
        public const int GLFW_CONTEXT_CREATION_API = 139275;
        public const int GLFW_NO_API = 0;
        public const int GLFW_OPENGL_API = 196609;
        public const int GLFW_OPENGL_ES_API = 196610;
        public const int GLFW_NO_ROBUSTNESS = 0;
        public const int GLFW_NO_RESET_NOTIFICATION = 200705;
        public const int GLFW_LOSE_CONTEXT_ON_RESET = 200706;
        public const int GLFW_OPENGL_ANY_PROFILE = 0;
        public const int GLFW_OPENGL_CORE_PROFILE = 204801;
        public const int GLFW_OPENGL_COMPAT_PROFILE = 204802;
        public const int GLFW_CURSOR = 208897;
        public const int GLFW_STICKY_KEYS = 208898;
        public const int GLFW_STICKY_MOUSE_BUTTONS = 208899;
        public const int GLFW_CURSOR_NORMAL = 212993;
        public const int GLFW_CURSOR_HIDDEN = 212994;
        public const int GLFW_CURSOR_DISABLED = 212995;
        public const int GLFW_ANY_RELEASE_BEHAVIOR = 0;
        public const int GLFW_RELEASE_BEHAVIOR_FLUSH = 217089;
        public const int GLFW_RELEASE_BEHAVIOR_NONE = 217090;
        public const int GLFW_NATIVE_CONTEXT_API = 221185;
        public const int GLFW_EGL_CONTEXT_API = 221186;
        public const int GLFW_ARROW_CURSOR = 221185;
        public const int GLFW_IBEAM_CURSOR = 221186;
        public const int GLFW_CROSSHAIR_CURSOR = 221187;
        public const int GLFW_HAND_CURSOR = 221188;
        public const int GLFW_HRESIZE_CURSOR = 221189;
        public const int GLFW_VRESIZE_CURSOR = 221190;
        public const int GLFW_CONNECTED = 262145;
        public const int GLFW_DISCONNECTED = 262146;
        public const int GLFW_DONT_CARE = -1;
        #endregion

        #region Delegates
        public delegate void GLFWvkproc();
        public delegate void GLFWerrorfun( int code, [In] [MarshalAs( UnmanagedType.LPStr )] string description );
        public delegate void GLFWwindowposfun( IntPtr window, int x, int y );
        public delegate void GLFWwindowsizefun( IntPtr window, int w, int h );
        public delegate void GLFWwindowclosefun( IntPtr window );
        public delegate void GLFWwindowrefreshfun( IntPtr window );
        public delegate void GLFWwindowfocusfun( IntPtr window, int got );
        public delegate void GLFWwindowiconifyfun( IntPtr window, int iconify );
        public delegate void GLFWframebuffersizefun( IntPtr window, int w, int h );
        public delegate void GLFWmousebuttonfun( IntPtr window, int button, int action, int mods );
        public delegate void GLFWcursorposfun( IntPtr window, double x, double y );
        public delegate void GLFWcursorenterfun( IntPtr window, int entered );
        public delegate void GLFWscrollfun( IntPtr window, double xoffset, double yoffset );
        public delegate void GLFWkeyfun( IntPtr window, int key, int scancode, int action, int mods );
        public delegate void GLFWcharfun( IntPtr window, uint codepoint );
        public delegate void GLFWcharmodsfun( IntPtr window, uint codepoint, int mods );
        public delegate void GLFWdropfun( IntPtr window, int count, [Out] string[] paths );
        public delegate void GLFWmonitorfun( IntPtr window, int ev );
        public delegate void GLFWjoystickfun( int window, int ev );

        public delegate int PFNGLFWINITPROC();
        public delegate void PFNGLFWTERMINATEPROC();
        public delegate void PFNGLFWGETVERSIONPROC( ref int major, ref int minor, ref int rev );
        public delegate IntPtr PFNGLFWGETVERSIONSTRINGPROC();
        public delegate GLFWerrorfun PFNGLFWSETERRORCALLBACKPROC( GLFWerrorfun cbfun );
        public delegate IntPtr PFNGLFWGETMONITORSPROC( ref int count );
        public delegate IntPtr PFNGLFWGETPRIMARYMONITORPROC();
        public delegate void PFNGLFWGETMONITORPOSPROC( IntPtr monitor, ref int xpos, ref int ypos );
        public delegate void PFNGLFWGETMONITORPHYSICALSIZEPROC( IntPtr monitor, ref int widthMM, ref int heightMM );
        public delegate IntPtr PFNGLFWGETMONITORNAMEPROC( IntPtr monitor );
        public delegate GLFWmonitorfun PFNGLFWSETMONITORCALLBACKPROC( GLFWmonitorfun cbfun );
        public delegate IntPtr PFNGLFWGETVIDEOMODESPROC( IntPtr monitor, ref int count );
        public delegate IntPtr PFNGLFWGETVIDEOMODEPROC( IntPtr monitor );
        public delegate void PFNGLFWSETGAMMAPROC( IntPtr monitor, float gamma );
        public delegate IntPtr PFNGLFWGETGAMMARAMPPROC( IntPtr monitor );
        public delegate void PFNGLFWSETGAMMARAMPPROC( IntPtr monitor, ref GLFWgammaramp ramp );
        public delegate void PFNGLFWDEFAULTWINDOWHINTSPROC();
        public delegate void PFNGLFWWINDOWHINTPROC( int hint, int value );
        public delegate IntPtr PFNGLFWCREATEWINDOWPROC( int width, int height, [In] [MarshalAs( UnmanagedType.LPStr )] string title, IntPtr monitor, IntPtr share );
        public delegate void PFNGLFWDESTROYWINDOWPROC( IntPtr window );
        public delegate int PFNGLFWWINDOWSHOULDCLOSEPROC( IntPtr window );
        public delegate void PFNGLFWSETWINDOWSHOULDCLOSEPROC( IntPtr window, int value );
        public delegate void PFNGLFWSETWINDOWTITLEPROC( IntPtr window, [In] [MarshalAs( UnmanagedType.LPStr )] string title );
        public delegate void PFNGLFWSETWINDOWICONPROC( IntPtr window, int count, ref GLFWimage images );
        public delegate void PFNGLFWGETWINDOWPOSPROC( IntPtr window, ref int xpos, ref int ypos );
        public delegate void PFNGLFWSETWINDOWPOSPROC( IntPtr window, int xpos, int ypos );
        public delegate void PFNGLFWGETWINDOWSIZEPROC( IntPtr window, ref int width, ref int height );
        public delegate void PFNGLFWSETWINDOWSIZELIMITSPROC( IntPtr window, int minwidth, int minheight, int maxwidth, int maxheight );
        public delegate void PFNGLFWSETWINDOWASPECTRATIOPROC( IntPtr window, int numer, int denom );
        public delegate void PFNGLFWSETWINDOWSIZEPROC( IntPtr window, int width, int height );
        public delegate void PFNGLFWGETFRAMEBUFFERSIZEPROC( IntPtr window, ref int width, ref int height );
        public delegate void PFNGLFWGETWINDOWFRAMESIZEPROC( IntPtr window, ref int left, ref int top, ref int right, ref int bottom );
        public delegate void PFNGLFWICONIFYWINDOWPROC( IntPtr window );
        public delegate void PFNGLFWRESTOREWINDOWPROC( IntPtr window );
        public delegate void PFNGLFWMAXIMIZEWINDOWPROC( IntPtr window );
        public delegate void PFNGLFWSHOWWINDOWPROC( IntPtr window );
        public delegate void PFNGLFWHIDEWINDOWPROC( IntPtr window );
        public delegate void PFNGLFWFOCUSWINDOWPROC( IntPtr window );
        public delegate IntPtr PFNGLFWGETWINDOWMONITORPROC( IntPtr window );
        public delegate void PFNGLFWSETWINDOWMONITORPROC( IntPtr window, IntPtr monitor, int xpos, int ypos, int width, int height, int refreshRate );
        public delegate int PFNGLFWGETWINDOWATTRIBPROC( IntPtr window, int attrib );
        public delegate void PFNGLFWSETWINDOWUSERPOINTERPROC( IntPtr window, IntPtr pointer );
        public delegate IntPtr PFNGLFWGETWINDOWUSERPOINTERPROC( IntPtr window );
        public delegate GLFWwindowposfun PFNGLFWSETWINDOWPOSCALLBACKPROC( IntPtr window, GLFWwindowposfun cbfun );
        public delegate GLFWwindowsizefun PFNGLFWSETWINDOWSIZECALLBACKPROC( IntPtr window, GLFWwindowsizefun cbfun );
        public delegate GLFWwindowclosefun PFNGLFWSETWINDOWCLOSECALLBACKPROC( IntPtr window, GLFWwindowclosefun cbfun );
        public delegate GLFWwindowrefreshfun PFNGLFWSETWINDOWREFRESHCALLBACKPROC( IntPtr window, GLFWwindowrefreshfun cbfun );
        public delegate GLFWwindowfocusfun PFNGLFWSETWINDOWFOCUSCALLBACKPROC( IntPtr window, GLFWwindowfocusfun cbfun );
        public delegate GLFWwindowiconifyfun PFNGLFWSETWINDOWICONIFYCALLBACKPROC( IntPtr window, GLFWwindowiconifyfun cbfun );
        public delegate GLFWframebuffersizefun PFNGLFWSETFRAMEBUFFERSIZECALLBACKPROC( IntPtr window, GLFWframebuffersizefun cbfun );
        public delegate void PFNGLFWPOLLEVENTSPROC();
        public delegate void PFNGLFWWAITEVENTSPROC();
        public delegate void PFNGLFWWAITEVENTSTIMEOUTPROC( double timeout );
        public delegate void PFNGLFWPOSTEMPTYEVENTPROC();
        public delegate int PFNGLFWGETINPUTMODEPROC( IntPtr window, int mode );
        public delegate void PFNGLFWSETINPUTMODEPROC( IntPtr window, int mode, int value );
        public delegate IntPtr PFNGLFWGETKEYNAMEPROC( int key, int scancode );
        public delegate int PFNGLFWGETKEYPROC( IntPtr window, int key );
        public delegate int PFNGLFWGETMOUSEBUTTONPROC( IntPtr window, int button );
        public delegate void PFNGLFWGETCURSORPOSPROC( IntPtr window, ref double xpos, ref double ypos );
        public delegate void PFNGLFWSETCURSORPOSPROC( IntPtr window, double xpos, double ypos );
        public delegate IntPtr PFNGLFWCREATECURSORPROC( ref GLFWimage image, int xhot, int yhot );
        public delegate IntPtr PFNGLFWCREATESTANDARDCURSORPROC( int shape );
        public delegate void PFNGLFWDESTROYCURSORPROC( IntPtr cursor );
        public delegate void PFNGLFWSETCURSORPROC( IntPtr window, IntPtr cursor );
        public delegate GLFWkeyfun PFNGLFWSETKEYCALLBACKPROC( IntPtr window, GLFWkeyfun cbfun );
        public delegate GLFWcharfun PFNGLFWSETCHARCALLBACKPROC( IntPtr window, GLFWcharfun cbfun );
        public delegate GLFWcharmodsfun PFNGLFWSETCHARMODSCALLBACKPROC( IntPtr window, GLFWcharmodsfun cbfun );
        public delegate GLFWmousebuttonfun PFNGLFWSETMOUSEBUTTONCALLBACKPROC( IntPtr window, GLFWmousebuttonfun cbfun );
        public delegate GLFWcursorposfun PFNGLFWSETCURSORPOSCALLBACKPROC( IntPtr window, GLFWcursorposfun cbfun );
        public delegate GLFWcursorenterfun PFNGLFWSETCURSORENTERCALLBACKPROC( IntPtr window, GLFWcursorenterfun cbfun );
        public delegate GLFWscrollfun PFNGLFWSETSCROLLCALLBACKPROC( IntPtr window, GLFWscrollfun cbfun );
        public delegate GLFWdropfun PFNGLFWSETDROPCALLBACKPROC( IntPtr window, GLFWdropfun cbfun );
        public delegate int PFNGLFWJOYSTICKPRESENTPROC( int joy );
        public delegate IntPtr PFNGLFWGETJOYSTICKAXESPROC( int joy, ref int count );
        public delegate IntPtr PFNGLFWGETJOYSTICKBUTTONSPROC( int joy, ref int count );
        public delegate IntPtr PFNGLFWGETJOYSTICKNAMEPROC( int joy );
        public delegate GLFWjoystickfun PFNGLFWSETJOYSTICKCALLBACKPROC( GLFWjoystickfun cbfun );
        public delegate void PFNGLFWSETCLIPBOARDSTRINGPROC( IntPtr window, [In] [MarshalAs( UnmanagedType.LPStr )] string @string );
        public delegate IntPtr PFNGLFWGETCLIPBOARDSTRINGPROC( IntPtr window );
        public delegate double PFNGLFWGETTIMEPROC();
        public delegate void PFNGLFWSETTIMEPROC( double time );
        public delegate uint PFNGLFWGETTIMERVALUEPROC();
        public delegate uint PFNGLFWGETTIMERFREQUENCYPROC();
        public delegate void PFNGLFWMAKECONTEXTCURRENTPROC( IntPtr window );
        public delegate IntPtr PFNGLFWGETCURRENTCONTEXTPROC();
        public delegate void PFNGLFWSWAPBUFFERSPROC( IntPtr window );
        public delegate void PFNGLFWSWAPINTERVALPROC( int interval );
        public delegate int PFNGLFWEXTENSIONSUPPORTEDPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string extension );
        public delegate IntPtr PFNGLFWGETPROCADDRESSPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string procname );
        public delegate int PFNGLFWVULKANSUPPORTEDPROC();
        public delegate IntPtr PFNGLFWGETREQUIREDINSTANCEEXTENSIONSPROC( ref uint count );
        #endregion

        #region Structures
        [StructLayout( LayoutKind.Sequential )]
        public struct GLFWvidmode
        {
            public int width;
            public int height;
            public int redBits;
            public int greenBits;
            public int blueBits;
            public int refreshRate;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct GLFWgammaramp
        {
            public IntPtr red;
            public IntPtr green;
            public IntPtr blue;
            public uint size;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct GLFWimage
        {
            public int width;
            public int height;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pixels;
        }
        #endregion

        #region Methods
        public static void csglLoadGlfw()
        {
            if ( __linux__ )
                _glfwHnd = csglDllLoad( GLFWLinux );
            else
                _glfwHnd = csglDllLoad( GLFWWindows );

            csglDllLinkAllDelegates( typeof( Glfw3 ), _glfwHnd );
        }

        public static PFNGLFWINITPROC glfwInit;
        public static PFNGLFWTERMINATEPROC glfwTerminate;
        public static PFNGLFWGETVERSIONPROC glfwGetVersion;
        public static PFNGLFWGETVERSIONSTRINGPROC glfwGetVersionString;
        public static PFNGLFWSETERRORCALLBACKPROC glfwSetErrorCallback;
        public static PFNGLFWGETMONITORSPROC glfwGetMonitors;
        public static PFNGLFWGETPRIMARYMONITORPROC glfwGetPrimaryMonitor;
        public static PFNGLFWGETMONITORPOSPROC glfwGetMonitorPos;
        public static PFNGLFWGETMONITORPHYSICALSIZEPROC glfwGetMonitorPhysicalSize;
        public static PFNGLFWGETMONITORNAMEPROC glfwGetMonitorName;
        public static PFNGLFWSETMONITORCALLBACKPROC glfwSetMonitorCallback;
        public static PFNGLFWGETVIDEOMODESPROC glfwGetVideoModes;
        public static PFNGLFWGETVIDEOMODEPROC glfwGetVideoMode;
        public static PFNGLFWSETGAMMAPROC glfwSetGamma;
        public static PFNGLFWGETGAMMARAMPPROC glfwGetGammaRamp;
        public static PFNGLFWSETGAMMARAMPPROC glfwSetGammaRamp;
        public static PFNGLFWDEFAULTWINDOWHINTSPROC glfwDefaultWindowHints;
        public static PFNGLFWWINDOWHINTPROC glfwWindowHint;
        public static PFNGLFWCREATEWINDOWPROC glfwCreateWindow;
        public static PFNGLFWDESTROYWINDOWPROC glfwDestroyWindow;
        public static PFNGLFWWINDOWSHOULDCLOSEPROC glfwWindowShouldClose;
        public static PFNGLFWSETWINDOWSHOULDCLOSEPROC glfwSetWindowShouldClose;
        public static PFNGLFWSETWINDOWTITLEPROC glfwSetWindowTitle;
        public static PFNGLFWSETWINDOWICONPROC glfwSetWindowIcon;
        public static PFNGLFWGETWINDOWPOSPROC glfwGetWindowPos;
        public static PFNGLFWSETWINDOWPOSPROC glfwSetWindowPos;
        public static PFNGLFWGETWINDOWSIZEPROC glfwGetWindowSize;
        public static PFNGLFWSETWINDOWSIZELIMITSPROC glfwSetWindowSizeLimits;
        public static PFNGLFWSETWINDOWASPECTRATIOPROC glfwSetWindowAspectRatio;
        public static PFNGLFWSETWINDOWSIZEPROC glfwSetWindowSize;
        public static PFNGLFWGETFRAMEBUFFERSIZEPROC glfwGetFramebufferSize;
        public static PFNGLFWGETWINDOWFRAMESIZEPROC glfwGetWindowFrameSize;
        public static PFNGLFWICONIFYWINDOWPROC glfwIconifyWindow;
        public static PFNGLFWRESTOREWINDOWPROC glfwRestoreWindow;
        public static PFNGLFWMAXIMIZEWINDOWPROC glfwMaximizeWindow;
        public static PFNGLFWSHOWWINDOWPROC glfwShowWindow;
        public static PFNGLFWHIDEWINDOWPROC glfwHideWindow;
        public static PFNGLFWFOCUSWINDOWPROC glfwFocusWindow;
        public static PFNGLFWGETWINDOWMONITORPROC glfwGetWindowMonitor;
        public static PFNGLFWSETWINDOWMONITORPROC glfwSetWindowMonitor;
        public static PFNGLFWGETWINDOWATTRIBPROC glfwGetWindowAttrib;
        public static PFNGLFWSETWINDOWUSERPOINTERPROC glfwSetWindowUserPointer;
        public static PFNGLFWGETWINDOWUSERPOINTERPROC glfwGetWindowUserPointer;
        public static PFNGLFWSETWINDOWPOSCALLBACKPROC glfwSetWindowPosCallback;
        public static PFNGLFWSETWINDOWSIZECALLBACKPROC glfwSetWindowSizeCallback;
        public static PFNGLFWSETWINDOWCLOSECALLBACKPROC glfwSetWindowCloseCallback;
        public static PFNGLFWSETWINDOWREFRESHCALLBACKPROC glfwSetWindowRefreshCallback;
        public static PFNGLFWSETWINDOWFOCUSCALLBACKPROC glfwSetWindowFocusCallback;
        public static PFNGLFWSETWINDOWICONIFYCALLBACKPROC glfwSetWindowIconifyCallback;
        public static PFNGLFWSETFRAMEBUFFERSIZECALLBACKPROC glfwSetFramebufferSizeCallback;
        public static PFNGLFWPOLLEVENTSPROC glfwPollEvents;
        public static PFNGLFWWAITEVENTSPROC glfwWaitEvents;
        public static PFNGLFWWAITEVENTSTIMEOUTPROC glfwWaitEventsTimeout;
        public static PFNGLFWPOSTEMPTYEVENTPROC glfwPostEmptyEvent;
        public static PFNGLFWGETINPUTMODEPROC glfwGetInputMode;
        public static PFNGLFWSETINPUTMODEPROC glfwSetInputMode;
        public static PFNGLFWGETKEYNAMEPROC glfwGetKeyName;
        public static PFNGLFWGETKEYPROC glfwGetKey;
        public static PFNGLFWGETMOUSEBUTTONPROC glfwGetMouseButton;
        public static PFNGLFWGETCURSORPOSPROC glfwGetCursorPos;
        public static PFNGLFWSETCURSORPOSPROC glfwSetCursorPos;
        public static PFNGLFWCREATECURSORPROC glfwCreateCursor;
        public static PFNGLFWCREATESTANDARDCURSORPROC glfwCreateStandardCursor;
        public static PFNGLFWDESTROYCURSORPROC glfwDestroyCursor;
        public static PFNGLFWSETCURSORPROC glfwSetCursor;
        public static PFNGLFWSETKEYCALLBACKPROC glfwSetKeyCallback;
        public static PFNGLFWSETCHARCALLBACKPROC glfwSetCharCallback;
        public static PFNGLFWSETCHARMODSCALLBACKPROC glfwSetCharModsCallback;
        public static PFNGLFWSETMOUSEBUTTONCALLBACKPROC glfwSetMouseButtonCallback;
        public static PFNGLFWSETCURSORPOSCALLBACKPROC glfwSetCursorPosCallback;
        public static PFNGLFWSETCURSORENTERCALLBACKPROC glfwSetCursorEnterCallback;
        public static PFNGLFWSETSCROLLCALLBACKPROC glfwSetScrollCallback;
        public static PFNGLFWSETDROPCALLBACKPROC glfwSetDropCallback;
        public static PFNGLFWJOYSTICKPRESENTPROC glfwJoystickPresent;
        public static PFNGLFWGETJOYSTICKAXESPROC glfwGetJoystickAxes;
        public static PFNGLFWGETJOYSTICKBUTTONSPROC glfwGetJoystickButtons;
        public static PFNGLFWGETJOYSTICKNAMEPROC glfwGetJoystickName;
        public static PFNGLFWSETJOYSTICKCALLBACKPROC glfwSetJoystickCallback;
        public static PFNGLFWSETCLIPBOARDSTRINGPROC glfwSetClipboardString;
        public static PFNGLFWGETCLIPBOARDSTRINGPROC glfwGetClipboardString;
        public static PFNGLFWGETTIMEPROC glfwGetTime;
        public static PFNGLFWSETTIMEPROC glfwSetTime;
        public static PFNGLFWGETTIMERVALUEPROC glfwGetTimerValue;
        public static PFNGLFWGETTIMERFREQUENCYPROC glfwGetTimerFrequency;
        public static PFNGLFWMAKECONTEXTCURRENTPROC glfwMakeContextCurrent;
        public static PFNGLFWGETCURRENTCONTEXTPROC glfwGetCurrentContext;
        public static PFNGLFWSWAPBUFFERSPROC glfwSwapBuffers;
        public static PFNGLFWSWAPINTERVALPROC glfwSwapInterval;
        public static PFNGLFWEXTENSIONSUPPORTEDPROC glfwExtensionSupported;
        public static PFNGLFWGETPROCADDRESSPROC glfwGetProcAddress;
        public static PFNGLFWVULKANSUPPORTEDPROC glfwVulkanSupported;
        public static PFNGLFWGETREQUIREDINSTANCEEXTENSIONSPROC glfwGetRequiredInstanceExtensions;
        #endregion
    }

    public static class OpenGL
    {
        #region OpenGL 1.0 + OpenGL 1.1
        #region Constants
        public const uint GL_ACCUM = 256;
        public const uint GL_LOAD = 257;
        public const uint GL_RETURN = 258;
        public const uint GL_MULT = 259;
        public const uint GL_ADD = 260;
        public const uint GL_NEVER = 512;
        public const uint GL_LESS = 513;
        public const uint GL_EQUAL = 514;
        public const uint GL_LEQUAL = 515;
        public const uint GL_GREATER = 516;
        public const uint GL_NOTEQUAL = 517;
        public const uint GL_GEQUAL = 518;
        public const uint GL_ALWAYS = 519;
        public const int GL_CURRENT_BIT = 1;
        public const int GL_POINT_BIT = 2;
        public const int GL_LINE_BIT = 4;
        public const int GL_POLYGON_BIT = 8;
        public const int GL_POLYGON_STIPPLE_BIT = 16;
        public const int GL_PIXEL_MODE_BIT = 32;
        public const int GL_LIGHTING_BIT = 64;
        public const int GL_FOG_BIT = 128;
        public const int GL_DEPTH_BUFFER_BIT = 256;
        public const int GL_ACCUM_BUFFER_BIT = 512;
        public const int GL_STENCIL_BUFFER_BIT = 1024;
        public const int GL_VIEWPORT_BIT = 2048;
        public const int GL_TRANSFORM_BIT = 4096;
        public const int GL_ENABLE_BIT = 8192;
        public const int GL_COLOR_BUFFER_BIT = 16384;
        public const int GL_HINT_BIT = 32768;
        public const int GL_EVAL_BIT = 65536;
        public const int GL_LIST_BIT = 131072;
        public const int GL_TEXTURE_BIT = 262144;
        public const int GL_SCISSOR_BIT = 524288;
        public const uint GL_ALL_ATTRIB_BITS = 1048575;
        public const uint GL_POINTS = 0;
        public const uint GL_LINES = 1;
        public const uint GL_LINE_LOOP = 2;
        public const uint GL_LINE_STRIP = 3;
        public const uint GL_TRIANGLES = 4;
        public const uint GL_TRIANGLE_STRIP = 5;
        public const uint GL_TRIANGLE_FAN = 6;
        public const uint GL_QUADS = 7;
        public const uint GL_QUAD_STRIP = 8;
        public const uint GL_POLYGON = 9;
        public const uint GL_ZERO = 0;
        public const uint GL_ONE = 1;
        public const uint GL_SRC_COLOR = 768;
        public const uint GL_ONE_MINUS_SRC_COLOR = 769;
        public const uint GL_SRC_ALPHA = 770;
        public const uint GL_ONE_MINUS_SRC_ALPHA = 771;
        public const uint GL_DST_ALPHA = 772;
        public const uint GL_ONE_MINUS_DST_ALPHA = 773;
        public const uint GL_DST_COLOR = 774;
        public const uint GL_ONE_MINUS_DST_COLOR = 775;
        public const uint GL_SRC_ALPHA_SATURATE = 776;
        public const byte GL_TRUE = 1;
        public const byte GL_FALSE = 0;
        public const uint GL_CLIP_PLANE0 = 12288;
        public const uint GL_CLIP_PLANE1 = 12289;
        public const uint GL_CLIP_PLANE2 = 12290;
        public const uint GL_CLIP_PLANE3 = 12291;
        public const uint GL_CLIP_PLANE4 = 12292;
        public const uint GL_CLIP_PLANE5 = 12293;
        public const uint GL_BYTE = 5120;
        public const uint GL_UNSIGNED_BYTE = 5121;
        public const uint GL_SHORT = 5122;
        public const uint GL_UNSIGNED_SHORT = 5123;
        public const uint GL_INT = 5124;
        public const uint GL_UNSIGNED_INT = 5125;
        public const uint GL_FLOAT = 5126;
        public const uint GL_2_BYTES = 5127;
        public const uint GL_3_BYTES = 5128;
        public const uint GL_4_BYTES = 5129;
        public const uint GL_DOUBLE = 5130;
        public const uint GL_NONE = 0;
        public const uint GL_FRONT_LEFT = 1024;
        public const uint GL_FRONT_RIGHT = 1025;
        public const uint GL_BACK_LEFT = 1026;
        public const uint GL_BACK_RIGHT = 1027;
        public const uint GL_FRONT = 1028;
        public const uint GL_BACK = 1029;
        public const uint GL_LEFT = 1030;
        public const uint GL_RIGHT = 1031;
        public const uint GL_FRONT_AND_BACK = 1032;
        public const uint GL_AUX0 = 1033;
        public const uint GL_AUX1 = 1034;
        public const uint GL_AUX2 = 1035;
        public const uint GL_AUX3 = 1036;
        public const uint GL_NO_ERROR = 0;
        public const uint GL_INVALID_ENUM = 1280;
        public const uint GL_INVALID_VALUE = 1281;
        public const uint GL_INVALID_OPERATION = 1282;
        public const uint GL_STACK_OVERFLOW = 1283;
        public const uint GL_STACK_UNDERFLOW = 1284;
        public const uint GL_OUT_OF_MEMORY = 1285;
        public const uint GL_2D = 1536;
        public const uint GL_3D = 1537;
        public const uint GL_3D_COLOR = 1538;
        public const uint GL_3D_COLOR_TEXTURE = 1539;
        public const uint GL_4D_COLOR_TEXTURE = 1540;
        public const uint GL_PASS_THROUGH_TOKEN = 1792;
        public const uint GL_POINT_TOKEN = 1793;
        public const uint GL_LINE_TOKEN = 1794;
        public const uint GL_POLYGON_TOKEN = 1795;
        public const uint GL_BITMAP_TOKEN = 1796;
        public const uint GL_DRAW_PIXEL_TOKEN = 1797;
        public const uint GL_COPY_PIXEL_TOKEN = 1798;
        public const uint GL_LINE_RESET_TOKEN = 1799;
        public const uint GL_EXP = 2048;
        public const uint GL_EXP2 = 2049;
        public const uint GL_CW = 2304;
        public const uint GL_CCW = 2305;
        public const uint GL_COEFF = 2560;
        public const uint GL_ORDER = 2561;
        public const uint GL_DOMAIN = 2562;
        public const uint GL_CURRENT_COLOR = 2816;
        public const uint GL_CURRENT_INDEX = 2817;
        public const uint GL_CURRENT_NORMAL = 2818;
        public const uint GL_CURRENT_TEXTURE_COORDS = 2819;
        public const uint GL_CURRENT_RASTER_COLOR = 2820;
        public const uint GL_CURRENT_RASTER_INDEX = 2821;
        public const uint GL_CURRENT_RASTER_TEXTURE_COORDS = 2822;
        public const uint GL_CURRENT_RASTER_POSITION = 2823;
        public const uint GL_CURRENT_RASTER_POSITION_VALID = 2824;
        public const uint GL_CURRENT_RASTER_DISTANCE = 2825;
        public const uint GL_POINT_SMOOTH = 2832;
        public const uint GL_POINT_SIZE = 2833;
        public const uint GL_POINT_SIZE_RANGE = 2834;
        public const uint GL_POINT_SIZE_GRANULARITY = 2835;
        public const uint GL_LINE_SMOOTH = 2848;
        public const uint GL_LINE_WIDTH = 2849;
        public const uint GL_LINE_WIDTH_RANGE = 2850;
        public const uint GL_LINE_WIDTH_GRANULARITY = 2851;
        public const uint GL_LINE_STIPPLE = 2852;
        public const uint GL_LINE_STIPPLE_PATTERN = 2853;
        public const uint GL_LINE_STIPPLE_REPEAT = 2854;
        public const uint GL_LIST_MODE = 2864;
        public const uint GL_MAX_LIST_NESTING = 2865;
        public const uint GL_LIST_BASE = 2866;
        public const uint GL_LIST_INDEX = 2867;
        public const uint GL_POLYGON_MODE = 2880;
        public const uint GL_POLYGON_SMOOTH = 2881;
        public const uint GL_POLYGON_STIPPLE = 2882;
        public const uint GL_EDGE_FLAG = 2883;
        public const uint GL_CULL_FACE = 2884;
        public const uint GL_CULL_FACE_MODE = 2885;
        public const uint GL_FRONT_FACE = 2886;
        public const uint GL_LIGHTING = 2896;
        public const uint GL_LIGHT_MODEL_LOCAL_VIEWER = 2897;
        public const uint GL_LIGHT_MODEL_TWO_SIDE = 2898;
        public const uint GL_LIGHT_MODEL_AMBIENT = 2899;
        public const uint GL_SHADE_MODEL = 2900;
        public const uint GL_COLOR_MATERIAL_FACE = 2901;
        public const uint GL_COLOR_MATERIAL_PARAMETER = 2902;
        public const uint GL_COLOR_MATERIAL = 2903;
        public const uint GL_FOG = 2912;
        public const uint GL_FOG_INDEX = 2913;
        public const uint GL_FOG_DENSITY = 2914;
        public const uint GL_FOG_START = 2915;
        public const uint GL_FOG_END = 2916;
        public const uint GL_FOG_MODE = 2917;
        public const uint GL_FOG_COLOR = 2918;
        public const uint GL_DEPTH_RANGE = 2928;
        public const uint GL_DEPTH_TEST = 2929;
        public const uint GL_DEPTH_WRITEMASK = 2930;
        public const uint GL_DEPTH_CLEAR_VALUE = 2931;
        public const uint GL_DEPTH_FUNC = 2932;
        public const uint GL_ACCUM_CLEAR_VALUE = 2944;
        public const uint GL_STENCIL_TEST = 2960;
        public const uint GL_STENCIL_CLEAR_VALUE = 2961;
        public const uint GL_STENCIL_FUNC = 2962;
        public const uint GL_STENCIL_VALUE_MASK = 2963;
        public const uint GL_STENCIL_FAIL = 2964;
        public const uint GL_STENCIL_PASS_DEPTH_FAIL = 2965;
        public const uint GL_STENCIL_PASS_DEPTH_PASS = 2966;
        public const uint GL_STENCIL_REF = 2967;
        public const uint GL_STENCIL_WRITEMASK = 2968;
        public const uint GL_MATRIX_MODE = 2976;
        public const uint GL_NORMALIZE = 2977;
        public const uint GL_VIEWPORT = 2978;
        public const uint GL_MODELVIEW_STACK_DEPTH = 2979;
        public const uint GL_PROJECTION_STACK_DEPTH = 2980;
        public const uint GL_TEXTURE_STACK_DEPTH = 2981;
        public const uint GL_MODELVIEW_MATRIX = 2982;
        public const uint GL_PROJECTION_MATRIX = 2983;
        public const uint GL_TEXTURE_MATRIX = 2984;
        public const uint GL_ATTRIB_STACK_DEPTH = 2992;
        public const uint GL_CLIENT_ATTRIB_STACK_DEPTH = 2993;
        public const uint GL_ALPHA_TEST = 3008;
        public const uint GL_ALPHA_TEST_FUNC = 3009;
        public const uint GL_ALPHA_TEST_REF = 3010;
        public const uint GL_DITHER = 3024;
        public const uint GL_BLEND_DST = 3040;
        public const uint GL_BLEND_SRC = 3041;
        public const uint GL_BLEND = 3042;
        public const uint GL_LOGIC_OP_MODE = 3056;
        public const uint GL_INDEX_LOGIC_OP = 3057;
        public const uint GL_COLOR_LOGIC_OP = 3058;
        public const uint GL_AUX_BUFFERS = 3072;
        public const uint GL_DRAW_BUFFER = 3073;
        public const uint GL_READ_BUFFER = 3074;
        public const uint GL_SCISSOR_BOX = 3088;
        public const uint GL_SCISSOR_TEST = 3089;
        public const uint GL_INDEX_CLEAR_VALUE = 3104;
        public const uint GL_INDEX_WRITEMASK = 3105;
        public const uint GL_COLOR_CLEAR_VALUE = 3106;
        public const uint GL_COLOR_WRITEMASK = 3107;
        public const uint GL_INDEX_MODE = 3120;
        public const uint GL_RGBA_MODE = 3121;
        public const uint GL_DOUBLEBUFFER = 3122;
        public const uint GL_STEREO = 3123;
        public const uint GL_RENDER_MODE = 3136;
        public const uint GL_PERSPECTIVE_CORRECTION_HINT = 3152;
        public const uint GL_POINT_SMOOTH_HINT = 3153;
        public const uint GL_LINE_SMOOTH_HINT = 3154;
        public const uint GL_POLYGON_SMOOTH_HINT = 3155;
        public const uint GL_FOG_HINT = 3156;
        public const uint GL_TEXTURE_GEN_S = 3168;
        public const uint GL_TEXTURE_GEN_T = 3169;
        public const uint GL_TEXTURE_GEN_R = 3170;
        public const uint GL_TEXTURE_GEN_Q = 3171;
        public const uint GL_PIXEL_MAP_I_TO_I = 3184;
        public const uint GL_PIXEL_MAP_S_TO_S = 3185;
        public const uint GL_PIXEL_MAP_I_TO_R = 3186;
        public const uint GL_PIXEL_MAP_I_TO_G = 3187;
        public const uint GL_PIXEL_MAP_I_TO_B = 3188;
        public const uint GL_PIXEL_MAP_I_TO_A = 3189;
        public const uint GL_PIXEL_MAP_R_TO_R = 3190;
        public const uint GL_PIXEL_MAP_G_TO_G = 3191;
        public const uint GL_PIXEL_MAP_B_TO_B = 3192;
        public const uint GL_PIXEL_MAP_A_TO_A = 3193;
        public const uint GL_PIXEL_MAP_I_TO_I_SIZE = 3248;
        public const uint GL_PIXEL_MAP_S_TO_S_SIZE = 3249;
        public const uint GL_PIXEL_MAP_I_TO_R_SIZE = 3250;
        public const uint GL_PIXEL_MAP_I_TO_G_SIZE = 3251;
        public const uint GL_PIXEL_MAP_I_TO_B_SIZE = 3252;
        public const uint GL_PIXEL_MAP_I_TO_A_SIZE = 3253;
        public const uint GL_PIXEL_MAP_R_TO_R_SIZE = 3254;
        public const uint GL_PIXEL_MAP_G_TO_G_SIZE = 3255;
        public const uint GL_PIXEL_MAP_B_TO_B_SIZE = 3256;
        public const uint GL_PIXEL_MAP_A_TO_A_SIZE = 3257;
        public const uint GL_UNPACK_SWAP_BYTES = 3312;
        public const uint GL_UNPACK_LSB_FIRST = 3313;
        public const uint GL_UNPACK_ROW_LENGTH = 3314;
        public const uint GL_UNPACK_SKIP_ROWS = 3315;
        public const uint GL_UNPACK_SKIP_PIXELS = 3316;
        public const uint GL_UNPACK_ALIGNMENT = 3317;
        public const uint GL_PACK_SWAP_BYTES = 3328;
        public const uint GL_PACK_LSB_FIRST = 3329;
        public const uint GL_PACK_ROW_LENGTH = 3330;
        public const uint GL_PACK_SKIP_ROWS = 3331;
        public const uint GL_PACK_SKIP_PIXELS = 3332;
        public const uint GL_PACK_ALIGNMENT = 3333;
        public const uint GL_MAP_COLOR = 3344;
        public const uint GL_MAP_STENCIL = 3345;
        public const uint GL_INDEX_SHIFT = 3346;
        public const uint GL_INDEX_OFFSET = 3347;
        public const uint GL_RED_SCALE = 3348;
        public const uint GL_RED_BIAS = 3349;
        public const uint GL_ZOOM_X = 3350;
        public const uint GL_ZOOM_Y = 3351;
        public const uint GL_GREEN_SCALE = 3352;
        public const uint GL_GREEN_BIAS = 3353;
        public const uint GL_BLUE_SCALE = 3354;
        public const uint GL_BLUE_BIAS = 3355;
        public const uint GL_ALPHA_SCALE = 3356;
        public const uint GL_ALPHA_BIAS = 3357;
        public const uint GL_DEPTH_SCALE = 3358;
        public const uint GL_DEPTH_BIAS = 3359;
        public const uint GL_MAX_EVAL_ORDER = 3376;
        public const uint GL_MAX_LIGHTS = 3377;
        public const uint GL_MAX_CLIP_PLANES = 3378;
        public const uint GL_MAX_TEXTURE_SIZE = 3379;
        public const uint GL_MAX_PIXEL_MAP_TABLE = 3380;
        public const uint GL_MAX_ATTRIB_STACK_DEPTH = 3381;
        public const uint GL_MAX_MODELVIEW_STACK_DEPTH = 3382;
        public const uint GL_MAX_NAME_STACK_DEPTH = 3383;
        public const uint GL_MAX_PROJECTION_STACK_DEPTH = 3384;
        public const uint GL_MAX_TEXTURE_STACK_DEPTH = 3385;
        public const uint GL_MAX_VIEWPORT_DIMS = 3386;
        public const uint GL_MAX_CLIENT_ATTRIB_STACK_DEPTH = 3387;
        public const uint GL_SUBPIXEL_BITS = 3408;
        public const uint GL_INDEX_BITS = 3409;
        public const uint GL_RED_BITS = 3410;
        public const uint GL_GREEN_BITS = 3411;
        public const uint GL_BLUE_BITS = 3412;
        public const uint GL_ALPHA_BITS = 3413;
        public const uint GL_DEPTH_BITS = 3414;
        public const uint GL_STENCIL_BITS = 3415;
        public const uint GL_ACCUM_RED_BITS = 3416;
        public const uint GL_ACCUM_GREEN_BITS = 3417;
        public const uint GL_ACCUM_BLUE_BITS = 3418;
        public const uint GL_ACCUM_ALPHA_BITS = 3419;
        public const uint GL_NAME_STACK_DEPTH = 3440;
        public const uint GL_AUTO_NORMAL = 3456;
        public const uint GL_MAP1_COLOR_4 = 3472;
        public const uint GL_MAP1_INDEX = 3473;
        public const uint GL_MAP1_NORMAL = 3474;
        public const uint GL_MAP1_TEXTURE_COORD_1 = 3475;
        public const uint GL_MAP1_TEXTURE_COORD_2 = 3476;
        public const uint GL_MAP1_TEXTURE_COORD_3 = 3477;
        public const uint GL_MAP1_TEXTURE_COORD_4 = 3478;
        public const uint GL_MAP1_VERTEX_3 = 3479;
        public const uint GL_MAP1_VERTEX_4 = 3480;
        public const uint GL_MAP2_COLOR_4 = 3504;
        public const uint GL_MAP2_INDEX = 3505;
        public const uint GL_MAP2_NORMAL = 3506;
        public const uint GL_MAP2_TEXTURE_COORD_1 = 3507;
        public const uint GL_MAP2_TEXTURE_COORD_2 = 3508;
        public const uint GL_MAP2_TEXTURE_COORD_3 = 3509;
        public const uint GL_MAP2_TEXTURE_COORD_4 = 3510;
        public const uint GL_MAP2_VERTEX_3 = 3511;
        public const uint GL_MAP2_VERTEX_4 = 3512;
        public const uint GL_MAP1_GRID_DOMAIN = 3536;
        public const uint GL_MAP1_GRID_SEGMENTS = 3537;
        public const uint GL_MAP2_GRID_DOMAIN = 3538;
        public const uint GL_MAP2_GRID_SEGMENTS = 3539;
        public const uint GL_TEXTURE_1D = 3552;
        public const uint GL_TEXTURE_2D = 3553;
        public const uint GL_FEEDBACK_BUFFER_POINTER = 3568;
        public const uint GL_FEEDBACK_BUFFER_SIZE = 3569;
        public const uint GL_FEEDBACK_BUFFER_TYPE = 3570;
        public const uint GL_SELECTION_BUFFER_POINTER = 3571;
        public const uint GL_SELECTION_BUFFER_SIZE = 3572;
        public const uint GL_TEXTURE_WIDTH = 4096;
        public const uint GL_TEXTURE_HEIGHT = 4097;
        public const uint GL_TEXTURE_INTERNAL_FORMAT = 4099;
        public const uint GL_TEXTURE_BORDER_COLOR = 4100;
        public const uint GL_TEXTURE_BORDER = 4101;
        public const uint GL_DONT_CARE = 4352;
        public const uint GL_FASTEST = 4353;
        public const uint GL_NICEST = 4354;
        public const uint GL_LIGHT0 = 16384;
        public const uint GL_LIGHT1 = 16385;
        public const uint GL_LIGHT2 = 16386;
        public const uint GL_LIGHT3 = 16387;
        public const uint GL_LIGHT4 = 16388;
        public const uint GL_LIGHT5 = 16389;
        public const uint GL_LIGHT6 = 16390;
        public const uint GL_LIGHT7 = 16391;
        public const uint GL_AMBIENT = 4608;
        public const uint GL_DIFFUSE = 4609;
        public const uint GL_SPECULAR = 4610;
        public const uint GL_POSITION = 4611;
        public const uint GL_SPOT_DIRECTION = 4612;
        public const uint GL_SPOT_EXPONENT = 4613;
        public const uint GL_SPOT_CUTOFF = 4614;
        public const uint GL_CONSTANT_ATTENUATION = 4615;
        public const uint GL_LINEAR_ATTENUATION = 4616;
        public const uint GL_QUADRATIC_ATTENUATION = 4617;
        public const uint GL_COMPILE = 4864;
        public const uint GL_COMPILE_AND_EXECUTE = 4865;
        public const uint GL_CLEAR = 5376;
        public const uint GL_AND = 5377;
        public const uint GL_AND_REVERSE = 5378;
        public const uint GL_COPY = 5379;
        public const uint GL_AND_INVERTED = 5380;
        public const uint GL_NOOP = 5381;
        public const uint GL_XOR = 5382;
        public const uint GL_OR = 5383;
        public const uint GL_NOR = 5384;
        public const uint GL_EQUIV = 5385;
        public const uint GL_INVERT = 5386;
        public const uint GL_OR_REVERSE = 5387;
        public const uint GL_COPY_INVERTED = 5388;
        public const uint GL_OR_INVERTED = 5389;
        public const uint GL_NAND = 5390;
        public const uint GL_SET = 5391;
        public const uint GL_EMISSION = 5632;
        public const uint GL_SHININESS = 5633;
        public const uint GL_AMBIENT_AND_DIFFUSE = 5634;
        public const uint GL_COLOR_INDEXES = 5635;
        public const uint GL_MODELVIEW = 5888;
        public const uint GL_PROJECTION = 5889;
        public const uint GL_TEXTURE = 5890;
        public const uint GL_COLOR = 6144;
        public const uint GL_DEPTH = 6145;
        public const uint GL_STENCIL = 6146;
        public const uint GL_COLOR_INDEX = 6400;
        public const uint GL_STENCIL_INDEX = 6401;
        public const uint GL_DEPTH_COMPONENT = 6402;
        public const uint GL_RED = 6403;
        public const uint GL_GREEN = 6404;
        public const uint GL_BLUE = 6405;
        public const uint GL_ALPHA = 6406;
        public const uint GL_RGB = 6407;
        public const uint GL_RGBA = 6408;
        public const uint GL_LUMINANCE = 6409;
        public const uint GL_LUMINANCE_ALPHA = 6410;
        public const uint GL_BITMAP = 6656;
        public const uint GL_POINT = 6912;
        public const uint GL_LINE = 6913;
        public const uint GL_FILL = 6914;
        public const uint GL_RENDER = 7168;
        public const uint GL_FEEDBACK = 7169;
        public const uint GL_SELECT = 7170;
        public const uint GL_FLAT = 7424;
        public const uint GL_SMOOTH = 7425;
        public const uint GL_KEEP = 7680;
        public const uint GL_REPLACE = 7681;
        public const uint GL_INCR = 7682;
        public const uint GL_DECR = 7683;
        public const uint GL_VENDOR = 7936;
        public const uint GL_RENDERER = 7937;
        public const uint GL_VERSION = 7938;
        public const uint GL_EXTENSIONS = 7939;
        public const uint GL_S = 8192;
        public const uint GL_T = 8193;
        public const uint GL_R = 8194;
        public const uint GL_Q = 8195;
        public const uint GL_MODULATE = 8448;
        public const uint GL_DECAL = 8449;
        public const uint GL_TEXTURE_ENV_MODE = 8704;
        public const uint GL_TEXTURE_ENV_COLOR = 8705;
        public const uint GL_TEXTURE_ENV = 8960;
        public const uint GL_EYE_LINEAR = 9216;
        public const uint GL_OBJECT_LINEAR = 9217;
        public const uint GL_SPHERE_MAP = 9218;
        public const uint GL_TEXTURE_GEN_MODE = 9472;
        public const uint GL_OBJECT_PLANE = 9473;
        public const uint GL_EYE_PLANE = 9474;
        public const uint GL_NEAREST = 9728;
        public const uint GL_LINEAR = 9729;
        public const uint GL_NEAREST_MIPMAP_NEAREST = 9984;
        public const uint GL_LINEAR_MIPMAP_NEAREST = 9985;
        public const uint GL_NEAREST_MIPMAP_LINEAR = 9986;
        public const uint GL_LINEAR_MIPMAP_LINEAR = 9987;
        public const uint GL_TEXTURE_MAG_FILTER = 10240;
        public const uint GL_TEXTURE_MIN_FILTER = 10241;
        public const uint GL_TEXTURE_WRAP_S = 10242;
        public const uint GL_TEXTURE_WRAP_T = 10243;
        public const uint GL_CLAMP = 10496;
        public const uint GL_REPEAT = 10497;
        public const int GL_CLIENT_PIXEL_STORE_BIT = 1;
        public const int GL_CLIENT_VERTEX_ARRAY_BIT = 2;
        public const int GL_CLIENT_ALL_ATTRIB_BITS = -1;
        public const uint GL_POLYGON_OFFSET_FACTOR = 32824;
        public const uint GL_POLYGON_OFFSET_UNITS = 10752;
        public const uint GL_POLYGON_OFFSET_POINT = 10753;
        public const uint GL_POLYGON_OFFSET_LINE = 10754;
        public const uint GL_POLYGON_OFFSET_FILL = 32823;
        public const uint GL_ALPHA4 = 32827;
        public const uint GL_ALPHA8 = 32828;
        public const uint GL_ALPHA12 = 32829;
        public const uint GL_ALPHA16 = 32830;
        public const uint GL_LUMINANCE4 = 32831;
        public const uint GL_LUMINANCE8 = 32832;
        public const uint GL_LUMINANCE12 = 32833;
        public const uint GL_LUMINANCE16 = 32834;
        public const uint GL_LUMINANCE4_ALPHA4 = 32835;
        public const uint GL_LUMINANCE6_ALPHA2 = 32836;
        public const uint GL_LUMINANCE8_ALPHA8 = 32837;
        public const uint GL_LUMINANCE12_ALPHA4 = 32838;
        public const uint GL_LUMINANCE12_ALPHA12 = 32839;
        public const uint GL_LUMINANCE16_ALPHA16 = 32840;
        public const uint GL_INTENSITY = 32841;
        public const uint GL_INTENSITY4 = 32842;
        public const uint GL_INTENSITY8 = 32843;
        public const uint GL_INTENSITY12 = 32844;
        public const uint GL_INTENSITY16 = 32845;
        public const uint GL_R3_G3_B2 = 10768;
        public const uint GL_RGB4 = 32847;
        public const uint GL_RGB5 = 32848;
        public const uint GL_RGB8 = 32849;
        public const uint GL_RGB10 = 32850;
        public const uint GL_RGB12 = 32851;
        public const uint GL_RGB16 = 32852;
        public const uint GL_RGBA2 = 32853;
        public const uint GL_RGBA4 = 32854;
        public const uint GL_RGB5_A1 = 32855;
        public const uint GL_RGBA8 = 32856;
        public const uint GL_RGB10_A2 = 32857;
        public const uint GL_RGBA12 = 32858;
        public const uint GL_RGBA16 = 32859;
        public const uint GL_TEXTURE_RED_SIZE = 32860;
        public const uint GL_TEXTURE_GREEN_SIZE = 32861;
        public const uint GL_TEXTURE_BLUE_SIZE = 32862;
        public const uint GL_TEXTURE_ALPHA_SIZE = 32863;
        public const uint GL_TEXTURE_LUMINANCE_SIZE = 32864;
        public const uint GL_TEXTURE_INTENSITY_SIZE = 32865;
        public const uint GL_PROXY_TEXTURE_1D = 32867;
        public const uint GL_PROXY_TEXTURE_2D = 32868;
        public const uint GL_TEXTURE_PRIORITY = 32870;
        public const uint GL_TEXTURE_RESIDENT = 32871;
        public const uint GL_TEXTURE_BINDING_1D = 32872;
        public const uint GL_TEXTURE_BINDING_2D = 32873;
        public const uint GL_VERTEX_ARRAY = 32884;
        public const uint GL_NORMAL_ARRAY = 32885;
        public const uint GL_COLOR_ARRAY = 32886;
        public const uint GL_INDEX_ARRAY = 32887;
        public const uint GL_TEXTURE_COORD_ARRAY = 32888;
        public const uint GL_EDGE_FLAG_ARRAY = 32889;
        public const uint GL_VERTEX_ARRAY_SIZE = 32890;
        public const uint GL_VERTEX_ARRAY_TYPE = 32891;
        public const uint GL_VERTEX_ARRAY_STRIDE = 32892;
        public const uint GL_NORMAL_ARRAY_TYPE = 32894;
        public const uint GL_NORMAL_ARRAY_STRIDE = 32895;
        public const uint GL_COLOR_ARRAY_SIZE = 32897;
        public const uint GL_COLOR_ARRAY_TYPE = 32898;
        public const uint GL_COLOR_ARRAY_STRIDE = 32899;
        public const uint GL_INDEX_ARRAY_TYPE = 32901;
        public const uint GL_INDEX_ARRAY_STRIDE = 32902;
        public const uint GL_TEXTURE_COORD_ARRAY_SIZE = 32904;
        public const uint GL_TEXTURE_COORD_ARRAY_TYPE = 32905;
        public const uint GL_TEXTURE_COORD_ARRAY_STRIDE = 32906;
        public const uint GL_EDGE_FLAG_ARRAY_STRIDE = 32908;
        public const uint GL_VERTEX_ARRAY_POINTER = 32910;
        public const uint GL_NORMAL_ARRAY_POINTER = 32911;
        public const uint GL_COLOR_ARRAY_POINTER = 32912;
        public const uint GL_INDEX_ARRAY_POINTER = 32913;
        public const uint GL_TEXTURE_COORD_ARRAY_POINTER = 32914;
        public const uint GL_EDGE_FLAG_ARRAY_POINTER = 32915;
        public const uint GL_V2F = 10784;
        public const uint GL_V3F = 10785;
        public const uint GL_C4UB_V2F = 10786;
        public const uint GL_C4UB_V3F = 10787;
        public const uint GL_C3F_V3F = 10788;
        public const uint GL_N3F_V3F = 10789;
        public const uint GL_C4F_N3F_V3F = 10790;
        public const uint GL_T2F_V3F = 10791;
        public const uint GL_T4F_V4F = 10792;
        public const uint GL_T2F_C4UB_V3F = 10793;
        public const uint GL_T2F_C3F_V3F = 10794;
        public const uint GL_T2F_N3F_V3F = 10795;
        public const uint GL_T2F_C4F_N3F_V3F = 10796;
        public const uint GL_T4F_C4F_N3F_V4F = 10797;
        public const uint GL_EXT_vertex_array = 1;
        public const uint GL_EXT_bgra = 1;
        public const uint GL_EXT_paletted_texture = 1;
        public const uint GL_WIN_swap_hint = 1;
        public const uint GL_WIN_draw_range_elements = 1;
        public const uint GL_VERTEX_ARRAY_EXT = 32884;
        public const uint GL_NORMAL_ARRAY_EXT = 32885;
        public const uint GL_COLOR_ARRAY_EXT = 32886;
        public const uint GL_INDEX_ARRAY_EXT = 32887;
        public const uint GL_TEXTURE_COORD_ARRAY_EXT = 32888;
        public const uint GL_EDGE_FLAG_ARRAY_EXT = 32889;
        public const uint GL_VERTEX_ARRAY_SIZE_EXT = 32890;
        public const uint GL_VERTEX_ARRAY_TYPE_EXT = 32891;
        public const uint GL_VERTEX_ARRAY_STRIDE_EXT = 32892;
        public const uint GL_VERTEX_ARRAY_COUNT_EXT = 32893;
        public const uint GL_NORMAL_ARRAY_TYPE_EXT = 32894;
        public const uint GL_NORMAL_ARRAY_STRIDE_EXT = 32895;
        public const uint GL_NORMAL_ARRAY_COUNT_EXT = 32896;
        public const uint GL_COLOR_ARRAY_SIZE_EXT = 32897;
        public const uint GL_COLOR_ARRAY_TYPE_EXT = 32898;
        public const uint GL_COLOR_ARRAY_STRIDE_EXT = 32899;
        public const uint GL_COLOR_ARRAY_COUNT_EXT = 32900;
        public const uint GL_INDEX_ARRAY_TYPE_EXT = 32901;
        public const uint GL_INDEX_ARRAY_STRIDE_EXT = 32902;
        public const uint GL_INDEX_ARRAY_COUNT_EXT = 32903;
        public const uint GL_TEXTURE_COORD_ARRAY_SIZE_EXT = 32904;
        public const uint GL_TEXTURE_COORD_ARRAY_TYPE_EXT = 32905;
        public const uint GL_TEXTURE_COORD_ARRAY_STRIDE_EXT = 32906;
        public const uint GL_TEXTURE_COORD_ARRAY_COUNT_EXT = 32907;
        public const uint GL_EDGE_FLAG_ARRAY_STRIDE_EXT = 32908;
        public const uint GL_EDGE_FLAG_ARRAY_COUNT_EXT = 32909;
        public const uint GL_VERTEX_ARRAY_POINTER_EXT = 32910;
        public const uint GL_NORMAL_ARRAY_POINTER_EXT = 32911;
        public const uint GL_COLOR_ARRAY_POINTER_EXT = 32912;
        public const uint GL_INDEX_ARRAY_POINTER_EXT = 32913;
        public const uint GL_TEXTURE_COORD_ARRAY_POINTER_EXT = 32914;
        public const uint GL_EDGE_FLAG_ARRAY_POINTER_EXT = 32915;
        public const uint GL_DOUBLE_EXT = GL_DOUBLE;
        public const uint GL_BGR_EXT = 32992;
        public const uint GL_BGRA_EXT = 32993;
        public const uint GL_COLOR_TABLE_FORMAT_EXT = 32984;
        public const uint GL_COLOR_TABLE_WIDTH_EXT = 32985;
        public const uint GL_COLOR_TABLE_RED_SIZE_EXT = 32986;
        public const uint GL_COLOR_TABLE_GREEN_SIZE_EXT = 32987;
        public const uint GL_COLOR_TABLE_BLUE_SIZE_EXT = 32988;
        public const uint GL_COLOR_TABLE_ALPHA_SIZE_EXT = 32989;
        public const uint GL_COLOR_TABLE_LUMINANCE_SIZE_EXT = 32990;
        public const uint GL_COLOR_TABLE_INTENSITY_SIZE_EXT = 32991;
        public const uint GL_COLOR_INDEX1_EXT = 32994;
        public const uint GL_COLOR_INDEX2_EXT = 32995;
        public const uint GL_COLOR_INDEX4_EXT = 32996;
        public const uint GL_COLOR_INDEX8_EXT = 32997;
        public const uint GL_COLOR_INDEX12_EXT = 32998;
        public const uint GL_COLOR_INDEX16_EXT = 32999;
        public const uint GL_MAX_ELEMENTS_VERTICES_WIN = 33000;
        public const uint GL_MAX_ELEMENTS_INDICES_WIN = 33001;
        public const uint GL_PHONG_WIN = 33002;
        public const uint GL_PHONG_HINT_WIN = 33003;
        public const uint GL_FOG_SPECULAR_TEXTURE_WIN = 33004;
        public const uint GL_LOGIC_OP = GL_INDEX_LOGIC_OP;
        public const uint GL_TEXTURE_COMPONENTS = GL_TEXTURE_INTERNAL_FORMAT;
        #endregion

        #region Delegates
        public delegate void PFNGLARRAYELEMENTEXTPROC( int i );
        public delegate void PFNGLDRAWARRAYSEXTPROC( uint mode, int first, int count );
        public delegate void PFNGLVERTEXPOINTEREXTPROC( int size, uint type, int stride, int count, IntPtr pointer );
        public delegate void PFNGLNORMALPOINTEREXTPROC( uint type, int stride, int count, IntPtr pointer );
        public delegate void PFNGLCOLORPOINTEREXTPROC( int size, uint type, int stride, int count, IntPtr pointer );
        public delegate void PFNGLINDEXPOINTEREXTPROC( uint type, int stride, int count, IntPtr pointer );
        public delegate void PFNGLTEXCOORDPOINTEREXTPROC( int size, uint type, int stride, int count, IntPtr pointer );
        public delegate void PFNGLEDGEFLAGPOINTEREXTPROC( int stride, int count, [In] [MarshalAs( UnmanagedType.LPStr )] string pointer );
        public delegate void PFNGLGETPOINTERVEXTPROC( uint pname, IntPtr[] parameters );
        public delegate void PFNGLARRAYELEMENTARRAYEXTPROC( uint mode, int count, IntPtr pi );
        public delegate void PFNGLDRAWRANGEELEMENTSWINPROC( uint mode, uint start, uint end, int count, uint type, IntPtr indices );
        public delegate void PFNGLADDSWAPHINTRECTWINPROC( int x, int y, int width, int height );
        public delegate void PFNGLCOLORTABLEEXTPROC( uint target, uint internalFormat, int width, uint format, uint type, IntPtr data );
        public delegate void PFNGLCOLORSUBTABLEEXTPROC( uint target, int start, int count, uint format, uint type, IntPtr data );
        public delegate void PFNGLGETCOLORTABLEEXTPROC( uint target, uint format, uint type, IntPtr data );
        public delegate void PFNGLGETCOLORTABLEPARAMETERIVEXTPROC( uint target, uint pname, int[] parameters );
        public delegate void PFNGLGETCOLORTABLEPARAMETERFVEXTPROC( uint target, uint pname, float[] parameters );
        public delegate void PFNGLACCUMPROC( uint op, float value );
        public delegate void PFNGLALPHAFUNCPROC( uint func, float alpha );
        public delegate byte PFNGLARETEXTURESRESIDENTPROC( int n, uint[] textures, IntPtr residences );
        public delegate void PFNGLARRAYELEMENTPROC( int i );
        public delegate void PFNGLBEGINPROC( uint mode );
        public delegate void PFNGLBINDTEXTUREPROC( uint target, uint texture );
        public delegate void PFNGLBITMAPPROC( int width, int height, float xorig, float yorig, float xmove, float ymove, [In] [MarshalAs( UnmanagedType.LPStr )] string bitmap );
        public delegate void PFNGLBLENDFUNCPROC( uint sfactor, uint dfactor );
        public delegate void PFNGLCALLLISTPROC( uint list );
        public delegate void PFNGLCALLLISTSPROC( int n, uint type, IntPtr lists );
        public delegate void PFNGLCLEARPROC( uint mask );
        public delegate void PFNGLCLEARACCUMPROC( float red, float green, float blue, float alpha );
        public delegate void PFNGLCLEARCOLORPROC( float red, float green, float blue, float alpha );
        public delegate void PFNGLCLEARDEPTHPROC( double depth );
        public delegate void PFNGLCLEARINDEXPROC( float c );
        public delegate void PFNGLCLEARSTENCILPROC( int s );
        public delegate void PFNGLCLIPPLANEPROC( uint plane, double[] equation );
        public delegate void PFNGLCOLOR3BPROC( byte red, byte green, byte blue );
        public delegate void PFNGLCOLOR3BVPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLCOLOR3DPROC( double red, double green, double blue );
        public delegate void PFNGLCOLOR3DVPROC( double[] v );
        public delegate void PFNGLCOLOR3FPROC( float red, float green, float blue );
        public delegate void PFNGLCOLOR3FVPROC( float[] v );
        public delegate void PFNGLCOLOR3IPROC( int red, int green, int blue );
        public delegate void PFNGLCOLOR3IVPROC( int[] v );
        public delegate void PFNGLCOLOR3SPROC( short red, short green, short blue );
        public delegate void PFNGLCOLOR3SVPROC( short[] v );
        public delegate void PFNGLCOLOR3UBPROC( byte red, byte green, byte blue );
        public delegate void PFNGLCOLOR3UBVPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLCOLOR3UIPROC( uint red, uint green, uint blue );
        public delegate void PFNGLCOLOR3UIVPROC( uint[] v );
        public delegate void PFNGLCOLOR3USPROC( ushort red, ushort green, ushort blue );
        public delegate void PFNGLCOLOR3USVPROC( ushort[] v );
        public delegate void PFNGLCOLOR4BPROC( byte red, byte green, byte blue, byte alpha );
        public delegate void PFNGLCOLOR4BVPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLCOLOR4DPROC( double red, double green, double blue, double alpha );
        public delegate void PFNGLCOLOR4DVPROC( double[] v );
        public delegate void PFNGLCOLOR4FPROC( float red, float green, float blue, float alpha );
        public delegate void PFNGLCOLOR4FVPROC( float[] v );
        public delegate void PFNGLCOLOR4IPROC( int red, int green, int blue, int alpha );
        public delegate void PFNGLCOLOR4IVPROC( int[] v );
        public delegate void PFNGLCOLOR4SPROC( short red, short green, short blue, short alpha );
        public delegate void PFNGLCOLOR4SVPROC( short[] v );
        public delegate void PFNGLCOLOR4UBPROC( byte red, byte green, byte blue, byte alpha );
        public delegate void PFNGLCOLOR4UBVPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLCOLOR4UIPROC( uint red, uint green, uint blue, uint alpha );
        public delegate void PFNGLCOLOR4UIVPROC( uint[] v );
        public delegate void PFNGLCOLOR4USPROC( ushort red, ushort green, ushort blue, ushort alpha );
        public delegate void PFNGLCOLOR4USVPROC( ushort[] v );
        public delegate void PFNGLCOLORMASKPROC( byte red, byte green, byte blue, byte alpha );
        public delegate void PFNGLCOLORMATERIALPROC( uint face, uint mode );
        public delegate void PFNGLCOLORPOINTERPROC( int size, uint type, int stride, IntPtr pointer );
        public delegate void PFNGLCOPYPIXELSPROC( int x, int y, int width, int height, uint type );
        public delegate void PFNGLCOPYTEXIMAGE1DPROC( uint target, int level, uint internalFormat, int x, int y, int width, int border );
        public delegate void PFNGLCOPYTEXIMAGE2DPROC( uint target, int level, uint internalFormat, int x, int y, int width, int height, int border );
        public delegate void PFNGLCOPYTEXSUBIMAGE1DPROC( uint target, int level, int xoffset, int x, int y, int width );
        public delegate void PFNGLCOPYTEXSUBIMAGE2DPROC( uint target, int level, int xoffset, int yoffset, int x, int y, int width, int height );
        public delegate void PFNGLCULLFACEPROC( uint mode );
        public delegate void PFNGLDELETELISTSPROC( uint list, int range );
        public delegate void PFNGLDELETETEXTURESPROC( int n, uint[] textures );
        public delegate void PFNGLDEPTHFUNCPROC( uint func );
        public delegate void PFNGLDEPTHMASKPROC( byte flag );
        public delegate void PFNGLDEPTHRANGEPROC( double zNear, double zFar );
        public delegate void PFNGLDISABLEPROC( uint cap );
        public delegate void PFNGLDISABLECLIENTSTATEPROC( uint array );
        public delegate void PFNGLDRAWARRAYSPROC( uint mode, int first, int count );
        public delegate void PFNGLDRAWBUFFERPROC( uint mode );
        public delegate void PFNGLDRAWELEMENTSPROC( uint mode, int count, uint type, IntPtr indices );
        public delegate void PFNGLDRAWPIXELSPROC( int width, int height, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLEDGEFLAGPROC( byte flag );
        public delegate void PFNGLEDGEFLAGPOINTERPROC( int stride, IntPtr pointer );
        public delegate void PFNGLEDGEFLAGVPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string flag );
        public delegate void PFNGLENABLEPROC( uint cap );
        public delegate void PFNGLENABLECLIENTSTATEPROC( uint array );
        public delegate void PFNGLENDPROC();
        public delegate void PFNGLENDLISTPROC();
        public delegate void PFNGLEVALCOORD1DPROC( double u );
        public delegate void PFNGLEVALCOORD1DVPROC( double[] u );
        public delegate void PFNGLEVALCOORD1FPROC( float u );
        public delegate void PFNGLEVALCOORD1FVPROC( float[] u );
        public delegate void PFNGLEVALCOORD2DPROC( double u, double v );
        public delegate void PFNGLEVALCOORD2DVPROC( double[] u );
        public delegate void PFNGLEVALCOORD2FPROC( float u, float v );
        public delegate void PFNGLEVALCOORD2FVPROC( float[] u );
        public delegate void PFNGLEVALMESH1PROC( uint mode, int i1, int i2 );
        public delegate void PFNGLEVALMESH2PROC( uint mode, int i1, int i2, int j1, int j2 );
        public delegate void PFNGLEVALPOINT1PROC( int i );
        public delegate void PFNGLEVALPOINT2PROC( int i, int j );
        public delegate void PFNGLFEEDBACKBUFFERPROC( int size, uint type, float[] buffer );
        public delegate void PFNGLFINISHPROC();
        public delegate void PFNGLFLUSHPROC();
        public delegate void PFNGLFOGFPROC( uint pname, float param );
        public delegate void PFNGLFOGFVPROC( uint pname, float[] parameters );
        public delegate void PFNGLFOGIPROC( uint pname, int param );
        public delegate void PFNGLFOGIVPROC( uint pname, int[] parameters );
        public delegate void PFNGLFRONTFACEPROC( uint mode );
        public delegate void PFNGLFRUSTUMPROC( double left, double right, double bottom, double top, double zNear, double zFar );
        public delegate uint PFNGLGENLISTSPROC( int range );
        public delegate void PFNGLGENTEXTURESPROC( int n, ref uint textures );
        public delegate void PFNGLGETBOOLEANVPROC( uint pname, IntPtr parameters );
        public delegate void PFNGLGETCLIPPLANEPROC( uint plane, double[] equation );
        public delegate void PFNGLGETDOUBLEVPROC( uint pname, double[] parameters );
        public delegate uint PFNGLGETERRORPROC();
        public delegate void PFNGLGETFLOATVPROC( uint pname, float[] parameters );
        public delegate void PFNGLGETINTEGERVPROC( uint pname, int[] parameters );
        public delegate void PFNGLGETLIGHTFVPROC( uint light, uint pname, float[] parameters );
        public delegate void PFNGLGETLIGHTIVPROC( uint light, uint pname, int[] parameters );
        public delegate void PFNGLGETMAPDVPROC( uint target, uint query, double[] v );
        public delegate void PFNGLGETMAPFVPROC( uint target, uint query, float[] v );
        public delegate void PFNGLGETMAPIVPROC( uint target, uint query, int[] v );
        public delegate void PFNGLGETMATERIALFVPROC( uint face, uint pname, float[] parameters );
        public delegate void PFNGLGETMATERIALIVPROC( uint face, uint pname, int[] parameters );
        public delegate void PFNGLGETPIXELMAPFVPROC( uint map, float[] values );
        public delegate void PFNGLGETPIXELMAPUIVPROC( uint map, uint[] values );
        public delegate void PFNGLGETPIXELMAPUSVPROC( uint map, ushort[] values );
        public delegate void PFNGLGETPOINTERVPROC( uint pname, ref IntPtr parameters );
        public delegate void PFNGLGETPOLYGONSTIPPLEPROC( IntPtr mask );
        public delegate void PFNGLGETTEXENVFVPROC( uint target, uint pname, float[] parameters );
        public delegate void PFNGLGETTEXENVIVPROC( uint target, uint pname, int[] parameters );
        public delegate void PFNGLGETTEXGENDVPROC( uint coord, uint pname, double[] parameters );
        public delegate void PFNGLGETTEXGENFVPROC( uint coord, uint pname, float[] parameters );
        public delegate void PFNGLGETTEXGENIVPROC( uint coord, uint pname, int[] parameters );
        public delegate void PFNGLGETTEXIMAGEPROC( uint target, int level, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLGETTEXLEVELPARAMETERFVPROC( uint target, int level, uint pname, float[] parameters );
        public delegate void PFNGLGETTEXLEVELPARAMETERIVPROC( uint target, int level, uint pname, int[] parameters );
        public delegate void PFNGLGETTEXPARAMETERFVPROC( uint target, uint pname, float[] parameters );
        public delegate void PFNGLGETTEXPARAMETERIVPROC( uint target, uint pname, int[] parameters );
        public delegate void PFNGLHINTPROC( uint target, uint mode );
        public delegate void PFNGLINDEXMASKPROC( uint mask );
        public delegate void PFNGLINDEXPOINTERPROC( uint type, int stride, IntPtr pointer );
        public delegate void PFNGLINDEXDPROC( double c );
        public delegate void PFNGLINDEXDVPROC( double[] c );
        public delegate void PFNGLINDEXFPROC( float c );
        public delegate void PFNGLINDEXFVPROC( float[] c );
        public delegate void PFNGLINDEXIPROC( int c );
        public delegate void PFNGLINDEXIVPROC( int[] c );
        public delegate void PFNGLINDEXSPROC( short c );
        public delegate void PFNGLINDEXSVPROC( short[] c );
        public delegate void PFNGLINDEXUBPROC( byte c );
        public delegate void PFNGLINDEXUBVPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string c );
        public delegate void PFNGLINITNAMESPROC();
        public delegate void PFNGLINTERLEAVEDARRAYSPROC( uint format, int stride, IntPtr pointer );
        public delegate byte PFNGLISENABLEDPROC( uint cap );
        public delegate byte PFNGLISLISTPROC( uint list );
        public delegate byte PFNGLISTEXTUREPROC( uint texture );
        public delegate void PFNGLLIGHTMODELFPROC( uint pname, float param );
        public delegate void PFNGLLIGHTMODELFVPROC( uint pname, float[] parameters );
        public delegate void PFNGLLIGHTMODELIPROC( uint pname, int param );
        public delegate void PFNGLLIGHTMODELIVPROC( uint pname, int[] parameters );
        public delegate void PFNGLLIGHTFPROC( uint light, uint pname, float param );
        public delegate void PFNGLLIGHTFVPROC( uint light, uint pname, float[] parameters );
        public delegate void PFNGLLIGHTIPROC( uint light, uint pname, int param );
        public delegate void PFNGLLIGHTIVPROC( uint light, uint pname, int[] parameters );
        public delegate void PFNGLLINESTIPPLEPROC( int factor, ushort pattern );
        public delegate void PFNGLLINEWIDTHPROC( float width );
        public delegate void PFNGLLISTBASEPROC( uint b );
        public delegate void PFNGLLOADIDENTITYPROC();
        public delegate void PFNGLLOADMATRIXDPROC( double[] m );
        public delegate void PFNGLLOADMATRIXFPROC( float[] m );
        public delegate void PFNGLLOADNAMEPROC( uint name );
        public delegate void PFNGLLOGICOPPROC( uint opcode );
        public delegate void PFNGLMAP1DPROC( uint target, double u1, double u2, int stride, int order, double[] points );
        public delegate void PFNGLMAP1FPROC( uint target, float u1, float u2, int stride, int order, float[] points );
        public delegate void PFNGLMAP2DPROC( uint target, double u1, double u2, int ustride, int uorder, double v1, double v2, int vstride, int vorder, double[] points );
        public delegate void PFNGLMAP2FPROC( uint target, float u1, float u2, int ustride, int uorder, float v1, float v2, int vstride, int vorder, float[] points );
        public delegate void PFNGLMAPGRID1DPROC( int un, double u1, double u2 );
        public delegate void PFNGLMAPGRID1FPROC( int un, float u1, float u2 );
        public delegate void PFNGLMAPGRID2DPROC( int un, double u1, double u2, int vn, double v1, double v2 );
        public delegate void PFNGLMAPGRID2FPROC( int un, float u1, float u2, int vn, float v1, float v2 );
        public delegate void PFNGLMATERIALFPROC( uint face, uint pname, float param );
        public delegate void PFNGLMATERIALFVPROC( uint face, uint pname, float[] parameters );
        public delegate void PFNGLMATERIALIPROC( uint face, uint pname, int param );
        public delegate void PFNGLMATERIALIVPROC( uint face, uint pname, int[] parameters );
        public delegate void PFNGLMATRIXMODEPROC( uint mode );
        public delegate void PFNGLMULTMATRIXDPROC( double[] m );
        public delegate void PFNGLMULTMATRIXFPROC( float[] m );
        public delegate void PFNGLNEWLISTPROC( uint list, uint mode );
        public delegate void PFNGLNORMAL3BPROC( byte nx, byte ny, byte nz );
        public delegate void PFNGLNORMAL3BVPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLNORMAL3DPROC( double nx, double ny, double nz );
        public delegate void PFNGLNORMAL3DVPROC( double[] v );
        public delegate void PFNGLNORMAL3FPROC( float nx, float ny, float nz );
        public delegate void PFNGLNORMAL3FVPROC( float[] v );
        public delegate void PFNGLNORMAL3IPROC( int nx, int ny, int nz );
        public delegate void PFNGLNORMAL3IVPROC( int[] v );
        public delegate void PFNGLNORMAL3SPROC( short nx, short ny, short nz );
        public delegate void PFNGLNORMAL3SVPROC( short[] v );
        public delegate void PFNGLNORMALPOINTERPROC( uint type, int stride, IntPtr pointer );
        public delegate void PFNGLORTHOPROC( double left, double right, double bottom, double top, double zNear, double zFar );
        public delegate void PFNGLPASSTHROUGHPROC( float token );
        public delegate void PFNGLPIXELMAPFVPROC( uint map, int mapsize, float[] values );
        public delegate void PFNGLPIXELMAPUIVPROC( uint map, int mapsize, uint[] values );
        public delegate void PFNGLPIXELMAPUSVPROC( uint map, int mapsize, ushort[] values );
        public delegate void PFNGLPIXELSTOREFPROC( uint pname, float param );
        public delegate void PFNGLPIXELSTOREIPROC( uint pname, int param );
        public delegate void PFNGLPIXELTRANSFERFPROC( uint pname, float param );
        public delegate void PFNGLPIXELTRANSFERIPROC( uint pname, int param );
        public delegate void PFNGLPIXELZOOMPROC( float xfactor, float yfactor );
        public delegate void PFNGLPOINTSIZEPROC( float size );
        public delegate void PFNGLPOLYGONMODEPROC( uint face, uint mode );
        public delegate void PFNGLPOLYGONOFFSETPROC( float factor, float units );
        public delegate void PFNGLPOLYGONSTIPPLEPROC( [In] [MarshalAs( UnmanagedType.LPStr )] string mask );
        public delegate void PFNGLPOPATTRIBPROC();
        public delegate void PFNGLPOPCLIENTATTRIBPROC();
        public delegate void PFNGLPOPMATRIXPROC();
        public delegate void PFNGLPOPNAMEPROC();
        public delegate void PFNGLPRIORITIZETEXTURESPROC( int n, uint[] textures, float[] priorities );
        public delegate void PFNGLPUSHATTRIBPROC( uint mask );
        public delegate void PFNGLPUSHCLIENTATTRIBPROC( uint mask );
        public delegate void PFNGLPUSHMATRIXPROC();
        public delegate void PFNGLPUSHNAMEPROC( uint name );
        public delegate void PFNGLRASTERPOS2DPROC( double x, double y );
        public delegate void PFNGLRASTERPOS2DVPROC( double[] v );
        public delegate void PFNGLRASTERPOS2FPROC( float x, float y );
        public delegate void PFNGLRASTERPOS2FVPROC( float[] v );
        public delegate void PFNGLRASTERPOS2IPROC( int x, int y );
        public delegate void PFNGLRASTERPOS2IVPROC( int[] v );
        public delegate void PFNGLRASTERPOS2SPROC( short x, short y );
        public delegate void PFNGLRASTERPOS2SVPROC( short[] v );
        public delegate void PFNGLRASTERPOS3DPROC( double x, double y, double z );
        public delegate void PFNGLRASTERPOS3DVPROC( double[] v );
        public delegate void PFNGLRASTERPOS3FPROC( float x, float y, float z );
        public delegate void PFNGLRASTERPOS3FVPROC( float[] v );
        public delegate void PFNGLRASTERPOS3IPROC( int x, int y, int z );
        public delegate void PFNGLRASTERPOS3IVPROC( int[] v );
        public delegate void PFNGLRASTERPOS3SPROC( short x, short y, short z );
        public delegate void PFNGLRASTERPOS3SVPROC( short[] v );
        public delegate void PFNGLRASTERPOS4DPROC( double x, double y, double z, double w );
        public delegate void PFNGLRASTERPOS4DVPROC( double[] v );
        public delegate void PFNGLRASTERPOS4FPROC( float x, float y, float z, float w );
        public delegate void PFNGLRASTERPOS4FVPROC( float[] v );
        public delegate void PFNGLRASTERPOS4IPROC( int x, int y, int z, int w );
        public delegate void PFNGLRASTERPOS4IVPROC( int[] v );
        public delegate void PFNGLRASTERPOS4SPROC( short x, short y, short z, short w );
        public delegate void PFNGLRASTERPOS4SVPROC( short[] v );
        public delegate void PFNGLREADBUFFERPROC( uint mode );
        public delegate void PFNGLREADPIXELSPROC( int x, int y, int width, int height, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLRECTDPROC( double x1, double y1, double x2, double y2 );
        public delegate void PFNGLRECTDVPROC( double[] v1, double[] v2 );
        public delegate void PFNGLRECTFPROC( float x1, float y1, float x2, float y2 );
        public delegate void PFNGLRECTFVPROC( float[] v1, float[] v2 );
        public delegate void PFNGLRECTIPROC( int x1, int y1, int x2, int y2 );
        public delegate void PFNGLRECTIVPROC( int[] v1, int[] v2 );
        public delegate void PFNGLRECTSPROC( short x1, short y1, short x2, short y2 );
        public delegate void PFNGLRECTSVPROC( short[] v1, short[] v2 );
        public delegate int PFNGLRENDERMODEPROC( uint mode );
        public delegate void PFNGLROTATEDPROC( double angle, double x, double y, double z );
        public delegate void PFNGLROTATEFPROC( float angle, float x, float y, float z );
        public delegate void PFNGLSCALEDPROC( double x, double y, double z );
        public delegate void PFNGLSCALEFPROC( float x, float y, float z );
        public delegate void PFNGLSCISSORPROC( int x, int y, int width, int height );
        public delegate void PFNGLSELECTBUFFERPROC( int size, uint[] buffer );
        public delegate void PFNGLSHADEMODELPROC( uint mode );
        public delegate void PFNGLSTENCILFUNCPROC( uint func, int refer, uint mask );
        public delegate void PFNGLSTENCILMASKPROC( uint mask );
        public delegate void PFNGLSTENCILOPPROC( uint fail, uint zfail, uint zpass );
        public delegate void PFNGLTEXCOORD1DPROC( double s );
        public delegate void PFNGLTEXCOORD1DVPROC( double[] v );
        public delegate void PFNGLTEXCOORD1FPROC( float s );
        public delegate void PFNGLTEXCOORD1FVPROC( float[] v );
        public delegate void PFNGLTEXCOORD1IPROC( int s );
        public delegate void PFNGLTEXCOORD1IVPROC( int[] v );
        public delegate void PFNGLTEXCOORD1SPROC( short s );
        public delegate void PFNGLTEXCOORD1SVPROC( short[] v );
        public delegate void PFNGLTEXCOORD2DPROC( double s, double t );
        public delegate void PFNGLTEXCOORD2DVPROC( double[] v );
        public delegate void PFNGLTEXCOORD2FPROC( float s, float t );
        public delegate void PFNGLTEXCOORD2FVPROC( float[] v );
        public delegate void PFNGLTEXCOORD2IPROC( int s, int t );
        public delegate void PFNGLTEXCOORD2IVPROC( int[] v );
        public delegate void PFNGLTEXCOORD2SPROC( short s, short t );
        public delegate void PFNGLTEXCOORD2SVPROC( short[] v );
        public delegate void PFNGLTEXCOORD3DPROC( double s, double t, double r );
        public delegate void PFNGLTEXCOORD3DVPROC( double[] v );
        public delegate void PFNGLTEXCOORD3FPROC( float s, float t, float r );
        public delegate void PFNGLTEXCOORD3FVPROC( float[] v );
        public delegate void PFNGLTEXCOORD3IPROC( int s, int t, int r );
        public delegate void PFNGLTEXCOORD3IVPROC( int[] v );
        public delegate void PFNGLTEXCOORD3SPROC( short s, short t, short r );
        public delegate void PFNGLTEXCOORD3SVPROC( short[] v );
        public delegate void PFNGLTEXCOORD4DPROC( double s, double t, double r, double q );
        public delegate void PFNGLTEXCOORD4DVPROC( double[] v );
        public delegate void PFNGLTEXCOORD4FPROC( float s, float t, float r, float q );
        public delegate void PFNGLTEXCOORD4FVPROC( float[] v );
        public delegate void PFNGLTEXCOORD4IPROC( int s, int t, int r, int q );
        public delegate void PFNGLTEXCOORD4IVPROC( int[] v );
        public delegate void PFNGLTEXCOORD4SPROC( short s, short t, short r, short q );
        public delegate void PFNGLTEXCOORD4SVPROC( short[] v );
        public delegate void PFNGLTEXCOORDPOINTERPROC( int size, uint type, int stride, IntPtr pointer );
        public delegate void PFNGLTEXENVFPROC( uint target, uint pname, float param );
        public delegate void PFNGLTEXENVFVPROC( uint target, uint pname, float[] parameters );
        public delegate void PFNGLTEXENVIPROC( uint target, uint pname, int param );
        public delegate void PFNGLTEXENVIVPROC( uint target, uint pname, int[] parameters );
        public delegate void PFNGLTEXGENDPROC( uint coord, uint pname, double param );
        public delegate void PFNGLTEXGENDVPROC( uint coord, uint pname, double[] parameters );
        public delegate void PFNGLTEXGENFPROC( uint coord, uint pname, float param );
        public delegate void PFNGLTEXGENFVPROC( uint coord, uint pname, float[] parameters );
        public delegate void PFNGLTEXGENIPROC( uint coord, uint pname, int param );
        public delegate void PFNGLTEXGENIVPROC( uint coord, uint pname, int[] parameters );
        public delegate void PFNGLTEXIMAGE1DPROC( uint target, int level, int internalformat, int width, int border, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLTEXIMAGE2DPROC( uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLTEXPARAMETERFPROC( uint target, uint pname, float param );
        public delegate void PFNGLTEXPARAMETERFVPROC( uint target, uint pname, float[] parameters );
        public delegate void PFNGLTEXPARAMETERIPROC( uint target, uint pname, int param );
        public delegate void PFNGLTEXPARAMETERIVPROC( uint target, uint pname, int[] parameters );
        public delegate void PFNGLTEXSUBIMAGE1DPROC( uint target, int level, int xoffset, int width, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLTEXSUBIMAGE2DPROC( uint target, int level, int xoffset, int yoffset, int width, int height, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLTRANSLATEDPROC( double x, double y, double z );
        public delegate void PFNGLTRANSLATEFPROC( float x, float y, float z );
        public delegate void PFNGLVERTEX2DPROC( double x, double y );
        public delegate void PFNGLVERTEX2DVPROC( double[] v );
        public delegate void PFNGLVERTEX2FPROC( float x, float y );
        public delegate void PFNGLVERTEX2FVPROC( float[] v );
        public delegate void PFNGLVERTEX2IPROC( int x, int y );
        public delegate void PFNGLVERTEX2IVPROC( int[] v );
        public delegate void PFNGLVERTEX2SPROC( short x, short y );
        public delegate void PFNGLVERTEX2SVPROC( short[] v );
        public delegate void PFNGLVERTEX3DPROC( double x, double y, double z );
        public delegate void PFNGLVERTEX3DVPROC( double[] v );
        public delegate void PFNGLVERTEX3FPROC( float x, float y, float z );
        public delegate void PFNGLVERTEX3FVPROC( float[] v );
        public delegate void PFNGLVERTEX3IPROC( int x, int y, int z );
        public delegate void PFNGLVERTEX3IVPROC( int[] v );
        public delegate void PFNGLVERTEX3SPROC( short x, short y, short z );
        public delegate void PFNGLVERTEX3SVPROC( short[] v );
        public delegate void PFNGLVERTEX4DPROC( double x, double y, double z, double w );
        public delegate void PFNGLVERTEX4DVPROC( double[] v );
        public delegate void PFNGLVERTEX4FPROC( float x, float y, float z, float w );
        public delegate void PFNGLVERTEX4FVPROC( float[] v );
        public delegate void PFNGLVERTEX4IPROC( int x, int y, int z, int w );
        public delegate void PFNGLVERTEX4IVPROC( int[] v );
        public delegate void PFNGLVERTEX4SPROC( short x, short y, short z, short w );
        public delegate void PFNGLVERTEX4SVPROC( short[] v );
        public delegate void PFNGLVERTEXPOINTERPROC( int size, uint type, int stride, IntPtr pointer );
        public delegate void PFNGLVIEWPORTPROC( int x, int y, int width, int height );
        public delegate IntPtr PFNGLGETSTRINGPROC( uint name );
        #endregion

        #region Methods
        public static PFNGLACCUMPROC glAccum;
        public static PFNGLALPHAFUNCPROC glAlphaFunc;
        public static PFNGLARETEXTURESRESIDENTPROC glAreTexturesResident;
        public static PFNGLARRAYELEMENTPROC glArrayElement;
        public static PFNGLBEGINPROC glBegin;
        public static PFNGLBINDTEXTUREPROC glBindTexture;
        public static PFNGLBITMAPPROC glBitmap;
        public static PFNGLBLENDFUNCPROC glBlendFunc;
        public static PFNGLCALLLISTPROC glCallList;
        public static PFNGLCALLLISTSPROC glCallLists;
        public static PFNGLCLEARPROC glClear;
        public static PFNGLCLEARACCUMPROC glClearAccum;
        public static PFNGLCLEARCOLORPROC glClearColor;
        public static PFNGLCLEARDEPTHPROC glClearDepth;
        public static PFNGLCLEARINDEXPROC glClearIndex;
        public static PFNGLCLEARSTENCILPROC glClearStencil;
        public static PFNGLCLIPPLANEPROC glClipPlane;
        public static PFNGLCOLOR3BPROC glColor3b;
        public static PFNGLCOLOR3BVPROC glColor3bv;
        public static PFNGLCOLOR3DPROC glColor3d;
        public static PFNGLCOLOR3DVPROC glColor3dv;
        public static PFNGLCOLOR3FPROC glColor3f;
        public static PFNGLCOLOR3FVPROC glColor3fv;
        public static PFNGLCOLOR3IPROC glColor3i;
        public static PFNGLCOLOR3IVPROC glColor3iv;
        public static PFNGLCOLOR3SPROC glColor3s;
        public static PFNGLCOLOR3SVPROC glColor3sv;
        public static PFNGLCOLOR3UBPROC glColor3ub;
        public static PFNGLCOLOR3UBVPROC glColor3ubv;
        public static PFNGLCOLOR3UIPROC glColor3ui;
        public static PFNGLCOLOR3UIVPROC glColor3uiv;
        public static PFNGLCOLOR3USPROC glColor3us;
        public static PFNGLCOLOR3USVPROC glColor3usv;
        public static PFNGLCOLOR4BPROC glColor4b;
        public static PFNGLCOLOR4BVPROC glColor4bv;
        public static PFNGLCOLOR4DPROC glColor4d;
        public static PFNGLCOLOR4DVPROC glColor4dv;
        public static PFNGLCOLOR4FPROC glColor4f;
        public static PFNGLCOLOR4FVPROC glColor4fv;
        public static PFNGLCOLOR4IPROC glColor4i;
        public static PFNGLCOLOR4IVPROC glColor4iv;
        public static PFNGLCOLOR4SPROC glColor4s;
        public static PFNGLCOLOR4SVPROC glColor4sv;
        public static PFNGLCOLOR4UBPROC glColor4ub;
        public static PFNGLCOLOR4UBVPROC glColor4ubv;
        public static PFNGLCOLOR4UIPROC glColor4ui;
        public static PFNGLCOLOR4UIVPROC glColor4uiv;
        public static PFNGLCOLOR4USPROC glColor4us;
        public static PFNGLCOLOR4USVPROC glColor4usv;
        public static PFNGLCOLORMASKPROC glColorMask;
        public static PFNGLCOLORMATERIALPROC glColorMaterial;
        public static PFNGLCOLORPOINTERPROC glColorPointer;
        public static PFNGLCOPYPIXELSPROC glCopyPixels;
        public static PFNGLCOPYTEXIMAGE1DPROC glCopyTexImage1D;
        public static PFNGLCOPYTEXIMAGE2DPROC glCopyTexImage2D;
        public static PFNGLCOPYTEXSUBIMAGE1DPROC glCopyTexSubImage1D;
        public static PFNGLCOPYTEXSUBIMAGE2DPROC glCopyTexSubImage2D;
        public static PFNGLCULLFACEPROC glCullFace;
        public static PFNGLDELETELISTSPROC glDeleteLists;
        public static PFNGLDELETETEXTURESPROC glDeleteTextures;
        public static PFNGLDEPTHFUNCPROC glDepthFunc;
        public static PFNGLDEPTHMASKPROC glDepthMask;
        public static PFNGLDEPTHRANGEPROC glDepthRange;
        public static PFNGLDISABLEPROC glDisable;
        public static PFNGLDISABLECLIENTSTATEPROC glDisableClientState;
        public static PFNGLDRAWARRAYSPROC glDrawArrays;
        public static PFNGLDRAWBUFFERPROC glDrawBuffer;
        public static PFNGLDRAWELEMENTSPROC glDrawElements;
        public static PFNGLDRAWPIXELSPROC glDrawPixels;
        public static PFNGLEDGEFLAGPROC glEdgeFlag;
        public static PFNGLEDGEFLAGPOINTERPROC glEdgeFlagPointer;
        public static PFNGLEDGEFLAGVPROC glEdgeFlagv;
        public static PFNGLENABLEPROC glEnable;
        public static PFNGLENABLECLIENTSTATEPROC glEnableClientState;
        public static PFNGLENDPROC glEnd;
        public static PFNGLENDLISTPROC glEndList;
        public static PFNGLEVALCOORD1DPROC glEvalCoord1d;
        public static PFNGLEVALCOORD1DVPROC glEvalCoord1dv;
        public static PFNGLEVALCOORD1FPROC glEvalCoord1f;
        public static PFNGLEVALCOORD1FVPROC glEvalCoord1fv;
        public static PFNGLEVALCOORD2DPROC glEvalCoord2d;
        public static PFNGLEVALCOORD2DVPROC glEvalCoord2dv;
        public static PFNGLEVALCOORD2FPROC glEvalCoord2f;
        public static PFNGLEVALCOORD2FVPROC glEvalCoord2fv;
        public static PFNGLEVALMESH1PROC glEvalMesh1;
        public static PFNGLEVALMESH2PROC glEvalMesh2;
        public static PFNGLEVALPOINT1PROC glEvalPoint1;
        public static PFNGLEVALPOINT2PROC glEvalPoint2;
        public static PFNGLFEEDBACKBUFFERPROC glFeedbackBuffer;
        public static PFNGLFINISHPROC glFinish;
        public static PFNGLFLUSHPROC glFlush;
        public static PFNGLFOGFPROC glFogf;
        public static PFNGLFOGFVPROC glFogfv;
        public static PFNGLFOGIPROC glFogi;
        public static PFNGLFOGIVPROC glFogiv;
        public static PFNGLFRONTFACEPROC glFrontFace;
        public static PFNGLFRUSTUMPROC glFrustum;
        public static PFNGLGENLISTSPROC glGenLists;
        public static PFNGLGENTEXTURESPROC glGenTextures;
        public static PFNGLGETBOOLEANVPROC glGetBooleanv;
        public static PFNGLGETCLIPPLANEPROC glGetClipPlane;
        public static PFNGLGETDOUBLEVPROC glGetDoublev;
        public static PFNGLGETERRORPROC glGetError;
        public static PFNGLGETFLOATVPROC glGetFloatv;
        public static PFNGLGETINTEGERVPROC glGetIntegerv;
        public static PFNGLGETLIGHTFVPROC glGetLightfv;
        public static PFNGLGETLIGHTIVPROC glGetLightiv;
        public static PFNGLGETMAPDVPROC glGetMapdv;
        public static PFNGLGETMAPFVPROC glGetMapfv;
        public static PFNGLGETMAPIVPROC glGetMapiv;
        public static PFNGLGETMATERIALFVPROC glGetMaterialfv;
        public static PFNGLGETMATERIALIVPROC glGetMaterialiv;
        public static PFNGLGETPIXELMAPFVPROC glGetPixelMapfv;
        public static PFNGLGETPIXELMAPUIVPROC glGetPixelMapuiv;
        public static PFNGLGETPIXELMAPUSVPROC glGetPixelMapusv;
        public static PFNGLGETPOINTERVPROC glGetPointerv;
        public static PFNGLGETPOLYGONSTIPPLEPROC glGetPolygonStipple;
        public static PFNGLGETSTRINGPROC glGetString;
        public static PFNGLGETTEXENVFVPROC glGetTexEnvfv;
        public static PFNGLGETTEXENVIVPROC glGetTexEnviv;
        public static PFNGLGETTEXGENDVPROC glGetTexGendv;
        public static PFNGLGETTEXGENFVPROC glGetTexGenfv;
        public static PFNGLGETTEXGENIVPROC glGetTexGeniv;
        public static PFNGLGETTEXIMAGEPROC glGetTexImage;
        public static PFNGLGETTEXLEVELPARAMETERFVPROC glGetTexLevelParameterfv;
        public static PFNGLGETTEXLEVELPARAMETERIVPROC glGetTexLevelParameteriv;
        public static PFNGLGETTEXPARAMETERFVPROC glGetTexParameterfv;
        public static PFNGLGETTEXPARAMETERIVPROC glGetTexParameteriv;
        public static PFNGLHINTPROC glHint;
        public static PFNGLINDEXMASKPROC glIndexMask;
        public static PFNGLINDEXPOINTERPROC glIndexPointer;
        public static PFNGLINDEXDPROC glIndexd;
        public static PFNGLINDEXDVPROC glIndexdv;
        public static PFNGLINDEXFPROC glIndexf;
        public static PFNGLINDEXFVPROC glIndexfv;
        public static PFNGLINDEXIPROC glIndexi;
        public static PFNGLINDEXIVPROC glIndexiv;
        public static PFNGLINDEXSPROC glIndexs;
        public static PFNGLINDEXSVPROC glIndexsv;
        public static PFNGLINDEXUBPROC glIndexub;
        public static PFNGLINDEXUBVPROC glIndexubv;
        public static PFNGLINITNAMESPROC glInitNames;
        public static PFNGLINTERLEAVEDARRAYSPROC glInterleavedArrays;
        public static PFNGLISENABLEDPROC glIsEnabled;
        public static PFNGLISLISTPROC glIsList;
        public static PFNGLISTEXTUREPROC glIsTexture;
        public static PFNGLLIGHTMODELFPROC glLightModelf;
        public static PFNGLLIGHTMODELFVPROC glLightModelfv;
        public static PFNGLLIGHTMODELIPROC glLightModeli;
        public static PFNGLLIGHTMODELIVPROC glLightModeliv;
        public static PFNGLLIGHTFPROC glLightf;
        public static PFNGLLIGHTFVPROC glLightfv;
        public static PFNGLLIGHTIPROC glLighti;
        public static PFNGLLIGHTIVPROC glLightiv;
        public static PFNGLLINESTIPPLEPROC glLineStipple;
        public static PFNGLLINEWIDTHPROC glLineWidth;
        public static PFNGLLISTBASEPROC glListBase;
        public static PFNGLLOADIDENTITYPROC glLoadIdentity;
        public static PFNGLLOADMATRIXDPROC glLoadMatrixd;
        public static PFNGLLOADMATRIXFPROC glLoadMatrixf;
        public static PFNGLLOADNAMEPROC glLoadName;
        public static PFNGLLOGICOPPROC glLogicOp;
        public static PFNGLMAP1DPROC glMap1d;
        public static PFNGLMAP1FPROC glMap1f;
        public static PFNGLMAP2DPROC glMap2d;
        public static PFNGLMAP2FPROC glMap2f;
        public static PFNGLMAPGRID1DPROC glMapGrid1d;
        public static PFNGLMAPGRID1FPROC glMapGrid1f;
        public static PFNGLMAPGRID2DPROC glMapGrid2d;
        public static PFNGLMAPGRID2FPROC glMapGrid2f;
        public static PFNGLMATERIALFPROC glMaterialf;
        public static PFNGLMATERIALFVPROC glMaterialfv;
        public static PFNGLMATERIALIPROC glMateriali;
        public static PFNGLMATERIALIVPROC glMaterialiv;
        public static PFNGLMATRIXMODEPROC glMatrixMode;
        public static PFNGLMULTMATRIXDPROC glMultMatrixd;
        public static PFNGLMULTMATRIXFPROC glMultMatrixf;
        public static PFNGLNEWLISTPROC glNewList;
        public static PFNGLNORMAL3BPROC glNormal3b;
        public static PFNGLNORMAL3BVPROC glNormal3bv;
        public static PFNGLNORMAL3DPROC glNormal3d;
        public static PFNGLNORMAL3DVPROC glNormal3dv;
        public static PFNGLNORMAL3FPROC glNormal3f;
        public static PFNGLNORMAL3FVPROC glNormal3fv;
        public static PFNGLNORMAL3IPROC glNormal3i;
        public static PFNGLNORMAL3IVPROC glNormal3iv;
        public static PFNGLNORMAL3SPROC glNormal3s;
        public static PFNGLNORMAL3SVPROC glNormal3sv;
        public static PFNGLNORMALPOINTERPROC glNormalPointer;
        public static PFNGLORTHOPROC glOrtho;
        public static PFNGLPASSTHROUGHPROC glPassThrough;
        public static PFNGLPIXELMAPFVPROC glPixelMapfv;
        public static PFNGLPIXELMAPUIVPROC glPixelMapuiv;
        public static PFNGLPIXELMAPUSVPROC glPixelMapusv;
        public static PFNGLPIXELSTOREFPROC glPixelStoref;
        public static PFNGLPIXELSTOREIPROC glPixelStorei;
        public static PFNGLPIXELTRANSFERFPROC glPixelTransferf;
        public static PFNGLPIXELTRANSFERIPROC glPixelTransferi;
        public static PFNGLPIXELZOOMPROC glPixelZoom;
        public static PFNGLPOINTSIZEPROC glPointSize;
        public static PFNGLPOLYGONMODEPROC glPolygonMode;
        public static PFNGLPOLYGONOFFSETPROC glPolygonOffset;
        public static PFNGLPOLYGONSTIPPLEPROC glPolygonStipple;
        public static PFNGLPOPATTRIBPROC glPopAttrib;
        public static PFNGLPOPCLIENTATTRIBPROC glPopClientAttrib;
        public static PFNGLPOPMATRIXPROC glPopMatrix;
        public static PFNGLPOPNAMEPROC glPopName;
        public static PFNGLPRIORITIZETEXTURESPROC glPrioritizeTextures;
        public static PFNGLPUSHATTRIBPROC glPushAttrib;
        public static PFNGLPUSHCLIENTATTRIBPROC glPushClientAttrib;
        public static PFNGLPUSHMATRIXPROC glPushMatrix;
        public static PFNGLPUSHNAMEPROC glPushName;
        public static PFNGLRASTERPOS2DPROC glRasterPos2d;
        public static PFNGLRASTERPOS2DVPROC glRasterPos2dv;
        public static PFNGLRASTERPOS2FPROC glRasterPos2f;
        public static PFNGLRASTERPOS2FVPROC glRasterPos2fv;
        public static PFNGLRASTERPOS2IPROC glRasterPos2i;
        public static PFNGLRASTERPOS2IVPROC glRasterPos2iv;
        public static PFNGLRASTERPOS2SPROC glRasterPos2s;
        public static PFNGLRASTERPOS2SVPROC glRasterPos2sv;
        public static PFNGLRASTERPOS3DPROC glRasterPos3d;
        public static PFNGLRASTERPOS3DVPROC glRasterPos3dv;
        public static PFNGLRASTERPOS3FPROC glRasterPos3f;
        public static PFNGLRASTERPOS3FVPROC glRasterPos3fv;
        public static PFNGLRASTERPOS3IPROC glRasterPos3i;
        public static PFNGLRASTERPOS3IVPROC glRasterPos3iv;
        public static PFNGLRASTERPOS3SPROC glRasterPos3s;
        public static PFNGLRASTERPOS3SVPROC glRasterPos3sv;
        public static PFNGLRASTERPOS4DPROC glRasterPos4d;
        public static PFNGLRASTERPOS4DVPROC glRasterPos4dv;
        public static PFNGLRASTERPOS4FPROC glRasterPos4f;
        public static PFNGLRASTERPOS4FVPROC glRasterPos4fv;
        public static PFNGLRASTERPOS4IPROC glRasterPos4i;
        public static PFNGLRASTERPOS4IVPROC glRasterPos4iv;
        public static PFNGLRASTERPOS4SPROC glRasterPos4s;
        public static PFNGLRASTERPOS4SVPROC glRasterPos4sv;
        public static PFNGLREADBUFFERPROC glReadBuffer;
        public static PFNGLREADPIXELSPROC glReadPixels;
        public static PFNGLRECTDPROC glRectd;
        public static PFNGLRECTDVPROC glRectdv;
        public static PFNGLRECTFPROC glRectf;
        public static PFNGLRECTFVPROC glRectfv;
        public static PFNGLRECTIPROC glRecti;
        public static PFNGLRECTIVPROC glRectiv;
        public static PFNGLRECTSPROC glRects;
        public static PFNGLRECTSVPROC glRectsv;
        public static PFNGLRENDERMODEPROC glRenderMode;
        public static PFNGLROTATEDPROC glRotated;
        public static PFNGLROTATEFPROC glRotatef;
        public static PFNGLSCALEDPROC glScaled;
        public static PFNGLSCALEFPROC glScalef;
        public static PFNGLSCISSORPROC glScissor;
        public static PFNGLSELECTBUFFERPROC glSelectBuffer;
        public static PFNGLSHADEMODELPROC glShadeModel;
        public static PFNGLSTENCILFUNCPROC glStencilFunc;
        public static PFNGLSTENCILMASKPROC glStencilMask;
        public static PFNGLSTENCILOPPROC glStencilOp;
        public static PFNGLTEXCOORD1DPROC glTexCoord1d;
        public static PFNGLTEXCOORD1DVPROC glTexCoord1dv;
        public static PFNGLTEXCOORD1FPROC glTexCoord1f;
        public static PFNGLTEXCOORD1FVPROC glTexCoord1fv;
        public static PFNGLTEXCOORD1IPROC glTexCoord1i;
        public static PFNGLTEXCOORD1IVPROC glTexCoord1iv;
        public static PFNGLTEXCOORD1SPROC glTexCoord1s;
        public static PFNGLTEXCOORD1SVPROC glTexCoord1sv;
        public static PFNGLTEXCOORD2DPROC glTexCoord2d;
        public static PFNGLTEXCOORD2DVPROC glTexCoord2dv;
        public static PFNGLTEXCOORD2FPROC glTexCoord2f;
        public static PFNGLTEXCOORD2FVPROC glTexCoord2fv;
        public static PFNGLTEXCOORD2IPROC glTexCoord2i;
        public static PFNGLTEXCOORD2IVPROC glTexCoord2iv;
        public static PFNGLTEXCOORD2SPROC glTexCoord2s;
        public static PFNGLTEXCOORD2SVPROC glTexCoord2sv;
        public static PFNGLTEXCOORD3DPROC glTexCoord3d;
        public static PFNGLTEXCOORD3DVPROC glTexCoord3dv;
        public static PFNGLTEXCOORD3FPROC glTexCoord3f;
        public static PFNGLTEXCOORD3FVPROC glTexCoord3fv;
        public static PFNGLTEXCOORD3IPROC glTexCoord3i;
        public static PFNGLTEXCOORD3IVPROC glTexCoord3iv;
        public static PFNGLTEXCOORD3SPROC glTexCoord3s;
        public static PFNGLTEXCOORD3SVPROC glTexCoord3sv;
        public static PFNGLTEXCOORD4DPROC glTexCoord4d;
        public static PFNGLTEXCOORD4DVPROC glTexCoord4dv;
        public static PFNGLTEXCOORD4FPROC glTexCoord4f;
        public static PFNGLTEXCOORD4FVPROC glTexCoord4fv;
        public static PFNGLTEXCOORD4IPROC glTexCoord4i;
        public static PFNGLTEXCOORD4IVPROC glTexCoord4iv;
        public static PFNGLTEXCOORD4SPROC glTexCoord4s;
        public static PFNGLTEXCOORD4SVPROC glTexCoord4sv;
        public static PFNGLTEXCOORDPOINTERPROC glTexCoordPointer;
        public static PFNGLTEXENVFPROC glTexEnvf;
        public static PFNGLTEXENVFVPROC glTexEnvfv;
        public static PFNGLTEXENVIPROC glTexEnvi;
        public static PFNGLTEXENVIVPROC glTexEnviv;
        public static PFNGLTEXGENDPROC glTexGend;
        public static PFNGLTEXGENDVPROC glTexGendv;
        public static PFNGLTEXGENFPROC glTexGenf;
        public static PFNGLTEXGENFVPROC glTexGenfv;
        public static PFNGLTEXGENIPROC glTexGeni;
        public static PFNGLTEXGENIVPROC glTexGeniv;
        public static PFNGLTEXIMAGE1DPROC glTexImage1D;
        public static PFNGLTEXIMAGE2DPROC glTexImage2D;
        public static PFNGLTEXPARAMETERFPROC glTexParameterf;
        public static PFNGLTEXPARAMETERFVPROC glTexParameterfv;
        public static PFNGLTEXPARAMETERIPROC glTexParameteri;
        public static PFNGLTEXPARAMETERIVPROC glTexParameteriv;
        public static PFNGLTEXSUBIMAGE1DPROC glTexSubImage1D;
        public static PFNGLTEXSUBIMAGE2DPROC glTexSubImage2D;
        public static PFNGLTRANSLATEDPROC glTranslated;
        public static PFNGLTRANSLATEFPROC glTranslatef;
        public static PFNGLVERTEX2DPROC glVertex2d;
        public static PFNGLVERTEX2DVPROC glVertex2dv;
        public static PFNGLVERTEX2FPROC glVertex2f;
        public static PFNGLVERTEX2FVPROC glVertex2fv;
        public static PFNGLVERTEX2IPROC glVertex2i;
        public static PFNGLVERTEX2IVPROC glVertex2iv;
        public static PFNGLVERTEX2SPROC glVertex2s;
        public static PFNGLVERTEX2SVPROC glVertex2sv;
        public static PFNGLVERTEX3DPROC glVertex3d;
        public static PFNGLVERTEX3DVPROC glVertex3dv;
        public static PFNGLVERTEX3FPROC glVertex3f;
        public static PFNGLVERTEX3FVPROC glVertex3fv;
        public static PFNGLVERTEX3IPROC glVertex3i;
        public static PFNGLVERTEX3IVPROC glVertex3iv;
        public static PFNGLVERTEX3SPROC glVertex3s;
        public static PFNGLVERTEX3SVPROC glVertex3sv;
        public static PFNGLVERTEX4DPROC glVertex4d;
        public static PFNGLVERTEX4DVPROC glVertex4dv;
        public static PFNGLVERTEX4FPROC glVertex4f;
        public static PFNGLVERTEX4FVPROC glVertex4fv;
        public static PFNGLVERTEX4IPROC glVertex4i;
        public static PFNGLVERTEX4IVPROC glVertex4iv;
        public static PFNGLVERTEX4SPROC glVertex4s;
        public static PFNGLVERTEX4SVPROC glVertex4sv;
        public static PFNGLVERTEXPOINTERPROC glVertexPointer;
        public static PFNGLVIEWPORTPROC glViewport;
        #endregion
        #endregion

        #region OpenGL 1.2
        #region Constants
        public const uint GL_UNSIGNED_BYTE_3_3_2 = 32818;
        public const uint GL_UNSIGNED_SHORT_4_4_4_4 = 32819;
        public const uint GL_UNSIGNED_SHORT_5_5_5_1 = 32820;
        public const uint GL_UNSIGNED_INT_8_8_8_8 = 32821;
        public const uint GL_UNSIGNED_INT_10_10_10_2 = 32822;
        public const uint GL_TEXTURE_BINDING_3D = 32874;
        public const uint GL_PACK_SKIP_IMAGES = 32875;
        public const uint GL_PACK_IMAGE_HEIGHT = 32876;
        public const uint GL_UNPACK_SKIP_IMAGES = 32877;
        public const uint GL_UNPACK_IMAGE_HEIGHT = 32878;
        public const uint GL_TEXTURE_3D = 32879;
        public const uint GL_PROXY_TEXTURE_3D = 32880;
        public const uint GL_TEXTURE_DEPTH = 32881;
        public const uint GL_TEXTURE_WRAP_R = 32882;
        public const uint GL_MAX_3D_TEXTURE_SIZE = 32883;
        public const uint GL_UNSIGNED_BYTE_2_3_3_REV = 33634;
        public const uint GL_UNSIGNED_SHORT_5_6_5 = 33635;
        public const uint GL_UNSIGNED_SHORT_5_6_5_REV = 33636;
        public const uint GL_UNSIGNED_SHORT_4_4_4_4_REV = 33637;
        public const uint GL_UNSIGNED_SHORT_1_5_5_5_REV = 33638;
        public const uint GL_UNSIGNED_INT_8_8_8_8_REV = 33639;
        public const uint GL_UNSIGNED_INT_2_10_10_10_REV = 33640;
        public const uint GL_BGR = 32992;
        public const uint GL_BGRA = 32993;
        public const uint GL_MAX_ELEMENTS_VERTICES = 33000;
        public const uint GL_MAX_ELEMENTS_INDICES = 33001;
        public const uint GL_CLAMP_TO_EDGE = 33071;
        public const uint GL_TEXTURE_MIN_LOD = 33082;
        public const uint GL_TEXTURE_MAX_LOD = 33083;
        public const uint GL_TEXTURE_BASE_LEVEL = 33084;
        public const uint GL_TEXTURE_MAX_LEVEL = 33085;
        public const uint GL_SMOOTH_POINT_SIZE_RANGE = 2834;
        public const uint GL_SMOOTH_POINT_SIZE_GRANULARITY = 2835;
        public const uint GL_SMOOTH_LINE_WIDTH_RANGE = 2850;
        public const uint GL_SMOOTH_LINE_WIDTH_GRANULARITY = 2851;
        public const uint GL_ALIASED_LINE_WIDTH_RANGE = 33902;
        #endregion

        #region Delegates
        public delegate void PFNGLDRAWRANGEELEMENTSPROC( uint mode, uint start, uint end, int count, uint type, IntPtr indices );
        public delegate void PFNGLTEXIMAGE3DPROC( uint target, int level, int internalformat, int width, int height, int depth, int border, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLTEXSUBIMAGE3DPROC( uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLCOPYTEXSUBIMAGE3DPROC( uint target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height );
        #endregion

        #region Methods
        public static PFNGLDRAWRANGEELEMENTSPROC glDrawRangeElements;
        public static PFNGLTEXIMAGE3DPROC glTexImage3D;
        public static PFNGLTEXSUBIMAGE3DPROC glTexSubImage3D;
        public static PFNGLCOPYTEXSUBIMAGE3DPROC glCopyTexSubImage3D;
        #endregion
        #endregion

        #region OpenGL 1.3
        #region Constants
        public const uint GL_TEXTURE0 = 33984;
        public const uint GL_TEXTURE1 = 33985;
        public const uint GL_TEXTURE2 = 33986;
        public const uint GL_TEXTURE3 = 33987;
        public const uint GL_TEXTURE4 = 33988;
        public const uint GL_TEXTURE5 = 33989;
        public const uint GL_TEXTURE6 = 33990;
        public const uint GL_TEXTURE7 = 33991;
        public const uint GL_TEXTURE8 = 33992;
        public const uint GL_TEXTURE9 = 33993;
        public const uint GL_TEXTURE10 = 33994;
        public const uint GL_TEXTURE11 = 33995;
        public const uint GL_TEXTURE12 = 33996;
        public const uint GL_TEXTURE13 = 33997;
        public const uint GL_TEXTURE14 = 33998;
        public const uint GL_TEXTURE15 = 33999;
        public const uint GL_TEXTURE16 = 34000;
        public const uint GL_TEXTURE17 = 34001;
        public const uint GL_TEXTURE18 = 34002;
        public const uint GL_TEXTURE19 = 34003;
        public const uint GL_TEXTURE20 = 34004;
        public const uint GL_TEXTURE21 = 34005;
        public const uint GL_TEXTURE22 = 34006;
        public const uint GL_TEXTURE23 = 34007;
        public const uint GL_TEXTURE24 = 34008;
        public const uint GL_TEXTURE25 = 34009;
        public const uint GL_TEXTURE26 = 34010;
        public const uint GL_TEXTURE27 = 34011;
        public const uint GL_TEXTURE28 = 34012;
        public const uint GL_TEXTURE29 = 34013;
        public const uint GL_TEXTURE30 = 34014;
        public const uint GL_TEXTURE31 = 34015;
        public const uint GL_ACTIVE_TEXTURE = 34016;
        public const uint GL_MULTISAMPLE = 32925;
        public const uint GL_SAMPLE_ALPHA_TO_COVERAGE = 32926;
        public const uint GL_SAMPLE_ALPHA_TO_ONE = 32927;
        public const uint GL_SAMPLE_COVERAGE = 32928;
        public const uint GL_SAMPLE_BUFFERS = 32936;
        public const uint GL_SAMPLES = 32937;
        public const uint GL_SAMPLE_COVERAGE_VALUE = 32938;
        public const uint GL_SAMPLE_COVERAGE_INVERT = 32939;
        public const uint GL_TEXTURE_CUBE_MAP = 34067;
        public const uint GL_TEXTURE_BINDING_CUBE_MAP = 34068;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_X = 34069;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_X = 34070;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_Y = 34071;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_Y = 34072;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_Z = 34073;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_Z = 34074;
        public const uint GL_PROXY_TEXTURE_CUBE_MAP = 34075;
        public const uint GL_MAX_CUBE_MAP_TEXTURE_SIZE = 34076;
        public const uint GL_COMPRESSED_RGB = 34029;
        public const uint GL_COMPRESSED_RGBA = 34030;
        public const uint GL_TEXTURE_COMPRESSION_HINT = 34031;
        public const uint GL_TEXTURE_COMPRESSED_IMAGE_SIZE = 34464;
        public const uint GL_TEXTURE_COMPRESSED = 34465;
        public const uint GL_NUM_COMPRESSED_TEXTURE_FORMATS = 34466;
        public const uint GL_COMPRESSED_TEXTURE_FORMATS = 34467;
        public const uint GL_CLAMP_TO_BORDER = 33069;
        #endregion

        #region Delegates
        public delegate void PFNGLACTIVETEXTUREPROC( uint texture );
        public delegate void PFNGLSAMPLECOVERAGEPROC( float value, byte invert );
        public delegate void PFNGLCOMPRESSEDTEXIMAGE3DPROC( uint target, int level, uint internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data );
        public delegate void PFNGLCOMPRESSEDTEXIMAGE2DPROC( uint target, int level, uint internalformat, int width, int height, int border, int imageSize, IntPtr data );
        public delegate void PFNGLCOMPRESSEDTEXIMAGE1DPROC( uint target, int level, uint internalformat, int width, int border, int imageSize, IntPtr data );
        public delegate void PFNGLCOMPRESSEDTEXSUBIMAGE3DPROC( uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, int imageSize, IntPtr data );
        public delegate void PFNGLCOMPRESSEDTEXSUBIMAGE2DPROC( uint target, int level, int xoffset, int yoffset, int width, int height, uint format, int imageSize, IntPtr data );
        public delegate void PFNGLCOMPRESSEDTEXSUBIMAGE1DPROC( uint target, int level, int xoffset, int width, uint format, int imageSize, IntPtr data );
        public delegate void PFNGLGETCOMPRESSEDTEXIMAGEPROC( uint target, int level, IntPtr img );
        #endregion

        #region Methods
        public static PFNGLACTIVETEXTUREPROC glActiveTexture;
        public static PFNGLSAMPLECOVERAGEPROC glSampleCoverage;
        public static PFNGLCOMPRESSEDTEXIMAGE3DPROC glCompressedTexImage3D;
        public static PFNGLCOMPRESSEDTEXIMAGE2DPROC glCompressedTexImage2D;
        public static PFNGLCOMPRESSEDTEXIMAGE1DPROC glCompressedTexImage1D;
        public static PFNGLCOMPRESSEDTEXSUBIMAGE3DPROC glCompressedTexSubImage3D;
        public static PFNGLCOMPRESSEDTEXSUBIMAGE2DPROC glCompressedTexSubImage2D;
        public static PFNGLCOMPRESSEDTEXSUBIMAGE1DPROC glCompressedTexSubImage1D;
        public static PFNGLGETCOMPRESSEDTEXIMAGEPROC glGetCompressedTexImage;
        #endregion
        #endregion

        #region OpenGL 1.4
        #region Constants
        public const uint GL_VERSION_1_4 = 1;
        public const uint GL_BLEND_DST_RGB = 32968;
        public const uint GL_BLEND_SRC_RGB = 32969;
        public const uint GL_BLEND_DST_ALPHA = 32970;
        public const uint GL_BLEND_SRC_ALPHA = 32971;
        public const uint GL_POINT_FADE_THRESHOLD_SIZE = 33064;
        public const uint GL_DEPTH_COMPONENT16 = 33189;
        public const uint GL_DEPTH_COMPONENT24 = 33190;
        public const uint GL_DEPTH_COMPONENT32 = 33191;
        public const uint GL_MIRRORED_REPEAT = 33648;
        public const uint GL_MAX_TEXTURE_LOD_BIAS = 34045;
        public const uint GL_TEXTURE_LOD_BIAS = 34049;
        public const uint GL_INCR_WRAP = 34055;
        public const uint GL_DECR_WRAP = 34056;
        public const uint GL_TEXTURE_DEPTH_SIZE = 34890;
        public const uint GL_TEXTURE_COMPARE_MODE = 34892;
        public const uint GL_TEXTURE_COMPARE_FUNC = 34893;
        public const uint GL_FUNC_ADD = 32774;
        public const uint GL_FUNC_SUBTRACT = 32778;
        public const uint GL_FUNC_REVERSE_SUBTRACT = 32779;
        public const uint GL_MIN = 32775;
        public const uint GL_MAX = 32776;
        public const uint GL_CONSTANT_COLOR = 32769;
        public const uint GL_ONE_MINUS_CONSTANT_COLOR = 32770;
        public const uint GL_CONSTANT_ALPHA = 32771;
        public const uint GL_ONE_MINUS_CONSTANT_ALPHA = 32772;
        #endregion

        #region 
        public delegate void PFNGLBLENDFUNCSEPARATEPROC( uint sfactorRGB, uint dfactorRGB, uint sfactorAlpha, uint dfactorAlpha );
        public delegate void PFNGLMULTIDRAWARRAYSPROC( uint mode, ref int first, ref int count, int drawcount );
        public delegate void PFNGLMULTIDRAWELEMENTSPROC( uint mode, ref int count, uint type, IntPtr indices, int drawcount );
        public delegate void PFNGLPOINTPARAMETERFPROC( uint pname, float param );
        public delegate void PFNGLPOINTPARAMETERFVPROC( uint pname, ref float parameters );
        public delegate void PFNGLPOINTPARAMETERIPROC( uint pname, int param );
        public delegate void PFNGLPOINTPARAMETERIVPROC( uint pname, ref int parameters );
        public delegate void PFNGLBLENDCOLORPROC( float red, float green, float blue, float alpha );
        public delegate void PFNGLBLENDEQUATIONPROC( uint mode );
        #endregion

        #region Methods
        public static PFNGLBLENDFUNCSEPARATEPROC glBlendFuncSeparate;
        public static PFNGLMULTIDRAWARRAYSPROC glMultiDrawArrays;
        public static PFNGLMULTIDRAWELEMENTSPROC glMultiDrawElements;
        public static PFNGLPOINTPARAMETERFPROC glPointParameterf;
        public static PFNGLPOINTPARAMETERFVPROC glPointParameterfv;
        public static PFNGLPOINTPARAMETERIPROC glPointParameteri;
        public static PFNGLPOINTPARAMETERIVPROC glPointParameteriv;
        public static PFNGLBLENDCOLORPROC glBlendColor;
        public static PFNGLBLENDEQUATIONPROC glBlendEquation;
        #endregion
        #endregion

        #region OpenGL 1.5
        #region Constants
        public const uint GL_BUFFER_SIZE = 34660;
        public const uint GL_BUFFER_USAGE = 34661;
        public const uint GL_QUERY_COUNTER_BITS = 34916;
        public const uint GL_CURRENT_QUERY = 34917;
        public const uint GL_QUERY_RESULT = 34918;
        public const uint GL_QUERY_RESULT_AVAILABLE = 34919;
        public const uint GL_ARRAY_BUFFER = 34962;
        public const uint GL_ELEMENT_ARRAY_BUFFER = 34963;
        public const uint GL_ARRAY_BUFFER_BINDING = 34964;
        public const uint GL_ELEMENT_ARRAY_BUFFER_BINDING = 34965;
        public const uint GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 34975;
        public const uint GL_READ_ONLY = 35000;
        public const uint GL_WRITE_ONLY = 35001;
        public const uint GL_READ_WRITE = 35002;
        public const uint GL_BUFFER_ACCESS = 35003;
        public const uint GL_BUFFER_MAPPED = 35004;
        public const uint GL_BUFFER_MAP_POINTER = 35005;
        public const uint GL_STREAM_DRAW = 35040;
        public const uint GL_STREAM_READ = 35041;
        public const uint GL_STREAM_COPY = 35042;
        public const uint GL_STATIC_DRAW = 35044;
        public const uint GL_STATIC_READ = 35045;
        public const uint GL_STATIC_COPY = 35046;
        public const uint GL_DYNAMIC_DRAW = 35048;
        public const uint GL_DYNAMIC_READ = 35049;
        public const uint GL_DYNAMIC_COPY = 35050;
        public const uint GL_SAMPLES_PASSED = 35092;
        public const uint GL_SRC1_ALPHA = 34185;
        #endregion

        #region Delegates
        public delegate void PFNGLGENQUERIESPROC( int n, ref uint ids );
        public delegate void PFNGLDELETEQUERIESPROC( int n, ref uint ids );
        public delegate byte PFNGLISQUERYPROC( uint id );
        public delegate void PFNGLBEGINQUERYPROC( uint target, uint id );
        public delegate void PFNGLENDQUERYPROC( uint target );
        public delegate void PFNGLGETQUERYIVPROC( uint target, uint pname, ref int parameters );
        public delegate void PFNGLGETQUERYOBJECTIVPROC( uint id, uint pname, ref int parameters );
        public delegate void PFNGLGETQUERYOBJECTUIVPROC( uint id, uint pname, ref uint parameters );
        public delegate void PFNGLBINDBUFFERPROC( uint target, uint buffer );
        public delegate void PFNGLDELETEBUFFERSPROC( int n, ref uint buffers );
        public delegate void PFNGLGENBUFFERSPROC( int n, ref uint buffers );
        public delegate byte PFNGLISBUFFERPROC( uint buffer );
        public delegate void PFNGLBUFFERDATAPROC( uint target, int size, IntPtr data, uint usage );
        public delegate void PFNGLBUFFERSUBDATAPROC( uint target, int offset, int size, IntPtr data );
        public delegate void PFNGLGETBUFFERSUBDATAPROC( uint target, int offset, int size, IntPtr data );
        public delegate IntPtr PFNGLMAPBUFFERPROC( uint target, uint access );
        public delegate byte PFNGLUNMAPBUFFERPROC( uint target );
        public delegate void PFNGLGETBUFFERPARAMETERIVPROC( uint target, uint pname, ref int parameters );
        public delegate void PFNGLGETBUFFERPOINTERVPROC( uint target, uint pname, ref IntPtr parameters );
        #endregion

        #region Methods
        public static PFNGLGENQUERIESPROC glGenQueries;
        public static PFNGLDELETEQUERIESPROC glDeleteQueries;
        public static PFNGLISQUERYPROC glIsQuery;
        public static PFNGLBEGINQUERYPROC glBeginQuery;
        public static PFNGLENDQUERYPROC glEndQuery;
        public static PFNGLGETQUERYIVPROC glGetQueryiv;
        public static PFNGLGETQUERYOBJECTIVPROC glGetQueryObjectiv;
        public static PFNGLGETQUERYOBJECTUIVPROC glGetQueryObjectuiv;
        public static PFNGLBINDBUFFERPROC glBindBuffer;
        public static PFNGLDELETEBUFFERSPROC glDeleteBuffers;
        public static PFNGLGENBUFFERSPROC glGenBuffers;
        public static PFNGLISBUFFERPROC glIsBuffer;
        public static PFNGLBUFFERDATAPROC glBufferData;
        public static PFNGLBUFFERSUBDATAPROC glBufferSubData;
        public static PFNGLGETBUFFERSUBDATAPROC glGetBufferSubData;
        public static PFNGLMAPBUFFERPROC glMapBuffer;
        public static PFNGLUNMAPBUFFERPROC glUnmapBuffer;
        public static PFNGLGETBUFFERPARAMETERIVPROC glGetBufferParameteriv;
        public static PFNGLGETBUFFERPOINTERVPROC glGetBufferPointerv;
        #endregion
        #endregion

        #region OpenGL 2.0
        #region Constants
        public const uint GL_BLEND_EQUATION_RGB = 32777;
        public const uint GL_VERTEX_ATTRIB_ARRAY_ENABLED = 34338;
        public const uint GL_VERTEX_ATTRIB_ARRAY_SIZE = 34339;
        public const uint GL_VERTEX_ATTRIB_ARRAY_STRIDE = 34340;
        public const uint GL_VERTEX_ATTRIB_ARRAY_TYPE = 34341;
        public const uint GL_CURRENT_VERTEX_ATTRIB = 34342;
        public const uint GL_VERTEX_PROGRAM_POINT_SIZE = 34370;
        public const uint GL_VERTEX_ATTRIB_ARRAY_POINTER = 34373;
        public const uint GL_STENCIL_BACK_FUNC = 34816;
        public const uint GL_STENCIL_BACK_FAIL = 34817;
        public const uint GL_STENCIL_BACK_PASS_DEPTH_FAIL = 34818;
        public const uint GL_STENCIL_BACK_PASS_DEPTH_PASS = 34819;
        public const uint GL_MAX_DRAW_BUFFERS = 34852;
        public const uint GL_DRAW_BUFFER0 = 34853;
        public const uint GL_DRAW_BUFFER1 = 34854;
        public const uint GL_DRAW_BUFFER2 = 34855;
        public const uint GL_DRAW_BUFFER3 = 34856;
        public const uint GL_DRAW_BUFFER4 = 34857;
        public const uint GL_DRAW_BUFFER5 = 34858;
        public const uint GL_DRAW_BUFFER6 = 34859;
        public const uint GL_DRAW_BUFFER7 = 34860;
        public const uint GL_DRAW_BUFFER8 = 34861;
        public const uint GL_DRAW_BUFFER9 = 34862;
        public const uint GL_DRAW_BUFFER10 = 34863;
        public const uint GL_DRAW_BUFFER11 = 34864;
        public const uint GL_DRAW_BUFFER12 = 34865;
        public const uint GL_DRAW_BUFFER13 = 34866;
        public const uint GL_DRAW_BUFFER14 = 34867;
        public const uint GL_DRAW_BUFFER15 = 34868;
        public const uint GL_BLEND_EQUATION_ALPHA = 34877;
        public const uint GL_MAX_VERTEX_ATTRIBS = 34921;
        public const uint GL_VERTEX_ATTRIB_ARRAY_NORMALIZED = 34922;
        public const uint GL_MAX_TEXTURE_IMAGE_UNITS = 34930;
        public const uint GL_FRAGMENT_SHADER = 35632;
        public const uint GL_VERTEX_SHADER = 35633;
        public const uint GL_MAX_FRAGMENT_UNIFORM_COMPONENTS = 35657;
        public const uint GL_MAX_VERTEX_UNIFORM_COMPONENTS = 35658;
        public const uint GL_MAX_VARYING_FLOATS = 35659;
        public const uint GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 35660;
        public const uint GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 35661;
        public const uint GL_SHADER_TYPE = 35663;
        public const uint GL_FLOAT_VEC2 = 35664;
        public const uint GL_FLOAT_VEC3 = 35665;
        public const uint GL_FLOAT_VEC4 = 35666;
        public const uint GL_INT_VEC2 = 35667;
        public const uint GL_INT_VEC3 = 35668;
        public const uint GL_INT_VEC4 = 35669;
        public const uint GL_BOOL = 35670;
        public const uint GL_BOOL_VEC2 = 35671;
        public const uint GL_BOOL_VEC3 = 35672;
        public const uint GL_BOOL_VEC4 = 35673;
        public const uint GL_FLOAT_MAT2 = 35674;
        public const uint GL_FLOAT_MAT3 = 35675;
        public const uint GL_FLOAT_MAT4 = 35676;
        public const uint GL_SAMPLER_1D = 35677;
        public const uint GL_SAMPLER_2D = 35678;
        public const uint GL_SAMPLER_3D = 35679;
        public const uint GL_SAMPLER_CUBE = 35680;
        public const uint GL_SAMPLER_1D_SHADOW = 35681;
        public const uint GL_SAMPLER_2D_SHADOW = 35682;
        public const uint GL_DELETE_STATUS = 35712;
        public const uint GL_COMPILE_STATUS = 35713;
        public const uint GL_LINK_STATUS = 35714;
        public const uint GL_VALIDATE_STATUS = 35715;
        public const uint GL_INFO_LOG_LENGTH = 35716;
        public const uint GL_ATTACHED_SHADERS = 35717;
        public const uint GL_ACTIVE_UNIFORMS = 35718;
        public const uint GL_ACTIVE_UNIFORM_MAX_LENGTH = 35719;
        public const uint GL_SHADER_SOURCE_LENGTH = 35720;
        public const uint GL_ACTIVE_ATTRIBUTES = 35721;
        public const uint GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 35722;
        public const uint GL_FRAGMENT_SHADER_DERIVATIVE_HINT = 35723;
        public const uint GL_SHADING_LANGUAGE_VERSION = 35724;
        public const uint GL_CURRENT_PROGRAM = 35725;
        public const uint GL_POINT_SPRITE_COORD_ORIGIN = 36000;
        public const uint GL_LOWER_LEFT = 36001;
        public const uint GL_UPPER_LEFT = 36002;
        public const uint GL_STENCIL_BACK_REF = 36003;
        public const uint GL_STENCIL_BACK_VALUE_MASK = 36004;
        public const uint GL_STENCIL_BACK_WRITEMASK = 36005;
        #endregion

        #region Delegates
        public delegate void PFNGLBLENDEQUATIONSEPARATEPROC( uint modeRGB, uint modeAlpha );
        public delegate void PFNGLDRAWBUFFERSPROC( int n, ref uint bufs );
        public delegate void PFNGLSTENCILOPSEPARATEPROC( uint face, uint sfail, uint dpfail, uint dppass );
        public delegate void PFNGLSTENCILFUNCSEPARATEPROC( uint face, uint func, int refer, uint mask );
        public delegate void PFNGLSTENCILMASKSEPARATEPROC( uint face, uint mask );
        public delegate void PFNGLATTACHSHADERPROC( uint program, uint shader );
        public delegate void PFNGLBINDATTRIBLOCATIONPROC( uint program, uint index, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate void PFNGLCOMPILESHADERPROC( uint shader );
        public delegate uint PFNGLCREATEPROGRAMPROC();
        public delegate uint PFNGLCREATESHADERPROC( uint type );
        public delegate void PFNGLDELETEPROGRAMPROC( uint program );
        public delegate void PFNGLDELETESHADERPROC( uint shader );
        public delegate void PFNGLDETACHSHADERPROC( uint program, uint shader );
        public delegate void PFNGLDISABLEVERTEXATTRIBARRAYPROC( uint index );
        public delegate void PFNGLENABLEVERTEXATTRIBARRAYPROC( uint index );
        public delegate void PFNGLGETACTIVEATTRIBPROC( uint program, uint index, int bufSize, ref int length, ref int size, ref uint type, IntPtr name );
        public delegate void PFNGLGETACTIVEUNIFORMPROC( uint program, uint index, int bufSize, ref int length, ref int size, ref uint type, IntPtr name );
        public delegate void PFNGLGETATTACHEDSHADERSPROC( uint program, int maxCount, ref int count, ref uint shaders );
        public delegate int PFNGLGETATTRIBLOCATIONPROC( uint program, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate void PFNGLGETPROGRAMIVPROC( uint program, uint pname, ref int parameters );
        public delegate void PFNGLGETPROGRAMINFOLOGPROC( uint program, int bufSize, ref int length, IntPtr infoLog );
        public delegate void PFNGLGETSHADERIVPROC( uint shader, uint pname, ref int parameters );
        public delegate void PFNGLGETSHADERINFOLOGPROC( uint shader, int bufSize, ref int length, IntPtr infoLog );
        public delegate void PFNGLGETSHADERSOURCEPROC( uint shader, int bufSize, ref int length, IntPtr source );
        public delegate int PFNGLGETUNIFORMLOCATIONPROC( uint program, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate void PFNGLGETUNIFORMFVPROC( uint program, int location, ref float parameters );
        public delegate void PFNGLGETUNIFORMIVPROC( uint program, int location, ref int parameters );
        public delegate void PFNGLGETVERTEXATTRIBDVPROC( uint index, uint pname, ref double parameters );
        public delegate void PFNGLGETVERTEXATTRIBFVPROC( uint index, uint pname, ref float parameters );
        public delegate void PFNGLGETVERTEXATTRIBIVPROC( uint index, uint pname, ref int parameters );
        public delegate void PFNGLGETVERTEXATTRIBPOINTERVPROC( uint index, uint pname, ref IntPtr pointer );
        public delegate byte PFNGLISPROGRAMPROC( uint program );
        public delegate byte PFNGLISSHADERPROC( uint shader );
        public delegate void PFNGLLINKPROGRAMPROC( uint program );
        public delegate void PFNGLSHADERSOURCEPROC( uint shader, int count, ref IntPtr str, ref int length );
        public delegate void PFNGLUSEPROGRAMPROC( uint program );
        public delegate void PFNGLUNIFORM1FPROC( int location, float v0 );
        public delegate void PFNGLUNIFORM2FPROC( int location, float v0, float v1 );
        public delegate void PFNGLUNIFORM3FPROC( int location, float v0, float v1, float v2 );
        public delegate void PFNGLUNIFORM4FPROC( int location, float v0, float v1, float v2, float v3 );
        public delegate void PFNGLUNIFORM1IPROC( int location, int v0 );
        public delegate void PFNGLUNIFORM2IPROC( int location, int v0, int v1 );
        public delegate void PFNGLUNIFORM3IPROC( int location, int v0, int v1, int v2 );
        public delegate void PFNGLUNIFORM4IPROC( int location, int v0, int v1, int v2, int v3 );
        public delegate void PFNGLUNIFORM1FVPROC( int location, int count, ref float value );
        public delegate void PFNGLUNIFORM2FVPROC( int location, int count, ref float value );
        public delegate void PFNGLUNIFORM3FVPROC( int location, int count, ref float value );
        public delegate void PFNGLUNIFORM4FVPROC( int location, int count, ref float value );
        public delegate void PFNGLUNIFORM1IVPROC( int location, int count, ref int value );
        public delegate void PFNGLUNIFORM2IVPROC( int location, int count, ref int value );
        public delegate void PFNGLUNIFORM3IVPROC( int location, int count, ref int value );
        public delegate void PFNGLUNIFORM4IVPROC( int location, int count, ref int value );
        public delegate void PFNGLUNIFORMMATRIX2FVPROC( int location, int count, byte transpose, ref float value );
        public delegate void PFNGLUNIFORMMATRIX3FVPROC( int location, int count, byte transpose, ref float value );
        public delegate void PFNGLUNIFORMMATRIX4FVPROC( int location, int count, byte transpose, ref float value );
        public delegate void PFNGLVALIDATEPROGRAMPROC( uint program );
        public delegate void PFNGLVERTEXATTRIB1DPROC( uint index, double x );
        public delegate void PFNGLVERTEXATTRIB1DVPROC( uint index, ref double v );
        public delegate void PFNGLVERTEXATTRIB1FPROC( uint index, float x );
        public delegate void PFNGLVERTEXATTRIB1FVPROC( uint index, ref float v );
        public delegate void PFNGLVERTEXATTRIB1SPROC( uint index, short x );
        public delegate void PFNGLVERTEXATTRIB1SVPROC( uint index, ref short v );
        public delegate void PFNGLVERTEXATTRIB2DPROC( uint index, double x, double y );
        public delegate void PFNGLVERTEXATTRIB2DVPROC( uint index, ref double v );
        public delegate void PFNGLVERTEXATTRIB2FPROC( uint index, float x, float y );
        public delegate void PFNGLVERTEXATTRIB2FVPROC( uint index, ref float v );
        public delegate void PFNGLVERTEXATTRIB2SPROC( uint index, short x, short y );
        public delegate void PFNGLVERTEXATTRIB2SVPROC( uint index, ref short v );
        public delegate void PFNGLVERTEXATTRIB3DPROC( uint index, double x, double y, double z );
        public delegate void PFNGLVERTEXATTRIB3DVPROC( uint index, ref double v );
        public delegate void PFNGLVERTEXATTRIB3FPROC( uint index, float x, float y, float z );
        public delegate void PFNGLVERTEXATTRIB3FVPROC( uint index, ref float v );
        public delegate void PFNGLVERTEXATTRIB3SPROC( uint index, short x, short y, short z );
        public delegate void PFNGLVERTEXATTRIB3SVPROC( uint index, ref short v );
        public delegate void PFNGLVERTEXATTRIB4NBVPROC( uint index, [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLVERTEXATTRIB4NIVPROC( uint index, ref int v );
        public delegate void PFNGLVERTEXATTRIB4NSVPROC( uint index, ref short v );
        public delegate void PFNGLVERTEXATTRIB4NUBPROC( uint index, byte x, byte y, byte z, byte w );
        public delegate void PFNGLVERTEXATTRIB4NUBVPROC( uint index, [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLVERTEXATTRIB4NUIVPROC( uint index, ref uint v );
        public delegate void PFNGLVERTEXATTRIB4NUSVPROC( uint index, ref ushort v );
        public delegate void PFNGLVERTEXATTRIB4BVPROC( uint index, [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLVERTEXATTRIB4DPROC( uint index, double x, double y, double z, double w );
        public delegate void PFNGLVERTEXATTRIB4DVPROC( uint index, ref double v );
        public delegate void PFNGLVERTEXATTRIB4FPROC( uint index, float x, float y, float z, float w );
        public delegate void PFNGLVERTEXATTRIB4FVPROC( uint index, ref float v );
        public delegate void PFNGLVERTEXATTRIB4IVPROC( uint index, ref int v );
        public delegate void PFNGLVERTEXATTRIB4SPROC( uint index, short x, short y, short z, short w );
        public delegate void PFNGLVERTEXATTRIB4SVPROC( uint index, ref short v );
        public delegate void PFNGLVERTEXATTRIB4UBVPROC( uint index, [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLVERTEXATTRIB4UIVPROC( uint index, ref uint v );
        public delegate void PFNGLVERTEXATTRIB4USVPROC( uint index, ref ushort v );
        public delegate void PFNGLVERTEXATTRIBPOINTERPROC( uint index, int size, uint type, byte normalized, int stride, IntPtr pointer );
        #endregion

        #region Methods
        public static PFNGLBLENDEQUATIONSEPARATEPROC glBlendEquationSeparate;
        public static PFNGLDRAWBUFFERSPROC glDrawBuffers;
        public static PFNGLSTENCILOPSEPARATEPROC glStencilOpSeparate;
        public static PFNGLSTENCILFUNCSEPARATEPROC glStencilFuncSeparate;
        public static PFNGLSTENCILMASKSEPARATEPROC glStencilMaskSeparate;
        public static PFNGLATTACHSHADERPROC glAttachShader;
        public static PFNGLBINDATTRIBLOCATIONPROC glBindAttribLocation;
        public static PFNGLCOMPILESHADERPROC glCompileShader;
        public static PFNGLCREATEPROGRAMPROC glCreateProgram;
        public static PFNGLCREATESHADERPROC glCreateShader;
        public static PFNGLDELETEPROGRAMPROC glDeleteProgram;
        public static PFNGLDELETESHADERPROC glDeleteShader;
        public static PFNGLDETACHSHADERPROC glDetachShader;
        public static PFNGLDISABLEVERTEXATTRIBARRAYPROC glDisableVertexAttribArray;
        public static PFNGLENABLEVERTEXATTRIBARRAYPROC glEnableVertexAttribArray;
        public static PFNGLGETACTIVEATTRIBPROC glGetActiveAttrib;
        public static PFNGLGETACTIVEUNIFORMPROC glGetActiveUniform;
        public static PFNGLGETATTACHEDSHADERSPROC glGetAttachedShaders;
        public static PFNGLGETATTRIBLOCATIONPROC glGetAttribLocation;
        public static PFNGLGETPROGRAMIVPROC glGetProgramiv;
        public static PFNGLGETPROGRAMINFOLOGPROC glGetProgramInfoLog;
        public static PFNGLGETSHADERIVPROC glGetShaderiv;
        public static PFNGLGETSHADERINFOLOGPROC glGetShaderInfoLog;
        public static PFNGLGETSHADERSOURCEPROC glGetShaderSource;
        public static PFNGLGETUNIFORMLOCATIONPROC glGetUniformLocation;
        public static PFNGLGETUNIFORMFVPROC glGetUniformfv;
        public static PFNGLGETUNIFORMIVPROC glGetUniformiv;
        public static PFNGLGETVERTEXATTRIBDVPROC glGetVertexAttribdv;
        public static PFNGLGETVERTEXATTRIBFVPROC glGetVertexAttribfv;
        public static PFNGLGETVERTEXATTRIBIVPROC glGetVertexAttribiv;
        public static PFNGLGETVERTEXATTRIBPOINTERVPROC glGetVertexAttribPointerv;
        public static PFNGLISPROGRAMPROC glIsProgram;
        public static PFNGLISSHADERPROC glIsShader;
        public static PFNGLLINKPROGRAMPROC glLinkProgram;
        public static PFNGLSHADERSOURCEPROC glShaderSource;
        public static PFNGLUSEPROGRAMPROC glUseProgram;
        public static PFNGLUNIFORM1FPROC glUniform1f;
        public static PFNGLUNIFORM2FPROC glUniform2f;
        public static PFNGLUNIFORM3FPROC glUniform3f;
        public static PFNGLUNIFORM4FPROC glUniform4f;
        public static PFNGLUNIFORM1IPROC glUniform1i;
        public static PFNGLUNIFORM2IPROC glUniform2i;
        public static PFNGLUNIFORM3IPROC glUniform3i;
        public static PFNGLUNIFORM4IPROC glUniform4i;
        public static PFNGLUNIFORM1FVPROC glUniform1fv;
        public static PFNGLUNIFORM2FVPROC glUniform2fv;
        public static PFNGLUNIFORM3FVPROC glUniform3fv;
        public static PFNGLUNIFORM4FVPROC glUniform4fv;
        public static PFNGLUNIFORM1IVPROC glUniform1iv;
        public static PFNGLUNIFORM2IVPROC glUniform2iv;
        public static PFNGLUNIFORM3IVPROC glUniform3iv;
        public static PFNGLUNIFORM4IVPROC glUniform4iv;
        public static PFNGLUNIFORMMATRIX2FVPROC glUniformMatrix2fv;
        public static PFNGLUNIFORMMATRIX3FVPROC glUniformMatrix3fv;
        public static PFNGLUNIFORMMATRIX4FVPROC glUniformMatrix4fv;
        public static PFNGLVALIDATEPROGRAMPROC glValidateProgram;
        public static PFNGLVERTEXATTRIB1DPROC glVertexAttrib1d;
        public static PFNGLVERTEXATTRIB1DVPROC glVertexAttrib1dv;
        public static PFNGLVERTEXATTRIB1FPROC glVertexAttrib1f;
        public static PFNGLVERTEXATTRIB1FVPROC glVertexAttrib1fv;
        public static PFNGLVERTEXATTRIB1SPROC glVertexAttrib1s;
        public static PFNGLVERTEXATTRIB1SVPROC glVertexAttrib1sv;
        public static PFNGLVERTEXATTRIB2DPROC glVertexAttrib2d;
        public static PFNGLVERTEXATTRIB2DVPROC glVertexAttrib2dv;
        public static PFNGLVERTEXATTRIB2FPROC glVertexAttrib2f;
        public static PFNGLVERTEXATTRIB2FVPROC glVertexAttrib2fv;
        public static PFNGLVERTEXATTRIB2SPROC glVertexAttrib2s;
        public static PFNGLVERTEXATTRIB2SVPROC glVertexAttrib2sv;
        public static PFNGLVERTEXATTRIB3DPROC glVertexAttrib3d;
        public static PFNGLVERTEXATTRIB3DVPROC glVertexAttrib3dv;
        public static PFNGLVERTEXATTRIB3FPROC glVertexAttrib3f;
        public static PFNGLVERTEXATTRIB3FVPROC glVertexAttrib3fv;
        public static PFNGLVERTEXATTRIB3SPROC glVertexAttrib3s;
        public static PFNGLVERTEXATTRIB3SVPROC glVertexAttrib3sv;
        public static PFNGLVERTEXATTRIB4NBVPROC glVertexAttrib4Nbv;
        public static PFNGLVERTEXATTRIB4NIVPROC glVertexAttrib4Niv;
        public static PFNGLVERTEXATTRIB4NSVPROC glVertexAttrib4Nsv;
        public static PFNGLVERTEXATTRIB4NUBPROC glVertexAttrib4Nub;
        public static PFNGLVERTEXATTRIB4NUBVPROC glVertexAttrib4Nubv;
        public static PFNGLVERTEXATTRIB4NUIVPROC glVertexAttrib4Nuiv;
        public static PFNGLVERTEXATTRIB4NUSVPROC glVertexAttrib4Nusv;
        public static PFNGLVERTEXATTRIB4BVPROC glVertexAttrib4bv;
        public static PFNGLVERTEXATTRIB4DPROC glVertexAttrib4d;
        public static PFNGLVERTEXATTRIB4DVPROC glVertexAttrib4dv;
        public static PFNGLVERTEXATTRIB4FPROC glVertexAttrib4f;
        public static PFNGLVERTEXATTRIB4FVPROC glVertexAttrib4fv;
        public static PFNGLVERTEXATTRIB4IVPROC glVertexAttrib4iv;
        public static PFNGLVERTEXATTRIB4SPROC glVertexAttrib4s;
        public static PFNGLVERTEXATTRIB4SVPROC glVertexAttrib4sv;
        public static PFNGLVERTEXATTRIB4UBVPROC glVertexAttrib4ubv;
        public static PFNGLVERTEXATTRIB4UIVPROC glVertexAttrib4uiv;
        public static PFNGLVERTEXATTRIB4USVPROC glVertexAttrib4usv;
        public static PFNGLVERTEXATTRIBPOINTERPROC glVertexAttribPointer;
        #endregion
        #endregion

        #region OpenGL 2.1
        #region Constants
        public const uint GL_PIXEL_PACK_BUFFER = 35051;
        public const uint GL_PIXEL_UNPACK_BUFFER = 35052;
        public const uint GL_PIXEL_PACK_BUFFER_BINDING = 35053;
        public const uint GL_PIXEL_UNPACK_BUFFER_BINDING = 35055;
        public const uint GL_FLOAT_MAT2x3 = 35685;
        public const uint GL_FLOAT_MAT2x4 = 35686;
        public const uint GL_FLOAT_MAT3x2 = 35687;
        public const uint GL_FLOAT_MAT3x4 = 35688;
        public const uint GL_FLOAT_MAT4x2 = 35689;
        public const uint GL_FLOAT_MAT4x3 = 35690;
        public const uint GL_SRGB = 35904;
        public const uint GL_SRGB8 = 35905;
        public const uint GL_SRGB_ALPHA = 35906;
        public const uint GL_SRGB8_ALPHA8 = 35907;
        public const uint GL_COMPRESSED_SRGB = 35912;
        public const uint GL_COMPRESSED_SRGB_ALPHA = 35913;
        #endregion

        #region Delegates
        public delegate void PFNGLUNIFORMMATRIX2X3FVPROC( int location, int count, byte transpose, ref float value );
        public delegate void PFNGLUNIFORMMATRIX3X2FVPROC( int location, int count, byte transpose, ref float value );
        public delegate void PFNGLUNIFORMMATRIX2X4FVPROC( int location, int count, byte transpose, ref float value );
        public delegate void PFNGLUNIFORMMATRIX4X2FVPROC( int location, int count, byte transpose, ref float value );
        public delegate void PFNGLUNIFORMMATRIX3X4FVPROC( int location, int count, byte transpose, ref float value );
        public delegate void PFNGLUNIFORMMATRIX4X3FVPROC( int location, int count, byte transpose, ref float value );
        #endregion

        #region Methods
        public static PFNGLUNIFORMMATRIX2X3FVPROC glUniformMatrix2x3fv;
        public static PFNGLUNIFORMMATRIX3X2FVPROC glUniformMatrix3x2fv;
        public static PFNGLUNIFORMMATRIX2X4FVPROC glUniformMatrix2x4fv;
        public static PFNGLUNIFORMMATRIX4X2FVPROC glUniformMatrix4x2fv;
        public static PFNGLUNIFORMMATRIX3X4FVPROC glUniformMatrix3x4fv;
        public static PFNGLUNIFORMMATRIX4X3FVPROC glUniformMatrix4x3fv;
        #endregion
        #endregion

        #region OpenGL 3.0
        #region Constants
        public const uint GL_COMPARE_REF_TO_TEXTURE = 34894;
        public const uint GL_CLIP_DISTANCE0 = 12288;
        public const uint GL_CLIP_DISTANCE1 = 12289;
        public const uint GL_CLIP_DISTANCE2 = 12290;
        public const uint GL_CLIP_DISTANCE3 = 12291;
        public const uint GL_CLIP_DISTANCE4 = 12292;
        public const uint GL_CLIP_DISTANCE5 = 12293;
        public const uint GL_CLIP_DISTANCE6 = 12294;
        public const uint GL_CLIP_DISTANCE7 = 12295;
        public const uint GL_MAX_CLIP_DISTANCES = 3378;
        public const uint GL_MAJOR_VERSION = 33307;
        public const uint GL_MINOR_VERSION = 33308;
        public const uint GL_NUM_EXTENSIONS = 33309;
        public const uint GL_CONTEXT_FLAGS = 33310;
        public const uint GL_COMPRESSED_RED = 33317;
        public const uint GL_COMPRESSED_RG = 33318;
        public const uint GL_CONTEXT_FLAG_FORWARD_COMPATIBLE_BIT = 1;
        public const uint GL_RGBA32F = 34836;
        public const uint GL_RGB32F = 34837;
        public const uint GL_RGBA16F = 34842;
        public const uint GL_RGB16F = 34843;
        public const uint GL_VERTEX_ATTRIB_ARRAY_INTEGER = 35069;
        public const uint GL_MAX_ARRAY_TEXTURE_LAYERS = 35071;
        public const uint GL_MIN_PROGRAM_TEXEL_OFFSET = 35076;
        public const uint GL_MAX_PROGRAM_TEXEL_OFFSET = 35077;
        public const uint GL_CLAMP_READ_COLOR = 35100;
        public const uint GL_FIXED_ONLY = 35101;
        public const uint GL_MAX_VARYING_COMPONENTS = 35659;
        public const uint GL_TEXTURE_1D_ARRAY = 35864;
        public const uint GL_PROXY_TEXTURE_1D_ARRAY = 35865;
        public const uint GL_TEXTURE_2D_ARRAY = 35866;
        public const uint GL_PROXY_TEXTURE_2D_ARRAY = 35867;
        public const uint GL_TEXTURE_BINDING_1D_ARRAY = 35868;
        public const uint GL_TEXTURE_BINDING_2D_ARRAY = 35869;
        public const uint GL_R11F_G11F_B10F = 35898;
        public const uint GL_UNSIGNED_INT_10F_11F_11F_REV = 35899;
        public const uint GL_RGB9_E5 = 35901;
        public const uint GL_UNSIGNED_INT_5_9_9_9_REV = 35902;
        public const uint GL_TEXTURE_SHARED_SIZE = 35903;
        public const uint GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 35958;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_MODE = 35967;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 35968;
        public const uint GL_TRANSFORM_FEEDBACK_VARYINGS = 35971;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_START = 35972;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_SIZE = 35973;
        public const uint GL_PRIMITIVES_GENERATED = 35975;
        public const uint GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 35976;
        public const uint GL_RASTERIZER_DISCARD = 35977;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 35978;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 35979;
        public const uint GL_INTERLEAVED_ATTRIBS = 35980;
        public const uint GL_SEPARATE_ATTRIBS = 35981;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER = 35982;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_BINDING = 35983;
        public const uint GL_RGBA32UI = 36208;
        public const uint GL_RGB32UI = 36209;
        public const uint GL_RGBA16UI = 36214;
        public const uint GL_RGB16UI = 36215;
        public const uint GL_RGBA8UI = 36220;
        public const uint GL_RGB8UI = 36221;
        public const uint GL_RGBA32I = 36226;
        public const uint GL_RGB32I = 36227;
        public const uint GL_RGBA16I = 36232;
        public const uint GL_RGB16I = 36233;
        public const uint GL_RGBA8I = 36238;
        public const uint GL_RGB8I = 36239;
        public const uint GL_RED_INTEGER = 36244;
        public const uint GL_GREEN_INTEGER = 36245;
        public const uint GL_BLUE_INTEGER = 36246;
        public const uint GL_RGB_INTEGER = 36248;
        public const uint GL_RGBA_INTEGER = 36249;
        public const uint GL_BGR_INTEGER = 36250;
        public const uint GL_BGRA_INTEGER = 36251;
        public const uint GL_SAMPLER_1D_ARRAY = 36288;
        public const uint GL_SAMPLER_2D_ARRAY = 36289;
        public const uint GL_SAMPLER_1D_ARRAY_SHADOW = 36291;
        public const uint GL_SAMPLER_2D_ARRAY_SHADOW = 36292;
        public const uint GL_SAMPLER_CUBE_SHADOW = 36293;
        public const uint GL_UNSIGNED_INT_VEC2 = 36294;
        public const uint GL_UNSIGNED_INT_VEC3 = 36295;
        public const uint GL_UNSIGNED_INT_VEC4 = 36296;
        public const uint GL_INT_SAMPLER_1D = 36297;
        public const uint GL_INT_SAMPLER_2D = 36298;
        public const uint GL_INT_SAMPLER_3D = 36299;
        public const uint GL_INT_SAMPLER_CUBE = 36300;
        public const uint GL_INT_SAMPLER_1D_ARRAY = 36302;
        public const uint GL_INT_SAMPLER_2D_ARRAY = 36303;
        public const uint GL_UNSIGNED_INT_SAMPLER_1D = 36305;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D = 36306;
        public const uint GL_UNSIGNED_INT_SAMPLER_3D = 36307;
        public const uint GL_UNSIGNED_INT_SAMPLER_CUBE = 36308;
        public const uint GL_UNSIGNED_INT_SAMPLER_1D_ARRAY = 36310;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D_ARRAY = 36311;
        public const uint GL_QUERY_WAIT = 36371;
        public const uint GL_QUERY_NO_WAIT = 36372;
        public const uint GL_QUERY_BY_REGION_WAIT = 36373;
        public const uint GL_QUERY_BY_REGION_NO_WAIT = 36374;
        public const uint GL_BUFFER_ACCESS_FLAGS = 37151;
        public const uint GL_BUFFER_MAP_LENGTH = 37152;
        public const uint GL_BUFFER_MAP_OFFSET = 37153;
        public const uint GL_DEPTH_COMPONENT32F = 36012;
        public const uint GL_DEPTH32F_STENCIL8 = 36013;
        public const uint GL_FLOAT_32_UNSIGNED_INT_24_8_REV = 36269;
        public const uint GL_INVALID_FRAMEBUFFER_OPERATION = 1286;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING = 33296;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE = 33297;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_RED_SIZE = 33298;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_GREEN_SIZE = 33299;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_BLUE_SIZE = 33300;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE = 33301;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE = 33302;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE = 33303;
        public const uint GL_FRAMEBUFFER_DEFAULT = 33304;
        public const uint GL_FRAMEBUFFER_UNDEFINED = 33305;
        public const uint GL_DEPTH_STENCIL_ATTACHMENT = 33306;
        public const uint GL_MAX_RENDERBUFFER_SIZE = 34024;
        public const uint GL_DEPTH_STENCIL = 34041;
        public const uint GL_UNSIGNED_INT_24_8 = 34042;
        public const uint GL_DEPTH24_STENCIL8 = 35056;
        public const uint GL_TEXTURE_STENCIL_SIZE = 35057;
        public const uint GL_TEXTURE_RED_TYPE = 35856;
        public const uint GL_TEXTURE_GREEN_TYPE = 35857;
        public const uint GL_TEXTURE_BLUE_TYPE = 35858;
        public const uint GL_TEXTURE_ALPHA_TYPE = 35859;
        public const uint GL_TEXTURE_DEPTH_TYPE = 35862;
        public const uint GL_UNSIGNED_NORMALIZED = 35863;
        public const uint GL_FRAMEBUFFER_BINDING = 36006;
        public const uint GL_DRAW_FRAMEBUFFER_BINDING = 36006;
        public const uint GL_RENDERBUFFER_BINDING = 36007;
        public const uint GL_READ_FRAMEBUFFER = 36008;
        public const uint GL_DRAW_FRAMEBUFFER = 36009;
        public const uint GL_READ_FRAMEBUFFER_BINDING = 36010;
        public const uint GL_RENDERBUFFER_SAMPLES = 36011;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE = 36048;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME = 36049;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL = 36050;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE = 36051;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER = 36052;
        public const uint GL_FRAMEBUFFER_COMPLETE = 36053;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 36054;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 36055;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER = 36059;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER = 36060;
        public const uint GL_FRAMEBUFFER_UNSUPPORTED = 36061;
        public const uint GL_MAX_COLOR_ATTACHMENTS = 36063;
        public const uint GL_COLOR_ATTACHMENT0 = 36064;
        public const uint GL_COLOR_ATTACHMENT1 = 36065;
        public const uint GL_COLOR_ATTACHMENT2 = 36066;
        public const uint GL_COLOR_ATTACHMENT3 = 36067;
        public const uint GL_COLOR_ATTACHMENT4 = 36068;
        public const uint GL_COLOR_ATTACHMENT5 = 36069;
        public const uint GL_COLOR_ATTACHMENT6 = 36070;
        public const uint GL_COLOR_ATTACHMENT7 = 36071;
        public const uint GL_COLOR_ATTACHMENT8 = 36072;
        public const uint GL_COLOR_ATTACHMENT9 = 36073;
        public const uint GL_COLOR_ATTACHMENT10 = 36074;
        public const uint GL_COLOR_ATTACHMENT11 = 36075;
        public const uint GL_COLOR_ATTACHMENT12 = 36076;
        public const uint GL_COLOR_ATTACHMENT13 = 36077;
        public const uint GL_COLOR_ATTACHMENT14 = 36078;
        public const uint GL_COLOR_ATTACHMENT15 = 36079;
        public const uint GL_COLOR_ATTACHMENT16 = 36080;
        public const uint GL_COLOR_ATTACHMENT17 = 36081;
        public const uint GL_COLOR_ATTACHMENT18 = 36082;
        public const uint GL_COLOR_ATTACHMENT19 = 36083;
        public const uint GL_COLOR_ATTACHMENT20 = 36084;
        public const uint GL_COLOR_ATTACHMENT21 = 36085;
        public const uint GL_COLOR_ATTACHMENT22 = 36086;
        public const uint GL_COLOR_ATTACHMENT23 = 36087;
        public const uint GL_COLOR_ATTACHMENT24 = 36088;
        public const uint GL_COLOR_ATTACHMENT25 = 36089;
        public const uint GL_COLOR_ATTACHMENT26 = 36090;
        public const uint GL_COLOR_ATTACHMENT27 = 36091;
        public const uint GL_COLOR_ATTACHMENT28 = 36092;
        public const uint GL_COLOR_ATTACHMENT29 = 36093;
        public const uint GL_COLOR_ATTACHMENT30 = 36094;
        public const uint GL_COLOR_ATTACHMENT31 = 36095;
        public const uint GL_DEPTH_ATTACHMENT = 36096;
        public const uint GL_STENCIL_ATTACHMENT = 36128;
        public const uint GL_FRAMEBUFFER = 36160;
        public const uint GL_RENDERBUFFER = 36161;
        public const uint GL_RENDERBUFFER_WIDTH = 36162;
        public const uint GL_RENDERBUFFER_HEIGHT = 36163;
        public const uint GL_RENDERBUFFER_INTERNAL_FORMAT = 36164;
        public const uint GL_STENCIL_INDEX1 = 36166;
        public const uint GL_STENCIL_INDEX4 = 36167;
        public const uint GL_STENCIL_INDEX8 = 36168;
        public const uint GL_STENCIL_INDEX16 = 36169;
        public const uint GL_RENDERBUFFER_RED_SIZE = 36176;
        public const uint GL_RENDERBUFFER_GREEN_SIZE = 36177;
        public const uint GL_RENDERBUFFER_BLUE_SIZE = 36178;
        public const uint GL_RENDERBUFFER_ALPHA_SIZE = 36179;
        public const uint GL_RENDERBUFFER_DEPTH_SIZE = 36180;
        public const uint GL_RENDERBUFFER_STENCIL_SIZE = 36181;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE = 36182;
        public const uint GL_MAX_SAMPLES = 36183;
        public const uint GL_FRAMEBUFFER_SRGB = 36281;
        public const uint GL_HALF_FLOAT = 5131;
        public const uint GL_MAP_READ_BIT = 1;
        public const uint GL_MAP_WRITE_BIT = 2;
        public const uint GL_MAP_INVALIDATE_RANGE_BIT = 4;
        public const uint GL_MAP_INVALIDATE_BUFFER_BIT = 8;
        public const uint GL_MAP_FLUSH_EXPLICIT_BIT = 16;
        public const uint GL_MAP_UNSYNCHRONIZED_BIT = 32;
        public const uint GL_COMPRESSED_RED_RGTC1 = 36283;
        public const uint GL_COMPRESSED_SIGNED_RED_RGTC1 = 36284;
        public const uint GL_COMPRESSED_RG_RGTC2 = 36285;
        public const uint GL_COMPRESSED_SIGNED_RG_RGTC2 = 36286;
        public const uint GL_RG = 33319;
        public const uint GL_RG_INTEGER = 33320;
        public const uint GL_R8 = 33321;
        public const uint GL_R16 = 33322;
        public const uint GL_RG8 = 33323;
        public const uint GL_RG16 = 33324;
        public const uint GL_R16F = 33325;
        public const uint GL_R32F = 33326;
        public const uint GL_RG16F = 33327;
        public const uint GL_RG32F = 33328;
        public const uint GL_R8I = 33329;
        public const uint GL_R8UI = 33330;
        public const uint GL_R16I = 33331;
        public const uint GL_R16UI = 33332;
        public const uint GL_R32I = 33333;
        public const uint GL_R32UI = 33334;
        public const uint GL_RG8I = 33335;
        public const uint GL_RG8UI = 33336;
        public const uint GL_RG16I = 33337;
        public const uint GL_RG16UI = 33338;
        public const uint GL_RG32I = 33339;
        public const uint GL_RG32UI = 33340;
        public const uint GL_VERTEX_ARRAY_BINDING = 34229;
        #endregion

        #region Delegates
        public delegate void PFNGLCOLORMASKIPROC( uint index, byte r, byte g, byte b, byte a );
        public delegate void PFNGLGETBOOLEANI_VPROC( uint target, uint index, IntPtr data );
        public delegate void PFNGLGETINTEGERI_VPROC( uint target, uint index, ref int data );
        public delegate void PFNGLENABLEIPROC( uint target, uint index );
        public delegate void PFNGLDISABLEIPROC( uint target, uint index );
        public delegate byte PFNGLISENABLEDIPROC( uint target, uint index );
        public delegate void PFNGLBEGINTRANSFORMFEEDBACKPROC( uint primitiveMode );
        public delegate void PFNGLENDTRANSFORMFEEDBACKPROC();
        public delegate void PFNGLBINDBUFFERRANGEPROC( uint target, uint index, uint buffer, int offset, int size );
        public delegate void PFNGLBINDBUFFERBASEPROC( uint target, uint index, uint buffer );
        public delegate void PFNGLTRANSFORMFEEDBACKVARYINGSPROC( uint program, int count, IntPtr varyings, uint bufferMode );
        public delegate void PFNGLGETTRANSFORMFEEDBACKVARYINGPROC( uint program, uint index, int bufSize, ref int length, ref int size, ref uint type, IntPtr name );
        public delegate void PFNGLCLAMPCOLORPROC( uint target, uint clamp );
        public delegate void PFNGLBEGINCONDITIONALRENDERPROC( uint id, uint mode );
        public delegate void PFNGLENDCONDITIONALRENDERPROC();
        public delegate void PFNGLVERTEXATTRIBIPOINTERPROC( uint index, int size, uint type, int stride, IntPtr pointer );
        public delegate void PFNGLGETVERTEXATTRIBIIVPROC( uint index, uint pname, ref int parameters );
        public delegate void PFNGLGETVERTEXATTRIBIUIVPROC( uint index, uint pname, ref uint parameters );
        public delegate void PFNGLVERTEXATTRIBI1IPROC( uint index, int x );
        public delegate void PFNGLVERTEXATTRIBI2IPROC( uint index, int x, int y );
        public delegate void PFNGLVERTEXATTRIBI3IPROC( uint index, int x, int y, int z );
        public delegate void PFNGLVERTEXATTRIBI4IPROC( uint index, int x, int y, int z, int w );
        public delegate void PFNGLVERTEXATTRIBI1UIPROC( uint index, uint x );
        public delegate void PFNGLVERTEXATTRIBI2UIPROC( uint index, uint x, uint y );
        public delegate void PFNGLVERTEXATTRIBI3UIPROC( uint index, uint x, uint y, uint z );
        public delegate void PFNGLVERTEXATTRIBI4UIPROC( uint index, uint x, uint y, uint z, uint w );
        public delegate void PFNGLVERTEXATTRIBI1IVPROC( uint index, ref int v );
        public delegate void PFNGLVERTEXATTRIBI2IVPROC( uint index, ref int v );
        public delegate void PFNGLVERTEXATTRIBI3IVPROC( uint index, ref int v );
        public delegate void PFNGLVERTEXATTRIBI4IVPROC( uint index, ref int v );
        public delegate void PFNGLVERTEXATTRIBI1UIVPROC( uint index, ref uint v );
        public delegate void PFNGLVERTEXATTRIBI2UIVPROC( uint index, ref uint v );
        public delegate void PFNGLVERTEXATTRIBI3UIVPROC( uint index, ref uint v );
        public delegate void PFNGLVERTEXATTRIBI4UIVPROC( uint index, ref uint v );
        public delegate void PFNGLVERTEXATTRIBI4BVPROC( uint index, [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLVERTEXATTRIBI4SVPROC( uint index, ref short v );
        public delegate void PFNGLVERTEXATTRIBI4UBVPROC( uint index, [In] [MarshalAs( UnmanagedType.LPStr )] string v );
        public delegate void PFNGLVERTEXATTRIBI4USVPROC( uint index, ref ushort v );
        public delegate void PFNGLGETUNIFORMUIVPROC( uint program, int location, ref uint parameters );
        public delegate void PFNGLBINDFRAGDATALOCATIONPROC( uint program, uint color, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate int PFNGLGETFRAGDATALOCATIONPROC( uint program, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate void PFNGLUNIFORM1UIPROC( int location, uint v0 );
        public delegate void PFNGLUNIFORM2UIPROC( int location, uint v0, uint v1 );
        public delegate void PFNGLUNIFORM3UIPROC( int location, uint v0, uint v1, uint v2 );
        public delegate void PFNGLUNIFORM4UIPROC( int location, uint v0, uint v1, uint v2, uint v3 );
        public delegate void PFNGLUNIFORM1UIVPROC( int location, int count, ref uint value );
        public delegate void PFNGLUNIFORM2UIVPROC( int location, int count, ref uint value );
        public delegate void PFNGLUNIFORM3UIVPROC( int location, int count, ref uint value );
        public delegate void PFNGLUNIFORM4UIVPROC( int location, int count, ref uint value );
        public delegate void PFNGLTEXPARAMETERIIVPROC( uint target, uint pname, ref int parameters );
        public delegate void PFNGLTEXPARAMETERIUIVPROC( uint target, uint pname, ref uint parameters );
        public delegate void PFNGLGETTEXPARAMETERIIVPROC( uint target, uint pname, ref int parameters );
        public delegate void PFNGLGETTEXPARAMETERIUIVPROC( uint target, uint pname, ref uint parameters );
        public delegate void PFNGLCLEARBUFFERIVPROC( uint buffer, int drawbuffer, ref int value );
        public delegate void PFNGLCLEARBUFFERUIVPROC( uint buffer, int drawbuffer, ref uint value );
        public delegate void PFNGLCLEARBUFFERFVPROC( uint buffer, int drawbuffer, ref float value );
        public delegate void PFNGLCLEARBUFFERFIPROC( uint buffer, int drawbuffer, float depth, int stencil );
        public delegate IntPtr PFNGLGETSTRINGIPROC( uint name, uint index );
        public delegate byte PFNGLISRENDERBUFFERPROC( uint renderbuffer );
        public delegate void PFNGLBINDRENDERBUFFERPROC( uint target, uint renderbuffer );
        public delegate void PFNGLDELETERENDERBUFFERSPROC( int n, ref uint renderbuffers );
        public delegate void PFNGLGENRENDERBUFFERSPROC( int n, ref uint renderbuffers );
        public delegate void PFNGLRENDERBUFFERSTORAGEPROC( uint target, uint internalformat, int width, int height );
        public delegate void PFNGLGETRENDERBUFFERPARAMETERIVPROC( uint target, uint pname, ref int parameters );
        public delegate byte PFNGLISFRAMEBUFFERPROC( uint framebuffer );
        public delegate void PFNGLBINDFRAMEBUFFERPROC( uint target, uint framebuffer );
        public delegate void PFNGLDELETEFRAMEBUFFERSPROC( int n, ref uint framebuffers );
        public delegate void PFNGLGENFRAMEBUFFERSPROC( int n, ref uint framebuffers );
        public delegate uint PFNGLCHECKFRAMEBUFFERSTATUSPROC( uint target );
        public delegate void PFNGLFRAMEBUFFERTEXTURE1DPROC( uint target, uint attachment, uint textarget, uint texture, int level );
        public delegate void PFNGLFRAMEBUFFERTEXTURE2DPROC( uint target, uint attachment, uint textarget, uint texture, int level );
        public delegate void PFNGLFRAMEBUFFERTEXTURE3DPROC( uint target, uint attachment, uint textarget, uint texture, int level, int zoffset );
        public delegate void PFNGLFRAMEBUFFERRENDERBUFFERPROC( uint target, uint attachment, uint renderbuffertarget, uint renderbuffer );
        public delegate void PFNGLGETFRAMEBUFFERATTACHMENTPARAMETERIVPROC( uint target, uint attachment, uint pname, ref int parameters );
        public delegate void PFNGLGENERATEMIPMAPPROC( uint target );
        public delegate void PFNGLBLITFRAMEBUFFERPROC( int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, uint mask, uint filter );
        public delegate void PFNGLRENDERBUFFERSTORAGEMULTISAMPLEPROC( uint target, int samples, uint internalformat, int width, int height );
        public delegate void PFNGLFRAMEBUFFERTEXTURELAYERPROC( uint target, uint attachment, uint texture, int level, int layer );
        public delegate IntPtr PFNGLMAPBUFFERRANGEPROC( uint target, int offset, int length, uint access );
        public delegate void PFNGLFLUSHMAPPEDBUFFERRANGEPROC( uint target, int offset, int length );
        public delegate void PFNGLBINDVERTEXARRAYPROC( uint array );
        public delegate void PFNGLDELETEVERTEXARRAYSPROC( int n, ref uint arrays );
        public delegate void PFNGLGENVERTEXARRAYSPROC( int n, ref uint arrays );
        public delegate byte PFNGLISVERTEXARRAYPROC( uint array );
        #endregion

        #region Methods
        public static PFNGLCOLORMASKIPROC glColorMaski;
        public static PFNGLGETBOOLEANI_VPROC glGetBooleani_v;
        public static PFNGLGETINTEGERI_VPROC glGetIntegeri_v;
        public static PFNGLENABLEIPROC glEnablei;
        public static PFNGLDISABLEIPROC glDisablei;
        public static PFNGLISENABLEDIPROC glIsEnabledi;
        public static PFNGLBEGINTRANSFORMFEEDBACKPROC glBeginTransformFeedback;
        public static PFNGLENDTRANSFORMFEEDBACKPROC glEndTransformFeedback;
        public static PFNGLBINDBUFFERRANGEPROC glBindBufferRange;
        public static PFNGLBINDBUFFERBASEPROC glBindBufferBase;
        public static PFNGLTRANSFORMFEEDBACKVARYINGSPROC glTransformFeedbackVaryings;
        public static PFNGLGETTRANSFORMFEEDBACKVARYINGPROC glGetTransformFeedbackVarying;
        public static PFNGLCLAMPCOLORPROC glClampColor;
        public static PFNGLBEGINCONDITIONALRENDERPROC glBeginConditionalRender;
        public static PFNGLENDCONDITIONALRENDERPROC glEndConditionalRender;
        public static PFNGLVERTEXATTRIBIPOINTERPROC glVertexAttribIPointer;
        public static PFNGLGETVERTEXATTRIBIIVPROC glGetVertexAttribIiv;
        public static PFNGLGETVERTEXATTRIBIUIVPROC glGetVertexAttribIuiv;
        public static PFNGLVERTEXATTRIBI1IPROC glVertexAttribI1i;
        public static PFNGLVERTEXATTRIBI2IPROC glVertexAttribI2i;
        public static PFNGLVERTEXATTRIBI3IPROC glVertexAttribI3i;
        public static PFNGLVERTEXATTRIBI4IPROC glVertexAttribI4i;
        public static PFNGLVERTEXATTRIBI1UIPROC glVertexAttribI1ui;
        public static PFNGLVERTEXATTRIBI2UIPROC glVertexAttribI2ui;
        public static PFNGLVERTEXATTRIBI3UIPROC glVertexAttribI3ui;
        public static PFNGLVERTEXATTRIBI4UIPROC glVertexAttribI4ui;
        public static PFNGLVERTEXATTRIBI1IVPROC glVertexAttribI1iv;
        public static PFNGLVERTEXATTRIBI2IVPROC glVertexAttribI2iv;
        public static PFNGLVERTEXATTRIBI3IVPROC glVertexAttribI3iv;
        public static PFNGLVERTEXATTRIBI4IVPROC glVertexAttribI4iv;
        public static PFNGLVERTEXATTRIBI1UIVPROC glVertexAttribI1uiv;
        public static PFNGLVERTEXATTRIBI2UIVPROC glVertexAttribI2uiv;
        public static PFNGLVERTEXATTRIBI3UIVPROC glVertexAttribI3uiv;
        public static PFNGLVERTEXATTRIBI4UIVPROC glVertexAttribI4uiv;
        public static PFNGLVERTEXATTRIBI4BVPROC glVertexAttribI4bv;
        public static PFNGLVERTEXATTRIBI4SVPROC glVertexAttribI4sv;
        public static PFNGLVERTEXATTRIBI4UBVPROC glVertexAttribI4ubv;
        public static PFNGLVERTEXATTRIBI4USVPROC glVertexAttribI4usv;
        public static PFNGLGETUNIFORMUIVPROC glGetUniformuiv;
        public static PFNGLBINDFRAGDATALOCATIONPROC glBindFragDataLocation;
        public static PFNGLGETFRAGDATALOCATIONPROC glGetFragDataLocation;
        public static PFNGLUNIFORM1UIPROC glUniform1ui;
        public static PFNGLUNIFORM2UIPROC glUniform2ui;
        public static PFNGLUNIFORM3UIPROC glUniform3ui;
        public static PFNGLUNIFORM4UIPROC glUniform4ui;
        public static PFNGLUNIFORM1UIVPROC glUniform1uiv;
        public static PFNGLUNIFORM2UIVPROC glUniform2uiv;
        public static PFNGLUNIFORM3UIVPROC glUniform3uiv;
        public static PFNGLUNIFORM4UIVPROC glUniform4uiv;
        public static PFNGLTEXPARAMETERIIVPROC glTexParameterIiv;
        public static PFNGLTEXPARAMETERIUIVPROC glTexParameterIuiv;
        public static PFNGLGETTEXPARAMETERIIVPROC glGetTexParameterIiv;
        public static PFNGLGETTEXPARAMETERIUIVPROC glGetTexParameterIuiv;
        public static PFNGLCLEARBUFFERIVPROC glClearBufferiv;
        public static PFNGLCLEARBUFFERUIVPROC glClearBufferuiv;
        public static PFNGLCLEARBUFFERFVPROC glClearBufferfv;
        public static PFNGLCLEARBUFFERFIPROC glClearBufferfi;
        public static PFNGLGETSTRINGIPROC glGetStringi;
        public static PFNGLISRENDERBUFFERPROC glIsRenderbuffer;
        public static PFNGLBINDRENDERBUFFERPROC glBindRenderbuffer;
        public static PFNGLDELETERENDERBUFFERSPROC glDeleteRenderbuffers;
        public static PFNGLGENRENDERBUFFERSPROC glGenRenderbuffers;
        public static PFNGLRENDERBUFFERSTORAGEPROC glRenderbufferStorage;
        public static PFNGLGETRENDERBUFFERPARAMETERIVPROC glGetRenderbufferParameteriv;
        public static PFNGLISFRAMEBUFFERPROC glIsFramebuffer;
        public static PFNGLBINDFRAMEBUFFERPROC glBindFramebuffer;
        public static PFNGLDELETEFRAMEBUFFERSPROC glDeleteFramebuffers;
        public static PFNGLGENFRAMEBUFFERSPROC glGenFramebuffers;
        public static PFNGLCHECKFRAMEBUFFERSTATUSPROC glCheckFramebufferStatus;
        public static PFNGLFRAMEBUFFERTEXTURE1DPROC glFramebufferTexture1D;
        public static PFNGLFRAMEBUFFERTEXTURE2DPROC glFramebufferTexture2D;
        public static PFNGLFRAMEBUFFERTEXTURE3DPROC glFramebufferTexture3D;
        public static PFNGLFRAMEBUFFERRENDERBUFFERPROC glFramebufferRenderbuffer;
        public static PFNGLGETFRAMEBUFFERATTACHMENTPARAMETERIVPROC glGetFramebufferAttachmentParameteriv;
        public static PFNGLGENERATEMIPMAPPROC glGenerateMipmap;
        public static PFNGLBLITFRAMEBUFFERPROC glBlitFramebuffer;
        public static PFNGLRENDERBUFFERSTORAGEMULTISAMPLEPROC glRenderbufferStorageMultisample;
        public static PFNGLFRAMEBUFFERTEXTURELAYERPROC glFramebufferTextureLayer;
        public static PFNGLMAPBUFFERRANGEPROC glMapBufferRange;
        public static PFNGLFLUSHMAPPEDBUFFERRANGEPROC glFlushMappedBufferRange;
        public static PFNGLBINDVERTEXARRAYPROC glBindVertexArray;
        public static PFNGLDELETEVERTEXARRAYSPROC glDeleteVertexArrays;
        public static PFNGLGENVERTEXARRAYSPROC glGenVertexArrays;
        public static PFNGLISVERTEXARRAYPROC glIsVertexArray;
        #endregion
        #endregion

        #region OpenGL 3.1
        #region Constants
        public const uint GL_SAMPLER_2D_RECT = 35683;
        public const uint GL_SAMPLER_2D_RECT_SHADOW = 35684;
        public const uint GL_SAMPLER_BUFFER = 36290;
        public const uint GL_INT_SAMPLER_2D_RECT = 36301;
        public const uint GL_INT_SAMPLER_BUFFER = 36304;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D_RECT = 36309;
        public const uint GL_UNSIGNED_INT_SAMPLER_BUFFER = 36312;
        public const uint GL_TEXTURE_BUFFER = 35882;
        public const uint GL_MAX_TEXTURE_BUFFER_SIZE = 35883;
        public const uint GL_TEXTURE_BINDING_BUFFER = 35884;
        public const uint GL_TEXTURE_BUFFER_DATA_STORE_BINDING = 35885;
        public const uint GL_TEXTURE_RECTANGLE = 34037;
        public const uint GL_TEXTURE_BINDING_RECTANGLE = 34038;
        public const uint GL_PROXY_TEXTURE_RECTANGLE = 34039;
        public const uint GL_MAX_RECTANGLE_TEXTURE_SIZE = 34040;
        public const uint GL_R8_SNORM = 36756;
        public const uint GL_RG8_SNORM = 36757;
        public const uint GL_RGB8_SNORM = 36758;
        public const uint GL_RGBA8_SNORM = 36759;
        public const uint GL_R16_SNORM = 36760;
        public const uint GL_RG16_SNORM = 36761;
        public const uint GL_RGB16_SNORM = 36762;
        public const uint GL_RGBA16_SNORM = 36763;
        public const uint GL_SIGNED_NORMALIZED = 36764;
        public const uint GL_PRIMITIVE_RESTART = 36765;
        public const uint GL_PRIMITIVE_RESTART_INDEX = 36766;
        public const uint GL_COPY_READ_BUFFER = 36662;
        public const uint GL_COPY_WRITE_BUFFER = 36663;
        public const uint GL_UNIFORM_BUFFER = 35345;
        public const uint GL_UNIFORM_BUFFER_BINDING = 35368;
        public const uint GL_UNIFORM_BUFFER_START = 35369;
        public const uint GL_UNIFORM_BUFFER_SIZE = 35370;
        public const uint GL_MAX_VERTEX_UNIFORM_BLOCKS = 35371;
        public const uint GL_MAX_GEOMETRY_UNIFORM_BLOCKS = 35372;
        public const uint GL_MAX_FRAGMENT_UNIFORM_BLOCKS = 35373;
        public const uint GL_MAX_COMBINED_UNIFORM_BLOCKS = 35374;
        public const uint GL_MAX_UNIFORM_BUFFER_BINDINGS = 35375;
        public const uint GL_MAX_UNIFORM_BLOCK_SIZE = 35376;
        public const uint GL_MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS = 35377;
        public const uint GL_MAX_COMBINED_GEOMETRY_UNIFORM_COMPONENTS = 35378;
        public const uint GL_MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS = 35379;
        public const uint GL_UNIFORM_BUFFER_OFFSET_ALIGNMENT = 35380;
        public const uint GL_ACTIVE_UNIFORM_BLOCK_MAX_NAME_LENGTH = 35381;
        public const uint GL_ACTIVE_UNIFORM_BLOCKS = 35382;
        public const uint GL_UNIFORM_TYPE = 35383;
        public const uint GL_UNIFORM_SIZE = 35384;
        public const uint GL_UNIFORM_NAME_LENGTH = 35385;
        public const uint GL_UNIFORM_BLOCK_INDEX = 35386;
        public const uint GL_UNIFORM_OFFSET = 35387;
        public const uint GL_UNIFORM_ARRAY_STRIDE = 35388;
        public const uint GL_UNIFORM_MATRIX_STRIDE = 35389;
        public const uint GL_UNIFORM_IS_ROW_MAJOR = 35390;
        public const uint GL_UNIFORM_BLOCK_BINDING = 35391;
        public const uint GL_UNIFORM_BLOCK_DATA_SIZE = 35392;
        public const uint GL_UNIFORM_BLOCK_NAME_LENGTH = 35393;
        public const uint GL_UNIFORM_BLOCK_ACTIVE_UNIFORMS = 35394;
        public const uint GL_UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES = 35395;
        public const uint GL_UNIFORM_BLOCK_REFERENCED_BY_VERTEX_SHADER = 35396;
        public const uint GL_UNIFORM_BLOCK_REFERENCED_BY_GEOMETRY_SHADER = 35397;
        public const uint GL_UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER = 35398;
        public const uint GL_INVALID_INDEX = 4294967295;
        #endregion

        #region Delegates
        public delegate void PFNGLDRAWARRAYSINSTANCEDPROC( uint mode, int first, int count, int instancecount );
        public delegate void PFNGLDRAWELEMENTSINSTANCEDPROC( uint mode, int count, uint type, IntPtr indices, int instancecount );
        public delegate void PFNGLTEXBUFFERPROC( uint target, uint internalformat, uint buffer );
        public delegate void PFNGLPRIMITIVERESTARTINDEXPROC( uint index );
        public delegate void PFNGLCOPYBUFFERSUBDATAPROC( uint readTarget, uint writeTarget, int readOffset, int writeOffset, int size );
        public delegate void PFNGLGETUNIFORMINDICESPROC( uint program, int uniformCount, IntPtr uniformNames, ref uint uniformIndices );
        public delegate void PFNGLGETACTIVEUNIFORMSIVPROC( uint program, int uniformCount, ref uint uniformIndices, uint pname, ref int parameters );
        public delegate void PFNGLGETACTIVEUNIFORMNAMEPROC( uint program, uint uniformIndex, int bufSize, ref int length, IntPtr uniformName );
        public delegate uint PFNGLGETUNIFORMBLOCKINDEXPROC( uint program, [In] [MarshalAs( UnmanagedType.LPStr )] string uniformBlockName );
        public delegate void PFNGLGETACTIVEUNIFORMBLOCKIVPROC( uint program, uint uniformBlockIndex, uint pname, ref int parameters );
        public delegate void PFNGLGETACTIVEUNIFORMBLOCKNAMEPROC( uint program, uint uniformBlockIndex, int bufSize, ref int length, IntPtr uniformBlockName );
        public delegate void PFNGLUNIFORMBLOCKBINDINGPROC( uint program, uint uniformBlockIndex, uint uniformBlockBinding );
        #endregion

        #region Methods
        public static PFNGLDRAWARRAYSINSTANCEDPROC glDrawArraysInstanced;
        public static PFNGLDRAWELEMENTSINSTANCEDPROC glDrawElementsInstanced;
        public static PFNGLTEXBUFFERPROC glTexBuffer;
        public static PFNGLPRIMITIVERESTARTINDEXPROC glPrimitiveRestartIndex;
        public static PFNGLCOPYBUFFERSUBDATAPROC glCopyBufferSubData;
        public static PFNGLGETUNIFORMINDICESPROC glGetUniformIndices;
        public static PFNGLGETACTIVEUNIFORMSIVPROC glGetActiveUniformsiv;
        public static PFNGLGETACTIVEUNIFORMNAMEPROC glGetActiveUniformName;
        public static PFNGLGETUNIFORMBLOCKINDEXPROC glGetUniformBlockIndex;
        public static PFNGLGETACTIVEUNIFORMBLOCKIVPROC glGetActiveUniformBlockiv;
        public static PFNGLGETACTIVEUNIFORMBLOCKNAMEPROC glGetActiveUniformBlockName;
        public static PFNGLUNIFORMBLOCKBINDINGPROC glUniformBlockBinding;
        #endregion

        #endregion

        #region OpenGL 3.2
        #region Constants
        public const uint GL_CONTEXT_CORE_PROFILE_BIT = 1;
        public const uint GL_CONTEXT_COMPATIBILITY_PROFILE_BIT = 2;
        public const uint GL_LINES_ADJACENCY = 10;
        public const uint GL_LINE_STRIP_ADJACENCY = 11;
        public const uint GL_TRIANGLES_ADJACENCY = 12;
        public const uint GL_TRIANGLE_STRIP_ADJACENCY = 13;
        public const uint GL_PROGRAM_POINT_SIZE = 34370;
        public const uint GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS = 35881;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_LAYERED = 36263;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS = 36264;
        public const uint GL_GEOMETRY_SHADER = 36313;
        public const uint GL_GEOMETRY_VERTICES_OUT = 35094;
        public const uint GL_GEOMETRY_INPUT_TYPE = 35095;
        public const uint GL_GEOMETRY_OUTPUT_TYPE = 35096;
        public const uint GL_MAX_GEOMETRY_UNIFORM_COMPONENTS = 36319;
        public const uint GL_MAX_GEOMETRY_OUTPUT_VERTICES = 36320;
        public const uint GL_MAX_GEOMETRY_TOTAL_OUTPUT_COMPONENTS = 36321;
        public const uint GL_MAX_VERTEX_OUTPUT_COMPONENTS = 37154;
        public const uint GL_MAX_GEOMETRY_INPUT_COMPONENTS = 37155;
        public const uint GL_MAX_GEOMETRY_OUTPUT_COMPONENTS = 37156;
        public const uint GL_MAX_FRAGMENT_INPUT_COMPONENTS = 37157;
        public const uint GL_CONTEXT_PROFILE_MASK = 37158;
        public const uint GL_DEPTH_CLAMP = 34383;
        public const uint GL_QUADS_FOLLOW_PROVOKING_VERTEX_CONVENTION = 36428;
        public const uint GL_FIRST_VERTEX_CONVENTION = 36429;
        public const uint GL_LAST_VERTEX_CONVENTION = 36430;
        public const uint GL_PROVOKING_VERTEX = 36431;
        public const uint GL_TEXTURE_CUBE_MAP_SEAMLESS = 34895;
        public const uint GL_MAX_SERVER_WAIT_TIMEOUT = 37137;
        public const uint GL_OBJECT_TYPE = 37138;
        public const uint GL_SYNC_CONDITION = 37139;
        public const uint GL_SYNC_STATUS = 37140;
        public const uint GL_SYNC_FLAGS = 37141;
        public const uint GL_SYNC_FENCE = 37142;
        public const uint GL_SYNC_GPU_COMMANDS_COMPLETE = 37143;
        public const uint GL_UNSIGNALED = 37144;
        public const uint GL_SIGNALED = 37145;
        public const uint GL_ALREADY_SIGNALED = 37146;
        public const uint GL_TIMEOUT_EXPIRED = 37147;
        public const uint GL_CONDITION_SATISFIED = 37148;
        public const uint GL_WAIT_FAILED = 37149;
        public const ulong GL_TIMEOUT_IGNORED = 18446744073709551615ul;
        public const uint GL_SYNC_FLUSH_COMMANDS_BIT = 1;
        public const uint GL_SAMPLE_POSITION = 36432;
        public const uint GL_SAMPLE_MASK = 36433;
        public const uint GL_SAMPLE_MASK_VALUE = 36434;
        public const uint GL_MAX_SAMPLE_MASK_WORDS = 36441;
        public const uint GL_TEXTURE_2D_MULTISAMPLE = 37120;
        public const uint GL_PROXY_TEXTURE_2D_MULTISAMPLE = 37121;
        public const uint GL_TEXTURE_2D_MULTISAMPLE_ARRAY = 37122;
        public const uint GL_PROXY_TEXTURE_2D_MULTISAMPLE_ARRAY = 37123;
        public const uint GL_TEXTURE_BINDING_2D_MULTISAMPLE = 37124;
        public const uint GL_TEXTURE_BINDING_2D_MULTISAMPLE_ARRAY = 37125;
        public const uint GL_TEXTURE_SAMPLES = 37126;
        public const uint GL_TEXTURE_FIXED_SAMPLE_LOCATIONS = 37127;
        public const uint GL_SAMPLER_2D_MULTISAMPLE = 37128;
        public const uint GL_INT_SAMPLER_2D_MULTISAMPLE = 37129;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE = 37130;
        public const uint GL_SAMPLER_2D_MULTISAMPLE_ARRAY = 37131;
        public const uint GL_INT_SAMPLER_2D_MULTISAMPLE_ARRAY = 37132;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE_ARRAY = 37133;
        public const uint GL_MAX_COLOR_TEXTURE_SAMPLES = 37134;
        public const uint GL_MAX_DEPTH_TEXTURE_SAMPLES = 37135;
        public const uint GL_MAX_INTEGER_SAMPLES = 37136;
        #endregion

        #region Delegates
        public delegate void PFNGLDRAWELEMENTSBASEVERTEXPROC( uint mode, int count, uint type, IntPtr indices, int basevertex );
        public delegate void PFNGLDRAWRANGEELEMENTSBASEVERTEXPROC( uint mode, uint start, uint end, int count, uint type, IntPtr indices, int basevertex );
        public delegate void PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXPROC( uint mode, int count, uint type, IntPtr indices, int instancecount, int basevertex );
        public delegate void PFNGLMULTIDRAWELEMENTSBASEVERTEXPROC( uint mode, ref int count, uint type, IntPtr indices, int drawcount, ref int basevertex );
        public delegate void PFNGLPROVOKINGVERTEXPROC( uint mode );
        public delegate IntPtr PFNGLFENCESYNCPROC( uint condition, uint flags );
        public delegate byte PFNGLISSYNCPROC( IntPtr sync );
        public delegate void PFNGLDELETESYNCPROC( IntPtr sync );
        public delegate uint PFNGLCLIENTWAITSYNCPROC( IntPtr sync, uint flags, uint timeout );
        public delegate void PFNGLWAITSYNCPROC( IntPtr sync, uint flags, uint timeout );
        public delegate void PFNGLGETINTEGER64VPROC( uint pname, ref int data );
        public delegate void PFNGLGETSYNCIVPROC( IntPtr sync, uint pname, int bufSize, ref int length, ref int values );
        public delegate void PFNGLGETINTEGER64I_VPROC( uint target, uint index, ref int data );
        public delegate void PFNGLGETBUFFERPARAMETERI64VPROC( uint target, uint pname, ref int parameters );
        public delegate void PFNGLFRAMEBUFFERTEXTUREPROC( uint target, uint attachment, uint texture, int level );
        public delegate void PFNGLTEXIMAGE2DMULTISAMPLEPROC( uint target, int samples, uint internalformat, int width, int height, byte fixedsamplelocations );
        public delegate void PFNGLTEXIMAGE3DMULTISAMPLEPROC( uint target, int samples, uint internalformat, int width, int height, int depth, byte fixedsamplelocations );
        public delegate void PFNGLGETMULTISAMPLEFVPROC( uint pname, uint index, ref float val );
        public delegate void PFNGLSAMPLEMASKIPROC( uint maskNumber, uint mask );
        #endregion

        #region Methods
        public static PFNGLDRAWELEMENTSBASEVERTEXPROC glDrawElementsBaseVertex;
        public static PFNGLDRAWRANGEELEMENTSBASEVERTEXPROC glDrawRangeElementsBaseVertex;
        public static PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXPROC glDrawElementsInstancedBaseVertex;
        public static PFNGLMULTIDRAWELEMENTSBASEVERTEXPROC glMultiDrawElementsBaseVertex;
        public static PFNGLPROVOKINGVERTEXPROC glProvokingVertex;
        public static PFNGLFENCESYNCPROC glFenceSync;
        public static PFNGLISSYNCPROC glIsSync;
        public static PFNGLDELETESYNCPROC glDeleteSync;
        public static PFNGLCLIENTWAITSYNCPROC glClientWaitSync;
        public static PFNGLWAITSYNCPROC glWaitSync;
        public static PFNGLGETINTEGER64VPROC glGetInteger64v;
        public static PFNGLGETSYNCIVPROC glGetSynciv;
        public static PFNGLGETINTEGER64I_VPROC glGetInteger64i_v;
        public static PFNGLGETBUFFERPARAMETERI64VPROC glGetBufferParameteri64v;
        public static PFNGLFRAMEBUFFERTEXTUREPROC glFramebufferTexture;
        public static PFNGLTEXIMAGE2DMULTISAMPLEPROC glTexImage2DMultisample;
        public static PFNGLTEXIMAGE3DMULTISAMPLEPROC glTexImage3DMultisample;
        public static PFNGLGETMULTISAMPLEFVPROC glGetMultisamplefv;
        public static PFNGLSAMPLEMASKIPROC glSampleMaski;
        #endregion
        #endregion

        #region OpenGL 3.3
        #region Constants
        public const uint GL_VERTEX_ATTRIB_ARRAY_DIVISOR = 35070;
        public const uint GL_SRC1_COLOR = 35065;
        public const uint GL_ONE_MINUS_SRC1_COLOR = 35066;
        public const uint GL_ONE_MINUS_SRC1_ALPHA = 35067;
        public const uint GL_MAX_DUAL_SOURCE_DRAW_BUFFERS = 35068;
        public const uint GL_ANY_SAMPLES_PASSED = 35887;
        public const uint GL_SAMPLER_BINDING = 35097;
        public const uint GL_RGB10_A2UI = 36975;
        public const uint GL_TEXTURE_SWIZZLE_R = 36418;
        public const uint GL_TEXTURE_SWIZZLE_G = 36419;
        public const uint GL_TEXTURE_SWIZZLE_B = 36420;
        public const uint GL_TEXTURE_SWIZZLE_A = 36421;
        public const uint GL_TEXTURE_SWIZZLE_RGBA = 36422;
        public const uint GL_TIME_ELAPSED = 35007;
        public const uint GL_TIMESTAMP = 36392;
        public const uint GL_INT_2_10_10_10_REV = 36255;
        #endregion

        #region Delegates
        public delegate void PFNGLBINDFRAGDATALOCATIONINDEXEDPROC( uint program, uint colorNumber, uint index, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate int PFNGLGETFRAGDATAINDEXPROC( uint program, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate void PFNGLGENSAMPLERSPROC( int count, ref uint samplers );
        public delegate void PFNGLDELETESAMPLERSPROC( int count, ref uint samplers );
        public delegate byte PFNGLISSAMPLERPROC( uint sampler );
        public delegate void PFNGLBINDSAMPLERPROC( uint unit, uint sampler );
        public delegate void PFNGLSAMPLERPARAMETERIPROC( uint sampler, uint pname, int param );
        public delegate void PFNGLSAMPLERPARAMETERIVPROC( uint sampler, uint pname, ref int param );
        public delegate void PFNGLSAMPLERPARAMETERFPROC( uint sampler, uint pname, float param );
        public delegate void PFNGLSAMPLERPARAMETERFVPROC( uint sampler, uint pname, ref float param );
        public delegate void PFNGLSAMPLERPARAMETERIIVPROC( uint sampler, uint pname, ref int param );
        public delegate void PFNGLSAMPLERPARAMETERIUIVPROC( uint sampler, uint pname, ref uint param );
        public delegate void PFNGLGETSAMPLERPARAMETERIVPROC( uint sampler, uint pname, ref int parameters );
        public delegate void PFNGLGETSAMPLERPARAMETERIIVPROC( uint sampler, uint pname, ref int parameters );
        public delegate void PFNGLGETSAMPLERPARAMETERFVPROC( uint sampler, uint pname, ref float parameters );
        public delegate void PFNGLGETSAMPLERPARAMETERIUIVPROC( uint sampler, uint pname, ref uint parameters );
        public delegate void PFNGLQUERYCOUNTERPROC( uint id, uint target );
        public delegate void PFNGLGETQUERYOBJECTI64VPROC( uint id, uint pname, ref int parameters );
        public delegate void PFNGLGETQUERYOBJECTUI64VPROC( uint id, uint pname, ref uint parameters );
        public delegate void PFNGLVERTEXATTRIBDIVISORPROC( uint index, uint divisor );
        public delegate void PFNGLVERTEXATTRIBP1UIPROC( uint index, uint type, byte normalized, uint value );
        public delegate void PFNGLVERTEXATTRIBP1UIVPROC( uint index, uint type, byte normalized, ref uint value );
        public delegate void PFNGLVERTEXATTRIBP2UIPROC( uint index, uint type, byte normalized, uint value );
        public delegate void PFNGLVERTEXATTRIBP2UIVPROC( uint index, uint type, byte normalized, ref uint value );
        public delegate void PFNGLVERTEXATTRIBP3UIPROC( uint index, uint type, byte normalized, uint value );
        public delegate void PFNGLVERTEXATTRIBP3UIVPROC( uint index, uint type, byte normalized, ref uint value );
        public delegate void PFNGLVERTEXATTRIBP4UIPROC( uint index, uint type, byte normalized, uint value );
        public delegate void PFNGLVERTEXATTRIBP4UIVPROC( uint index, uint type, byte normalized, ref uint value );
        #endregion

        #region Methods
        public static PFNGLBINDFRAGDATALOCATIONINDEXEDPROC glBindFragDataLocationIndexed;
        public static PFNGLGETFRAGDATAINDEXPROC glGetFragDataIndex;
        public static PFNGLGENSAMPLERSPROC glGenSamplers;
        public static PFNGLDELETESAMPLERSPROC glDeleteSamplers;
        public static PFNGLISSAMPLERPROC glIsSampler;
        public static PFNGLBINDSAMPLERPROC glBindSampler;
        public static PFNGLSAMPLERPARAMETERIPROC glSamplerParameteri;
        public static PFNGLSAMPLERPARAMETERIVPROC glSamplerParameteriv;
        public static PFNGLSAMPLERPARAMETERFPROC glSamplerParameterf;
        public static PFNGLSAMPLERPARAMETERFVPROC glSamplerParameterfv;
        public static PFNGLSAMPLERPARAMETERIIVPROC glSamplerParameterIiv;
        public static PFNGLSAMPLERPARAMETERIUIVPROC glSamplerParameterIuiv;
        public static PFNGLGETSAMPLERPARAMETERIVPROC glGetSamplerParameteriv;
        public static PFNGLGETSAMPLERPARAMETERIIVPROC glGetSamplerParameterIiv;
        public static PFNGLGETSAMPLERPARAMETERFVPROC glGetSamplerParameterfv;
        public static PFNGLGETSAMPLERPARAMETERIUIVPROC glGetSamplerParameterIuiv;
        public static PFNGLQUERYCOUNTERPROC glQueryCounter;
        public static PFNGLGETQUERYOBJECTI64VPROC glGetQueryObjecti64v;
        public static PFNGLGETQUERYOBJECTUI64VPROC glGetQueryObjectui64v;
        public static PFNGLVERTEXATTRIBDIVISORPROC glVertexAttribDivisor;
        public static PFNGLVERTEXATTRIBP1UIPROC glVertexAttribP1ui;
        public static PFNGLVERTEXATTRIBP1UIVPROC glVertexAttribP1uiv;
        public static PFNGLVERTEXATTRIBP2UIPROC glVertexAttribP2ui;
        public static PFNGLVERTEXATTRIBP2UIVPROC glVertexAttribP2uiv;
        public static PFNGLVERTEXATTRIBP3UIPROC glVertexAttribP3ui;
        public static PFNGLVERTEXATTRIBP3UIVPROC glVertexAttribP3uiv;
        public static PFNGLVERTEXATTRIBP4UIPROC glVertexAttribP4ui;
        public static PFNGLVERTEXATTRIBP4UIVPROC glVertexAttribP4uiv;
        #endregion
        #endregion

        #region OpenGL 4.0
        #region Constants
        public const uint GL_SAMPLE_SHADING = 35894;
        public const uint GL_MIN_SAMPLE_SHADING_VALUE = 35895;
        public const uint GL_MIN_PROGRAM_TEXTURE_GATHER_OFFSET = 36446;
        public const uint GL_MAX_PROGRAM_TEXTURE_GATHER_OFFSET = 36447;
        public const uint GL_TEXTURE_CUBE_MAP_ARRAY = 36873;
        public const uint GL_TEXTURE_BINDING_CUBE_MAP_ARRAY = 36874;
        public const uint GL_PROXY_TEXTURE_CUBE_MAP_ARRAY = 36875;
        public const uint GL_SAMPLER_CUBE_MAP_ARRAY = 36876;
        public const uint GL_SAMPLER_CUBE_MAP_ARRAY_SHADOW = 36877;
        public const uint GL_INT_SAMPLER_CUBE_MAP_ARRAY = 36878;
        public const uint GL_UNSIGNED_INT_SAMPLER_CUBE_MAP_ARRAY = 36879;
        public const uint GL_DRAW_INDIRECT_BUFFER = 36671;
        public const uint GL_DRAW_INDIRECT_BUFFER_BINDING = 36675;
        public const uint GL_GEOMETRY_SHADER_INVOCATIONS = 34943;
        public const uint GL_MAX_GEOMETRY_SHADER_INVOCATIONS = 36442;
        public const uint GL_MIN_FRAGMENT_INTERPOLATION_OFFSET = 36443;
        public const uint GL_MAX_FRAGMENT_INTERPOLATION_OFFSET = 36444;
        public const uint GL_FRAGMENT_INTERPOLATION_OFFSET_BITS = 36445;
        public const uint GL_MAX_VERTEX_STREAMS = 36465;
        public const uint GL_DOUBLE_VEC2 = 36860;
        public const uint GL_DOUBLE_VEC3 = 36861;
        public const uint GL_DOUBLE_VEC4 = 36862;
        public const uint GL_DOUBLE_MAT2 = 36678;
        public const uint GL_DOUBLE_MAT3 = 36679;
        public const uint GL_DOUBLE_MAT4 = 36680;
        public const uint GL_DOUBLE_MAT2x3 = 36681;
        public const uint GL_DOUBLE_MAT2x4 = 36682;
        public const uint GL_DOUBLE_MAT3x2 = 36683;
        public const uint GL_DOUBLE_MAT3x4 = 36684;
        public const uint GL_DOUBLE_MAT4x2 = 36685;
        public const uint GL_DOUBLE_MAT4x3 = 36686;
        public const uint GL_ACTIVE_SUBROUTINES = 36325;
        public const uint GL_ACTIVE_SUBROUTINE_UNIFORMS = 36326;
        public const uint GL_ACTIVE_SUBROUTINE_UNIFORM_LOCATIONS = 36423;
        public const uint GL_ACTIVE_SUBROUTINE_MAX_LENGTH = 36424;
        public const uint GL_ACTIVE_SUBROUTINE_UNIFORM_MAX_LENGTH = 36425;
        public const uint GL_MAX_SUBROUTINES = 36327;
        public const uint GL_MAX_SUBROUTINE_UNIFORM_LOCATIONS = 36328;
        public const uint GL_NUM_COMPATIBLE_SUBROUTINES = 36426;
        public const uint GL_COMPATIBLE_SUBROUTINES = 36427;
        public const uint GL_PATCHES = 14;
        public const uint GL_PATCH_VERTICES = 36466;
        public const uint GL_PATCH_DEFAULT_INNER_LEVEL = 36467;
        public const uint GL_PATCH_DEFAULT_OUTER_LEVEL = 36468;
        public const uint GL_TESS_CONTROL_OUTPUT_VERTICES = 36469;
        public const uint GL_TESS_GEN_MODE = 36470;
        public const uint GL_TESS_GEN_SPACING = 36471;
        public const uint GL_TESS_GEN_VERTEX_ORDER = 36472;
        public const uint GL_TESS_GEN_POINT_MODE = 36473;
        public const uint GL_ISOLINES = 36474;
        public const uint GL_FRACTIONAL_ODD = 36475;
        public const uint GL_FRACTIONAL_EVEN = 36476;
        public const uint GL_MAX_PATCH_VERTICES = 36477;
        public const uint GL_MAX_TESS_GEN_LEVEL = 36478;
        public const uint GL_MAX_TESS_CONTROL_UNIFORM_COMPONENTS = 36479;
        public const uint GL_MAX_TESS_EVALUATION_UNIFORM_COMPONENTS = 36480;
        public const uint GL_MAX_TESS_CONTROL_TEXTURE_IMAGE_UNITS = 36481;
        public const uint GL_MAX_TESS_EVALUATION_TEXTURE_IMAGE_UNITS = 36482;
        public const uint GL_MAX_TESS_CONTROL_OUTPUT_COMPONENTS = 36483;
        public const uint GL_MAX_TESS_PATCH_COMPONENTS = 36484;
        public const uint GL_MAX_TESS_CONTROL_TOTAL_OUTPUT_COMPONENTS = 36485;
        public const uint GL_MAX_TESS_EVALUATION_OUTPUT_COMPONENTS = 36486;
        public const uint GL_MAX_TESS_CONTROL_UNIFORM_BLOCKS = 36489;
        public const uint GL_MAX_TESS_EVALUATION_UNIFORM_BLOCKS = 36490;
        public const uint GL_MAX_TESS_CONTROL_INPUT_COMPONENTS = 34924;
        public const uint GL_MAX_TESS_EVALUATION_INPUT_COMPONENTS = 34925;
        public const uint GL_MAX_COMBINED_TESS_CONTROL_UNIFORM_COMPONENTS = 36382;
        public const uint GL_MAX_COMBINED_TESS_EVALUATION_UNIFORM_COMPONENTS = 36383;
        public const uint GL_UNIFORM_BLOCK_REFERENCED_BY_TESS_CONTROL_SHADER = 34032;
        public const uint GL_UNIFORM_BLOCK_REFERENCED_BY_TESS_EVALUATION_SHADER = 34033;
        public const uint GL_TESS_EVALUATION_SHADER = 36487;
        public const uint GL_TESS_CONTROL_SHADER = 36488;
        public const uint GL_TRANSFORM_FEEDBACK = 36386;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_PAUSED = 36387;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_ACTIVE = 36388;
        public const uint GL_TRANSFORM_FEEDBACK_BINDING = 36389;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_BUFFERS = 36464;
        #endregion

        #region Delegates
        public delegate void PFNGLMINSAMPLESHADINGPROC( float value );
        public delegate void PFNGLBLENDEQUATIONIPROC( uint buf, uint mode );
        public delegate void PFNGLBLENDEQUATIONSEPARATEIPROC( uint buf, uint modeRGB, uint modeAlpha );
        public delegate void PFNGLBLENDFUNCIPROC( uint buf, uint src, uint dst );
        public delegate void PFNGLBLENDFUNCSEPARATEIPROC( uint buf, uint srcRGB, uint dstRGB, uint srcAlpha, uint dstAlpha );
        public delegate void PFNGLDRAWARRAYSINDIRECTPROC( uint mode, IntPtr indirect );
        public delegate void PFNGLDRAWELEMENTSINDIRECTPROC( uint mode, uint type, IntPtr indirect );
        public delegate void PFNGLUNIFORM1DPROC( int location, double x );
        public delegate void PFNGLUNIFORM2DPROC( int location, double x, double y );
        public delegate void PFNGLUNIFORM3DPROC( int location, double x, double y, double z );
        public delegate void PFNGLUNIFORM4DPROC( int location, double x, double y, double z, double w );
        public delegate void PFNGLUNIFORM1DVPROC( int location, int count, ref double value );
        public delegate void PFNGLUNIFORM2DVPROC( int location, int count, ref double value );
        public delegate void PFNGLUNIFORM3DVPROC( int location, int count, ref double value );
        public delegate void PFNGLUNIFORM4DVPROC( int location, int count, ref double value );
        public delegate void PFNGLUNIFORMMATRIX2DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLUNIFORMMATRIX3DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLUNIFORMMATRIX4DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLUNIFORMMATRIX2X3DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLUNIFORMMATRIX2X4DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLUNIFORMMATRIX3X2DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLUNIFORMMATRIX3X4DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLUNIFORMMATRIX4X2DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLUNIFORMMATRIX4X3DVPROC( int location, int count, byte transpose, ref double value );
        public delegate void PFNGLGETUNIFORMDVPROC( uint program, int location, ref double parameters );
        public delegate int PFNGLGETSUBROUTINEUNIFORMLOCATIONPROC( uint program, uint shadertype, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate uint PFNGLGETSUBROUTINEINDEXPROC( uint program, uint shadertype, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate void PFNGLGETACTIVESUBROUTINEUNIFORMIVPROC( uint program, uint shadertype, uint index, uint pname, ref int values );
        public delegate void PFNGLGETACTIVESUBROUTINEUNIFORMNAMEPROC( uint program, uint shadertype, uint index, int bufsize, ref int length, IntPtr name );
        public delegate void PFNGLGETACTIVESUBROUTINENAMEPROC( uint program, uint shadertype, uint index, int bufsize, ref int length, IntPtr name );
        public delegate void PFNGLUNIFORMSUBROUTINESUIVPROC( uint shadertype, int count, ref uint indices );
        public delegate void PFNGLGETUNIFORMSUBROUTINEUIVPROC( uint shadertype, int location, ref uint parameters );
        public delegate void PFNGLGETPROGRAMSTAGEIVPROC( uint program, uint shadertype, uint pname, ref int values );
        public delegate void PFNGLPATCHPARAMETERIPROC( uint pname, int value );
        public delegate void PFNGLPATCHPARAMETERFVPROC( uint pname, ref float values );
        public delegate void PFNGLBINDTRANSFORMFEEDBACKPROC( uint target, uint id );
        public delegate void PFNGLDELETETRANSFORMFEEDBACKSPROC( int n, ref uint ids );
        public delegate void PFNGLGENTRANSFORMFEEDBACKSPROC( int n, ref uint ids );
        public delegate byte PFNGLISTRANSFORMFEEDBACKPROC( uint id );
        public delegate void PFNGLPAUSETRANSFORMFEEDBACKPROC();
        public delegate void PFNGLRESUMETRANSFORMFEEDBACKPROC();
        public delegate void PFNGLDRAWTRANSFORMFEEDBACKPROC( uint mode, uint id );
        public delegate void PFNGLDRAWTRANSFORMFEEDBACKSTREAMPROC( uint mode, uint id, uint stream );
        public delegate void PFNGLBEGINQUERYINDEXEDPROC( uint target, uint index, uint id );
        public delegate void PFNGLENDQUERYINDEXEDPROC( uint target, uint index );
        public delegate void PFNGLGETQUERYINDEXEDIVPROC( uint target, uint index, uint pname, ref int parameters );
        #endregion

        #region Methods
        public static PFNGLMINSAMPLESHADINGPROC glMinSampleShading;
        public static PFNGLBLENDEQUATIONIPROC glBlendEquationi;
        public static PFNGLBLENDEQUATIONSEPARATEIPROC glBlendEquationSeparatei;
        public static PFNGLBLENDFUNCIPROC glBlendFunci;
        public static PFNGLBLENDFUNCSEPARATEIPROC glBlendFuncSeparatei;
        public static PFNGLDRAWARRAYSINDIRECTPROC glDrawArraysIndirect;
        public static PFNGLDRAWELEMENTSINDIRECTPROC glDrawElementsIndirect;
        public static PFNGLUNIFORM1DPROC glUniform1d;
        public static PFNGLUNIFORM2DPROC glUniform2d;
        public static PFNGLUNIFORM3DPROC glUniform3d;
        public static PFNGLUNIFORM4DPROC glUniform4d;
        public static PFNGLUNIFORM1DVPROC glUniform1dv;
        public static PFNGLUNIFORM2DVPROC glUniform2dv;
        public static PFNGLUNIFORM3DVPROC glUniform3dv;
        public static PFNGLUNIFORM4DVPROC glUniform4dv;
        public static PFNGLUNIFORMMATRIX2DVPROC glUniformMatrix2dv;
        public static PFNGLUNIFORMMATRIX3DVPROC glUniformMatrix3dv;
        public static PFNGLUNIFORMMATRIX4DVPROC glUniformMatrix4dv;
        public static PFNGLUNIFORMMATRIX2X3DVPROC glUniformMatrix2x3dv;
        public static PFNGLUNIFORMMATRIX2X4DVPROC glUniformMatrix2x4dv;
        public static PFNGLUNIFORMMATRIX3X2DVPROC glUniformMatrix3x2dv;
        public static PFNGLUNIFORMMATRIX3X4DVPROC glUniformMatrix3x4dv;
        public static PFNGLUNIFORMMATRIX4X2DVPROC glUniformMatrix4x2dv;
        public static PFNGLUNIFORMMATRIX4X3DVPROC glUniformMatrix4x3dv;
        public static PFNGLGETUNIFORMDVPROC glGetUniformdv;
        public static PFNGLGETSUBROUTINEUNIFORMLOCATIONPROC glGetSubroutineUniformLocation;
        public static PFNGLGETSUBROUTINEINDEXPROC glGetSubroutineIndex;
        public static PFNGLGETACTIVESUBROUTINEUNIFORMIVPROC glGetActiveSubroutineUniformiv;
        public static PFNGLGETACTIVESUBROUTINEUNIFORMNAMEPROC glGetActiveSubroutineUniformName;
        public static PFNGLGETACTIVESUBROUTINENAMEPROC glGetActiveSubroutineName;
        public static PFNGLUNIFORMSUBROUTINESUIVPROC glUniformSubroutinesuiv;
        public static PFNGLGETUNIFORMSUBROUTINEUIVPROC glGetUniformSubroutineuiv;
        public static PFNGLGETPROGRAMSTAGEIVPROC glGetProgramStageiv;
        public static PFNGLPATCHPARAMETERIPROC glPatchParameteri;
        public static PFNGLPATCHPARAMETERFVPROC glPatchParameterfv;
        public static PFNGLBINDTRANSFORMFEEDBACKPROC glBindTransformFeedback;
        public static PFNGLDELETETRANSFORMFEEDBACKSPROC glDeleteTransformFeedbacks;
        public static PFNGLGENTRANSFORMFEEDBACKSPROC glGenTransformFeedbacks;
        public static PFNGLISTRANSFORMFEEDBACKPROC glIsTransformFeedback;
        public static PFNGLPAUSETRANSFORMFEEDBACKPROC glPauseTransformFeedback;
        public static PFNGLRESUMETRANSFORMFEEDBACKPROC glResumeTransformFeedback;
        public static PFNGLDRAWTRANSFORMFEEDBACKPROC glDrawTransformFeedback;
        public static PFNGLDRAWTRANSFORMFEEDBACKSTREAMPROC glDrawTransformFeedbackStream;
        public static PFNGLBEGINQUERYINDEXEDPROC glBeginQueryIndexed;
        public static PFNGLENDQUERYINDEXEDPROC glEndQueryIndexed;
        public static PFNGLGETQUERYINDEXEDIVPROC glGetQueryIndexediv;
        #endregion
        #endregion

        #region OpenGL 4.1
        #region Constants
        public const uint GL_FIXED = 5132;
        public const uint GL_IMPLEMENTATION_COLOR_READ_TYPE = 35738;
        public const uint GL_IMPLEMENTATION_COLOR_READ_FORMAT = 35739;
        public const uint GL_LOW_FLOAT = 36336;
        public const uint GL_MEDIUM_FLOAT = 36337;
        public const uint GL_HIGH_FLOAT = 36338;
        public const uint GL_LOW_INT = 36339;
        public const uint GL_MEDIUM_INT = 36340;
        public const uint GL_HIGH_INT = 36341;
        public const uint GL_SHADER_COMPILER = 36346;
        public const uint GL_SHADER_BINARY_FORMATS = 36344;
        public const uint GL_NUM_SHADER_BINARY_FORMATS = 36345;
        public const uint GL_MAX_VERTEX_UNIFORM_VECTORS = 36347;
        public const uint GL_MAX_VARYING_VECTORS = 36348;
        public const uint GL_MAX_FRAGMENT_UNIFORM_VECTORS = 36349;
        public const uint GL_RGB565 = 36194;
        public const uint GL_PROGRAM_BINARY_RETRIEVABLE_HINT = 33367;
        public const uint GL_PROGRAM_BINARY_LENGTH = 34625;
        public const uint GL_NUM_PROGRAM_BINARY_FORMATS = 34814;
        public const uint GL_PROGRAM_BINARY_FORMATS = 34815;
        public const int GL_VERTEX_SHADER_BIT = 1;
        public const int GL_FRAGMENT_SHADER_BIT = 2;
        public const int GL_GEOMETRY_SHADER_BIT = 4;
        public const int GL_TESS_CONTROL_SHADER_BIT = 8;
        public const int GL_TESS_EVALUATION_SHADER_BIT = 16;
        public const int GL_ALL_SHADER_BITS = -1;
        public const uint GL_PROGRAM_SEPARABLE = 33368;
        public const uint GL_ACTIVE_PROGRAM = 33369;
        public const uint GL_PROGRAM_PIPELINE_BINDING = 33370;
        public const uint GL_MAX_VIEWPORTS = 33371;
        public const uint GL_VIEWPORT_SUBPIXEL_BITS = 33372;
        public const uint GL_VIEWPORT_BOUNDS_RANGE = 33373;
        public const uint GL_LAYER_PROVOKING_VERTEX = 33374;
        public const uint GL_VIEWPORT_INDEX_PROVOKING_VERTEX = 33375;
        public const uint GL_UNDEFINED_VERTEX = 33376;
        #endregion

        #region Delegates
        public delegate void PFNGLRELEASESHADERCOMPILERPROC();
        public delegate void PFNGLSHADERBINARYPROC( int count, ref uint shaders, uint binaryformat, IntPtr binary, int length );
        public delegate void PFNGLGETSHADERPRECISIONFORMATPROC( uint shadertype, uint precisiontype, ref int range, ref int precision );
        public delegate void PFNGLDEPTHRANGEFPROC( float n, float f );
        public delegate void PFNGLCLEARDEPTHFPROC( float d );
        public delegate void PFNGLGETPROGRAMBINARYPROC( uint program, int bufSize, ref int length, ref uint binaryFormat, IntPtr binary );
        public delegate void PFNGLPROGRAMBINARYPROC( uint program, uint binaryFormat, IntPtr binary, int length );
        public delegate void PFNGLPROGRAMPARAMETERIPROC( uint program, uint pname, int value );
        public delegate void PFNGLUSEPROGRAMSTAGESPROC( uint pipeline, uint stages, uint program );
        public delegate void PFNGLACTIVESHADERPROGRAMPROC( uint pipeline, uint program );
        public delegate uint PFNGLCREATESHADERPROGRAMVPROC( uint type, int count, IntPtr strings );
        public delegate void PFNGLBINDPROGRAMPIPELINEPROC( uint pipeline );
        public delegate void PFNGLDELETEPROGRAMPIPELINESPROC( int n, ref uint pipelines );
        public delegate void PFNGLGENPROGRAMPIPELINESPROC( int n, ref uint pipelines );
        public delegate byte PFNGLISPROGRAMPIPELINEPROC( uint pipeline );
        public delegate void PFNGLGETPROGRAMPIPELINEIVPROC( uint pipeline, uint pname, ref int parameters );
        public delegate void PFNGLPROGRAMUNIFORM1IPROC( uint program, int location, int v0 );
        public delegate void PFNGLPROGRAMUNIFORM1IVPROC( uint program, int location, int count, ref int value );
        public delegate void PFNGLPROGRAMUNIFORM1FPROC( uint program, int location, float v0 );
        public delegate void PFNGLPROGRAMUNIFORM1FVPROC( uint program, int location, int count, ref float value );
        public delegate void PFNGLPROGRAMUNIFORM1DPROC( uint program, int location, double v0 );
        public delegate void PFNGLPROGRAMUNIFORM1DVPROC( uint program, int location, int count, ref double value );
        public delegate void PFNGLPROGRAMUNIFORM1UIPROC( uint program, int location, uint v0 );
        public delegate void PFNGLPROGRAMUNIFORM1UIVPROC( uint program, int location, int count, ref uint value );
        public delegate void PFNGLPROGRAMUNIFORM2IPROC( uint program, int location, int v0, int v1 );
        public delegate void PFNGLPROGRAMUNIFORM2IVPROC( uint program, int location, int count, ref int value );
        public delegate void PFNGLPROGRAMUNIFORM2FPROC( uint program, int location, float v0, float v1 );
        public delegate void PFNGLPROGRAMUNIFORM2FVPROC( uint program, int location, int count, ref float value );
        public delegate void PFNGLPROGRAMUNIFORM2DPROC( uint program, int location, double v0, double v1 );
        public delegate void PFNGLPROGRAMUNIFORM2DVPROC( uint program, int location, int count, ref double value );
        public delegate void PFNGLPROGRAMUNIFORM2UIPROC( uint program, int location, uint v0, uint v1 );
        public delegate void PFNGLPROGRAMUNIFORM2UIVPROC( uint program, int location, int count, ref uint value );
        public delegate void PFNGLPROGRAMUNIFORM3IPROC( uint program, int location, int v0, int v1, int v2 );
        public delegate void PFNGLPROGRAMUNIFORM3IVPROC( uint program, int location, int count, ref int value );
        public delegate void PFNGLPROGRAMUNIFORM3FPROC( uint program, int location, float v0, float v1, float v2 );
        public delegate void PFNGLPROGRAMUNIFORM3FVPROC( uint program, int location, int count, ref float value );
        public delegate void PFNGLPROGRAMUNIFORM3DPROC( uint program, int location, double v0, double v1, double v2 );
        public delegate void PFNGLPROGRAMUNIFORM3DVPROC( uint program, int location, int count, ref double value );
        public delegate void PFNGLPROGRAMUNIFORM3UIPROC( uint program, int location, uint v0, uint v1, uint v2 );
        public delegate void PFNGLPROGRAMUNIFORM3UIVPROC( uint program, int location, int count, ref uint value );
        public delegate void PFNGLPROGRAMUNIFORM4IPROC( uint program, int location, int v0, int v1, int v2, int v3 );
        public delegate void PFNGLPROGRAMUNIFORM4IVPROC( uint program, int location, int count, ref int value );
        public delegate void PFNGLPROGRAMUNIFORM4FPROC( uint program, int location, float v0, float v1, float v2, float v3 );
        public delegate void PFNGLPROGRAMUNIFORM4FVPROC( uint program, int location, int count, ref float value );
        public delegate void PFNGLPROGRAMUNIFORM4DPROC( uint program, int location, double v0, double v1, double v2, double v3 );
        public delegate void PFNGLPROGRAMUNIFORM4DVPROC( uint program, int location, int count, ref double value );
        public delegate void PFNGLPROGRAMUNIFORM4UIPROC( uint program, int location, uint v0, uint v1, uint v2, uint v3 );
        public delegate void PFNGLPROGRAMUNIFORM4UIVPROC( uint program, int location, int count, ref uint value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX2FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX3FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX4FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX2DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX3DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX4DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX2X3FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX3X2FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX2X4FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX4X2FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX3X4FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX4X3FVPROC( uint program, int location, int count, byte transpose, ref float value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX2X3DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX3X2DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX2X4DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX4X2DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX3X4DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLPROGRAMUNIFORMMATRIX4X3DVPROC( uint program, int location, int count, byte transpose, ref double value );
        public delegate void PFNGLVALIDATEPROGRAMPIPELINEPROC( uint pipeline );
        public delegate void PFNGLGETPROGRAMPIPELINEINFOLOGPROC( uint pipeline, int bufSize, ref int length, IntPtr infoLog );
        public delegate void PFNGLVERTEXATTRIBL1DPROC( uint index, double x );
        public delegate void PFNGLVERTEXATTRIBL2DPROC( uint index, double x, double y );
        public delegate void PFNGLVERTEXATTRIBL3DPROC( uint index, double x, double y, double z );
        public delegate void PFNGLVERTEXATTRIBL4DPROC( uint index, double x, double y, double z, double w );
        public delegate void PFNGLVERTEXATTRIBL1DVPROC( uint index, ref double v );
        public delegate void PFNGLVERTEXATTRIBL2DVPROC( uint index, ref double v );
        public delegate void PFNGLVERTEXATTRIBL3DVPROC( uint index, ref double v );
        public delegate void PFNGLVERTEXATTRIBL4DVPROC( uint index, ref double v );
        public delegate void PFNGLVERTEXATTRIBLPOINTERPROC( uint index, int size, uint type, int stride, IntPtr pointer );
        public delegate void PFNGLGETVERTEXATTRIBLDVPROC( uint index, uint pname, ref double parameters );
        public delegate void PFNGLVIEWPORTARRAYVPROC( uint first, int count, ref float v );
        public delegate void PFNGLVIEWPORTINDEXEDFPROC( uint index, float x, float y, float w, float h );
        public delegate void PFNGLVIEWPORTINDEXEDFVPROC( uint index, ref float v );
        public delegate void PFNGLSCISSORARRAYVPROC( uint first, int count, ref int v );
        public delegate void PFNGLSCISSORINDEXEDPROC( uint index, int left, int bottom, int width, int height );
        public delegate void PFNGLSCISSORINDEXEDVPROC( uint index, ref int v );
        public delegate void PFNGLDEPTHRANGEARRAYVPROC( uint first, int count, ref double v );
        public delegate void PFNGLDEPTHRANGEINDEXEDPROC( uint index, double n, double f );
        public delegate void PFNGLGETFLOATI_VPROC( uint target, uint index, ref float data );
        public delegate void PFNGLGETDOUBLEI_VPROC( uint target, uint index, ref double data );
        #endregion

        #region Methods
        public static PFNGLRELEASESHADERCOMPILERPROC glReleaseShaderCompiler;
        public static PFNGLSHADERBINARYPROC glShaderBinary;
        public static PFNGLGETSHADERPRECISIONFORMATPROC glGetShaderPrecisionFormat;
        public static PFNGLDEPTHRANGEFPROC glDepthRangef;
        public static PFNGLCLEARDEPTHFPROC glClearDepthf;
        public static PFNGLGETPROGRAMBINARYPROC glGetProgramBinary;
        public static PFNGLPROGRAMBINARYPROC glProgramBinary;
        public static PFNGLPROGRAMPARAMETERIPROC glProgramParameteri;
        public static PFNGLUSEPROGRAMSTAGESPROC glUseProgramStages;
        public static PFNGLACTIVESHADERPROGRAMPROC glActiveShaderProgram;
        public static PFNGLCREATESHADERPROGRAMVPROC glCreateShaderProgramv;
        public static PFNGLBINDPROGRAMPIPELINEPROC glBindProgramPipeline;
        public static PFNGLDELETEPROGRAMPIPELINESPROC glDeleteProgramPipelines;
        public static PFNGLGENPROGRAMPIPELINESPROC glGenProgramPipelines;
        public static PFNGLISPROGRAMPIPELINEPROC glIsProgramPipeline;
        public static PFNGLGETPROGRAMPIPELINEIVPROC glGetProgramPipelineiv;
        public static PFNGLPROGRAMUNIFORM1IPROC glProgramUniform1i;
        public static PFNGLPROGRAMUNIFORM1IVPROC glProgramUniform1iv;
        public static PFNGLPROGRAMUNIFORM1FPROC glProgramUniform1f;
        public static PFNGLPROGRAMUNIFORM1FVPROC glProgramUniform1fv;
        public static PFNGLPROGRAMUNIFORM1DPROC glProgramUniform1d;
        public static PFNGLPROGRAMUNIFORM1DVPROC glProgramUniform1dv;
        public static PFNGLPROGRAMUNIFORM1UIPROC glProgramUniform1ui;
        public static PFNGLPROGRAMUNIFORM1UIVPROC glProgramUniform1uiv;
        public static PFNGLPROGRAMUNIFORM2IPROC glProgramUniform2i;
        public static PFNGLPROGRAMUNIFORM2IVPROC glProgramUniform2iv;
        public static PFNGLPROGRAMUNIFORM2FPROC glProgramUniform2f;
        public static PFNGLPROGRAMUNIFORM2FVPROC glProgramUniform2fv;
        public static PFNGLPROGRAMUNIFORM2DPROC glProgramUniform2d;
        public static PFNGLPROGRAMUNIFORM2DVPROC glProgramUniform2dv;
        public static PFNGLPROGRAMUNIFORM2UIPROC glProgramUniform2ui;
        public static PFNGLPROGRAMUNIFORM2UIVPROC glProgramUniform2uiv;
        public static PFNGLPROGRAMUNIFORM3IPROC glProgramUniform3i;
        public static PFNGLPROGRAMUNIFORM3IVPROC glProgramUniform3iv;
        public static PFNGLPROGRAMUNIFORM3FPROC glProgramUniform3f;
        public static PFNGLPROGRAMUNIFORM3FVPROC glProgramUniform3fv;
        public static PFNGLPROGRAMUNIFORM3DPROC glProgramUniform3d;
        public static PFNGLPROGRAMUNIFORM3DVPROC glProgramUniform3dv;
        public static PFNGLPROGRAMUNIFORM3UIPROC glProgramUniform3ui;
        public static PFNGLPROGRAMUNIFORM3UIVPROC glProgramUniform3uiv;
        public static PFNGLPROGRAMUNIFORM4IPROC glProgramUniform4i;
        public static PFNGLPROGRAMUNIFORM4IVPROC glProgramUniform4iv;
        public static PFNGLPROGRAMUNIFORM4FPROC glProgramUniform4f;
        public static PFNGLPROGRAMUNIFORM4FVPROC glProgramUniform4fv;
        public static PFNGLPROGRAMUNIFORM4DPROC glProgramUniform4d;
        public static PFNGLPROGRAMUNIFORM4DVPROC glProgramUniform4dv;
        public static PFNGLPROGRAMUNIFORM4UIPROC glProgramUniform4ui;
        public static PFNGLPROGRAMUNIFORM4UIVPROC glProgramUniform4uiv;
        public static PFNGLPROGRAMUNIFORMMATRIX2FVPROC glProgramUniformMatrix2fv;
        public static PFNGLPROGRAMUNIFORMMATRIX3FVPROC glProgramUniformMatrix3fv;
        public static PFNGLPROGRAMUNIFORMMATRIX4FVPROC glProgramUniformMatrix4fv;
        public static PFNGLPROGRAMUNIFORMMATRIX2DVPROC glProgramUniformMatrix2dv;
        public static PFNGLPROGRAMUNIFORMMATRIX3DVPROC glProgramUniformMatrix3dv;
        public static PFNGLPROGRAMUNIFORMMATRIX4DVPROC glProgramUniformMatrix4dv;
        public static PFNGLPROGRAMUNIFORMMATRIX2X3FVPROC glProgramUniformMatrix2x3fv;
        public static PFNGLPROGRAMUNIFORMMATRIX3X2FVPROC glProgramUniformMatrix3x2fv;
        public static PFNGLPROGRAMUNIFORMMATRIX2X4FVPROC glProgramUniformMatrix2x4fv;
        public static PFNGLPROGRAMUNIFORMMATRIX4X2FVPROC glProgramUniformMatrix4x2fv;
        public static PFNGLPROGRAMUNIFORMMATRIX3X4FVPROC glProgramUniformMatrix3x4fv;
        public static PFNGLPROGRAMUNIFORMMATRIX4X3FVPROC glProgramUniformMatrix4x3fv;
        public static PFNGLPROGRAMUNIFORMMATRIX2X3DVPROC glProgramUniformMatrix2x3dv;
        public static PFNGLPROGRAMUNIFORMMATRIX3X2DVPROC glProgramUniformMatrix3x2dv;
        public static PFNGLPROGRAMUNIFORMMATRIX2X4DVPROC glProgramUniformMatrix2x4dv;
        public static PFNGLPROGRAMUNIFORMMATRIX4X2DVPROC glProgramUniformMatrix4x2dv;
        public static PFNGLPROGRAMUNIFORMMATRIX3X4DVPROC glProgramUniformMatrix3x4dv;
        public static PFNGLPROGRAMUNIFORMMATRIX4X3DVPROC glProgramUniformMatrix4x3dv;
        public static PFNGLVALIDATEPROGRAMPIPELINEPROC glValidateProgramPipeline;
        public static PFNGLGETPROGRAMPIPELINEINFOLOGPROC glGetProgramPipelineInfoLog;
        public static PFNGLVERTEXATTRIBL1DPROC glVertexAttribL1d;
        public static PFNGLVERTEXATTRIBL2DPROC glVertexAttribL2d;
        public static PFNGLVERTEXATTRIBL3DPROC glVertexAttribL3d;
        public static PFNGLVERTEXATTRIBL4DPROC glVertexAttribL4d;
        public static PFNGLVERTEXATTRIBL1DVPROC glVertexAttribL1dv;
        public static PFNGLVERTEXATTRIBL2DVPROC glVertexAttribL2dv;
        public static PFNGLVERTEXATTRIBL3DVPROC glVertexAttribL3dv;
        public static PFNGLVERTEXATTRIBL4DVPROC glVertexAttribL4dv;
        public static PFNGLVERTEXATTRIBLPOINTERPROC glVertexAttribLPointer;
        public static PFNGLGETVERTEXATTRIBLDVPROC glGetVertexAttribLdv;
        public static PFNGLVIEWPORTARRAYVPROC glViewportArrayv;
        public static PFNGLVIEWPORTINDEXEDFPROC glViewportIndexedf;
        public static PFNGLVIEWPORTINDEXEDFVPROC glViewportIndexedfv;
        public static PFNGLSCISSORARRAYVPROC glScissorArrayv;
        public static PFNGLSCISSORINDEXEDPROC glScissorIndexed;
        public static PFNGLSCISSORINDEXEDVPROC glScissorIndexedv;
        public static PFNGLDEPTHRANGEARRAYVPROC glDepthRangeArrayv;
        public static PFNGLDEPTHRANGEINDEXEDPROC glDepthRangeIndexed;
        public static PFNGLGETFLOATI_VPROC glGetFloati_v;
        public static PFNGLGETDOUBLEI_VPROC glGetDoublei_v;
        #endregion
        #endregion

        #region OpenGL 4.2
        #region Constants
        public const uint GL_COPY_READ_BUFFER_BINDING = 36662;
        public const uint GL_COPY_WRITE_BUFFER_BINDING = 36663;
        public const uint GL_TRANSFORM_FEEDBACK_ACTIVE = 36388;
        public const uint GL_TRANSFORM_FEEDBACK_PAUSED = 36387;
        public const uint GL_UNPACK_COMPRESSED_BLOCK_WIDTH = 37159;
        public const uint GL_UNPACK_COMPRESSED_BLOCK_HEIGHT = 37160;
        public const uint GL_UNPACK_COMPRESSED_BLOCK_DEPTH = 37161;
        public const uint GL_UNPACK_COMPRESSED_BLOCK_SIZE = 37162;
        public const uint GL_PACK_COMPRESSED_BLOCK_WIDTH = 37163;
        public const uint GL_PACK_COMPRESSED_BLOCK_HEIGHT = 37164;
        public const uint GL_PACK_COMPRESSED_BLOCK_DEPTH = 37165;
        public const uint GL_PACK_COMPRESSED_BLOCK_SIZE = 37166;
        public const uint GL_NUM_SAMPLE_COUNTS = 37760;
        public const uint GL_MIN_MAP_BUFFER_ALIGNMENT = 37052;
        public const uint GL_ATOMIC_COUNTER_BUFFER = 37568;
        public const uint GL_ATOMIC_COUNTER_BUFFER_BINDING = 37569;
        public const uint GL_ATOMIC_COUNTER_BUFFER_START = 37570;
        public const uint GL_ATOMIC_COUNTER_BUFFER_SIZE = 37571;
        public const uint GL_ATOMIC_COUNTER_BUFFER_DATA_SIZE = 37572;
        public const uint GL_ATOMIC_COUNTER_BUFFER_ACTIVE_ATOMIC_COUNTERS = 37573;
        public const uint GL_ATOMIC_COUNTER_BUFFER_ACTIVE_ATOMIC_COUNTER_INDICES = 37574;
        public const uint GL_ATOMIC_COUNTER_BUFFER_REFERENCED_BY_VERTEX_SHADER = 37575;
        public const uint GL_ATOMIC_COUNTER_BUFFER_REFERENCED_BY_TESS_CONTROL_SHADER = 37576;
        public const uint GL_ATOMIC_COUNTER_BUFFER_REFERENCED_BY_TESS_EVALUATION_SHADER = 37577;
        public const uint GL_ATOMIC_COUNTER_BUFFER_REFERENCED_BY_GEOMETRY_SHADER = 37578;
        public const uint GL_ATOMIC_COUNTER_BUFFER_REFERENCED_BY_FRAGMENT_SHADER = 37579;
        public const uint GL_MAX_VERTEX_ATOMIC_COUNTER_BUFFERS = 37580;
        public const uint GL_MAX_TESS_CONTROL_ATOMIC_COUNTER_BUFFERS = 37581;
        public const uint GL_MAX_TESS_EVALUATION_ATOMIC_COUNTER_BUFFERS = 37582;
        public const uint GL_MAX_GEOMETRY_ATOMIC_COUNTER_BUFFERS = 37583;
        public const uint GL_MAX_FRAGMENT_ATOMIC_COUNTER_BUFFERS = 37584;
        public const uint GL_MAX_COMBINED_ATOMIC_COUNTER_BUFFERS = 37585;
        public const uint GL_MAX_VERTEX_ATOMIC_COUNTERS = 37586;
        public const uint GL_MAX_TESS_CONTROL_ATOMIC_COUNTERS = 37587;
        public const uint GL_MAX_TESS_EVALUATION_ATOMIC_COUNTERS = 37588;
        public const uint GL_MAX_GEOMETRY_ATOMIC_COUNTERS = 37589;
        public const uint GL_MAX_FRAGMENT_ATOMIC_COUNTERS = 37590;
        public const uint GL_MAX_COMBINED_ATOMIC_COUNTERS = 37591;
        public const uint GL_MAX_ATOMIC_COUNTER_BUFFER_SIZE = 37592;
        public const uint GL_MAX_ATOMIC_COUNTER_BUFFER_BINDINGS = 37596;
        public const uint GL_ACTIVE_ATOMIC_COUNTER_BUFFERS = 37593;
        public const uint GL_UNIFORM_ATOMIC_COUNTER_BUFFER_INDEX = 37594;
        public const uint GL_UNSIGNED_INT_ATOMIC_COUNTER = 37595;
        public const int GL_VERTEX_ATTRIB_ARRAY_BARRIER_BIT = 1;
        public const int GL_ELEMENT_ARRAY_BARRIER_BIT = 2;
        public const int GL_UNIFORM_BARRIER_BIT = 4;
        public const int GL_TEXTURE_FETCH_BARRIER_BIT = 8;
        public const int GL_SHADER_IMAGE_ACCESS_BARRIER_BIT = 32;
        public const int GL_COMMAND_BARRIER_BIT = 64;
        public const int GL_PIXEL_BUFFER_BARRIER_BIT = 128;
        public const int GL_TEXTURE_UPDATE_BARRIER_BIT = 256;
        public const int GL_BUFFER_UPDATE_BARRIER_BIT = 512;
        public const int GL_FRAMEBUFFER_BARRIER_BIT = 1024;
        public const int GL_TRANSFORM_FEEDBACK_BARRIER_BIT = 2048;
        public const int GL_ATOMIC_COUNTER_BARRIER_BIT = 4096;
        public const int GL_ALL_BARRIER_BITS = -1;
        public const uint GL_MAX_IMAGE_UNITS = 36664;
        public const uint GL_MAX_COMBINED_IMAGE_UNITS_AND_FRAGMENT_OUTPUTS = 36665;
        public const uint GL_IMAGE_BINDING_NAME = 36666;
        public const uint GL_IMAGE_BINDING_LEVEL = 36667;
        public const uint GL_IMAGE_BINDING_LAYERED = 36668;
        public const uint GL_IMAGE_BINDING_LAYER = 36669;
        public const uint GL_IMAGE_BINDING_ACCESS = 36670;
        public const uint GL_IMAGE_1D = 36940;
        public const uint GL_IMAGE_2D = 36941;
        public const uint GL_IMAGE_3D = 36942;
        public const uint GL_IMAGE_2D_RECT = 36943;
        public const uint GL_IMAGE_CUBE = 36944;
        public const uint GL_IMAGE_BUFFER = 36945;
        public const uint GL_IMAGE_1D_ARRAY = 36946;
        public const uint GL_IMAGE_2D_ARRAY = 36947;
        public const uint GL_IMAGE_CUBE_MAP_ARRAY = 36948;
        public const uint GL_IMAGE_2D_MULTISAMPLE = 36949;
        public const uint GL_IMAGE_2D_MULTISAMPLE_ARRAY = 36950;
        public const uint GL_INT_IMAGE_1D = 36951;
        public const uint GL_INT_IMAGE_2D = 36952;
        public const uint GL_INT_IMAGE_3D = 36953;
        public const uint GL_INT_IMAGE_2D_RECT = 36954;
        public const uint GL_INT_IMAGE_CUBE = 36955;
        public const uint GL_INT_IMAGE_BUFFER = 36956;
        public const uint GL_INT_IMAGE_1D_ARRAY = 36957;
        public const uint GL_INT_IMAGE_2D_ARRAY = 36958;
        public const uint GL_INT_IMAGE_CUBE_MAP_ARRAY = 36959;
        public const uint GL_INT_IMAGE_2D_MULTISAMPLE = 36960;
        public const uint GL_INT_IMAGE_2D_MULTISAMPLE_ARRAY = 36961;
        public const uint GL_UNSIGNED_INT_IMAGE_1D = 36962;
        public const uint GL_UNSIGNED_INT_IMAGE_2D = 36963;
        public const uint GL_UNSIGNED_INT_IMAGE_3D = 36964;
        public const uint GL_UNSIGNED_INT_IMAGE_2D_RECT = 36965;
        public const uint GL_UNSIGNED_INT_IMAGE_CUBE = 36966;
        public const uint GL_UNSIGNED_INT_IMAGE_BUFFER = 36967;
        public const uint GL_UNSIGNED_INT_IMAGE_1D_ARRAY = 36968;
        public const uint GL_UNSIGNED_INT_IMAGE_2D_ARRAY = 36969;
        public const uint GL_UNSIGNED_INT_IMAGE_CUBE_MAP_ARRAY = 36970;
        public const uint GL_UNSIGNED_INT_IMAGE_2D_MULTISAMPLE = 36971;
        public const uint GL_UNSIGNED_INT_IMAGE_2D_MULTISAMPLE_ARRAY = 36972;
        public const uint GL_MAX_IMAGE_SAMPLES = 36973;
        public const uint GL_IMAGE_BINDING_FORMAT = 36974;
        public const uint GL_IMAGE_FORMAT_COMPATIBILITY_TYPE = 37063;
        public const uint GL_IMAGE_FORMAT_COMPATIBILITY_BY_SIZE = 37064;
        public const uint GL_IMAGE_FORMAT_COMPATIBILITY_BY_CLASS = 37065;
        public const uint GL_MAX_VERTEX_IMAGE_UNIFORMS = 37066;
        public const uint GL_MAX_TESS_CONTROL_IMAGE_UNIFORMS = 37067;
        public const uint GL_MAX_TESS_EVALUATION_IMAGE_UNIFORMS = 37068;
        public const uint GL_MAX_GEOMETRY_IMAGE_UNIFORMS = 37069;
        public const uint GL_MAX_FRAGMENT_IMAGE_UNIFORMS = 37070;
        public const uint GL_MAX_COMBINED_IMAGE_UNIFORMS = 37071;
        public const uint GL_COMPRESSED_RGBA_BPTC_UNORM = 36492;
        public const uint GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM = 36493;
        public const uint GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT = 36494;
        public const uint GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT = 36495;
        public const uint GL_TEXTURE_IMMUTABLE_FORMAT = 37167;
        #endregion

        #region Delegates
        public delegate void PFNGLDRAWARRAYSINSTANCEDBASEINSTANCEPROC( uint mode, int first, int count, int instancecount, uint baseinstance );
        public delegate void PFNGLDRAWELEMENTSINSTANCEDBASEINSTANCEPROC( uint mode, int count, uint type, IntPtr indices, int instancecount, uint baseinstance );
        public delegate void PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXBASEINSTANCEPROC( uint mode, int count, uint type, IntPtr indices, int instancecount, int basevertex, uint baseinstance );
        public delegate void PFNGLGETINTERNALFORMATIVPROC( uint target, uint internalformat, uint pname, int bufSize, ref int parameters );
        public delegate void PFNGLGETACTIVEATOMICCOUNTERBUFFERIVPROC( uint program, uint bufferIndex, uint pname, ref int parameters );
        public delegate void PFNGLBINDIMAGETEXTUREPROC( uint unit, uint texture, int level, byte layered, int layer, uint access, uint format );
        public delegate void PFNGLMEMORYBARRIERPROC( uint barriers );
        public delegate void PFNGLTEXSTORAGE1DPROC( uint target, int levels, uint internalformat, int width );
        public delegate void PFNGLTEXSTORAGE2DPROC( uint target, int levels, uint internalformat, int width, int height );
        public delegate void PFNGLTEXSTORAGE3DPROC( uint target, int levels, uint internalformat, int width, int height, int depth );
        public delegate void PFNGLDRAWTRANSFORMFEEDBACKINSTANCEDPROC( uint mode, uint id, int instancecount );
        public delegate void PFNGLDRAWTRANSFORMFEEDBACKSTREAMINSTANCEDPROC( uint mode, uint id, uint stream, int instancecount );
        #endregion

        #region Methods
        public static PFNGLDRAWARRAYSINSTANCEDBASEINSTANCEPROC glDrawArraysInstancedBaseInstance;
        public static PFNGLDRAWELEMENTSINSTANCEDBASEINSTANCEPROC glDrawElementsInstancedBaseInstance;
        public static PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXBASEINSTANCEPROC glDrawElementsInstancedBaseVertexBaseInstance;
        public static PFNGLGETINTERNALFORMATIVPROC glGetInternalformativ;
        public static PFNGLGETACTIVEATOMICCOUNTERBUFFERIVPROC glGetActiveAtomicCounterBufferiv;
        public static PFNGLBINDIMAGETEXTUREPROC glBindImageTexture;
        public static PFNGLMEMORYBARRIERPROC glMemoryBarrier;
        public static PFNGLTEXSTORAGE1DPROC glTexStorage1D;
        public static PFNGLTEXSTORAGE2DPROC glTexStorage2D;
        public static PFNGLTEXSTORAGE3DPROC glTexStorage3D;
        public static PFNGLDRAWTRANSFORMFEEDBACKINSTANCEDPROC glDrawTransformFeedbackInstanced;
        public static PFNGLDRAWTRANSFORMFEEDBACKSTREAMINSTANCEDPROC glDrawTransformFeedbackStreamInstanced;
        #endregion
        #endregion

        #region OpenGL 4.3
        #region Constants
        public const uint GL_NUM_SHADING_LANGUAGE_VERSIONS = 33513;
        public const uint GL_VERTEX_ATTRIB_ARRAY_LONG = 34638;
        public const uint GL_COMPRESSED_RGB8_ETC2 = 37492;
        public const uint GL_COMPRESSED_SRGB8_ETC2 = 37493;
        public const uint GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 37494;
        public const uint GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 37495;
        public const uint GL_COMPRESSED_RGBA8_ETC2_EAC = 37496;
        public const uint GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 37497;
        public const uint GL_COMPRESSED_R11_EAC = 37488;
        public const uint GL_COMPRESSED_SIGNED_R11_EAC = 37489;
        public const uint GL_COMPRESSED_RG11_EAC = 37490;
        public const uint GL_COMPRESSED_SIGNED_RG11_EAC = 37491;
        public const uint GL_PRIMITIVE_RESTART_FIXED_INDEX = 36201;
        public const uint GL_ANY_SAMPLES_PASSED_CONSERVATIVE = 36202;
        public const uint GL_MAX_ELEMENT_INDEX = 36203;
        public const uint GL_COMPUTE_SHADER = 37305;
        public const uint GL_MAX_COMPUTE_UNIFORM_BLOCKS = 37307;
        public const uint GL_MAX_COMPUTE_TEXTURE_IMAGE_UNITS = 37308;
        public const uint GL_MAX_COMPUTE_IMAGE_UNIFORMS = 37309;
        public const uint GL_MAX_COMPUTE_SHARED_MEMORY_SIZE = 33378;
        public const uint GL_MAX_COMPUTE_UNIFORM_COMPONENTS = 33379;
        public const uint GL_MAX_COMPUTE_ATOMIC_COUNTER_BUFFERS = 33380;
        public const uint GL_MAX_COMPUTE_ATOMIC_COUNTERS = 33381;
        public const uint GL_MAX_COMBINED_COMPUTE_UNIFORM_COMPONENTS = 33382;
        public const uint GL_MAX_COMPUTE_WORK_GROUP_INVOCATIONS = 37099;
        public const uint GL_MAX_COMPUTE_WORK_GROUP_COUNT = 37310;
        public const uint GL_MAX_COMPUTE_WORK_GROUP_SIZE = 37311;
        public const uint GL_COMPUTE_WORK_GROUP_SIZE = 33383;
        public const uint GL_UNIFORM_BLOCK_REFERENCED_BY_COMPUTE_SHADER = 37100;
        public const uint GL_ATOMIC_COUNTER_BUFFER_REFERENCED_BY_COMPUTE_SHADER = 37101;
        public const uint GL_DISPATCH_INDIRECT_BUFFER = 37102;
        public const uint GL_DISPATCH_INDIRECT_BUFFER_BINDING = 37103;
        public const uint GL_COMPUTE_SHADER_BIT = 32;
        public const uint GL_DEBUG_OUTPUT_SYNCHRONOUS = 33346;
        public const uint GL_DEBUG_NEXT_LOGGED_MESSAGE_LENGTH = 33347;
        public const uint GL_DEBUG_CALLBACK_FUNCTION = 33348;
        public const uint GL_DEBUG_CALLBACK_USER_PARAM = 33349;
        public const uint GL_DEBUG_SOURCE_API = 33350;
        public const uint GL_DEBUG_SOURCE_WINDOW_SYSTEM = 33351;
        public const uint GL_DEBUG_SOURCE_SHADER_COMPILER = 33352;
        public const uint GL_DEBUG_SOURCE_THIRD_PARTY = 33353;
        public const uint GL_DEBUG_SOURCE_APPLICATION = 33354;
        public const uint GL_DEBUG_SOURCE_OTHER = 33355;
        public const uint GL_DEBUG_TYPE_ERROR = 33356;
        public const uint GL_DEBUG_TYPE_DEPRECATED_BEHAVIOR = 33357;
        public const uint GL_DEBUG_TYPE_UNDEFINED_BEHAVIOR = 33358;
        public const uint GL_DEBUG_TYPE_PORTABILITY = 33359;
        public const uint GL_DEBUG_TYPE_PERFORMANCE = 33360;
        public const uint GL_DEBUG_TYPE_OTHER = 33361;
        public const uint GL_MAX_DEBUG_MESSAGE_LENGTH = 37187;
        public const uint GL_MAX_DEBUG_LOGGED_MESSAGES = 37188;
        public const uint GL_DEBUG_LOGGED_MESSAGES = 37189;
        public const uint GL_DEBUG_SEVERITY_HIGH = 37190;
        public const uint GL_DEBUG_SEVERITY_MEDIUM = 37191;
        public const uint GL_DEBUG_SEVERITY_LOW = 37192;
        public const uint GL_DEBUG_TYPE_MARKER = 33384;
        public const uint GL_DEBUG_TYPE_PUSH_GROUP = 33385;
        public const uint GL_DEBUG_TYPE_POP_GROUP = 33386;
        public const uint GL_DEBUG_SEVERITY_NOTIFICATION = 33387;
        public const uint GL_MAX_DEBUG_GROUP_STACK_DEPTH = 33388;
        public const uint GL_DEBUG_GROUP_STACK_DEPTH = 33389;
        public const uint GL_BUFFER = 33504;
        public const uint GL_SHADER = 33505;
        public const uint GL_PROGRAM = 33506;
        public const uint GL_QUERY = 33507;
        public const uint GL_PROGRAM_PIPELINE = 33508;
        public const uint GL_SAMPLER = 33510;
        public const uint GL_MAX_LABEL_LENGTH = 33512;
        public const uint GL_DEBUG_OUTPUT = 37600;
        public const int GL_CONTEXT_FLAG_DEBUG_BIT = 2;
        public const uint GL_MAX_UNIFORM_LOCATIONS = 33390;
        public const uint GL_FRAMEBUFFER_DEFAULT_WIDTH = 37648;
        public const uint GL_FRAMEBUFFER_DEFAULT_HEIGHT = 37649;
        public const uint GL_FRAMEBUFFER_DEFAULT_LAYERS = 37650;
        public const uint GL_FRAMEBUFFER_DEFAULT_SAMPLES = 37651;
        public const uint GL_FRAMEBUFFER_DEFAULT_FIXED_SAMPLE_LOCATIONS = 37652;
        public const uint GL_MAX_FRAMEBUFFER_WIDTH = 37653;
        public const uint GL_MAX_FRAMEBUFFER_HEIGHT = 37654;
        public const uint GL_MAX_FRAMEBUFFER_LAYERS = 37655;
        public const uint GL_MAX_FRAMEBUFFER_SAMPLES = 37656;
        public const uint GL_INTERNALFORMAT_SUPPORTED = 33391;
        public const uint GL_INTERNALFORMAT_PREFERRED = 33392;
        public const uint GL_INTERNALFORMAT_RED_SIZE = 33393;
        public const uint GL_INTERNALFORMAT_GREEN_SIZE = 33394;
        public const uint GL_INTERNALFORMAT_BLUE_SIZE = 33395;
        public const uint GL_INTERNALFORMAT_ALPHA_SIZE = 33396;
        public const uint GL_INTERNALFORMAT_DEPTH_SIZE = 33397;
        public const uint GL_INTERNALFORMAT_STENCIL_SIZE = 33398;
        public const uint GL_INTERNALFORMAT_SHARED_SIZE = 33399;
        public const uint GL_INTERNALFORMAT_RED_TYPE = 33400;
        public const uint GL_INTERNALFORMAT_GREEN_TYPE = 33401;
        public const uint GL_INTERNALFORMAT_BLUE_TYPE = 33402;
        public const uint GL_INTERNALFORMAT_ALPHA_TYPE = 33403;
        public const uint GL_INTERNALFORMAT_DEPTH_TYPE = 33404;
        public const uint GL_INTERNALFORMAT_STENCIL_TYPE = 33405;
        public const uint GL_MAX_WIDTH = 33406;
        public const uint GL_MAX_HEIGHT = 33407;
        public const uint GL_MAX_DEPTH = 33408;
        public const uint GL_MAX_LAYERS = 33409;
        public const uint GL_MAX_COMBINED_DIMENSIONS = 33410;
        public const uint GL_COLOR_COMPONENTS = 33411;
        public const uint GL_DEPTH_COMPONENTS = 33412;
        public const uint GL_STENCIL_COMPONENTS = 33413;
        public const uint GL_COLOR_RENDERABLE = 33414;
        public const uint GL_DEPTH_RENDERABLE = 33415;
        public const uint GL_STENCIL_RENDERABLE = 33416;
        public const uint GL_FRAMEBUFFER_RENDERABLE = 33417;
        public const uint GL_FRAMEBUFFER_RENDERABLE_LAYERED = 33418;
        public const uint GL_FRAMEBUFFER_BLEND = 33419;
        public const uint GL_READ_PIXELS = 33420;
        public const uint GL_READ_PIXELS_FORMAT = 33421;
        public const uint GL_READ_PIXELS_TYPE = 33422;
        public const uint GL_TEXTURE_IMAGE_FORMAT = 33423;
        public const uint GL_TEXTURE_IMAGE_TYPE = 33424;
        public const uint GL_GET_TEXTURE_IMAGE_FORMAT = 33425;
        public const uint GL_GET_TEXTURE_IMAGE_TYPE = 33426;
        public const uint GL_MIPMAP = 33427;
        public const uint GL_MANUAL_GENERATE_MIPMAP = 33428;
        public const uint GL_AUTO_GENERATE_MIPMAP = 33429;
        public const uint GL_COLOR_ENCODING = 33430;
        public const uint GL_SRGB_READ = 33431;
        public const uint GL_SRGB_WRITE = 33432;
        public const uint GL_FILTER = 33434;
        public const uint GL_VERTEX_TEXTURE = 33435;
        public const uint GL_TESS_CONTROL_TEXTURE = 33436;
        public const uint GL_TESS_EVALUATION_TEXTURE = 33437;
        public const uint GL_GEOMETRY_TEXTURE = 33438;
        public const uint GL_FRAGMENT_TEXTURE = 33439;
        public const uint GL_COMPUTE_TEXTURE = 33440;
        public const uint GL_TEXTURE_SHADOW = 33441;
        public const uint GL_TEXTURE_GATHER = 33442;
        public const uint GL_TEXTURE_GATHER_SHADOW = 33443;
        public const uint GL_SHADER_IMAGE_LOAD = 33444;
        public const uint GL_SHADER_IMAGE_STORE = 33445;
        public const uint GL_SHADER_IMAGE_ATOMIC = 33446;
        public const uint GL_IMAGE_TEXEL_SIZE = 33447;
        public const uint GL_IMAGE_COMPATIBILITY_CLASS = 33448;
        public const uint GL_IMAGE_PIXEL_FORMAT = 33449;
        public const uint GL_IMAGE_PIXEL_TYPE = 33450;
        public const uint GL_SIMULTANEOUS_TEXTURE_AND_DEPTH_TEST = 33452;
        public const uint GL_SIMULTANEOUS_TEXTURE_AND_STENCIL_TEST = 33453;
        public const uint GL_SIMULTANEOUS_TEXTURE_AND_DEPTH_WRITE = 33454;
        public const uint GL_SIMULTANEOUS_TEXTURE_AND_STENCIL_WRITE = 33455;
        public const uint GL_TEXTURE_COMPRESSED_BLOCK_WIDTH = 33457;
        public const uint GL_TEXTURE_COMPRESSED_BLOCK_HEIGHT = 33458;
        public const uint GL_TEXTURE_COMPRESSED_BLOCK_SIZE = 33459;
        public const uint GL_CLEAR_BUFFER = 33460;
        public const uint GL_TEXTURE_VIEW = 33461;
        public const uint GL_VIEW_COMPATIBILITY_CLASS = 33462;
        public const uint GL_FULL_SUPPORT = 33463;
        public const uint GL_CAVEAT_SUPPORT = 33464;
        public const uint GL_IMAGE_CLASS_4_X_32 = 33465;
        public const uint GL_IMAGE_CLASS_2_X_32 = 33466;
        public const uint GL_IMAGE_CLASS_1_X_32 = 33467;
        public const uint GL_IMAGE_CLASS_4_X_16 = 33468;
        public const uint GL_IMAGE_CLASS_2_X_16 = 33469;
        public const uint GL_IMAGE_CLASS_1_X_16 = 33470;
        public const uint GL_IMAGE_CLASS_4_X_8 = 33471;
        public const uint GL_IMAGE_CLASS_2_X_8 = 33472;
        public const uint GL_IMAGE_CLASS_1_X_8 = 33473;
        public const uint GL_IMAGE_CLASS_11_11_10 = 33474;
        public const uint GL_IMAGE_CLASS_10_10_10_2 = 33475;
        public const uint GL_VIEW_CLASS_128_BITS = 33476;
        public const uint GL_VIEW_CLASS_96_BITS = 33477;
        public const uint GL_VIEW_CLASS_64_BITS = 33478;
        public const uint GL_VIEW_CLASS_48_BITS = 33479;
        public const uint GL_VIEW_CLASS_32_BITS = 33480;
        public const uint GL_VIEW_CLASS_24_BITS = 33481;
        public const uint GL_VIEW_CLASS_16_BITS = 33482;
        public const uint GL_VIEW_CLASS_8_BITS = 33483;
        public const uint GL_VIEW_CLASS_S3TC_DXT1_RGB = 33484;
        public const uint GL_VIEW_CLASS_S3TC_DXT1_RGBA = 33485;
        public const uint GL_VIEW_CLASS_S3TC_DXT3_RGBA = 33486;
        public const uint GL_VIEW_CLASS_S3TC_DXT5_RGBA = 33487;
        public const uint GL_VIEW_CLASS_RGTC1_RED = 33488;
        public const uint GL_VIEW_CLASS_RGTC2_RG = 33489;
        public const uint GL_VIEW_CLASS_BPTC_UNORM = 33490;
        public const uint GL_VIEW_CLASS_BPTC_FLOAT = 33491;
        public const uint GL_UNIFORM = 37601;
        public const uint GL_UNIFORM_BLOCK = 37602;
        public const uint GL_PROGRAM_INPUT = 37603;
        public const uint GL_PROGRAM_OUTPUT = 37604;
        public const uint GL_BUFFER_VARIABLE = 37605;
        public const uint GL_SHADER_STORAGE_BLOCK = 37606;
        public const uint GL_VERTEX_SUBROUTINE = 37608;
        public const uint GL_TESS_CONTROL_SUBROUTINE = 37609;
        public const uint GL_TESS_EVALUATION_SUBROUTINE = 37610;
        public const uint GL_GEOMETRY_SUBROUTINE = 37611;
        public const uint GL_FRAGMENT_SUBROUTINE = 37612;
        public const uint GL_COMPUTE_SUBROUTINE = 37613;
        public const uint GL_VERTEX_SUBROUTINE_UNIFORM = 37614;
        public const uint GL_TESS_CONTROL_SUBROUTINE_UNIFORM = 37615;
        public const uint GL_TESS_EVALUATION_SUBROUTINE_UNIFORM = 37616;
        public const uint GL_GEOMETRY_SUBROUTINE_UNIFORM = 37617;
        public const uint GL_FRAGMENT_SUBROUTINE_UNIFORM = 37618;
        public const uint GL_COMPUTE_SUBROUTINE_UNIFORM = 37619;
        public const uint GL_TRANSFORM_FEEDBACK_VARYING = 37620;
        public const uint GL_ACTIVE_RESOURCES = 37621;
        public const uint GL_MAX_NAME_LENGTH = 37622;
        public const uint GL_MAX_NUM_ACTIVE_VARIABLES = 37623;
        public const uint GL_MAX_NUM_COMPATIBLE_SUBROUTINES = 37624;
        public const uint GL_NAME_LENGTH = 37625;
        public const uint GL_TYPE = 37626;
        public const uint GL_ARRAY_SIZE = 37627;
        public const uint GL_OFFSET = 37628;
        public const uint GL_BLOCK_INDEX = 37629;
        public const uint GL_ARRAY_STRIDE = 37630;
        public const uint GL_MATRIX_STRIDE = 37631;
        public const uint GL_IS_ROW_MAJOR = 37632;
        public const uint GL_ATOMIC_COUNTER_BUFFER_INDEX = 37633;
        public const uint GL_BUFFER_BINDING = 37634;
        public const uint GL_BUFFER_DATA_SIZE = 37635;
        public const uint GL_NUM_ACTIVE_VARIABLES = 37636;
        public const uint GL_ACTIVE_VARIABLES = 37637;
        public const uint GL_REFERENCED_BY_VERTEX_SHADER = 37638;
        public const uint GL_REFERENCED_BY_TESS_CONTROL_SHADER = 37639;
        public const uint GL_REFERENCED_BY_TESS_EVALUATION_SHADER = 37640;
        public const uint GL_REFERENCED_BY_GEOMETRY_SHADER = 37641;
        public const uint GL_REFERENCED_BY_FRAGMENT_SHADER = 37642;
        public const uint GL_REFERENCED_BY_COMPUTE_SHADER = 37643;
        public const uint GL_TOP_LEVEL_ARRAY_SIZE = 37644;
        public const uint GL_TOP_LEVEL_ARRAY_STRIDE = 37645;
        public const uint GL_LOCATION = 37646;
        public const uint GL_LOCATION_INDEX = 37647;
        public const uint GL_IS_PER_PATCH = 37607;
        public const uint GL_SHADER_STORAGE_BUFFER = 37074;
        public const uint GL_SHADER_STORAGE_BUFFER_BINDING = 37075;
        public const uint GL_SHADER_STORAGE_BUFFER_START = 37076;
        public const uint GL_SHADER_STORAGE_BUFFER_SIZE = 37077;
        public const uint GL_MAX_VERTEX_SHADER_STORAGE_BLOCKS = 37078;
        public const uint GL_MAX_GEOMETRY_SHADER_STORAGE_BLOCKS = 37079;
        public const uint GL_MAX_TESS_CONTROL_SHADER_STORAGE_BLOCKS = 37080;
        public const uint GL_MAX_TESS_EVALUATION_SHADER_STORAGE_BLOCKS = 37081;
        public const uint GL_MAX_FRAGMENT_SHADER_STORAGE_BLOCKS = 37082;
        public const uint GL_MAX_COMPUTE_SHADER_STORAGE_BLOCKS = 37083;
        public const uint GL_MAX_COMBINED_SHADER_STORAGE_BLOCKS = 37084;
        public const uint GL_MAX_SHADER_STORAGE_BUFFER_BINDINGS = 37085;
        public const uint GL_MAX_SHADER_STORAGE_BLOCK_SIZE = 37086;
        public const uint GL_SHADER_STORAGE_BUFFER_OFFSET_ALIGNMENT = 37087;
        public const uint GL_SHADER_STORAGE_BARRIER_BIT = 8192;
        public const uint GL_MAX_COMBINED_SHADER_OUTPUT_RESOURCES = 36665;
        public const uint GL_DEPTH_STENCIL_TEXTURE_MODE = 37098;
        public const uint GL_TEXTURE_BUFFER_OFFSET = 37277;
        public const uint GL_TEXTURE_BUFFER_SIZE = 37278;
        public const uint GL_TEXTURE_BUFFER_OFFSET_ALIGNMENT = 37279;
        public const uint GL_TEXTURE_VIEW_MIN_LEVEL = 33499;
        public const uint GL_TEXTURE_VIEW_NUM_LEVELS = 33500;
        public const uint GL_TEXTURE_VIEW_MIN_LAYER = 33501;
        public const uint GL_TEXTURE_VIEW_NUM_LAYERS = 33502;
        public const uint GL_TEXTURE_IMMUTABLE_LEVELS = 33503;
        public const uint GL_VERTEX_ATTRIB_BINDING = 33492;
        public const uint GL_VERTEX_ATTRIB_RELATIVE_OFFSET = 33493;
        public const uint GL_VERTEX_BINDING_DIVISOR = 33494;
        public const uint GL_VERTEX_BINDING_OFFSET = 33495;
        public const uint GL_VERTEX_BINDING_STRIDE = 33496;
        public const uint GL_MAX_VERTEX_ATTRIB_RELATIVE_OFFSET = 33497;
        public const uint GL_MAX_VERTEX_ATTRIB_BINDINGS = 33498;
        public const uint GL_VERTEX_BINDING_BUFFER = 36687;
        #endregion

        #region Delegates
        public delegate void GLDEBUGPROC( uint source, uint type, uint id, uint severity, int length, [In] [MarshalAs( UnmanagedType.LPStr )] string message, IntPtr userParam );
        public delegate void PFNGLCLEARBUFFERDATAPROC( uint target, uint internalformat, uint format, uint type, IntPtr data );
        public delegate void PFNGLCLEARBUFFERSUBDATAPROC( uint target, uint internalformat, int offset, int size, uint format, uint type, IntPtr data );
        public delegate void PFNGLDISPATCHCOMPUTEPROC( uint num_groups_x, uint num_groups_y, uint num_groups_z );
        public delegate void PFNGLDISPATCHCOMPUTEINDIRECTPROC( int indirect );
        public delegate void PFNGLCOPYIMAGESUBDATAPROC( uint srcName, uint srcTarget, int srcLevel, int srcX, int srcY, int srcZ, uint dstName, uint dstTarget, int dstLevel, int dstX, int dstY, int dstZ, int srcWidth, int srcHeight, int srcDepth );
        public delegate void PFNGLFRAMEBUFFERPARAMETERIPROC( uint target, uint pname, int param );
        public delegate void PFNGLGETFRAMEBUFFERPARAMETERIVPROC( uint target, uint pname, ref int parameters );
        public delegate void PFNGLGETINTERNALFORMATI64VPROC( uint target, uint internalformat, uint pname, int bufSize, ref int parameters );
        public delegate void PFNGLINVALIDATETEXSUBIMAGEPROC( uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth );
        public delegate void PFNGLINVALIDATETEXIMAGEPROC( uint texture, int level );
        public delegate void PFNGLINVALIDATEBUFFERSUBDATAPROC( uint buffer, int offset, int length );
        public delegate void PFNGLINVALIDATEBUFFERDATAPROC( uint buffer );
        public delegate void PFNGLINVALIDATEFRAMEBUFFERPROC( uint target, int numAttachments, ref uint attachments );
        public delegate void PFNGLINVALIDATESUBFRAMEBUFFERPROC( uint target, int numAttachments, ref uint attachments, int x, int y, int width, int height );
        public delegate void PFNGLMULTIDRAWARRAYSINDIRECTPROC( uint mode, IntPtr indirect, int drawcount, int stride );
        public delegate void PFNGLMULTIDRAWELEMENTSINDIRECTPROC( uint mode, uint type, IntPtr indirect, int drawcount, int stride );
        public delegate void PFNGLGETPROGRAMINTERFACEIVPROC( uint program, uint programInterface, uint pname, ref int parameters );
        public delegate uint PFNGLGETPROGRAMRESOURCEINDEXPROC( uint program, uint programInterface, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate void PFNGLGETPROGRAMRESOURCENAMEPROC( uint program, uint programInterface, uint index, int bufSize, ref int length, IntPtr name );
        public delegate void PFNGLGETPROGRAMRESOURCEIVPROC( uint program, uint programInterface, uint index, int propCount, ref uint props, int bufSize, ref int length, ref int parameters );
        public delegate int PFNGLGETPROGRAMRESOURCELOCATIONPROC( uint program, uint programInterface, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate int PFNGLGETPROGRAMRESOURCELOCATIONINDEXPROC( uint program, uint programInterface, [In] [MarshalAs( UnmanagedType.LPStr )] string name );
        public delegate void PFNGLSHADERSTORAGEBLOCKBINDINGPROC( uint program, uint storageBlockIndex, uint storageBlockBinding );
        public delegate void PFNGLTEXBUFFERRANGEPROC( uint target, uint internalformat, uint buffer, int offset, int size );
        public delegate void PFNGLTEXSTORAGE2DMULTISAMPLEPROC( uint target, int samples, uint internalformat, int width, int height, byte fixedsamplelocations );
        public delegate void PFNGLTEXSTORAGE3DMULTISAMPLEPROC( uint target, int samples, uint internalformat, int width, int height, int depth, byte fixedsamplelocations );
        public delegate void PFNGLTEXTUREVIEWPROC( uint texture, uint target, uint origtexture, uint internalformat, uint minlevel, uint numlevels, uint minlayer, uint numlayers );
        public delegate void PFNGLBINDVERTEXBUFFERPROC( uint bindingindex, uint buffer, int offset, int stride );
        public delegate void PFNGLVERTEXATTRIBFORMATPROC( uint attribindex, int size, uint type, byte normalized, uint relativeoffset );
        public delegate void PFNGLVERTEXATTRIBIFORMATPROC( uint attribindex, int size, uint type, uint relativeoffset );
        public delegate void PFNGLVERTEXATTRIBLFORMATPROC( uint attribindex, int size, uint type, uint relativeoffset );
        public delegate void PFNGLVERTEXATTRIBBINDINGPROC( uint attribindex, uint bindingindex );
        public delegate void PFNGLVERTEXBINDINGDIVISORPROC( uint bindingindex, uint divisor );
        public delegate void PFNGLDEBUGMESSAGECONTROLPROC( uint source, uint type, uint severity, int count, ref uint ids, byte enabled );
        public delegate void PFNGLDEBUGMESSAGEINSERTPROC( uint source, uint type, uint id, uint severity, int length, [In] [MarshalAs( UnmanagedType.LPStr )] string buf );
        public delegate void PFNGLDEBUGMESSAGECALLBACKPROC( GLDEBUGPROC callback, IntPtr userParam );
        public delegate uint PFNGLGETDEBUGMESSAGELOGPROC( uint count, int bufSize, ref uint sources, ref uint types, ref uint ids, ref uint severities, ref int lengths, IntPtr messageLog );
        public delegate void PFNGLPUSHDEBUGGROUPPROC( uint source, uint id, int length, [In] [MarshalAs( UnmanagedType.LPStr )] string message );
        public delegate void PFNGLPOPDEBUGGROUPPROC();
        public delegate void PFNGLOBJECTLABELPROC( uint identifier, uint name, int length, [In] [MarshalAs( UnmanagedType.LPStr )] string label );
        public delegate void PFNGLGETOBJECTLABELPROC( uint identifier, uint name, int bufSize, ref int length, IntPtr label );
        public delegate void PFNGLOBJECTPTRLABELPROC( IntPtr ptr, int length, [In] [MarshalAs( UnmanagedType.LPStr )] string label );
        public delegate void PFNGLGETOBJECTPTRLABELPROC( IntPtr ptr, int bufSize, ref int length, IntPtr label );
        #endregion

        #region Methods
        public static PFNGLCLEARBUFFERDATAPROC glClearBufferData;
        public static PFNGLCLEARBUFFERSUBDATAPROC glClearBufferSubData;
        public static PFNGLDISPATCHCOMPUTEPROC glDispatchCompute;
        public static PFNGLDISPATCHCOMPUTEINDIRECTPROC glDispatchComputeIndirect;
        public static PFNGLCOPYIMAGESUBDATAPROC glCopyImageSubData;
        public static PFNGLFRAMEBUFFERPARAMETERIPROC glFramebufferParameteri;
        public static PFNGLGETFRAMEBUFFERPARAMETERIVPROC glGetFramebufferParameteriv;
        public static PFNGLGETINTERNALFORMATI64VPROC glGetInternalformati64v;
        public static PFNGLINVALIDATETEXSUBIMAGEPROC glInvalidateTexSubImage;
        public static PFNGLINVALIDATETEXIMAGEPROC glInvalidateTexImage;
        public static PFNGLINVALIDATEBUFFERSUBDATAPROC glInvalidateBufferSubData;
        public static PFNGLINVALIDATEBUFFERDATAPROC glInvalidateBufferData;
        public static PFNGLINVALIDATEFRAMEBUFFERPROC glInvalidateFramebuffer;
        public static PFNGLINVALIDATESUBFRAMEBUFFERPROC glInvalidateSubFramebuffer;
        public static PFNGLMULTIDRAWARRAYSINDIRECTPROC glMultiDrawArraysIndirect;
        public static PFNGLMULTIDRAWELEMENTSINDIRECTPROC glMultiDrawElementsIndirect;
        public static PFNGLGETPROGRAMINTERFACEIVPROC glGetProgramInterfaceiv;
        public static PFNGLGETPROGRAMRESOURCEINDEXPROC glGetProgramResourceIndex;
        public static PFNGLGETPROGRAMRESOURCENAMEPROC glGetProgramResourceName;
        public static PFNGLGETPROGRAMRESOURCEIVPROC glGetProgramResourceiv;
        public static PFNGLGETPROGRAMRESOURCELOCATIONPROC glGetProgramResourceLocation;
        public static PFNGLGETPROGRAMRESOURCELOCATIONINDEXPROC glGetProgramResourceLocationIndex;
        public static PFNGLSHADERSTORAGEBLOCKBINDINGPROC glShaderStorageBlockBinding;
        public static PFNGLTEXBUFFERRANGEPROC glTexBufferRange;
        public static PFNGLTEXSTORAGE2DMULTISAMPLEPROC glTexStorage2DMultisample;
        public static PFNGLTEXSTORAGE3DMULTISAMPLEPROC glTexStorage3DMultisample;
        public static PFNGLTEXTUREVIEWPROC glTextureView;
        public static PFNGLBINDVERTEXBUFFERPROC glBindVertexBuffer;
        public static PFNGLVERTEXATTRIBFORMATPROC glVertexAttribFormat;
        public static PFNGLVERTEXATTRIBIFORMATPROC glVertexAttribIFormat;
        public static PFNGLVERTEXATTRIBLFORMATPROC glVertexAttribLFormat;
        public static PFNGLVERTEXATTRIBBINDINGPROC glVertexAttribBinding;
        public static PFNGLVERTEXBINDINGDIVISORPROC glVertexBindingDivisor;
        public static PFNGLDEBUGMESSAGECONTROLPROC glDebugMessageControl;
        public static PFNGLDEBUGMESSAGEINSERTPROC glDebugMessageInsert;
        public static PFNGLDEBUGMESSAGECALLBACKPROC glDebugMessageCallback;
        public static PFNGLGETDEBUGMESSAGELOGPROC glGetDebugMessageLog;
        public static PFNGLPUSHDEBUGGROUPPROC glPushDebugGroup;
        public static PFNGLPOPDEBUGGROUPPROC glPopDebugGroup;
        public static PFNGLOBJECTLABELPROC glObjectLabel;
        public static PFNGLGETOBJECTLABELPROC glGetObjectLabel;
        public static PFNGLOBJECTPTRLABELPROC glObjectPtrLabel;
        public static PFNGLGETOBJECTPTRLABELPROC glGetObjectPtrLabel;
        #endregion
        #endregion

        #region OpenGL 4.4
        #region Constants
        public const uint GL_MAX_VERTEX_ATTRIB_STRIDE = 33509;
        public const uint GL_PRIMITIVE_RESTART_FOR_PATCHES_SUPPORTED = 33313;
        public const uint GL_TEXTURE_BUFFER_BINDING = 35882;
        public const int GL_MAP_PERSISTENT_BIT = 64;
        public const int GL_MAP_COHERENT_BIT = 128;
        public const int GL_DYNAMIC_STORAGE_BIT = 256;
        public const int GL_CLIENT_STORAGE_BIT = 512;
        public const int GL_CLIENT_MAPPED_BUFFER_BARRIER_BIT = 16384;
        public const uint GL_BUFFER_IMMUTABLE_STORAGE = 33311;
        public const uint GL_BUFFER_STORAGE_FLAGS = 33312;
        public const uint GL_CLEAR_TEXTURE = 37733;
        public const uint GL_LOCATION_COMPONENT = 37706;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_INDEX = 37707;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_STRIDE = 37708;
        public const uint GL_QUERY_BUFFER = 37266;
        public const uint GL_QUERY_BUFFER_BARRIER_BIT = 32768;
        public const uint GL_QUERY_BUFFER_BINDING = 37267;
        public const uint GL_QUERY_RESULT_NO_WAIT = 37268;
        public const uint GL_MIRROR_CLAMP_TO_EDGE = 34627;
        #endregion

        #region Delegates
        public delegate void PFNGLBUFFERSTORAGEPROC( uint target, int size, IntPtr data, uint flags );
        public delegate void PFNGLCLEARTEXIMAGEPROC( uint texture, int level, uint format, uint type, IntPtr data );
        public delegate void PFNGLCLEARTEXSUBIMAGEPROC( uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, uint type, IntPtr data );
        public delegate void PFNGLBINDBUFFERSBASEPROC( uint target, uint first, int count, ref uint buffers );
        public delegate void PFNGLBINDBUFFERSRANGEPROC( uint target, uint first, int count, ref uint buffers, ref int offsets, ref int sizes );
        public delegate void PFNGLBINDTEXTURESPROC( uint first, int count, ref uint textures );
        public delegate void PFNGLBINDSAMPLERSPROC( uint first, int count, ref uint samplers );
        public delegate void PFNGLBINDIMAGETEXTURESPROC( uint first, int count, ref uint textures );
        public delegate void PFNGLBINDVERTEXBUFFERSPROC( uint first, int count, ref uint buffers, ref int offsets, ref int strides );
        #endregion

        #region Methods
        public static PFNGLBUFFERSTORAGEPROC glBufferStorage;
        public static PFNGLCLEARTEXIMAGEPROC glClearTexImage;
        public static PFNGLCLEARTEXSUBIMAGEPROC glClearTexSubImage;
        public static PFNGLBINDBUFFERSBASEPROC glBindBuffersBase;
        public static PFNGLBINDBUFFERSRANGEPROC glBindBuffersRange;
        public static PFNGLBINDTEXTURESPROC glBindTextures;
        public static PFNGLBINDSAMPLERSPROC glBindSamplers;
        public static PFNGLBINDIMAGETEXTURESPROC glBindImageTextures;
        public static PFNGLBINDVERTEXBUFFERSPROC glBindVertexBuffers;
        #endregion

        #endregion

        #region OpenGL 4.5
        #region Constants
        public const uint GL_CONTEXT_LOST = 1287;
        public const uint GL_NEGATIVE_ONE_TO_ONE = 37726;
        public const uint GL_ZERO_TO_ONE = 37727;
        public const uint GL_CLIP_ORIGIN = 37724;
        public const uint GL_CLIP_DEPTH_MODE = 37725;
        public const uint GL_QUERY_WAIT_INVERTED = 36375;
        public const uint GL_QUERY_NO_WAIT_INVERTED = 36376;
        public const uint GL_QUERY_BY_REGION_WAIT_INVERTED = 36377;
        public const uint GL_QUERY_BY_REGION_NO_WAIT_INVERTED = 36378;
        public const uint GL_MAX_CULL_DISTANCES = 33529;
        public const uint GL_MAX_COMBINED_CLIP_AND_CULL_DISTANCES = 33530;
        public const uint GL_TEXTURE_TARGET = 4102;
        public const uint GL_QUERY_TARGET = 33514;
        public const uint GL_GUILTY_CONTEXT_RESET = 33363;
        public const uint GL_INNOCENT_CONTEXT_RESET = 33364;
        public const uint GL_UNKNOWN_CONTEXT_RESET = 33365;
        public const uint GL_RESET_NOTIFICATION_STRATEGY = 33366;
        public const uint GL_LOSE_CONTEXT_ON_RESET = 33362;
        public const uint GL_NO_RESET_NOTIFICATION = 33377;
        public const int GL_CONTEXT_FLAG_ROBUST_ACCESS_BIT = 4;
        public const uint GL_CONTEXT_RELEASE_BEHAVIOR = 33531;
        public const uint GL_CONTEXT_RELEASE_BEHAVIOR_FLUSH = 33532;
        #endregion

        #region Delegates
        public delegate void PFNGLCLIPCONTROLPROC( uint origin, uint depth );
        public delegate void PFNGLCREATETRANSFORMFEEDBACKSPROC( int n, ref uint ids );
        public delegate void PFNGLTRANSFORMFEEDBACKBUFFERBASEPROC( uint xfb, uint index, uint buffer );
        public delegate void PFNGLTRANSFORMFEEDBACKBUFFERRANGEPROC( uint xfb, uint index, uint buffer, int offset, int size );
        public delegate void PFNGLGETTRANSFORMFEEDBACKIVPROC( uint xfb, uint pname, ref int param );
        public delegate void PFNGLGETTRANSFORMFEEDBACKI_VPROC( uint xfb, uint pname, uint index, ref int param );
        public delegate void PFNGLGETTRANSFORMFEEDBACKI64_VPROC( uint xfb, uint pname, uint index, ref int param );
        public delegate void PFNGLCREATEBUFFERSPROC( int n, ref uint buffers );
        public delegate void PFNGLNAMEDBUFFERSTORAGEPROC( uint buffer, int size, IntPtr data, uint flags );
        public delegate void PFNGLNAMEDBUFFERDATAPROC( uint buffer, int size, IntPtr data, uint usage );
        public delegate void PFNGLNAMEDBUFFERSUBDATAPROC( uint buffer, int offset, int size, IntPtr data );
        public delegate void PFNGLCOPYNAMEDBUFFERSUBDATAPROC( uint readBuffer, uint writeBuffer, int readOffset, int writeOffset, int size );
        public delegate void PFNGLCLEARNAMEDBUFFERDATAPROC( uint buffer, uint internalformat, uint format, uint type, IntPtr data );
        public delegate void PFNGLCLEARNAMEDBUFFERSUBDATAPROC( uint buffer, uint internalformat, int offset, int size, uint format, uint type, IntPtr data );
        public delegate IntPtr PFNGLMAPNAMEDBUFFERPROC( uint buffer, uint access );
        public delegate IntPtr PFNGLMAPNAMEDBUFFERRANGEPROC( uint buffer, int offset, int length, uint access );
        public delegate byte PFNGLUNMAPNAMEDBUFFERPROC( uint buffer );
        public delegate void PFNGLFLUSHMAPPEDNAMEDBUFFERRANGEPROC( uint buffer, int offset, int length );
        public delegate void PFNGLGETNAMEDBUFFERPARAMETERIVPROC( uint buffer, uint pname, ref int parameters );
        public delegate void PFNGLGETNAMEDBUFFERPARAMETERI64VPROC( uint buffer, uint pname, ref int parameters );
        public delegate void PFNGLGETNAMEDBUFFERPOINTERVPROC( uint buffer, uint pname, ref IntPtr parameters );
        public delegate void PFNGLGETNAMEDBUFFERSUBDATAPROC( uint buffer, int offset, int size, IntPtr data );
        public delegate void PFNGLCREATEFRAMEBUFFERSPROC( int n, ref uint framebuffers );
        public delegate void PFNGLNAMEDFRAMEBUFFERRENDERBUFFERPROC( uint framebuffer, uint attachment, uint renderbuffertarget, uint renderbuffer );
        public delegate void PFNGLNAMEDFRAMEBUFFERPARAMETERIPROC( uint framebuffer, uint pname, int param );
        public delegate void PFNGLNAMEDFRAMEBUFFERTEXTUREPROC( uint framebuffer, uint attachment, uint texture, int level );
        public delegate void PFNGLNAMEDFRAMEBUFFERTEXTURELAYERPROC( uint framebuffer, uint attachment, uint texture, int level, int layer );
        public delegate void PFNGLNAMEDFRAMEBUFFERDRAWBUFFERPROC( uint framebuffer, uint buf );
        public delegate void PFNGLNAMEDFRAMEBUFFERDRAWBUFFERSPROC( uint framebuffer, int n, ref uint bufs );
        public delegate void PFNGLNAMEDFRAMEBUFFERREADBUFFERPROC( uint framebuffer, uint src );
        public delegate void PFNGLINVALIDATENAMEDFRAMEBUFFERDATAPROC( uint framebuffer, int numAttachments, ref uint attachments );
        public delegate void PFNGLINVALIDATENAMEDFRAMEBUFFERSUBDATAPROC( uint framebuffer, int numAttachments, ref uint attachments, int x, int y, int width, int height );
        public delegate void PFNGLCLEARNAMEDFRAMEBUFFERIVPROC( uint framebuffer, uint buffer, int drawbuffer, ref int value );
        public delegate void PFNGLCLEARNAMEDFRAMEBUFFERUIVPROC( uint framebuffer, uint buffer, int drawbuffer, ref uint value );
        public delegate void PFNGLCLEARNAMEDFRAMEBUFFERFVPROC( uint framebuffer, uint buffer, int drawbuffer, ref float value );
        public delegate void PFNGLCLEARNAMEDFRAMEBUFFERFIPROC( uint framebuffer, uint buffer, int drawbuffer, float depth, int stencil );
        public delegate void PFNGLBLITNAMEDFRAMEBUFFERPROC( uint readFramebuffer, uint drawFramebuffer, int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, uint mask, uint filter );
        public delegate uint PFNGLCHECKNAMEDFRAMEBUFFERSTATUSPROC( uint framebuffer, uint target );
        public delegate void PFNGLGETNAMEDFRAMEBUFFERPARAMETERIVPROC( uint framebuffer, uint pname, ref int param );
        public delegate void PFNGLGETNAMEDFRAMEBUFFERATTACHMENTPARAMETERIVPROC( uint framebuffer, uint attachment, uint pname, ref int parameters );
        public delegate void PFNGLCREATERENDERBUFFERSPROC( int n, ref uint renderbuffers );
        public delegate void PFNGLNAMEDRENDERBUFFERSTORAGEPROC( uint renderbuffer, uint internalformat, int width, int height );
        public delegate void PFNGLNAMEDRENDERBUFFERSTORAGEMULTISAMPLEPROC( uint renderbuffer, int samples, uint internalformat, int width, int height );
        public delegate void PFNGLGETNAMEDRENDERBUFFERPARAMETERIVPROC( uint renderbuffer, uint pname, ref int parameters );
        public delegate void PFNGLCREATETEXTURESPROC( uint target, int n, ref uint textures );
        public delegate void PFNGLTEXTUREBUFFERPROC( uint texture, uint internalformat, uint buffer );
        public delegate void PFNGLTEXTUREBUFFERRANGEPROC( uint texture, uint internalformat, uint buffer, int offset, int size );
        public delegate void PFNGLTEXTURESTORAGE1DPROC( uint texture, int levels, uint internalformat, int width );
        public delegate void PFNGLTEXTURESTORAGE2DPROC( uint texture, int levels, uint internalformat, int width, int height );
        public delegate void PFNGLTEXTURESTORAGE3DPROC( uint texture, int levels, uint internalformat, int width, int height, int depth );
        public delegate void PFNGLTEXTURESTORAGE2DMULTISAMPLEPROC( uint texture, int samples, uint internalformat, int width, int height, byte fixedsamplelocations );
        public delegate void PFNGLTEXTURESTORAGE3DMULTISAMPLEPROC( uint texture, int samples, uint internalformat, int width, int height, int depth, byte fixedsamplelocations );
        public delegate void PFNGLTEXTURESUBIMAGE1DPROC( uint texture, int level, int xoffset, int width, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLTEXTURESUBIMAGE2DPROC( uint texture, int level, int xoffset, int yoffset, int width, int height, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLTEXTURESUBIMAGE3DPROC( uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, uint type, IntPtr pixels );
        public delegate void PFNGLCOMPRESSEDTEXTURESUBIMAGE1DPROC( uint texture, int level, int xoffset, int width, uint format, int imageSize, IntPtr data );
        public delegate void PFNGLCOMPRESSEDTEXTURESUBIMAGE2DPROC( uint texture, int level, int xoffset, int yoffset, int width, int height, uint format, int imageSize, IntPtr data );
        public delegate void PFNGLCOMPRESSEDTEXTURESUBIMAGE3DPROC( uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, int imageSize, IntPtr data );
        public delegate void PFNGLCOPYTEXTURESUBIMAGE1DPROC( uint texture, int level, int xoffset, int x, int y, int width );
        public delegate void PFNGLCOPYTEXTURESUBIMAGE2DPROC( uint texture, int level, int xoffset, int yoffset, int x, int y, int width, int height );
        public delegate void PFNGLCOPYTEXTURESUBIMAGE3DPROC( uint texture, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height );
        public delegate void PFNGLTEXTUREPARAMETERFPROC( uint texture, uint pname, float param );
        public delegate void PFNGLTEXTUREPARAMETERFVPROC( uint texture, uint pname, ref float param );
        public delegate void PFNGLTEXTUREPARAMETERIPROC( uint texture, uint pname, int param );
        public delegate void PFNGLTEXTUREPARAMETERIIVPROC( uint texture, uint pname, ref int parameters );
        public delegate void PFNGLTEXTUREPARAMETERIUIVPROC( uint texture, uint pname, ref uint parameters );
        public delegate void PFNGLTEXTUREPARAMETERIVPROC( uint texture, uint pname, ref int param );
        public delegate void PFNGLGENERATETEXTUREMIPMAPPROC( uint texture );
        public delegate void PFNGLBINDTEXTUREUNITPROC( uint unit, uint texture );
        public delegate void PFNGLGETTEXTUREIMAGEPROC( uint texture, int level, uint format, uint type, int bufSize, IntPtr pixels );
        public delegate void PFNGLGETCOMPRESSEDTEXTUREIMAGEPROC( uint texture, int level, int bufSize, IntPtr pixels );
        public delegate void PFNGLGETTEXTURELEVELPARAMETERFVPROC( uint texture, int level, uint pname, ref float parameters );
        public delegate void PFNGLGETTEXTURELEVELPARAMETERIVPROC( uint texture, int level, uint pname, ref int parameters );
        public delegate void PFNGLGETTEXTUREPARAMETERFVPROC( uint texture, uint pname, ref float parameters );
        public delegate void PFNGLGETTEXTUREPARAMETERIIVPROC( uint texture, uint pname, ref int parameters );
        public delegate void PFNGLGETTEXTUREPARAMETERIUIVPROC( uint texture, uint pname, ref uint parameters );
        public delegate void PFNGLGETTEXTUREPARAMETERIVPROC( uint texture, uint pname, ref int parameters );
        public delegate void PFNGLCREATEVERTEXARRAYSPROC( int n, ref uint arrays );
        public delegate void PFNGLDISABLEVERTEXARRAYATTRIBPROC( uint vaobj, uint index );
        public delegate void PFNGLENABLEVERTEXARRAYATTRIBPROC( uint vaobj, uint index );
        public delegate void PFNGLVERTEXARRAYELEMENTBUFFERPROC( uint vaobj, uint buffer );
        public delegate void PFNGLVERTEXARRAYVERTEXBUFFERPROC( uint vaobj, uint bindingindex, uint buffer, int offset, int stride );
        public delegate void PFNGLVERTEXARRAYVERTEXBUFFERSPROC( uint vaobj, uint first, int count, ref uint buffers, ref int offsets, ref int strides );
        public delegate void PFNGLVERTEXARRAYATTRIBBINDINGPROC( uint vaobj, uint attribindex, uint bindingindex );
        public delegate void PFNGLVERTEXARRAYATTRIBFORMATPROC( uint vaobj, uint attribindex, int size, uint type, byte normalized, uint relativeoffset );
        public delegate void PFNGLVERTEXARRAYATTRIBIFORMATPROC( uint vaobj, uint attribindex, int size, uint type, uint relativeoffset );
        public delegate void PFNGLVERTEXARRAYATTRIBLFORMATPROC( uint vaobj, uint attribindex, int size, uint type, uint relativeoffset );
        public delegate void PFNGLVERTEXARRAYBINDINGDIVISORPROC( uint vaobj, uint bindingindex, uint divisor );
        public delegate void PFNGLGETVERTEXARRAYIVPROC( uint vaobj, uint pname, ref int param );
        public delegate void PFNGLGETVERTEXARRAYINDEXEDIVPROC( uint vaobj, uint index, uint pname, ref int param );
        public delegate void PFNGLGETVERTEXARRAYINDEXED64IVPROC( uint vaobj, uint index, uint pname, ref int param );
        public delegate void PFNGLCREATESAMPLERSPROC( int n, ref uint samplers );
        public delegate void PFNGLCREATEPROGRAMPIPELINESPROC( int n, ref uint pipelines );
        public delegate void PFNGLCREATEQUERIESPROC( uint target, int n, ref uint ids );
        public delegate void PFNGLGETQUERYBUFFEROBJECTI64VPROC( uint id, uint buffer, uint pname, int offset );
        public delegate void PFNGLGETQUERYBUFFEROBJECTIVPROC( uint id, uint buffer, uint pname, int offset );
        public delegate void PFNGLGETQUERYBUFFEROBJECTUI64VPROC( uint id, uint buffer, uint pname, int offset );
        public delegate void PFNGLGETQUERYBUFFEROBJECTUIVPROC( uint id, uint buffer, uint pname, int offset );
        public delegate void PFNGLMEMORYBARRIERBYREGIONPROC( uint barriers );
        public delegate void PFNGLGETTEXTURESUBIMAGEPROC( uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, uint type, int bufSize, IntPtr pixels );
        public delegate void PFNGLGETCOMPRESSEDTEXTURESUBIMAGEPROC( uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, int bufSize, IntPtr pixels );
        public delegate uint PFNGLGETGRAPHICSRESETSTATUSPROC();
        public delegate void PFNGLGETNCOMPRESSEDTEXIMAGEPROC( uint target, int lod, int bufSize, IntPtr pixels );
        public delegate void PFNGLGETNTEXIMAGEPROC( uint target, int level, uint format, uint type, int bufSize, IntPtr pixels );
        public delegate void PFNGLGETNUNIFORMDVPROC( uint program, int location, int bufSize, ref double parameters );
        public delegate void PFNGLGETNUNIFORMFVPROC( uint program, int location, int bufSize, ref float parameters );
        public delegate void PFNGLGETNUNIFORMIVPROC( uint program, int location, int bufSize, ref int parameters );
        public delegate void PFNGLGETNUNIFORMUIVPROC( uint program, int location, int bufSize, ref uint parameters );
        public delegate void PFNGLREADNPIXELSPROC( int x, int y, int width, int height, uint format, uint type, int bufSize, IntPtr data );
        public delegate void PFNGLTEXTUREBARRIERPROC();
        #endregion

        #region Methods
        public static PFNGLCLIPCONTROLPROC glClipControl;
        public static PFNGLCREATETRANSFORMFEEDBACKSPROC glCreateTransformFeedbacks;
        public static PFNGLTRANSFORMFEEDBACKBUFFERBASEPROC glTransformFeedbackBufferBase;
        public static PFNGLTRANSFORMFEEDBACKBUFFERRANGEPROC glTransformFeedbackBufferRange;
        public static PFNGLGETTRANSFORMFEEDBACKIVPROC glGetTransformFeedbackiv;
        public static PFNGLGETTRANSFORMFEEDBACKI_VPROC glGetTransformFeedbacki_v;
        public static PFNGLGETTRANSFORMFEEDBACKI64_VPROC glGetTransformFeedbacki64_v;
        public static PFNGLCREATEBUFFERSPROC glCreateBuffers;
        public static PFNGLNAMEDBUFFERSTORAGEPROC glNamedBufferStorage;
        public static PFNGLNAMEDBUFFERDATAPROC glNamedBufferData;
        public static PFNGLNAMEDBUFFERSUBDATAPROC glNamedBufferSubData;
        public static PFNGLCOPYNAMEDBUFFERSUBDATAPROC glCopyNamedBufferSubData;
        public static PFNGLCLEARNAMEDBUFFERDATAPROC glClearNamedBufferData;
        public static PFNGLCLEARNAMEDBUFFERSUBDATAPROC glClearNamedBufferSubData;
        public static PFNGLMAPNAMEDBUFFERPROC glMapNamedBuffer;
        public static PFNGLMAPNAMEDBUFFERRANGEPROC glMapNamedBufferRange;
        public static PFNGLUNMAPNAMEDBUFFERPROC glUnmapNamedBuffer;
        public static PFNGLFLUSHMAPPEDNAMEDBUFFERRANGEPROC glFlushMappedNamedBufferRange;
        public static PFNGLGETNAMEDBUFFERPARAMETERIVPROC glGetNamedBufferParameteriv;
        public static PFNGLGETNAMEDBUFFERPARAMETERI64VPROC glGetNamedBufferParameteri64v;
        public static PFNGLGETNAMEDBUFFERPOINTERVPROC glGetNamedBufferPointerv;
        public static PFNGLGETNAMEDBUFFERSUBDATAPROC glGetNamedBufferSubData;
        public static PFNGLCREATEFRAMEBUFFERSPROC glCreateFramebuffers;
        public static PFNGLNAMEDFRAMEBUFFERRENDERBUFFERPROC glNamedFramebufferRenderbuffer;
        public static PFNGLNAMEDFRAMEBUFFERPARAMETERIPROC glNamedFramebufferParameteri;
        public static PFNGLNAMEDFRAMEBUFFERTEXTUREPROC glNamedFramebufferTexture;
        public static PFNGLNAMEDFRAMEBUFFERTEXTURELAYERPROC glNamedFramebufferTextureLayer;
        public static PFNGLNAMEDFRAMEBUFFERDRAWBUFFERPROC glNamedFramebufferDrawBuffer;
        public static PFNGLNAMEDFRAMEBUFFERDRAWBUFFERSPROC glNamedFramebufferDrawBuffers;
        public static PFNGLNAMEDFRAMEBUFFERREADBUFFERPROC glNamedFramebufferReadBuffer;
        public static PFNGLINVALIDATENAMEDFRAMEBUFFERDATAPROC glInvalidateNamedFramebufferData;
        public static PFNGLINVALIDATENAMEDFRAMEBUFFERSUBDATAPROC glInvalidateNamedFramebufferSubData;
        public static PFNGLCLEARNAMEDFRAMEBUFFERIVPROC glClearNamedFramebufferiv;
        public static PFNGLCLEARNAMEDFRAMEBUFFERUIVPROC glClearNamedFramebufferuiv;
        public static PFNGLCLEARNAMEDFRAMEBUFFERFVPROC glClearNamedFramebufferfv;
        public static PFNGLCLEARNAMEDFRAMEBUFFERFIPROC glClearNamedFramebufferfi;
        public static PFNGLBLITNAMEDFRAMEBUFFERPROC glBlitNamedFramebuffer;
        public static PFNGLCHECKNAMEDFRAMEBUFFERSTATUSPROC glCheckNamedFramebufferStatus;
        public static PFNGLGETNAMEDFRAMEBUFFERPARAMETERIVPROC glGetNamedFramebufferParameteriv;
        public static PFNGLGETNAMEDFRAMEBUFFERATTACHMENTPARAMETERIVPROC glGetNamedFramebufferAttachmentParameteriv;
        public static PFNGLCREATERENDERBUFFERSPROC glCreateRenderbuffers;
        public static PFNGLNAMEDRENDERBUFFERSTORAGEPROC glNamedRenderbufferStorage;
        public static PFNGLNAMEDRENDERBUFFERSTORAGEMULTISAMPLEPROC glNamedRenderbufferStorageMultisample;
        public static PFNGLGETNAMEDRENDERBUFFERPARAMETERIVPROC glGetNamedRenderbufferParameteriv;
        public static PFNGLCREATETEXTURESPROC glCreateTextures;
        public static PFNGLTEXTUREBUFFERPROC glTextureBuffer;
        public static PFNGLTEXTUREBUFFERRANGEPROC glTextureBufferRange;
        public static PFNGLTEXTURESTORAGE1DPROC glTextureStorage1D;
        public static PFNGLTEXTURESTORAGE2DPROC glTextureStorage2D;
        public static PFNGLTEXTURESTORAGE3DPROC glTextureStorage3D;
        public static PFNGLTEXTURESTORAGE2DMULTISAMPLEPROC glTextureStorage2DMultisample;
        public static PFNGLTEXTURESTORAGE3DMULTISAMPLEPROC glTextureStorage3DMultisample;
        public static PFNGLTEXTURESUBIMAGE1DPROC glTextureSubImage1D;
        public static PFNGLTEXTURESUBIMAGE2DPROC glTextureSubImage2D;
        public static PFNGLTEXTURESUBIMAGE3DPROC glTextureSubImage3D;
        public static PFNGLCOMPRESSEDTEXTURESUBIMAGE1DPROC glCompressedTextureSubImage1D;
        public static PFNGLCOMPRESSEDTEXTURESUBIMAGE2DPROC glCompressedTextureSubImage2D;
        public static PFNGLCOMPRESSEDTEXTURESUBIMAGE3DPROC glCompressedTextureSubImage3D;
        public static PFNGLCOPYTEXTURESUBIMAGE1DPROC glCopyTextureSubImage1D;
        public static PFNGLCOPYTEXTURESUBIMAGE2DPROC glCopyTextureSubImage2D;
        public static PFNGLCOPYTEXTURESUBIMAGE3DPROC glCopyTextureSubImage3D;
        public static PFNGLTEXTUREPARAMETERFPROC glTextureParameterf;
        public static PFNGLTEXTUREPARAMETERFVPROC glTextureParameterfv;
        public static PFNGLTEXTUREPARAMETERIPROC glTextureParameteri;
        public static PFNGLTEXTUREPARAMETERIIVPROC glTextureParameterIiv;
        public static PFNGLTEXTUREPARAMETERIUIVPROC glTextureParameterIuiv;
        public static PFNGLTEXTUREPARAMETERIVPROC glTextureParameteriv;
        public static PFNGLGENERATETEXTUREMIPMAPPROC glGenerateTextureMipmap;
        public static PFNGLBINDTEXTUREUNITPROC glBindTextureUnit;
        public static PFNGLGETTEXTUREIMAGEPROC glGetTextureImage;
        public static PFNGLGETCOMPRESSEDTEXTUREIMAGEPROC glGetCompressedTextureImage;
        public static PFNGLGETTEXTURELEVELPARAMETERFVPROC glGetTextureLevelParameterfv;
        public static PFNGLGETTEXTURELEVELPARAMETERIVPROC glGetTextureLevelParameteriv;
        public static PFNGLGETTEXTUREPARAMETERFVPROC glGetTextureParameterfv;
        public static PFNGLGETTEXTUREPARAMETERIIVPROC glGetTextureParameterIiv;
        public static PFNGLGETTEXTUREPARAMETERIUIVPROC glGetTextureParameterIuiv;
        public static PFNGLGETTEXTUREPARAMETERIVPROC glGetTextureParameteriv;
        public static PFNGLCREATEVERTEXARRAYSPROC glCreateVertexArrays;
        public static PFNGLDISABLEVERTEXARRAYATTRIBPROC glDisableVertexArrayAttrib;
        public static PFNGLENABLEVERTEXARRAYATTRIBPROC glEnableVertexArrayAttrib;
        public static PFNGLVERTEXARRAYELEMENTBUFFERPROC glVertexArrayElementBuffer;
        public static PFNGLVERTEXARRAYVERTEXBUFFERPROC glVertexArrayVertexBuffer;
        public static PFNGLVERTEXARRAYVERTEXBUFFERSPROC glVertexArrayVertexBuffers;
        public static PFNGLVERTEXARRAYATTRIBBINDINGPROC glVertexArrayAttribBinding;
        public static PFNGLVERTEXARRAYATTRIBFORMATPROC glVertexArrayAttribFormat;
        public static PFNGLVERTEXARRAYATTRIBIFORMATPROC glVertexArrayAttribIFormat;
        public static PFNGLVERTEXARRAYATTRIBLFORMATPROC glVertexArrayAttribLFormat;
        public static PFNGLVERTEXARRAYBINDINGDIVISORPROC glVertexArrayBindingDivisor;
        public static PFNGLGETVERTEXARRAYIVPROC glGetVertexArrayiv;
        public static PFNGLGETVERTEXARRAYINDEXEDIVPROC glGetVertexArrayIndexediv;
        public static PFNGLGETVERTEXARRAYINDEXED64IVPROC glGetVertexArrayIndexed64iv;
        public static PFNGLCREATESAMPLERSPROC glCreateSamplers;
        public static PFNGLCREATEPROGRAMPIPELINESPROC glCreateProgramPipelines;
        public static PFNGLCREATEQUERIESPROC glCreateQueries;
        public static PFNGLGETQUERYBUFFEROBJECTI64VPROC glGetQueryBufferObjecti64v;
        public static PFNGLGETQUERYBUFFEROBJECTIVPROC glGetQueryBufferObjectiv;
        public static PFNGLGETQUERYBUFFEROBJECTUI64VPROC glGetQueryBufferObjectui64v;
        public static PFNGLGETQUERYBUFFEROBJECTUIVPROC glGetQueryBufferObjectuiv;
        public static PFNGLMEMORYBARRIERBYREGIONPROC glMemoryBarrierByRegion;
        public static PFNGLGETTEXTURESUBIMAGEPROC glGetTextureSubImage;
        public static PFNGLGETCOMPRESSEDTEXTURESUBIMAGEPROC glGetCompressedTextureSubImage;
        public static PFNGLGETGRAPHICSRESETSTATUSPROC glGetGraphicsResetStatus;
        public static PFNGLGETNCOMPRESSEDTEXIMAGEPROC glGetnCompressedTexImage;
        public static PFNGLGETNTEXIMAGEPROC glGetnTexImage;
        public static PFNGLGETNUNIFORMDVPROC glGetnUniformdv;
        public static PFNGLGETNUNIFORMFVPROC glGetnUniformfv;
        public static PFNGLGETNUNIFORMIVPROC glGetnUniformiv;
        public static PFNGLGETNUNIFORMUIVPROC glGetnUniformuiv;
        public static PFNGLREADNPIXELSPROC glReadnPixels;
        public static PFNGLTEXTUREBARRIERPROC glTextureBarrier;
        #endregion
        #endregion
    }
}
#endregion

#region Abstraction
namespace CSGL
{
    using static OpenGL;
    using static Glfw3;
    using static CSGL;
    using static DLL;

    #region Prototypes
    public enum CSGLWindowStyle : int
    {
        Normal = 1,
        Borderless = 2,
        Fullscreen = 4
    }

    #region Delegates
    public delegate void CSGLDrawEvent( CSGLWindow sender, double deltaTime );
    public delegate void CSGLUpdateEvent( CSGLWindow sender, double deltaTime );
    #endregion
    #endregion

    public static class CSGL
    {
        #region Extension
        #region Fields
        public static IntPtr NULL = (IntPtr)0;

        private static Type _glType = typeof( OpenGL );
        private static Type _delegateType = typeof( MulticastDelegate );
        public static int CSGL_GLVERSION = 0;
        #endregion

        #region Methods
        public static bool csglLoadGL()
        {
            #region Loader
            FieldInfo[] fields = _glType.GetFields( BindingFlags.Public | BindingFlags.Static );

            foreach ( FieldInfo fi in fields )
            {
                if ( fi.FieldType.BaseType == _delegateType )
                {
                    IntPtr ptr = glfwGetProcAddress( fi.Name );

                    if ( ptr != IntPtr.Zero )
                        fi.SetValue( null, Marshal.GetDelegateForFunctionPointer( ptr, fi.FieldType ) );
                }
            }
            #endregion

            #region Detect version
            CSGL_GLVERSION = 0;

            if( glAccum != null )
                CSGL_GLVERSION = 110;

            if ( glDrawRangeElements != null )
                CSGL_GLVERSION = 120;

            if ( glActiveTexture != null )
                CSGL_GLVERSION = 130;

            if ( glBlendFuncSeparate != null )
                CSGL_GLVERSION = 140;

            if ( glGenQueries != null )
                CSGL_GLVERSION = 150;

            if ( glBlendEquationSeparate != null )
                CSGL_GLVERSION = 200;

            if ( glUniformMatrix2x3fv != null )
                CSGL_GLVERSION = 210;

            if ( glColorMaski != null )
                CSGL_GLVERSION = 300;

            if ( glDrawArraysInstanced != null )
                CSGL_GLVERSION = 310;

            if ( glDrawElementsBaseVertex != null )
                CSGL_GLVERSION = 320;

            if ( glBindFragDataLocationIndexed != null )
                CSGL_GLVERSION = 330;

            if ( glMinSampleShading != null )
                CSGL_GLVERSION = 400;

            if ( glReleaseShaderCompiler != null )
                CSGL_GLVERSION = 410;

            if ( glDrawArraysInstancedBaseInstance != null )
                CSGL_GLVERSION = 420;

            if ( glClearBufferData != null )
                CSGL_GLVERSION = 430;

            if ( glBufferStorage != null )
                CSGL_GLVERSION = 440;

            if ( glClipControl != null )
                CSGL_GLVERSION = 450;

            if( CSGL_GLVERSION == 0 )
                throw new Exception( "Could not load OpenGL" );
            #endregion

            Console.WriteLine( "Linked 'OpenGL' -> VERSION {0}", CSGL_GLVERSION );

            return true;
        }

        public static void csglAssert()
        {
            uint glError = glGetError();

            if ( glError != GL_NO_ERROR )
                throw new Exception( "OpenGL error (" + glError + ")" );
        }
        #endregion

        #region Macros
        #region csglBuffer
        public static uint csglBuffer( float[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( float ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length * sizeof( float ) );
            Marshal.Copy( data, 0, ptrData, data.Length );

            glBufferData( target, sizeof( float ) * data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }

        public static uint csglBuffer( double[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( double ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length * sizeof( double ) );
            Marshal.Copy( data, 0, ptrData, data.Length );

            glBufferData( target, sizeof( double ) * data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }

        public static uint csglBuffer( byte[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( byte ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length );
            Marshal.Copy( data, 0, ptrData, data.Length );

            glBufferData( target, data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }

        public static uint csglBuffer( ushort[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( ushort ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length * sizeof( ushort ) );
            Marshal.Copy( (short[])(Array)data, 0, ptrData, data.Length );

            glBufferData( target, sizeof( ushort ) * data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }

        public static uint csglBuffer( short[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( short ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length * sizeof( short ) );
            Marshal.Copy( data, 0, ptrData, data.Length );

            glBufferData( target, sizeof( short ) * data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }

        public static uint csglBuffer( uint[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( uint ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length * sizeof( uint ) );
            Marshal.Copy( (int[])(Array)data, 0, ptrData, data.Length );
            
            glBufferData( target, sizeof( uint ) * data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }

        public static uint csglBuffer( int[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( int ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length * sizeof( int ) );
            Marshal.Copy( data, 0, ptrData, data.Length );

            glBufferData( target, sizeof( int ) * data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }

        public static uint csglBuffer( ulong[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( ulong ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length * sizeof( ulong ) );
            Marshal.Copy( (long[])(Array)data, 0, ptrData, data.Length );

            glBufferData( target, sizeof( ulong ) * data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }

        public static uint csglBuffer( long[] data, uint buffer = 0, uint target = GL_ARRAY_BUFFER, uint usage = GL_STATIC_DRAW )
        {
            uint BUFF = buffer;

            if ( BUFF == 0 )
                glGenBuffers( 1, ref BUFF );

            glBindBuffer( target, BUFF );

#if UNSAFE 
            unsafe
            {
                fixed ( void* ptrData = data )
                    glBufferData( target, sizeof( long ) * data.Length, (IntPtr)ptrData, usage );
            }
#else
            IntPtr ptrData = Marshal.AllocHGlobal( data.Length * sizeof( long ) );
            Marshal.Copy( data, 0, ptrData, data.Length );

            glBufferData( target, sizeof( long ) * data.Length, ptrData, usage );

            Marshal.FreeHGlobal( ptrData );
#endif

            return BUFF;
        }
        #endregion

        #region csglShader
        public static uint csglShader( IntPtr shaderSource, uint type, int length = 0 )
        {
            #region Compile
            uint shader = glCreateShader( type );

            glShaderSource( shader, 1, ref shaderSource, ref length );
            glCompileShader( shader );
            #endregion

            #region Assert
            int success = 0;
            glGetShaderiv( shader, GL_COMPILE_STATUS, ref success );

            if ( success == 0 )
            {
                IntPtr log = Marshal.AllocHGlobal( 512 );
                glGetShaderInfoLog( shader, 512, ref length, log );

                byte[] buffer = new byte[ length ];
                Marshal.Copy( log, buffer, 0, length );
                Marshal.FreeHGlobal( log );

                throw new Exception( Encoding.ASCII.GetString( buffer ) );
            }
            #endregion

            return shader;
        }

        public static uint csglShader( byte[] shaderSource, uint type )
        {
            IntPtr ptrSource = Marshal.AllocHGlobal( shaderSource.Length );
            Marshal.Copy( shaderSource, 0, ptrSource, shaderSource.Length );

            uint shader = csglShader( ptrSource, type, shaderSource.Length );

            Marshal.FreeHGlobal( ptrSource );

            return shader;
        }

        public static uint csglShader( string shaderSource, uint type )
        {
            return csglShader( Encoding.ASCII.GetBytes( shaderSource ), type );
        }

        public static uint csglShaderFile( string filename, uint type, bool ascii = true )
        {
            if ( ascii )
                return csglShader( File.ReadAllBytes( filename ), type );
            else
                return csglShader( File.ReadAllText( filename ), type );
        }

        public static uint csglShaderProgram( params uint[] shaders )
        {
            #region Link
            uint shaderProgram = glCreateProgram();

            foreach ( uint shader in shaders )
                glAttachShader( shaderProgram, shader );

            glLinkProgram( shaderProgram );
            #endregion

            #region Assert
            int success = 0;
            glGetProgramiv( shaderProgram, GL_LINK_STATUS, ref success );

            if ( success == 0 )
            {
                int length = 0;
                IntPtr log = Marshal.AllocHGlobal( 512 );
                glGetProgramInfoLog( shaderProgram, 512, ref length, log );

                byte[] buffer = new byte[ length ];
                Marshal.Copy( log, buffer, 0, length );
                Marshal.FreeHGlobal( log );

                throw new Exception( System.Text.Encoding.ASCII.GetString( buffer ) );
            }
            #endregion

            #region Clean
            foreach ( uint shader in shaders )
                glDeleteShader( shader );
            #endregion

            return shaderProgram;
        }
        #endregion

        #region csglVertex
        public static void csglVertexAttribPointer( uint index, int size, uint type, bool normalized, int stride, int offset = 0 )
        {
            glVertexAttribPointer( index, size, type, normalized ? GL_TRUE : GL_FALSE, stride, NULL + offset );
        }

        public static void csglVertexAttribPointer( uint index, int size, uint type, int stride, int offset = 0 )
        {
            glVertexAttribPointer( index, size, type, GL_FALSE, stride, NULL + offset );
        }
        #endregion

        #region csglTexture
        private static Stack<uint> _availTexPtrs = new Stack<uint>();

        public static uint csglTexture( int width, int height, IntPtr pixels, uint externalFormat = GL_RGBA, bool generateMipmaps = true, uint texture = 0 )
        {
            if ( texture == 0 )
            {
                if ( _availTexPtrs.Count > 0 )
                    texture = _availTexPtrs.Pop();
                else
                    glGenTextures( 1, ref texture );
            }

            glBindTexture( GL_TEXTURE_2D, texture );

            if ( CSGL_GLVERSION < 300 & generateMipmaps )
            {
                glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAX_LEVEL, 4 );
                glTexParameteri( GL_TEXTURE_2D, 0x8191, GL_TRUE );
            }

            glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int)GL_CLAMP_TO_EDGE );
            glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int)GL_CLAMP_TO_EDGE );
            
            if( generateMipmaps )
                glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR_MIPMAP_NEAREST );
            else
                glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR );

            glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_NEAREST );

            glTexImage2D( GL_TEXTURE_2D, 0, (int)GL_RGBA, width, height, 0, externalFormat, GL_UNSIGNED_BYTE, pixels );

            if( CSGL_GLVERSION > 210 & generateMipmaps )
                glGenerateMipmap( GL_TEXTURE_2D );

            csglAssert();

            return texture;
        }

        public static void csglTextureClear( uint texture )
        {
            _availTexPtrs.Push( texture );
        }
        #endregion

        #region csglClone
        public static byte[] csglClone( byte[] from )
        {
            byte[] result = new byte[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length );

            return result;
        }

        public static ushort[] csglClone( ushort[] from )
        {
            ushort[] result = new ushort[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length * sizeof( ushort ) );

            return result;
        }

        public static short[] csglClone( short[] from )
        {
            short[] result = new short[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length * sizeof( short ) );

            return result;
        }

        public static uint[] csglClone( uint[] from )
        {
            uint[] result = new uint[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length * sizeof( uint ) );

            return result;
        }

        public static int[] csglClone( int[] from )
        {
            int[] result = new int[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length * sizeof( int ) );

            return result;
        }

        public static ulong[] csglClone( ulong[] from )
        {
            ulong[] result = new ulong[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length * sizeof( ulong ) );

            return result;
        }

        public static long[] csglClone( long[] from )
        {
            long[] result = new long[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length * sizeof( long ) );

            return result;
        }

        public static float[] csglClone( float[] from )
        {
            float[] result = new float[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length * sizeof( float ) );

            return result;
        }

        public static double[] csglClone( double[] from )
        {
            double[] result = new double[ from.Length ];
            Buffer.BlockCopy( from, 0, result, 0, from.Length * sizeof( double ) );

            return result;
        }

        public static void csglClone( byte[] from, byte[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length );
        }

        public static void csglClone( ushort[] from, ushort[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length * sizeof( ushort ) );
        }

        public static void csglClone( short[] from, short[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length * sizeof( short ) );
        }

        public static void csglClone( uint[] from, uint[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length * sizeof( uint ) );
        }

        public static void csglClone( int[] from, int[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length * sizeof( int ) );
        }

        public static void csglClone( ulong[] from, ulong[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length * sizeof( ulong ) );
        }

        public static void csglClone( long[] from, long[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length * sizeof( long ) );
        }

        public static void csglClone( float[] from, float[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length * sizeof( float ) );
        }

        public static void csglClone( double[] from, double[] to )
        {
            Buffer.BlockCopy( from, 0, to, 0, to.Length * sizeof( double ) );
        }
        #endregion
        #endregion
        #endregion

        #region Math
        #region Unsafe
#if UNSAFE
        public static float csglFastSqrt( float value )
        {
            float result = value;

            unsafe
            {
                int tmp = *(int*)&result;

                tmp -= 1 << 23;
                tmp >>= 1;
                tmp += 1 << 29;

                result = *(float*)&tmp;
            }
            return result;
        }

        public static float csglFastInvSqrt( float value )
        {
            float x = value;

            unsafe
            {
                float xhalf = 0.5f*x;

                int i = *(int*)&x;
                i = 0x5f3759df - ( i >> 1 );
                x = *(float*)&i;
                x = x * ( 1.5f - xhalf * x * x );
            }

            return x;
        }
#endif
        #endregion

        #region Safe
#if !UNSAFE
        public static float csglFastSqrt( float value ) { return (float)Math.Sqrt( value ); }

        public static float csglFastInvSqrt( float value ) { return (float)( 1.0 / Math.Sqrt( value ) ); }
#endif
        #endregion

        #region Vector
        #region Vector scalar
        public static void csglVectorAdd( float[] vector, float value )
        {
            for ( int i = 0; i < vector.Length; i++ )
                vector[ i ] += value;
        }

        public static void csglVectorSub( float[] vector, float value )
        {
            for ( int i = 0; i < vector.Length; i++ )
                vector[ i ] -= value;
        }

        public static void csglVectorMul( float[] vector, float value )
        {
            for ( int i = 0; i < vector.Length; i++ )
                vector[ i ] *= value;
        }

        public static void csglVectorDiv( float[] vector, float value )
        {
            for ( int i = 0; i < vector.Length; i++ )
                vector[ i ] /= value;
        }
        #endregion

        #region Vector vector
        public static void csglVectorAdd( float[] vector, float[] value )
        {
            for ( int i = 0; i < vector.Length; i++ )
                vector[ i ] += value[ i ];
        }

        public static void csglVectorSub( float[] vector, float[] value )
        {
            for ( int i = 0; i < vector.Length; i++ )
                vector[ i ] -= value[ i ];
        }

        public static void csglVectorMul( float[] vector, float[] value )
        {
            for ( int i = 0; i < vector.Length; i++ )
                vector[ i ] *= value[ i ];
        }

        public static void csglVectorDiv( float[] vector, float[] value )
        {
            for ( int i = 0; i < vector.Length; i++ )
                vector[ i ] /= value[ i ];
        }
        #endregion

        #region Vector misc
        public static float csglVectorLength( float[] vector )
        {
            float pow = 0;

            for ( int i = 0; i < vector.Length; i++ )
                pow += vector[ i ] * vector[ i ];

            return csglFastSqrt( pow );
        }

        public static float csglVectorDot( float[] v1, float[] v2 )
        {
            float dot = 0;
            int min = v1.Length < v2.Length ? v1.Length : v2.Length;

            for ( int i = 0; i < min; i++ )
                dot += v1[ i ] * v2[ i ];

            return dot;
        }

        public static float[] csglVector3Cross( float[] v1, float[] v2 )
        {
            return new float[ 3 ]
            {
                v1[ 1 ] * v2[ 2 ] - v1[ 2 ] * v2[ 1 ],
                v1[ 2 ] * v2[ 0 ] - v1[ 0 ] * v2[ 2 ],
                v1[ 0 ] * v2[ 1 ] - v1[ 1 ] * v2[ 0 ]
            };
        }
        #endregion
        #endregion

        #region Matrix
        #region Identity
        public static float[] csglIdentity2()
        {
            return new float[ 4 ]
            {
                1, 0,
                0, 1
            };
        }

        public static float[] csglIdentity3()
        {
            return new float[ 9 ]
            {
                1, 0, 0,
                0, 1, 0,
                0, 0, 1
            };
        }

        public static float[] csglIdentity4()
        {
            return new float[ 16 ]
            {
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            };
        }
        #endregion

        #region Matrix scalar
        // Vector scalar rules apply
        #endregion

        #region Matrix matrix
        public static float[] csglMatrixMul( float[] m1, float[] m2, int stride )
        {
            int rows = (int)( m1.Length / (float)stride );

            int cols0 = (int)( 1 / ( (float)stride / m2.Length ) );
            int cols1 = (int)( 1 / ( (float)stride / m1.Length ) );

            if ( rows != cols0 )
                return new float[ 0 ] { };

            float[] result = new float[ rows * cols0 ];

            for ( int i = 0; i < rows * cols0; i++ )
                result[ i ] = 0f;

            for ( int i = 0; i < rows; i++ )
            {
                for ( int j = 0; j < cols0; j++ )
                {
                    for ( int k = 0; k < cols1; k++ )
                        result[ i * stride + j ] += m1[ i * stride + k ] * m2[ k * stride + j ];
                }
            }

            return result;
        }
        #endregion

        #region Matrix misc
        public static void csglMatrixColumn( float[] source, ref float[] destination, int stride, int column )
        {
            for ( int i = stride * column; i < stride * column + stride; i++ )
                destination[ i ] = source[ i ];
        }

        public static float[] csglMatrixVector( float[] matrix, float[] vector, int stride )
        {
            int columns = (int)( 1 / ( vector.Length / (float) stride ) );

            float[] result = new float[ stride ];

            for ( int i = 0; i < columns; i++ )
            {
                float column = 0;

                for ( int j = 0; j < stride; j++ )
                    column += matrix[ i * j + i ] * vector[ j ];

                result[ i ] = column;
            }

            return result;
        }

        public static void csglMatrixScale( float[] matrix, float[] vector, int stride )
        {
            int rows = (int)( matrix.Length / (float)stride );

            for ( int i = 0; i < rows; i++ )
            {
                for ( int j = 0; j < stride; j++ )
                    matrix[ i * stride + j ] *= ( i < vector.Length ? vector[ i ] : 1 );
            }
        }

        public static void csglMatrixTranslate( float[] matrix, float[] vector, int stride )
        {
            int last = matrix.Length - stride;
            int min = vector.Length < matrix.Length ? vector.Length : matrix.Length;

            for ( int i = 0; i < min; i++ )
            {
                for ( int j = 0; j < stride; j++ )
                    matrix[ last + j ] += matrix[ i * stride + j ] * vector[ i ];
            }
        }

        public static float[] csglMatrixOrtho( float left, float right, float bottom, float top, float zNear = 0f, float zFar = 1f )
        {
            return new float[ 16 ]
            {
                2f / ( right - left ), 0, 0, 0,
                0, 2f / ( top - bottom ), 0, 0,
                0, 0, -2f / ( zFar - zNear ), 0,
                -( right + left )/( right - left ), -( top + bottom )/( top - bottom ), -( zFar + zNear )/( zFar - zNear ), 1
            };
        }
        #endregion

        #endregion
        #endregion
    }

    public class CSGLWindow
    {
        #region Fields
        private IntPtr _glfwWindow;
        public IntPtr Pointer { get { return _glfwWindow; } }

        private double _lastDrawTime;
        private double _lastUpdatetime;

        #region Abstracted
        private int _x;
        public int X
        {
            get { return _x; }

            set
            {
                _x = value;
                glfwSetWindowPos( _glfwWindow, value, _y );
            }
        }

        private int _y;
        public int Y
        {
            get { return _y; }

            set
            {
                _y = value;
                glfwSetWindowPos( _glfwWindow, _x, value );
            }
        }

        private int _width;
        public int Width
        {
            get { return _width; }

            set
            {
                _width = value;
                glfwSetWindowSize( _glfwWindow, value, _height );
            }
        }

        private int _height;
        public int Height
        {
            get { return _height; }

            set
            {
                _height = value;
                glfwSetWindowSize( _glfwWindow, _width, value );
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }

            set
            {
                _title = value;
                glfwSetWindowTitle( _glfwWindow, value );
            }
        }

        private CSGLWindowStyle _style;
        public CSGLWindowStyle Style
        {
            get { return _style; }

            set
            {
                // TODO: Replace as soon as GLFW 3.3 gets released

                _style = value;

                bool borderless = ( value & CSGLWindowStyle.Borderless ) != 0;
                bool fullscreen = ( value & CSGLWindowStyle.Fullscreen ) != 0;

                glfwWindowHint( GLFW_DECORATED, borderless ? GL_FALSE : GL_TRUE );

                if ( fullscreen )
                {
                    if ( borderless )
                    {
                        IntPtr _tempWindow = IntPtr.Zero;
                        IntPtr monitor = glfwGetPrimaryMonitor();
                        IntPtr mode = glfwGetVideoMode( monitor );

#if UNSAFE
                        unsafe
                        {
                            GLFWvidmode* ptrMode = (GLFWvidmode*)mode;

                            glfwWindowHint( GLFW_REFRESH_RATE, ptrMode->refreshRate );

                            _tempWindow = glfwCreateWindow( ptrMode->width, ptrMode->height, _title, monitor, _glfwWindow );

                            _width = ptrMode->width;
                            _height = ptrMode->height;
                        }
#else
                        GLFWvidmode vidmode = Marshal.PtrToStructure<GLFWvidmode>( mode );

                        glfwWindowHint( GLFW_REFRESH_RATE, vidmode.refreshRate );

                        _tempWindow = glfwCreateWindow( vidmode.width, vidmode.height, _title, monitor, _glfwWindow );

                        _width = vidmode.width;
                        _height = vidmode.height;
#endif

                        glfwDestroyWindow( _glfwWindow );
                        _glfwWindow = _tempWindow;

                        glfwMakeContextCurrent( _glfwWindow );
                        _setCallbacks();

                        glViewport( 0, 0, _width, _height );
                    }
                    else
                    {
                        IntPtr _tempWindow = glfwCreateWindow( _width, _height, _title, glfwGetPrimaryMonitor(), _glfwWindow );
                        glfwDestroyWindow( _glfwWindow );
                        _glfwWindow = _tempWindow;

                        glfwMakeContextCurrent( _glfwWindow );
                        _setCallbacks();

                        glViewport( 0, 0, _width, _height );
                    }
                }
                else
                {
                    IntPtr _tempWindow = glfwCreateWindow( _width, _height, _title, IntPtr.Zero, _glfwWindow );
                    glfwDestroyWindow( _glfwWindow );
                    _glfwWindow = _tempWindow;

                    glfwMakeContextCurrent( _glfwWindow );
                    _setCallbacks();

                    glViewport( 0, 0, _width, _height );
                }
            }
        }
#endregion

#region Events
        public event GLFWkeyfun OnKeyboard;
        public event GLFWcursorposfun OnCursorMoved;
        public event GLFWcursorenterfun OnCursorEnteredLeft;
        public event GLFWmousebuttonfun OnMouse;
        public event GLFWscrollfun OnScroll;

        public CSGLDrawEvent OnDraw;
        public CSGLUpdateEvent OnUpdate;
#endregion
#endregion

#region Constructor
        public CSGLWindow( int width = 640, int height = 480, string title = "CSGLWindow" )
        {
            glfwWindowHint( GLFW_RESIZABLE, GL_FALSE );

            _glfwWindow = glfwCreateWindow( width, height, title, IntPtr.Zero, IntPtr.Zero );
            glfwMakeContextCurrent( _glfwWindow );

            glfwShowWindow( _glfwWindow );

            if ( CSGL_GLVERSION == 0 )
                csglLoadGL();

            glViewport( 0, 0, width, height );

            // Initialize fields
            _x = 0;
            _y = 0;
            _width = width;
            _height = height;
            _title = title;
        }
#endregion

#region Destructor
        ~CSGLWindow()
        {
            
        }
#endregion

#region Methods
        public void MakeContextCurrent()
        {
            glfwMakeContextCurrent( _glfwWindow );
        }

        private void _setCallbacks()
        {
            glfwSetKeyCallback( _glfwWindow, OnKeyboard );
            glfwSetCursorPosCallback( _glfwWindow, OnCursorMoved );
            glfwSetCursorEnterCallback( _glfwWindow, OnCursorEnteredLeft );
            glfwSetMouseButtonCallback( _glfwWindow, OnMouse );
            glfwSetScrollCallback( _glfwWindow, OnScroll );
        }

        public void Run()
        {
            _setCallbacks();

            _lastDrawTime = glfwGetTime();
            _lastUpdatetime = glfwGetTime();

            while ( glfwWindowShouldClose( _glfwWindow ) == 0 )
            {
                glClear( GL_COLOR_BUFFER_BIT );

                OnUpdate?.Invoke( this, glfwGetTime() - _lastUpdatetime );
                _lastUpdatetime = glfwGetTime();

                OnDraw?.Invoke( this, glfwGetTime() - _lastDrawTime );
                _lastDrawTime = glfwGetTime();

                Thread.Sleep( 1 );

                glfwSwapBuffers( _glfwWindow );
                glfwPollEvents();
            }
        }

        public void Dispose()
        {
            glfwDestroyWindow( _glfwWindow );
        }
#endregion
    }
}
#endregion
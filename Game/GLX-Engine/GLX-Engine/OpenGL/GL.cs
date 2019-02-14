using System;
using System.Security;
using System.Runtime.InteropServices;

namespace GLXEngine.OpenGL
{

    public partial class GL
    {

        //----------------------------------------------------------------------------------------------------------------------
        //														openGL
        //----------------------------------------------------------------------------------------------------------------------
        public const int GL_TEXTURE_2D = 0x0DE1;
        public const int GL_BLEND = 0x0BE2;
        public const int GL_MODELVIEW = 0x1700;
        public const int GL_PROJECTION = 0x1701;
        public const int GL_COLOR_BUFFER_BIT = 0x4000;
        public const int GL_QUADS = 0x0007;
        public const int GL_TRIANGLES = 0x0004;
        public const int GL_LINES = 0x0001;
        public const int GL_TEXTURE_MIN_FILTER = 0x2801;
        public const int GL_TEXTURE_MAG_FILTER = 0x2800;
        public const int GL_LINEAR = 0x2601;
        public const int GL_TEXTURE_WRAP_S = 0x2802;
        public const int GL_TEXTURE_WRAP_T = 0x2803;
        public const int GL_CLAMP = 0x2900;
        public const int GL_CLAMP_TO_EDGE_EXT = 0x812F;
        public const int GL_RGBA = 0x1908;
        public const int GL_BGRA = 0x80E1;
        public const int GL_UNSIGNED_BYTE = 0x1401;
        public const int GL_PERSPECTIVE_CORRECTION = 0x0C50;
        public const int GL_FASTEST = 0x1101;
        public const int GL_NICEST = 0x1102;
        public const int GL_NEAREST = 0x2600;
        public const int GL_POLYGON_SMOOTH = 0x0B41;
        public const int GL_LINE_SMOOTH = 0x0B20;
        public const int GL_MULTISAMPLE = 0x809D;
        public const int GL_FLOAT = 0x1406;
        public const int GL_UNSIGNED_INT = 0x1405;
        public const int GL_VERTEX_ARRAY = 0x8074;
        public const int GL_INT = 0x1404;
        public const int GL_DOUBLE = 0x140A;
        public const int GL_INDEX_ARRAY = 0x8077;
        public const int GL_TEXTURE_COORD_ARRAY = 0x8078;
        public const int GL_SCISSOR_TEST = 0x0C11;
        public const int GL_MAX_TEXTURE_SIZE = 0x0D33;
        public const int GL_ZERO = 0x0000;
        public const int GL_ONE = 0x0001;
        public const int GL_SRC_COLOR = 0x0300;
        public const int GL_ONE_MINUS_SRC_COLOR = 0x0301;
        public const int GL_DST_COLOR = 0x0306;
        public const int GL_ONE_MINUS_DST_COLOR = 0x0307;
        public const int GL_SRC_ALPHA = 0x0302;
        public const int GL_ONE_MINUS_SRC_ALPHA = 0x0303;
        public const int GL_DST_ALPHA = 0x0304;
        public const int GL_ONE_MINUS_DST_ALPHA = 0x0305;
        public const int GL_CONSTANT_COLOR = 0x8001;
        public const int GL_ONE_MINUS_CONSTANT_COLOR = 0x8002;
        public const int GL_CONSTANT_ALPHA = 0x8003;
        public const int GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004;
        public const int GL_SRC_ALPHA_SATURATE = 0x0308;
        public const int GL_MIN = 0x8007;
        public const int GL_MAX = 0x8008;
        public const int GL_FUNC_ADD = 0x8006;
        public const int GL_FUNC_SUBTRACT = 0x800A;
        public const int GL_FUNC_REVERSE_SUBTRACT = 0x800B;
        public const int GL_REPEAT = 0x2901;
        public const int GL_COLOR_ARRAY = 0x8076;

        [DllImport("opengl32.dll")]
        public static extern void glEnable(int cap);
        [DllImport("opengl32.dll")]
        public static extern void glDisable(int cap);
        [DllImport("opengl32.dll")]
        public static extern void glBlendFunc(int sourceFactor, int destFactor);

        [DllImport("opengl32.dll")]
        public static extern void glBlendEquation(int mode);

        [DllImport("opengl32.dll")]
        public static extern void glClearColor(float r, float g, float b, float a);

        [DllImport("opengl32.dll")]
        public static extern void glMatrixMode(int mode);

        [DllImport("opengl32.dll")]
        public static extern void glLoadIdentity();

        [DllImport("opengl32.dll")]
        public static extern void glOrtho(double left, double right, double top, double bottom, double near, double far);

        [DllImport("opengl32.dll")]
        public static extern void glClear(int mask);

        [DllImport("opengl32.dll")]
        public static extern void glPushMatrix();

        [DllImport("opengl32.dll")]
        public static extern void glMultMatrixf(float[] matrix);

        [DllImport("opengl32.dll")]
        public static extern void glPopMatrix();

        [DllImport("opengl32.dll")]
        public static extern void glBegin(int mode);



        [DllImport("opengl32.dll")]
        public static extern void glTexCoord2f(float u, float v);

        [DllImport("opengl32.dll")]
        public static extern void glVertex2f(float x, float y);

        [DllImport("opengl32.dll")]
        public static extern void glVertex3f(float x, float y, float z);

        [DllImport("opengl32.dll")]
        public static extern void glEnd();

        [DllImport("opengl32.dll")]
        public static extern void glBindTexture(int target, int texture);

        [DllImport("opengl32.dll")]
        public static extern void glGenTextures(int count, int[] textures);

        [DllImport("opengl32.dll")]
        public static extern void glTexParameteri(int target, int name, int value);

        [DllImport("opengl32.dll")]
        public static extern void glTexImage2D(int target, int level, int internalFormat, int width, int height,
                                             int border, int format, int type, IntPtr pixels);

        [DllImport("opengl32.dll")]
        public static extern void glDeleteTextures(int count, int[] textures);

        [DllImport("opengl32.dll")]
        public static extern void glFinish();

        [DllImport("opengl32.dll")]
        public static extern void glHint(int target, int mode);

        [DllImport("opengl32.dll")]
        public static extern void glViewport(int x, int y, int width, int height);

        [DllImport("opengl32.dll")]
        public static extern void glScissor(int x, int y, int width, int height);

        [DllImport("opengl32.dll")]
        public static extern void glVertexPointer(int size, int type, int stride, float[] pointer);

        [DllImport("opengl32.dll")]
        public static extern void glColorPointer(int size, int type, int stride, float[] pointer);

        [DllImport("opengl32.dll")]
        public static extern void glTexCoordPointer(int size, int type, int stride, float[] pointer);
        
        [DllImport("opengl32.dll")]
        public static extern void glDrawElements(int mode, int count, int type, int[] indices);

        [DllImport("opengl32.dll")]
        public static extern void glEnableClientState(int array);

        [DllImport("opengl32.dll")]
        public static extern void glArrayElement(int element);

        [DllImport("opengl32.dll")]
        public static extern void glDrawArrays(int mode, int offset, int count);

        [DllImport("opengl32.dll")]
        public static extern void glDisableClientState(int state);

        [DllImport("opengl32.dll")]
        public static extern int glGetError();

        [DllImport("opengl32.dll")]
        public static extern void glGetIntegerv(int name, int[] param);

        [DllImport("opengl32.dll")]
        public static extern void glLineWidth(float width);

        //----------------------------------------------------------------------------------------------------------------------
        //														GLFW
        //----------------------------------------------------------------------------------------------------------------------

        public const int GLFW_OPENED = 0x00020001;
        public const int GLFW_WINDOWED = 0x00010001;
        public const int GLFW_FULLSCREEN = 0x00010002;
        public const int GLFW_ACTIVE = 0x00020001;
        public const int GLFW_MOUSE_CURSOR = 0x00030001;
        public const int GLFW_TRUE = 1;
        public const int GLFW_FALSE = 0;

        public const int GLFW_NO_WINDOW_CONTEXT = 0x0001000A;

        public const int GLFW_FOCUSED = 0x00020001;
        public const int GLFW_ICONIFIED = 0x00020002;
        public const int GLFW_RESIZABLE = 0x00020003;
        public const int GLFW_VISIBLE = 0x00020004;
        public const int GLFW_DECORATED = 0x00020005;
        public const int GLFW_AUTO_ICONIFY = 0x00020006;
        public const int GLFW_FLOATING = 0x00020007;
        public const int GLFW_MAXIMIZED = 0x00020008;

        public const int GLFW_RED_BITS = 0x00021001;
        public const int GLFW_GREEN_BITS = 0x00021002;
        public const int GLFW_BLUE_BITS = 0x00021003;
        public const int GLFW_ALPHA_BITS = 0x00021004;
        public const int GLFW_DEPTH_BITS = 0x00021005;
        public const int GLFW_STENCIL_BITS = 0x00021006;
        public const int GLFW_ACCUM_RED_BITS = 0x00021007;
        public const int GLFW_ACCUM_GREEN_BITS = 0x00021008;
        public const int GLFW_ACCUM_BLUE_BITS = 0x00021009;
        public const int GLFW_ACCUM_ALPHA_BITS = 0x0002100A;
        public const int GLFW_AUX_BUFFERS = 0x0002100B;
        public const int GLFW_STEREO = 0x0002100C;
        public const int GLFW_SAMPLES = 0x0002100D;
        public const int GLFW_SRGB_CAPABLE = 0x0002100E;
        public const int GLFW_REFRESH_RATE = 0x0002100F;
        public const int GLFW_DOUBLEBUFFER = 0x00021010;

        public const int GLFW_CLIENT_API = 0x00022001;
        public const int GLFW_CONTEXT_VERSION_MAJOR = 0x00022002;
        public const int GLFW_CONTEXT_VERSION_MINOR = 0x00022003;
        public const int GLFW_CONTEXT_REVISION = 0x00022004;
        public const int GLFW_CONTEXT_ROBUSTNESS = 0x00022005;
        public const int GLFW_OPENGL_FORWARD_COMPAT = 0x00022006;
        public const int GLFW_OPENGL_DEBUG_CONTEXT = 0x00022007;
        public const int GLFW_OPENGL_PROFILE = 0x00022008;
        public const int GLFW_CONTEXT_RELEASE_BEHAVIOR = 0x00022009;
        public const int GLFW_CONTEXT_NO_ERROR = 0x0002200A;
        public const int GLFW_CONTEXT_CREATION_API = 0x0002200B;

        public const int GLFW_NO_API = 0;
        public const int GLFW_OPENGL_API = 0x00030001;
        public const int GLFW_OPENGL_ES_API = 0x00030002;

        public const int GLFW_NO_ROBUSTNESS = 0;
        public const int GLFW_NO_RESET_NOTIFICATION = 0x00031001;
        public const int GLFW_LOSE_CONTEXT_ON_RESET = 0x00031002;

        public const int GLFW_OPENGL_ANY_PROFILE = 0;
        public const int GLFW_OPENGL_CORE_PROFILE = 0x00032001;
        public const int GLFW_OPENGL_COMPAT_PROFILE = 0x00032002;

        public const int GLFW_CURSOR = 0x00033001;
        public const int GLFW_STICKY_KEYS = 0x00033002;
        public const int GLFW_STICKY_MOUSE_BUTTONS = 0x00033003;

        public const int GLFW_CURSOR_NORMAL = 0x00034001;
        public const int GLFW_CURSOR_HIDDEN = 0x00034002;
        public const int GLFW_CURSOR_DISABLED = 0x00034003;

        public const int GLFW_ANY_RELEASE_BEHAVIOR = 0;
        public const int GLFW_RELEASE_BEHAVIOR_FLUSH = 0x00035001;
        public const int GLFW_RELEASE_BEHAVIOR_NONE = 0x00035002;

        public const int GLFW_NATIVE_CONTEXT_API = 0x00036001;
        public const int GLFW_EGL_CONTEXT_API = 0x00036002;

        public const int GLFW_ARROW_CURSOR = 0x00036001;

        public const int GLFW_IBEAM_CURSOR = 0x00036002;

        public const int GLFW_CROSSHAIR_CURSOR = 0x00036003;

        public const int GLFW_HAND_CURSOR = 0x00036004;

        public const int GLFW_HRESIZE_CURSOR = 0x00036005;

        public const int GLFW_VRESIZE_CURSOR = 0x00036006;

        public const int GLFW_CONNECTED = 0x00040001;
        public const int GLFW_DISCONNECTED = 0x00040002;

        public const int GLFW_DONT_CARE = -1;

        public enum GlfwError
        {
            NoError = 0,
            NotInitialized = 0x10001,
            NoCurrentContext,
            InvalidEnum,
            InvalidValue,
            OutOfMemory,
            APIUnavailable,
            VersionUnavailable,
            PlatformError,
            FormatUnavailable,
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public delegate void GLFWFrameBufferSizeCallback(IntPtr window, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public delegate void GLFWKeyCallback(int key, int action);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public delegate void GlfwErrorCallback(GlfwError code, [MarshalAs(UnmanagedType.LPStr)] string desc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public delegate void GLFWMouseButtonCallback(int button, int action);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public delegate void GLFWJoyStickCallback(int joystick, int action);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwSetTime(double time);
        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double glfwGetTime();

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwPollEvents();

        public static int glfwGetWindowAttrib(GLFWWindow? a_window, int a_param)
            => glfwGetWindowAttrib(a_window.HasValue ? a_window.Value.Ptr : IntPtr.Zero, a_param);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int glfwGetWindowAttrib(IntPtr a_window, int a_param);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int glfwInit();

        public static GLFWWindow glfwCreateWindow(int a_width, int a_height, string a_title, GLFWMonitor? a_monitor = null, GLFWWindow? a_share = null)
            => glfwCreateWindow(a_width, a_height, a_title, a_monitor.HasValue ? a_monitor.Value.Ptr : IntPtr.Zero, a_share.HasValue ? a_share.Value.Ptr : IntPtr.Zero);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Struct)]
        static extern GLFWWindow glfwCreateWindow(int a_width, int a_height, [MarshalAs(UnmanagedType.LPStr)] string a_title, IntPtr a_monitor, IntPtr a_share);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwSetWindowTitle(string title);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwSwapInterval(int a_interval);

        public static void glfwSetFramebufferSizeCallback(GLFWWindow? window, GLFWFrameBufferSizeCallback callback)
            => glfwSetFramebufferSizeCallback(window.HasValue ? window.Value.Ptr : IntPtr.Zero, callback);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwSetFramebufferSizeCallback(IntPtr window, GLFWFrameBufferSizeCallback callback);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwCloseWindow();
        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwTerminate();

        public static void glfwMakeContextCurrent(GLFWWindow? window)
            => glfwMakeContextCurrent(window.HasValue ? window.Value.Ptr : IntPtr.Zero);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void glfwMakeContextCurrent(IntPtr window);

        public static void glfwSwapBuffers(GLFWWindow? window)
            => glfwSwapBuffers(window.HasValue ? window.Value.Ptr : IntPtr.Zero);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void glfwSwapBuffers(IntPtr window);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool glfwGetKey(int key);

        public static void glfwSetKeyCallback(GLFWWindow? window, GLFWKeyCallback callback)
            => glfwSetKeyCallback(window.HasValue ? window.Value.Ptr : IntPtr.Zero, callback);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwSetKeyCallback(IntPtr window, GLFWKeyCallback callback);

        public static void glfwSetFramebufferSizeCallback(GLFWWindow? window, GLFWKeyCallback callback)
            => glfwSetFramebufferSizeCallback(window.HasValue ? window.Value.Ptr : IntPtr.Zero, callback);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwSetFramebufferSizeCallback(IntPtr window, GLFWKeyCallback callback);


        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern GlfwErrorCallback glfwSetErrorCallback(GlfwErrorCallback callback);
        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwSetJoystickCallback(GLFWJoyStickCallback callback);
        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwWindowHint(int name, int value);
        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool glfwGetCursorPos(out int x, out int y);
        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwSetMouseButtonCallback(GLFWMouseButtonCallback callback);
        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwEnable(int property);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void glfwDisable(int property);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Struct)]
        public static extern GLFWMonitor glfwGetPrimaryMonitor();

        public static GLFWVideoMode glfwGetVideoMode(GLFWMonitor? a_monitor)
            => glfwGetVideoMode(a_monitor.HasValue ? a_monitor.Value.Ptr : IntPtr.Zero);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Struct)]
        public static extern GLFWVideoMode glfwGetVideoMode(IntPtr a_monitor);

        public static bool glfwWindowShouldClose(GLFWWindow? window)
            => glfwWindowShouldClose(window.HasValue ? window.Value.Ptr : IntPtr.Zero);

        [DllImport("lib/glfw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool glfwWindowShouldClose(IntPtr window);

    }
}

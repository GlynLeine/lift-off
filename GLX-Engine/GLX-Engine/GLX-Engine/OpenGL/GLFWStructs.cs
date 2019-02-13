using System;
using System.Security;
using System.Runtime.InteropServices;

namespace GLXEngine.OpenGL
{
    public partial class GL
    {
        /// <summary>
        /// <para>Opaque window object.</para>
        /// </summary>
        /// <seealso cref="CreateWindow(int, int, string, GLFWMonitor, GLFWWindow)"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct GLFWWindow : IEquatable<GLFWWindow>
        {
            /// <summary>
            /// <para>Null window pointer.</para>
            /// </summary>
            public static readonly GLFWWindow None = new GLFWWindow(IntPtr.Zero);

            /// <summary>
            /// Pointer to an internal GLFWwindow.
            /// </summary>
            public IntPtr Ptr;

            internal GLFWWindow(IntPtr ptr)
            {
                Ptr = ptr;
            }

            public override bool Equals(object obj)
            {
                if (obj is GLFWWindow)
                    return Equals((GLFWWindow)obj);

                return false;
            }

            public bool Equals(GLFWWindow obj) => Ptr == obj.Ptr;

            public override string ToString() => Ptr.ToString();

            public override int GetHashCode() => Ptr.GetHashCode();

            public static bool operator ==(GLFWWindow a, GLFWWindow b) => a.Equals(b);

            public static bool operator !=(GLFWWindow a, GLFWWindow b) => !a.Equals(b);

            public static implicit operator bool(GLFWWindow obj) => obj.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// <para>Opaque monitor object.</para>
        /// </summary>
        /// <seealso cref="GetMonitors"/>
        /// <seealso cref="GetPrimaryMonitor"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct GLFWMonitor : IEquatable<GLFWMonitor>
        {
            /// <summary>
            /// <para>Null monitor pointer.</para>
            /// </summary>
            public static readonly GLFWMonitor None = new GLFWMonitor(IntPtr.Zero);
            
            /// <summary>
            /// Pointer to an internal GLFWmonitor.
            /// </summary>
            public IntPtr Ptr;

            internal GLFWMonitor(IntPtr ptr)
            {
                Ptr = ptr;
            }

            public override bool Equals(object obj)
            {
                if (obj is GLFWMonitor)
                    return Equals((GLFWMonitor)obj);

                return false;
            }

            public bool Equals(GLFWMonitor obj) => Ptr == obj.Ptr;

            public override string ToString() => Ptr.ToString();

            public override int GetHashCode() => Ptr.GetHashCode();

            public static bool operator ==(GLFWMonitor a, GLFWMonitor b) => a.Equals(b);

            public static bool operator !=(GLFWMonitor a, GLFWMonitor b) => !a.Equals(b);

            public static implicit operator bool(GLFWMonitor obj) => obj.Ptr != IntPtr.Zero;
        }
    }
}

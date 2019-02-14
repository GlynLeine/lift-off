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

        /// <summary>
        /// This describes a single video mode.
        /// </summary>
        /// <seealso cref="GetVideoMode(GLFWMonitor)"/>
        /// <seealso cref="GetVideoModes(GLFWMonitor)"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct GLFWVideoMode : IEquatable<GLFWVideoMode>
        {
            /// <summary>
            /// The width, in screen coordinates, of the video mode.
            /// </summary>
            public int Width;

            /// <summary>
            /// The height, in screen coordinates, of the video mode.
            /// </summary>
            public int Height;

            /// <summary>
            /// The bit depth of the red channel of the video mode.
            /// </summary>
            public int RedBits;

            /// <summary>
            /// The bit depth of the green channel of the video mode.
            /// </summary>
            public int GreenBits;

            /// <summary>
            /// The bit depth of the blue channel of the video mode.
            /// </summary>
            public int BlueBits;

            /// <summary>
            /// The refresh rate, in Hz, of the video mode.
            /// </summary>
            public int RefreshRate;

            public override bool Equals(object obj)
            {
                if (obj is GLFWVideoMode)
                    return Equals((GLFWVideoMode)obj);

                return false;
            }

            public bool Equals(GLFWVideoMode obj)
            {
                return obj.Width == Width
                    && obj.Height == Height
                    && obj.RedBits == RedBits
                    && obj.GreenBits == GreenBits
                    && obj.BlueBits == BlueBits
                    && obj.RefreshRate == RefreshRate;
            }

            public override string ToString()
            {
                return string.Format("VideoMode(width: {0}, height: {1}, redBits: {2}, greenBits: {3}, blueBits: {4}, refreshRate: {5})",
                    Width.ToString(),
                    Height.ToString(),
                    RedBits.ToString(),
                    GreenBits.ToString(),
                    BlueBits.ToString(),
                    RefreshRate.ToString()
                );
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + Width.GetHashCode();
                    hash = hash * 23 + Height.GetHashCode();
                    hash = hash * 23 + RedBits.GetHashCode();
                    hash = hash * 23 + GreenBits.GetHashCode();
                    hash = hash * 23 + BlueBits.GetHashCode();
                    hash = hash * 23 + RefreshRate.GetHashCode();
                    return hash;
                }
            }

            public static bool operator ==(GLFWVideoMode a, GLFWVideoMode b) => a.Equals(b);

            public static bool operator !=(GLFWVideoMode a, GLFWVideoMode b) => !a.Equals(b);
        }
    }
}

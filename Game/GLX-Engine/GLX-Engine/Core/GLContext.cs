using System;
using System.Collections.Generic;
using static GLXEngine.OpenGL.GL;
using GLXEngine.OpenGL;

namespace GLXEngine.Core
{

    class WindowSize
    {
        public static WindowSize instance = new WindowSize();
        public int width, height;
    }

    public class GLContext
    {

        public const int MAXKEYS = 65535;
        public const int MAXBUTTONS = 255;

        public static Dictionary<Key, bool> m_keydown = new Dictionary<Key, bool>();
        public static Dictionary<Key, bool> m_keyup = new Dictionary<Key, bool>();
        public static Dictionary<Key, bool> keys = new Dictionary<Key, bool>();

        private static bool[] buttons = new bool[MAXBUTTONS + 1];
        private static bool[] mousehits = new bool[MAXBUTTONS + 1];
        private static bool[] mouseup = new bool[MAXBUTTONS + 1]; //mouseup kindly donated by LeonB

        public static int mouseX = 0;
        public static int mouseY = 0;

        private Game _owner;
        GLFWWindow m_window;

        private int _targetFrameRate = 60;
        private long _lastFrameTime = 0;
        private long _lastFPSTime = 0;
        private int _frameCount = 0;
        private int _lastFPS = 0;
        private bool _vsyncEnabled = false;

        private static double _realToLogicWidthRatio;
        private static double _realToLogicHeightRatio;

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public GLContext(Game owner)
        {
            _owner = owner;
            _lastFPS = _targetFrameRate;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Width
        //------------------------------------------------------------------------------------------------------------------------
        public int width
        {
            get { return WindowSize.instance.width; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Height
        //------------------------------------------------------------------------------------------------------------------------
        public int height
        {
            get { return WindowSize.instance.height; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														setupWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public void CreateWindow(int width, int height, string name, bool fullScreen, bool vSync, int realWidth, int realHeight, int msaa = 8)
        {
            // This stores the "logical" width, used by all the game logic:
            WindowSize.instance.width = width;
            WindowSize.instance.height = height;
            _realToLogicWidthRatio = (double)realWidth / width;
            _realToLogicHeightRatio = (double)realHeight / height;
            _vsyncEnabled = vSync;

            if (glfwInit() != GLFW_TRUE)
                throw new Exception("Could not initialise GLFW.");

            GLFWMonitor monitor = glfwGetPrimaryMonitor();
            GLFWVideoMode videoMode = glfwGetVideoMode(monitor);

            glfwWindowHint(GLFW_RED_BITS, videoMode.RedBits);
	        glfwWindowHint(GLFW_GREEN_BITS, videoMode.GreenBits);
	        glfwWindowHint(GLFW_BLUE_BITS, videoMode.BlueBits);
	        glfwWindowHint(GLFW_REFRESH_RATE, videoMode.RefreshRate);

            glfwWindowHint(GLFW_SAMPLES, msaa);

            if (fullScreen)
                m_window = glfwCreateWindow(realWidth, realHeight, name, monitor);
            else
                m_window = glfwCreateWindow(realWidth, realHeight, name);

            int major = glfwGetWindowAttrib(m_window, GLFW_CONTEXT_VERSION_MAJOR);
            int minor = glfwGetWindowAttrib(m_window, GLFW_CONTEXT_VERSION_MINOR);
            int revision = glfwGetWindowAttrib(m_window, GLFW_CONTEXT_REVISION);
            Console.WriteLine("OpenGL Version " + major + "." + minor + "." + revision);

            if (!m_window)
	        {
		        glfwTerminate();
                Environment.Exit(1);
	        }

            glfwMakeContextCurrent(m_window);

            if(vSync)
                glfwSwapInterval(1);

            glfwSetErrorCallback((GlfwError a_error, string a_message) =>
                {
                    Console.WriteLine("OpenGL Error: " + a_error + " - " + a_message);
                });

            glfwSetKeyCallback(m_window, (IntPtr a_window, int a_key, int a_scancode, int a_action, int a_mods) =>
                {
                    Key key = (Key)a_key;
                    bool press = (a_action == 1);
                    if (press)
                        if (m_keydown.ContainsKey(key))
                            m_keydown[key] = true;
                        else
                            m_keydown.Add(key, true);
                    else
                        if (m_keyup.ContainsKey(key))
                            m_keyup[key] = true;
                    else
                        m_keyup.Add(key, true);

                    if(!keys.ContainsKey(key))
                        keys.Add(key, press);
                    else
                        keys[key] = press;
                });

            glfwSetMouseButtonCallback((int _button, int _mode) =>
                {
                    bool press = (_mode == 1);
                    if (press) mousehits[_button] = true;
                    else mouseup[_button] = true;
                    buttons[_button] = press;
                });

            glfwSetFramebufferSizeCallback(m_window, (IntPtr window, int newWidth, int newHeight) =>
            {
                glViewport(0, 0, newWidth, newHeight);
                glEnable(GL_MULTISAMPLE);
                glEnable(GL_TEXTURE_2D);
                glEnable(GL_BLEND);
                glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
                glHint(GL_PERSPECTIVE_CORRECTION_HINT, GL_FASTEST);
                //Enable (POLYGON_SMOOTH);
                //ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

                // Load the basic projection settings:
                glMatrixMode(GL_PROJECTION);
                glLoadIdentity();
                // Here's where the conversion from logical width/height to real width/height happens: 
                glOrtho(0.0f, newWidth / _realToLogicWidthRatio, newHeight / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);

                lock (WindowSize.instance)
                {
                    WindowSize.instance.width = (int)(newWidth / _realToLogicWidthRatio);
                    WindowSize.instance.height = (int)(newHeight / _realToLogicHeightRatio);
                }
            });

            glEnable(GL_DEBUG_OUTPUT);
            glEnable(GL_DEBUG_OUTPUT_SYNCHRONOUS);

        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ShowCursor()
        //------------------------------------------------------------------------------------------------------------------------
        public void ShowCursor(bool enable)
        {
            if (enable)
            {
                glfwSetInputMode(m_window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);
            }
            else
            {
                glfwSetInputMode(m_window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetScissor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetScissor(int x, int y, int width, int height)
        {
            if ((width == WindowSize.instance.width) && (height == WindowSize.instance.height))
            {
                glDisable(GL_SCISSOR_TEST);
            }
            else
            {
                glEnable(GL_SCISSOR_TEST);
            }

            glScissor(
                (int)(x * _realToLogicWidthRatio),
                (int)(y * _realToLogicHeightRatio),
                (int)(width * _realToLogicWidthRatio),
                (int)(height * _realToLogicHeightRatio)
            );
            //Scissor(x, y, width, height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Close()
        //------------------------------------------------------------------------------------------------------------------------
        public void Close()
        {
            glfwDestroyWindow(m_window);
            glfwTerminate();
            System.Environment.Exit(0);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Run()
        //------------------------------------------------------------------------------------------------------------------------
        public void Run()
        {
            //Update();
            glfwSetTime(0.0);

            do
            {
                if (_vsyncEnabled || (Time.time - _lastFrameTime > (1000 / _targetFrameRate)))
                {
                    _lastFrameTime = Time.time;

                    //actual fps count tracker
                    _frameCount++;
                    if (Time.time - _lastFPSTime > 1000)
                    {
                        _lastFPS = (int)(_frameCount / ((Time.time - _lastFPSTime) / 1000.0f));
                        _lastFPSTime = Time.time;
                        _frameCount = 0;
                    }

                    UpdateMouseInput();
                    _owner.Step();

                    ResetHitCounters();

                    Display();

                    Time.newFrame();
                    glfwPollEvents();
                }


            } while (!glfwWindowShouldClose(m_window));
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														display()
        //------------------------------------------------------------------------------------------------------------------------
        private void Display()
        {
            //Clear(COLOR_BUFFER_BIT);

            glMatrixMode(GL_MODELVIEW);
            glLoadIdentity();

            _owner.Render(this);

            glfwSwapBuffers(m_window);
            if (GetKey(Key.ESCAPE)) this.Close();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PushMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PushMatrix(float[] matrix)
        {
            GL.glPushMatrix();
            glMultMatrixf(matrix);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PopMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PopMatrix()
        {
            glPopMatrix();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														DrawQuad()
        //------------------------------------------------------------------------------------------------------------------------
        public void DrawQuad(float[] a_vertexArray, float[] a_colorArray, float[] a_texcoordArray)
        {
            //Console.WriteLine(GetError());
            glEnableClientState(GL_TEXTURE_COORD_ARRAY);
            glEnableClientState(GL_VERTEX_ARRAY);
            glEnableClientState(GL_COLOR_ARRAY);
            glTexCoordPointer(2, GL_FLOAT, 0, a_texcoordArray);
            glVertexPointer(2, GL_FLOAT, 0, a_vertexArray);
            glColorPointer(4, GL_FLOAT, 0, a_colorArray);
            glDrawArrays(GL_QUADS, 0, a_vertexArray.Length / 2);
            glDisableClientState(GL_COLOR_ARRAY);
            glDisableClientState(GL_VERTEX_ARRAY);
            glDisableClientState(GL_TEXTURE_COORD_ARRAY);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														DrawLine()
        //------------------------------------------------------------------------------------------------------------------------
        public void DrawLine(float[] vertices, float width)
        {
            if (vertices.Length % 2 == 0)
            {
                glEnable(GL_LINE_SMOOTH);
                glLineWidth(width);
                glEnableClientState(GL_VERTEX_ARRAY);
                glVertexPointer(2, GL_FLOAT, 0, vertices);
                glDrawArrays(GL_LINES, 0, vertices.Length / 2);
                glDisableClientState(GL_VERTEX_ARRAY);
                glLineWidth(1);
                glDisable(GL_LINE_SMOOTH);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKey()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKey(Key key)
        {
            return (!keys.ContainsKey(key) ? false : keys[key]);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyDown()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKeyDown(Key key)
        {
            return (!m_keydown.ContainsKey(key) ? false : m_keydown[key]);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyUp()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKeyUp(Key key)
        {
            return !m_keyup.ContainsKey(key) ? false : m_keyup[key];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButton()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButton(int button)
        {
            return buttons[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonDown()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButtonDown(int button)
        {
            return mousehits[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonUp()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButtonUp(int button)
        {
            return mouseup[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ResetHitCounters()
        //------------------------------------------------------------------------------------------------------------------------
        public static void ResetHitCounters()
        {
            m_keydown.Clear();
            m_keyup.Clear();
            Array.Clear(mousehits, 0, MAXBUTTONS);
            Array.Clear(mouseup, 0, MAXBUTTONS);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														UpdateMouseInput()
        //------------------------------------------------------------------------------------------------------------------------
        public static void UpdateMouseInput()
        {
            glfwGetCursorPos(out mouseX, out mouseY);
            mouseX = (int)(mouseX / _realToLogicWidthRatio);
            mouseY = (int)(mouseY / _realToLogicHeightRatio);
        }

        public int currentFps
        {
            get { return _lastFPS; }
        }

        public int targetFps
        {
            get { return _targetFrameRate; }
            set
            {
                if (value < 1)
                {
                    _targetFrameRate = 1;
                }
                else
                {
                    _targetFrameRate = value;
                }
            }
        }

    }

}
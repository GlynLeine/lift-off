using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace GLXEngine
{
    public class Timer
    {
        public float m_timeBuffer = 0;
        public float m_timeTrigger;

        public delegate void TimeTriggerFunc();

        public TimeTriggerFunc triggerFunc;
        public TimeTriggerFunc offFunc;

        public Timer(float a_timeTrigger = 0, TimeTriggerFunc a_triggerFunc = null, TimeTriggerFunc a_offFunc = null)
        {
            m_timeTrigger = a_timeTrigger;
            triggerFunc = a_triggerFunc;
            offFunc = a_offFunc;
            Time.timers.Add(this);
        }
    }

    /// <summary>
    /// Contains various time related functions.
    /// </summary>
    public class Time
    {
        public static List<Timer> timers = new List<Timer>();

        public static int previousTime;

        static Time()
        {
        }

        /// <summary>
        /// Returns the current system time in milliseconds
        /// </summary>
        public static int now
        {
            get { return System.Environment.TickCount; }
        }

        /// <summary>
        /// Returns this time in milliseconds since the program started		
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public static int time
        {
            get { return (int)(OpenGL.GL.glfwGetTime() * 1000); }
        }

        /// <summary>
        /// Returns the time in milliseconds that has passed since the previous frame
        /// </summary>
        /// <value>
        /// The delta time.
        /// </value>
        private static int previousFrameTime;
        public static int deltaTime
        {
            get
            {
                return previousFrameTime;
            }
        }

        internal static void newFrame()
        {
            previousFrameTime = time - previousTime;
            previousTime = time;

            foreach(Timer t in timers)
            {
                t.m_timeBuffer += previousFrameTime/1000f;
                if(t.m_timeBuffer >= t.m_timeTrigger)
                {
                    t.m_timeBuffer -= t.m_timeTrigger;
                    t.triggerFunc?.Invoke();
                }
                else
                    t.offFunc?.Invoke();
            }
        }
    }
}


using System;
using GLXEngine;								// GLXEngine contains the engine

namespace GameProject
{
    public class Hp
    {
        public float max = 100f;
        public float current = 100f;
        public float regen = 0.1f;
    }

    public class Program : Game
    {
        Overworld overworld;
        public Program() : base(1280, 720)        // Create a window that's 800x600 and NOT fullscreen
        {
            m_keyInputHandler.CreateEvent("MoveForward");
            m_keyInputHandler.CreateEvent("MoveRight");
            m_keyInputHandler.CreateEvent("FaceRight");
            m_keyInputHandler.CreateEvent("FaceForward");
            m_keyInputHandler.CreateEvent("Shoot");
            m_keyInputHandler.CreateEvent("Dodge");
            m_keyInputHandler.CreateEvent("PrintDiagnostics");

            m_keyInputHandler.MapEventToKeyAction("PrintDiagnostics", Key.TILDE);

            m_keyInputHandler.MapEventToKeyAction("Shoot", Key.SPACE);

            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.W, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.S, -1f);

            m_keyInputHandler.MapEventToKeyAxis("MoveRight", Key.D, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveRight", Key.A, -1f);

            m_keyInputHandler.MapEventToKeyAxis("FaceRight", Key.RIGHT, 1f);
            m_keyInputHandler.MapEventToKeyAxis("FaceRight", Key.LEFT, -1f);

            m_keyInputHandler.MapEventToKeyAxis("FaceForward", Key.UP, 1f);
            m_keyInputHandler.MapEventToKeyAxis("FaceForward", Key.DOWN, -1f);

            m_keyInputHandler.ScanObject(this);

            overworld = new Overworld();
            overworld.m_active = true;
            AddChild(overworld);
            Console.WriteLine(GetDiagnostics());
        }

        public override void Restart()
        {
            overworld.Restart();
        }

        public void PrintDiagnostics(bool a_pressed)
        {
            if (!a_pressed)
                Console.WriteLine(GetDiagnostics());

            Console.WriteLine(1f / (Time.deltaTime / 1000f));
        }

        void Update(float a_dt)
        {
        }

        static void Main()                          // Main() is the first method that's called when the program is run
        {
            new Program().Start();                  // Create a "MyGame" and start it
        }
    }
}
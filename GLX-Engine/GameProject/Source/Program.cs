using System;
using System.Drawing;
using System.Collections.Generic;
using GLXEngine;								// GLXEngine contains the engine
using GLXEngine.Core;

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
        public Program() : base(1280, 720, false)        // Create a window that's 800x600 and NOT fullscreen
        {
            GLContext.clearColor = Color.FromArgb(255, 128, 128, 128);

            m_keyInputHandler.CreateEvent("MoveForward");
            m_keyInputHandler.CreateEvent("MoveRight");
            m_keyInputHandler.CreateEvent("FaceRight");
            m_keyInputHandler.CreateEvent("FaceForward");
            m_keyInputHandler.CreateEvent("Shoot");
            m_keyInputHandler.CreateEvent("Dodge");
            m_keyInputHandler.CreateEvent("Reload");
            m_keyInputHandler.CreateEvent("SwitchWeapon");

            m_keyInputHandler.CreateEvent("PrintDiagnostics");

            m_keyInputHandler.MapEventToKeyAction("PrintDiagnostics", Key.TILDE);

            m_keyInputHandler.MapEventToKeyAction("Shoot", Key.SPACE);
            m_keyInputHandler.MapEventToKeyAction("Shoot", Key.DIGITAL0);
            m_keyInputHandler.MapEventToKeyAction("Reload", Key.R);
            m_keyInputHandler.MapEventToKeyAction("SwitchWeapon", Key.E);
            m_keyInputHandler.MapEventToKeyAction("Dodge", Key.NUMPAD_0);

            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.W, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.S, -1f);

            m_keyInputHandler.MapEventToKeyAxis("MoveRight", Key.JOYSTICK_LEFT_X, 1f);
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

        public override void Step()
        {
            base.Step();
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
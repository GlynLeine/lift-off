using System;									// System contains a lot of default C# libraries 
using GXPEngine;								// GXPEngine contains the engine

public class MyGame : Game
{
    Sprite test;

    public MyGame() : base(1920, 1280, true, true, -1, -1, true)     // Create a window that's 800x600 and NOT fullscreen
    {
        test = new Sprite("shooter.png");
        test.SetOrigin(test.width / 2f, test.height / 2f);
        test.SetScaleXY(20f);
        test.x = width / 2f;
        test.y = height / 2f;
        AddChild(test);
    }


    void Update()
    {
        test.rotation++;
    }

    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new MyGame().Start();                   // Create a "MyGame" and start it
    }
}
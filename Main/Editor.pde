enum EditMode
{
  ROTATE, 
    TRANSLATE, 
    SCALE, 
    DRAW
}

class Editor
{
  Object m_selected;

  EditMode editMode;

  Editor()
  {
    inputHandler.AddMousePressType("Click");
    inputHandler.BindMousePress(new Click(), "Click");
    inputHandler.AddMouseMoveType("Drag");
    inputHandler.BindMouseMove(new Drag(), "Drag");
  }

  void Update()
  {
  }

  class Drag implements MouseCommand
  {
    public void Execute(float a_dt, MouseInputData a_mouseData)
    {
      if (a_mouseData.m_pressed)
      {
        switch(editMode)
        {
        case ROTATE:
          break;
        case TRANSLATE:
          break;
        case SCALE:
          break;
        case DRAW:
          break;
        default:
          break;
        }
      }
    }
  }

  class Click implements MouseCommand
  {
    public void Execute(float a_dt, MouseInputData a_mouseData)
    {
      if (!a_mouseData.m_pressed)
      {
        switch(editMode)
        {
        case ROTATE:
          break;
        case TRANSLATE:
          break;
        case SCALE:
          break;
        case DRAW:
        
          break;
        default:
          break;
        }
      }
    }
  }
}

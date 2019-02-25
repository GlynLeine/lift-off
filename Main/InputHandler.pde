//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  COMMAND INTERFACES  //////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public interface Command
{
  public void Execute();
}


public interface ActionCommand
{
  public void Execute(float a_dt);
}


public interface AxisCommand
{
  public void Execute(float a_dt, float a_axisValue);
}


class MouseInputData
{
  PVector m_prevMousePos;
  PVector m_currentMousePos;
  boolean m_pressed;
  PVector m_lastMousePress;
  PVector m_lastMouseRelease;  
}

public interface MouseCommand
{
  public void Execute(float a_dt, MouseInputData a_mouseData);
}

// END OF COMMAND INTERFACES //
//----------------------------------------------------------------------------------------------------------------------------






//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  INPUT HANDLER  ///////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class Input
{
  Map<String, ArrayList<ActionCommand>> m_actionCommands;
  Map<String, ArrayList<AxisCommand>> m_axisCommands;
  Map<String, ArrayList<MouseCommand>> m_mousePressCommands;
  Map<String, ArrayList<MouseCommand>> m_mouseMoveCommands;
  Map<Integer, ArrayList<String>> m_inputTypes;
  ArrayList<Integer> m_pressedKeys;
  
  PVector m_lastMousePosition;
  PVector m_lastMousePress;
  PVector m_lastMouseRelease;
  boolean m_mousePress;
  boolean m_mouseRelease;
  
  boolean m_showCursor = true;
  int m_cursorShader;

//  CONSTRUCTOR  /////////////////////////////////////////////////////////////////////////////////////////////////////////////
  Input()
  {
    m_actionCommands = new TreeMap();
    m_axisCommands = new TreeMap();
    m_inputTypes = new TreeMap();
    m_mousePressCommands = new TreeMap();
    m_mouseMoveCommands = new TreeMap();
    m_pressedKeys = new ArrayList();
    m_lastMousePosition = new PVector();
    m_lastMousePress = new PVector();
    m_lastMouseRelease = new PVector();

  }
  
  

  /* Add action type to bind commands to. */
  public void AddActionType(String a_inputName, int a_actionCode)
  {
    println("  Adding input action: " + a_inputName, color(200));
    if (m_inputTypes.containsKey(a_actionCode))
      m_inputTypes.get(a_actionCode).add("Action " + a_inputName);
    else
    {
      ArrayList<String> inputNames = new ArrayList();
      inputNames.add("Action " + a_inputName);
      m_inputTypes.put(a_actionCode, inputNames);
    }

    if (!m_actionCommands.containsKey(a_inputName))
      m_actionCommands.put(a_inputName, new ArrayList<ActionCommand>());
  }
  
  

  /* Add axis type to bind commands to. */
  public void AddAxisType(String a_inputName, int a_actionCode, float a_keyValue)
  {
    println("  Adding input axis: " + a_inputName + " with key value: " + String.format(" %.00f", a_keyValue), color(200));
    if (m_inputTypes.containsKey(a_actionCode))
      m_inputTypes.get(a_actionCode).add("Axis " + a_inputName + String.format(" %.00f", a_keyValue));
    else
    {
      ArrayList<String> inputNames = new ArrayList();
      inputNames.add("Axis " + a_inputName + String.format(" %.00f", a_keyValue));
      m_inputTypes.put(a_actionCode, inputNames);
    }

    if (!m_axisCommands.containsKey(a_inputName + String.format(" %.00f", a_keyValue)))
      m_axisCommands.put(a_inputName, new ArrayList<AxisCommand>());
  }



  /* Add mouse press action type to bind commands to. */
  public void AddMousePressType(String a_inputName)
  {
    println("  Adding mouse press action: " + a_inputName, color(200));

    if (!m_mousePressCommands.containsKey(a_inputName))
      m_mousePressCommands.put(a_inputName, new ArrayList<MouseCommand>());
  }
  
  
  
  /* Add mouse movement action type to bind commands to. */
  public void AddMouseMoveType(String a_inputName)
  {
    println("  Adding mouse movement action: " + a_inputName, color(200));

    if (!m_mouseMoveCommands.containsKey(a_inputName))
      m_mouseMoveCommands.put(a_inputName, new ArrayList<MouseCommand>());
  }



  /* Bind action commands to a certain input action. */
  public void BindAction(ActionCommand a_command, String a_inputType)
  {
    if (m_actionCommands.containsKey(a_inputType))
      m_actionCommands.get(a_inputType).add(a_command);
    else
      println("Bind command to unexisting input action type: " + a_inputType, "Engine.Input", new Exception().getStackTrace()[0]);
  }
  
  

  /* Bind axis commands to a certain input axis. */
  public void BindAxis(AxisCommand a_command, String a_inputType)
  {    
    if (m_axisCommands.containsKey(a_inputType))
      m_axisCommands.get(a_inputType).add(a_command);
    else
      println("Bind command to unexisting input axis type: " + a_inputType, "Engine.Input", new Exception().getStackTrace()[0]);
  }
  
  
  
  /* Bind mouse commands to a mouse press action. */
  public void BindMousePress(MouseCommand a_command, String a_inputType)
  {    
    if (m_mousePressCommands.containsKey(a_inputType))
      m_mousePressCommands.get(a_inputType).add(a_command);
    else
      println("Bind command to unexisting mouse press input type: " + a_inputType, "Engine.Input", new Exception().getStackTrace()[0]);
  }
  
  
  
  /* Bind mouse commands to a mouse press action. */
  public void BindMouseMove(MouseCommand a_command, String a_inputType)
  {    
    if (m_mouseMoveCommands.containsKey(a_inputType))
      m_mouseMoveCommands.get(a_inputType).add(a_command);
    else
      println("Bind command to unexisting mouse movement input type: " + a_inputType, "Engine.Input", new Exception().getStackTrace()[0]);
  }



  //  INPUT DEFAULT FUNCTIONS  /////////////////////////////////////////////////////////////////////////////////////////////////

  ////  UPDATE  ////
  public void Update()
  {    
    for (int keyType : m_pressedKeys)
    {
      if (m_inputTypes.containsKey(keyType))
      {
        CheckInputAction(keyType);
        CheckInputAxis(keyType);
      }
    }
    
    MouseInputData mouseData = new MouseInputData();
    mouseData.m_currentMousePos = new PVector(mouseX, mouseY);
    mouseData.m_lastMousePress = m_lastMousePress.copy();
    mouseData.m_lastMouseRelease = m_lastMouseRelease.copy();
    mouseData.m_pressed = m_mousePress;
    mouseData.m_prevMousePos = m_lastMousePosition.copy();
    
    if(m_mousePress || m_mouseRelease)
    {
      for(Map.Entry<String, ArrayList<MouseCommand>> entry : m_mousePressCommands.entrySet())
      {
        for(MouseCommand command : entry.getValue())
        {
          command.Execute(0, mouseData);
        }
      }
      m_mouseRelease = false;
    }
    
    if(!m_lastMousePosition.equals(mouseData.m_currentMousePos))
    {
      for(Map.Entry<String, ArrayList<MouseCommand>> entry : m_mouseMoveCommands.entrySet())
      {
        for(MouseCommand command : entry.getValue())
        {
          command.Execute(0, mouseData);
        }
      }
    }
  }
  
  
  
  //  INPUT REPORT FUNCTIONS  //////////////////////////////////////////////////////////////////////////////////////////////////

  // Check for input action commands to be triggered.
  void CheckInputAction(int a_keyType)
  {
    ArrayList<String> inputActions = m_inputTypes.get(a_keyType);
    for (String action : inputActions)
    {
      int typeSeperator = action.indexOf(" ");
      if (action.substring(0, typeSeperator).equals("Action"))
      {
        action = action.substring(typeSeperator+1);
        if (m_actionCommands.containsKey(action))
        {
          for (ActionCommand c : m_actionCommands.get(action))
            c.Execute(0);
        } else
          println("Input action and command list badly initialised.", "Engine.Input", new Exception().getStackTrace()[0]);
      }
    }
  }
  
  
  
  // Check for input axis commands to be triggered.
  void CheckInputAxis(int a_keyType)
  {
    ArrayList<String> inputAxes = m_inputTypes.get(a_keyType);
    for (String axis : inputAxes)
    {
      int typeSeperator = axis.indexOf(" ");
      if (axis.substring(0, typeSeperator).equals("Axis"))
      {
        int keySeperator = axis.lastIndexOf(" ");
        float keyValue = Float.parseFloat(axis.substring(keySeperator+1));
        axis = axis.substring(typeSeperator+1, keySeperator);
        if (m_axisCommands.containsKey(axis))
        {
          for (AxisCommand c : m_axisCommands.get(axis))
            c.Execute(0, keyValue);
        } else
          println("Input axis and command list badly initialised.", "Engine.Input", new Exception().getStackTrace()[0]);
      }
    }
  }
  
  

  // Function that will register a key press.
  public void KeyPress(int a_keyCode)
  {
    if(!m_pressedKeys.contains(a_keyCode))
      m_pressedKeys.add(a_keyCode);
  }



  // Function that will register a key release.
  public void KeyRelease(int a_keyCode)
  {
    int removeKey = -1;
    for(int i = 0; i < m_pressedKeys.size(); i++)
    {
      if(m_pressedKeys.get(i) == a_keyCode)
      {
        removeKey = i;
      }
    }
    
    if(removeKey != -1)
      m_pressedKeys.remove(removeKey);
  }
  
  
  
  // Function that will register a mouse press.
  public void MousePress()
  {
    m_lastMousePress.set(mouseX, mouseY);
    m_mousePress = true;
  }
  
  
  
  // Function that will register a mouse release.
  public void MouseRelease()
  {
    m_lastMouseRelease.set(mouseX, mouseY);
    m_mousePress = false;
    m_mouseRelease = true;
  }
}

// END OF INPUTHANDLER //
//----------------------------------------------------------------------------------------------------------------------------

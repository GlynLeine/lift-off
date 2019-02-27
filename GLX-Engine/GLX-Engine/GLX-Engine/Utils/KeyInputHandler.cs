using GLXEngine.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO.Ports;


namespace GLXEngine
{
    // Type aliasing for keeping track which integer is to represent a controller state.
    using ControllerID = System.Int32;

    // Type aliasing instead of creating a custom delegate in order to gain easier compatibility with any function for the scan.
    public delegate void KeyActionDelegate(bool a_pressed, ControllerID a_controllerID = -1);
    public delegate void KeyAxisDelegate(float a_value, List<ControllerID> a_controllerID);
    public delegate void KeyButtonDelegate(Key a_key, bool a_pressed, ControllerID a_controllerID = -1);

    #region Input Event Structs
    abstract class InputEvent
    {
        public Delegate m_delegate = null;
    }

    class InputButton : InputEvent
    {
        public new KeyButtonDelegate m_delegate = null;
    }

    class InputAxis : InputEvent
    {
        public new KeyAxisDelegate m_delegate = null;
        public float m_value = 0;
    }

    class InputAction : InputEvent
    {
        public new KeyActionDelegate m_delegate = null;
    }

    sealed class InputEventReference
    {
        private Func<object> Get { get; set; }
        private Action<object> Set { get; set; }

        public object Object { get { return Get(); } set { Set(value); } }

        public InputEventReference(object a)
        { Get = () => a; Set = val => { a = Convert.ChangeType(val, a.GetType()); }; }
    }
    #endregion

    public class KeyInputHandler
    {
        #region Event Mapping
        // Mapping of action events to keys.
        private Dictionary<Key, List<string>> m_actionsMap;
        // Mapping of axis events to keys.
        private Dictionary<Key, Dictionary<string, float>> m_axisMap;
        #endregion

        #region Event Tracking
        // Mapping of event names to their ID's.
        private Dictionary<string, Dictionary<Type, InputEventReference>> m_events;
        #endregion

        #region Key Tracking
        // List of currently pressed keys.
        private Dictionary<Key, List<ControllerID>> m_pressedKeys = new Dictionary<Key, List<ControllerID>>();
        private Dictionary<Key, List<ControllerID>> m_releasedKeys = new Dictionary<Key, List<ControllerID>>();
        #endregion

        #region Storage for duplication
        private Dictionary<string, Dictionary<Key, float>> m_axisStorage = new Dictionary<string, Dictionary<Key, float>>();
        private Dictionary<string, List<Key>> m_actionStorage = new Dictionary<string, List<Key>>();
        #endregion

        #region Controller tracking
        private Dictionary<ControllerID, GameController> m_controllers = new Dictionary<ControllerID, GameController>();
        #endregion

        // Handler constructor.
        public KeyInputHandler()
        {
            m_actionsMap = new Dictionary<Key, List<string>>();
            m_axisMap = new Dictionary<Key, Dictionary<string, float>>();
            m_events = new Dictionary<string, Dictionary<Type, InputEventReference>>();

            foreach (string portName in SerialPort.GetPortNames())
            {
                GameController gameController = new GameController(true);
                m_controllers.Add(gameController.ID, gameController);
            }
        }

        // Handler copy constructor.
        public KeyInputHandler(KeyInputHandler a_source)
        {
            m_actionsMap = new Dictionary<Key, List<string>>();
            m_axisMap = new Dictionary<Key, Dictionary<string, float>>();
            m_events = new Dictionary<string, Dictionary<Type, InputEventReference>>();

            foreach (string portName in SerialPort.GetPortNames())
            {
                GameController gameController = new GameController(true);
                m_controllers.Add(gameController.ID, gameController);
            }

            foreach (string eventName in a_source.m_events.Keys)
            {
                CreateEvent(eventName);
            }

            foreach (KeyValuePair<string, Dictionary<Key, float>> axisEvent in a_source.m_axisStorage)
                foreach (KeyValuePair<Key, float> axisMapping in axisEvent.Value)
                    MapEventToKeyAxis(axisEvent.Key, axisMapping.Key, axisMapping.Value);

            foreach (KeyValuePair<string, List<Key>> actionEvent in a_source.m_actionStorage)
                foreach (Key actionMapping in actionEvent.Value)
                    MapEventToKeyAction(actionEvent.Key, actionMapping);

        }

        // Retrieve input from all input sources.
        public void RetrieveInput()
        {
            m_releasedKeys.Clear();

            #region KeyData
            foreach (Key key in GLContext.keyup.Keys)
                m_releasedKeys.Add(key, new List<ControllerID> { -1 });

            foreach (Key key in GLContext.keydown.Keys)
            {
                if (!m_pressedKeys.ContainsKey(key))
                    m_pressedKeys.Add(key, new List<ControllerID> { -1 });
                else if (!m_pressedKeys[key].Contains(-1))
                    m_pressedKeys[key].Add(-1);
            }

            foreach (Key key in GLContext.keyup.Keys)
            {
                if (m_pressedKeys.ContainsKey(key))
                    if (m_pressedKeys[key].Contains(-1))
                        m_pressedKeys[key].Remove(-1);
            }
            #endregion

            #region ControllerData
            foreach (ControllerID controllerID in m_controllers.Keys)
            {
                GameController controller = m_controllers[controllerID];
                controller.Search();

                if (controller.connected)
                {
                    foreach (Key key in controller.analogs)
                    {
                        if (!m_pressedKeys.ContainsKey(key)) m_pressedKeys.Add(key, new List<ControllerID> { controller.ID });
                        else if (!m_pressedKeys[key].Contains(controller.ID)) m_pressedKeys[key].Add(controller.ID);
                    }

                    foreach (Key key in controller.digitals)
                    {
                        if (controller.GetDigital(key))
                        {
                            if (!m_pressedKeys.ContainsKey(key)) m_pressedKeys.Add(key, new List<ControllerID> { controller.ID });
                            else if (!m_pressedKeys[key].Contains(controller.ID)) m_pressedKeys[key].Add(controller.ID);
                        }
                        else
                        {
                            if (m_pressedKeys.ContainsKey(key))
                            { if (m_pressedKeys[key].Contains(controller.ID)) m_pressedKeys[key].Remove(controller.ID); }
                        }
                    }
                }
            }
            #endregion

            List<Key> temp = new List<Key>(m_pressedKeys.Keys);
            foreach (Key key in temp)
            {
                if (m_pressedKeys[key].Count == 0)
                    m_pressedKeys.Remove(key);
            }
        }

        // Handle all the updates on the key inputs between frames, this should
        // be run before the update loop to get the latest updated inputs.
        public void Step()
        {
            RetrieveInput();

            #region Reset Axes
            foreach (Dictionary<Type, InputEventReference> events in m_events.Values)
                if (events.ContainsKey(typeof(InputAxis))) ((InputAxis)events[typeof(InputAxis)].Object).m_value = 0;
            #endregion

            #region Handle Press Events
            foreach (KeyValuePair<Key, List<ControllerID>> keyData in m_pressedKeys)
            {
                foreach (ControllerID controllerID in keyData.Value)
                {
                    if (m_actionsMap.ContainsKey(keyData.Key))
                        InvokeInputActions(keyData.Key, true, controllerID);

                    if (m_axisMap.ContainsKey(keyData.Key))
                        UpdateAxesForKey(keyData.Key, controllerID);

                    foreach (Dictionary<Type, InputEventReference> events in m_events.Values)
                        if (events.ContainsKey(typeof(InputButton))) ((InputButton)events[typeof(InputButton)].Object).m_delegate?.Invoke(keyData.Key, true, controllerID);
                }
            }
            #endregion

            #region Handle Release Events
            foreach (KeyValuePair<Key, List<ControllerID>> keyData in m_releasedKeys)
            {
                foreach (ControllerID controllerID in keyData.Value)
                {
                    if (m_actionsMap.ContainsKey(keyData.Key))
                        InvokeInputActions(keyData.Key, false, controllerID);

                    foreach (Dictionary<Type, InputEventReference> events in m_events.Values)
                        if (events.ContainsKey(typeof(InputButton))) ((InputButton)events[typeof(InputButton)].Object).m_delegate?.Invoke(keyData.Key, true, controllerID);
                }
            }
            #endregion

            #region Invoke Axis Events
            foreach (KeyValuePair<Key, Dictionary<string, float>> keyMap in m_axisMap)
            {
                foreach (string axisName in keyMap.Value.Keys)
                {
                    InputAxis axis = m_events[axisName][typeof(InputAxis)].Object as InputAxis;
                    if (m_pressedKeys.ContainsKey(keyMap.Key))
                        axis.m_delegate?.Invoke(axis.m_value, m_pressedKeys[keyMap.Key]);
                    else
                        axis.m_delegate?.Invoke(axis.m_value, new List<ControllerID> { -1 });


                }

            }
            #endregion

            //Console.Clear();
            //foreach (KeyValuePair<Key, List<ControllerID>> keyData in m_pressedKeys)
            //{
            //    string controllerIds = "";
            //    foreach (ControllerID controllerID in keyData.Value)
            //        controllerIds += " " + controllerID;
            //    Console.WriteLine(keyData.Key + controllerIds);
            //}

        }

        // Invoke all the function calls for given key.
        private void InvokeInputActions(Key a_key, bool a_pressed, ControllerID a_controllerID)
        {
            if (m_actionsMap.ContainsKey(a_key))
            {
                List<string> inputActions = m_actionsMap[a_key];
                foreach (string actionName in inputActions)
                {
                    (m_events[actionName][typeof(InputAction)].Object as InputAction).m_delegate?.Invoke(a_pressed, a_controllerID);
                }
            }
        }

        // Update all axes bound to the given key.
        private void UpdateAxesForKey(Key a_key, ControllerID a_controllerID)
        {
            if (m_axisMap.ContainsKey(a_key))
            {
                Dictionary<string, float> axesInputs = m_axisMap[a_key];
                foreach (KeyValuePair<string, float> axisInput in axesInputs)
                {
                    if (a_controllerID != -1)
                    {
                        (m_events[axisInput.Key][typeof(InputAxis)].Object as InputAxis).m_value += axisInput.Value * m_controllers[a_controllerID].GetAnalog(a_key);
                    }
                    else
                        (m_events[axisInput.Key][typeof(InputAxis)].Object as InputAxis).m_value += axisInput.Value;
                }
            }
        }

        // Public function for creating a new event.
        public void CreateEvent(string a_name)
        {
            m_events.Add(a_name, new Dictionary<Type, InputEventReference>());
        }

        // Public function for mapping a premade event to an input for an input action.
        public void MapEventToKeyAction(string a_name, Key a_key)
        {
            if (m_events.ContainsKey(a_name))
            {
                InputEventReference actionRef;
                if (m_events[a_name].ContainsKey(typeof(InputAction)))
                    actionRef = m_events[a_name][typeof(InputAction)];
                else
                {
                    actionRef = new InputEventReference(new InputAction());
                    m_events[a_name].Add(typeof(InputAction), actionRef);
                }

                // Check if the key has been mapped to any action events before or whether it needs to be added first.
                if (m_actionsMap.ContainsKey(a_key))
                {
                    if (!m_actionsMap[a_key].Contains(a_name))
                        m_actionsMap[a_key].Add(a_name);
                }
                else
                    m_actionsMap.Add(a_key, new List<string> { a_name });

                if (m_actionStorage.ContainsKey(a_name))
                {
                    if (!m_actionStorage[a_name].Contains(a_key))
                        m_actionStorage[a_name].Add(a_key);
                }
                else
                    m_actionStorage.Add(a_name, new List<Key> { a_key });
            }
            else
            {
                throw new Exception("Trying to bind non existing event.");
            }
        }

        // Public function for mapping a premade event to an input for an input axis.
        public void MapEventToKeyAxis(string a_name, Key a_key, float a_value)
        {
            if (m_events.ContainsKey(a_name))
            {
                InputEventReference axisRef;
                if (m_events[a_name].ContainsKey(typeof(InputAxis)))
                    axisRef = m_events[a_name][typeof(InputAxis)];
                else
                {
                    axisRef = new InputEventReference(new InputAxis());
                    m_events[a_name].Add(typeof(InputAxis), axisRef);
                }

                // Check if the key has been mapped to any axis events before or whether it needs to be added first.
                if (m_axisMap.ContainsKey(a_key))
                    if (m_axisMap[a_key].ContainsKey(a_name))
                        m_axisMap[a_key][a_name] = a_value;
                    else
                        m_axisMap[a_key].Add(a_name, a_value);
                else
                    m_axisMap.Add(a_key, new Dictionary<string, float> { { a_name, a_value } });

                if (m_axisStorage.ContainsKey(a_name))
                {
                    if (m_axisStorage[a_name].ContainsKey(a_key))
                        m_axisStorage[a_name][a_key] = a_value;
                    else
                        m_axisStorage[a_name].Add(a_key, a_value);
                }
                else
                    m_axisStorage.Add(a_name, new Dictionary<Key, float> { { a_key, a_value } });
            }
            else
            {
                throw new Exception("Trying to bind non existing event.");
            }
        }

        // Public function for binding a function to an event that was mapped to an action.
        public void BindFunctionToAction(string a_name, KeyActionDelegate a_function)
        {
            if (m_events.ContainsKey(a_name))
            {
                if (m_events[a_name].ContainsKey(typeof(InputAction)))
                {
                    InputAction action = (InputAction)m_events[a_name][typeof(InputAction)].Object;
                    if (action.m_delegate == null)
                        action.m_delegate = a_function;
                    else
                        action.m_delegate += a_function;
                }
                else
                    throw new Exception("Trying to bind function to an event that wasn't mapped to an action.");
            }
            else
            {
                throw new Exception("Trying to bind function to non existing event.");
            }
        }

        // Public function for binding a function to an event that was mapped to an action.
        public void BindFunctionToButtonEvent(string a_name, KeyButtonDelegate a_function)
        {
            if (m_events.ContainsKey(a_name))
            {
                if (m_events[a_name].ContainsKey(typeof(InputButton)))
                {
                    InputButton buttonEvent = (InputButton)m_events[a_name][typeof(InputButton)].Object;
                    if (buttonEvent.m_delegate == null)
                        buttonEvent.m_delegate = a_function;
                    else
                        buttonEvent.m_delegate += a_function;
                }
                else
                    throw new Exception("Trying to bind function to an event that wasn't mapped to an button event.");
            }
            else
            {
                throw new Exception("Trying to bind function to non existing event.");
            }
        }

        // Public function for binding a function to an event that was mapped to an axis.
        public void BindFunctionToAxis(string a_name, KeyAxisDelegate a_function)
        {
            if (m_events.ContainsKey(a_name))
            {
                if (m_events[a_name].ContainsKey(typeof(InputAxis)))
                {
                    InputAxis axis = (InputAxis)m_events[a_name][typeof(InputAxis)].Object;
                    if (axis.m_delegate == null)
                        axis.m_delegate = a_function;
                    else
                        axis.m_delegate += a_function;
                }
                else
                    throw new Exception("Trying to bind function to an event that wasn't mapped to an axis.");
            }
            else
            {
                throw new Exception("Trying to bind non existing event.");
            }
        }

        // Public function to automatically scan and decide what kind of event it needs to be bound to.
        public void BindFunction(string a_name, Delegate a_function, GameObject a_owner)
        {
            // Check if the given function fits any delegate at all by return type.
            if (a_function.Method.ReturnType.Equals(typeof(void)))
            {
                // Check if the given function fits any delegate at all by argument list length.
                ParameterInfo[] parameters = a_function.Method.GetParameters();
                if (parameters.Length == 1)
                {
                    // Check whether the given function was meant for an input action or an input axis.
                    if (parameters[0].ParameterType.Equals(typeof(bool)))
                        BindFunctionToAction(a_name, (KeyActionDelegate)a_function);
                    else if (parameters[0].ParameterType.Equals(typeof(float)))
                        BindFunctionToAxis(a_name, (KeyAxisDelegate)a_function);
                }
                else if (parameters.Length == 2)
                {
                    if (parameters[0].ParameterType.Equals(typeof(Key)) && parameters[1].ParameterType.Equals(typeof(bool)))
                        BindFunctionToButtonEvent(a_name, (KeyButtonDelegate)a_function);
                }
            }
            throw new Exception("Function incompatible with input handler.");
        }

        public void ScanObject(GameObject a_object)
        {
            foreach (KeyValuePair<string, Dictionary<Type, InputEventReference>> eventName in m_events)
            {
                MethodInfo eventMethod = a_object.GetType().GetMethod(eventName.Key, new Type[] { typeof(bool), typeof(ControllerID) });
                if (eventMethod != null)
                {
                    if (eventMethod.ReturnType == typeof(void))
                    {
                        KeyActionDelegate keyActionEvent = (KeyActionDelegate)Delegate.CreateDelegate(typeof(KeyActionDelegate), a_object, eventMethod, false);
                        if (keyActionEvent != null)
                            BindFunctionToAction(eventName.Key, keyActionEvent);
                    }
                }

                eventMethod = a_object.GetType().GetMethod(eventName.Key, new Type[] { typeof(float), typeof(List<ControllerID>) });
                if (eventMethod != null)
                {
                    if (eventMethod.ReturnType == typeof(void))
                    {
                        KeyAxisDelegate keyAxisEvent = (KeyAxisDelegate)Delegate.CreateDelegate(typeof(KeyAxisDelegate), a_object, eventMethod, false);
                        if (keyAxisEvent != null)
                            BindFunctionToAxis(eventName.Key, keyAxisEvent);
                    }
                }

                eventMethod = a_object.GetType().GetMethod(eventName.Key, new Type[] { typeof(Key), typeof(bool), typeof(ControllerID) });
                if (eventMethod != null)
                {
                    if (eventMethod.ReturnType == typeof(void))
                    {
                        KeyButtonDelegate keyButtonEvent = (KeyButtonDelegate)Delegate.CreateDelegate(typeof(KeyButtonDelegate), a_object, eventMethod, false);
                        if (keyButtonEvent != null)
                            BindFunctionToButtonEvent(eventName.Key, keyButtonEvent);
                    }
                }
            }
        }

        public void Destroy()
        {
            foreach (ControllerID controllerID in m_controllers.Keys)
            {
                m_controllers[controllerID].Destroy();
            }
        }

        public string GetDiagnostics()
        {
            string output = "";
            int actions = 0, axes = 0, buttonEvents = 0;
            int delegateCount = 0;
            foreach (Dictionary<Type, InputEventReference> eventType in m_events.Values)
            {
                if (eventType.ContainsKey(typeof(InputAction)))
                {
                    InputAction inputAction = eventType[typeof(InputAction)].Object as InputAction;
                    delegateCount += inputAction.m_delegate == null ? 0 : inputAction.m_delegate.GetInvocationList().Length;
                    actions++;
                }
                if (eventType.ContainsKey(typeof(InputAxis)))
                {
                    InputAxis inputAxis = eventType[typeof(InputAxis)].Object as InputAxis;
                    delegateCount += inputAxis.m_delegate == null ? 0 : inputAxis.m_delegate.GetInvocationList().Length;
                    axes++;
                }
                if (eventType.ContainsKey(typeof(InputButton)))
                {
                    InputButton inputButton = eventType[typeof(InputButton)].Object as InputButton;
                    delegateCount += inputButton.m_delegate == null ? 0 : inputButton.m_delegate.GetInvocationList().Length;
                    buttonEvents++;
                }
            }

            output += "Number of key events: " + m_events.Count + '\n';
            output += "   Of which " + actions + " are mapped to actions\n";
            output += "   And " + axes + " are mapped to axes\n";
            output += "   And " + buttonEvents + " are mapped to button events\n";
            output += "Number of key input delegates: " + delegateCount + '\n';
            return output;
        }

    }
}

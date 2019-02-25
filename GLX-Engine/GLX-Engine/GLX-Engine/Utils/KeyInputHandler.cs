using GLXEngine.Core;
using GLXEngine.OpenGL;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace GLXEngine
{
    // Type aliasing for keeping track which integer is to represent an event ID and which is to represent a key.
    using EventID = System.Int32;

    // Type aliasing instead of creating a custom delegate in order to gain easier compatibility with any function for the scan.
    using KeyActionDelegate = System.Action<bool>;
    using KeyAxisDelegate = System.Action<float>;
    using KeyButtonDelegate = System.Action<Key, bool>;

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

    public class KeyInputHandler
    {
        // Mapping of action events to keys.
        private Dictionary<Key, List<InputAction>> m_actionsMap;
        // Mapping of axis events to keys.
        private Dictionary<Key, Dictionary<InputAxis, float>> m_axisMap;

        // Mapping of event names to their ID's.
        private Dictionary<String, Dictionary<Type, InputEvent>> m_events;

        // List of currently pressed keys.
        private List<Key> m_pressedKeys = new List<Key>();
        private List<Key> m_releasedKeys = new List<Key>();

        private Dictionary<string, Dictionary<Key, float>> m_axisStorage = new Dictionary<string, Dictionary<Key, float>>();
        private Dictionary<string, List<Key>> m_actionStorage = new Dictionary<string, List<Key>>();

        //private Dictionary<int, GameController> m_controllers = new Dictionary<int, GameController>();

        static int count = 0;

        // Handler constructor.
        public KeyInputHandler()
        {
            count++;
            m_actionsMap = new Dictionary<Key, List<InputAction>>();
            m_axisMap = new Dictionary<Key, Dictionary<InputAxis, float>>();
            m_events = new Dictionary<string, Dictionary<Type, InputEvent>>();
        }

        public KeyInputHandler(KeyInputHandler a_source)
        {
            count++;
            m_actionsMap = new Dictionary<Key, List<InputAction>>();
            m_axisMap = new Dictionary<Key, Dictionary<InputAxis, float>>();
            m_events = new Dictionary<string, Dictionary<Type, InputEvent>>();

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

            //GL.glfwSetJoystickCallback((int a_joystick, int a_event) =>
            //    {
            //        if(a_event == GL.GLFW_CONNECTED)
            //        {

            //        }
            //    });
        }


        public void RetrieveInput()
        {
            m_releasedKeys.Clear();
            m_releasedKeys.AddRange(GLContext.keyup.Keys);
            foreach (Key key in GLContext.keydown.Keys)
                if (!m_pressedKeys.Contains(key)) m_pressedKeys.Add(key);
            foreach (Key key in GLContext.keyup.Keys)
                if (m_pressedKeys.Contains(key)) m_pressedKeys.Remove(key);
        }

        // Handle all the updates on the key inputs between frames, this should
        // be run before the update loop to get the latest updated inputs.
        public void Step()
        {
            RetrieveInput();

            foreach (Dictionary<Type, InputEvent> events in m_events.Values)
                if (events.ContainsKey(typeof(InputAxis))) ((InputAxis)events[typeof(InputAxis)]).m_value = 0;

            foreach (Key keyType in m_pressedKeys)
            {
                if (m_actionsMap.ContainsKey(keyType))
                    InvokeInputActions(keyType, true);

                if (m_axisMap.ContainsKey(keyType))
                    UpdateAxesForKey(keyType);

                foreach (Dictionary<Type, InputEvent> events in m_events.Values)
                    if (events.ContainsKey(typeof(InputButton))) ((InputButton)events[typeof(InputButton)]).m_delegate?.Invoke(keyType, true);
            }

            foreach (Key keyType in m_releasedKeys)
            {
                if (m_actionsMap.ContainsKey(keyType))
                    InvokeInputActions(keyType, false);

                foreach (Dictionary<Type, InputEvent> events in m_events.Values)
                    if (events.ContainsKey(typeof(InputButton))) ((InputButton)events[typeof(InputButton)]).m_delegate?.Invoke(keyType, true);
            }

            foreach (KeyValuePair<Key, Dictionary<InputAxis, float>> keyMap in m_axisMap)
            {
                foreach (InputAxis axis in keyMap.Value.Keys)
                { axis.m_delegate?.Invoke(axis.m_value); }
            }

            //Console.Clear();
            for (int i = 0; i < m_pressedKeys.Count; i++)
            {
                Console.WriteLine(m_pressedKeys[i]);
            }

        }

        // Invoke all the function calls for given key.
        private void InvokeInputActions(Key a_key, bool a_pressed)
        {
            if (m_actionsMap.ContainsKey(a_key))
            {
                List<InputAction> inputActions = m_actionsMap[a_key];
                foreach (InputAction action in inputActions)
                {
                    action.m_delegate?.Invoke(a_pressed);
                }
            }
        }

        // Update all axes bound to the given key.
        private void UpdateAxesForKey(Key a_key)
        {
            if (m_axisMap.ContainsKey(a_key))
            {
                Dictionary<InputAxis, float> axesInputs = m_axisMap[a_key];
                foreach (KeyValuePair<InputAxis, float> axisInput in axesInputs)
                {
                    axisInput.Key.m_value += axisInput.Value;
                }
            }
        }

        // Public function for creating a new event.
        public void CreateEvent(String a_name)
        {
            m_events.Add(a_name, new Dictionary<Type, InputEvent>());
        }

        // Public function for mapping a premade event to an input for an input action.
        public void MapEventToKeyAction(String a_name, Key a_key)
        {
            if (m_events.ContainsKey(a_name))
            {
                InputAction action;
                if (m_events[a_name].ContainsKey(typeof(InputAction)))
                    action = (InputAction)m_events[a_name][typeof(InputAction)];
                else
                {
                    action = new InputAction();
                    m_events[a_name].Add(typeof(InputAction), action);
                }

                // Check if the key has been mapped to any action events before or whether it needs to be added first.
                if (m_actionsMap.ContainsKey(a_key))
                {
                    if (!m_actionsMap[a_key].Contains(action))
                        m_actionsMap[a_key].Add(action);
                }
                else
                    m_actionsMap.Add(a_key, new List<InputAction> { action });

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
        public void MapEventToKeyAxis(String a_name, Key a_key, float a_value)
        {
            if (m_events.ContainsKey(a_name))
            {
                InputAxis axis;
                if (m_events[a_name].ContainsKey(typeof(InputAxis)))
                    axis = (InputAxis)m_events[a_name][typeof(InputAxis)];
                else
                {
                    axis = new InputAxis();
                    m_events[a_name].Add(typeof(InputAxis), axis);
                }

                // Check if the key has been mapped to any axis events before or whether it needs to be added first.
                if (m_axisMap.ContainsKey(a_key))
                    if (m_axisMap[a_key].ContainsKey(axis))
                        m_axisMap[a_key][axis] = a_value;
                    else
                        m_axisMap[a_key].Add(axis, a_value);
                else
                    m_axisMap.Add(a_key, new Dictionary<InputAxis, float> { { axis, a_value } });

                if (m_axisStorage.ContainsKey(a_name))
                {
                    if (m_axisStorage[a_name].ContainsKey(a_key))
                        m_axisStorage[a_name][a_key] = a_value;
                    else
                        m_axisStorage[a_name].Add(a_key, a_value);
                }
                else
                    m_axisStorage.Add(a_name, new Dictionary<Key, float> { { a_key, a_value} });
            }
            else
            {
                throw new Exception("Trying to bind non existing event.");
            }
        }

        // Public function for binding a function to an event that was mapped to an action.
        public void BindFunctionToAction(String a_name, KeyActionDelegate a_function)
        {
            if (m_events.ContainsKey(a_name))
            {
                if (m_events[a_name].ContainsKey(typeof(InputAction)))
                {
                    InputAction action = (InputAction)m_events[a_name][typeof(InputAction)];
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
        public void BindFunctionToButtonEvent(String a_name, KeyButtonDelegate a_function)
        {
            if (m_events.ContainsKey(a_name))
            {
                if (m_events[a_name].ContainsKey(typeof(InputButton)))
                {
                    InputButton buttonEvent = (InputButton)m_events[a_name][typeof(InputButton)];
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
        public void BindFunctionToAxis(String a_name, KeyAxisDelegate a_function)
        {
            if (m_events.ContainsKey(a_name))
            {
                if (m_events[a_name].ContainsKey(typeof(InputAxis)))
                {
                    InputAxis axis = (InputAxis)m_events[a_name][typeof(InputAxis)];
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
        public void BindFunction(String a_name, Delegate a_function, GameObject a_owner)
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
            foreach (KeyValuePair<String, Dictionary<Type, InputEvent>> eventName in m_events)
            {
                MethodInfo eventMethod = a_object.GetType().GetMethod(eventName.Key, new Type[] { typeof(bool) });
                if (eventMethod != null)
                {
                    if (eventMethod.ReturnType == typeof(void))
                    {
                        KeyActionDelegate keyActionEvent = (KeyActionDelegate)Delegate.CreateDelegate(typeof(KeyActionDelegate), a_object, eventMethod, false);
                        if (keyActionEvent != null)
                            BindFunctionToAction(eventName.Key, keyActionEvent);
                    }
                }

                eventMethod = a_object.GetType().GetMethod(eventName.Key, new Type[] { typeof(float) });
                if (eventMethod != null)
                {
                    if (eventMethod.ReturnType == typeof(void))
                    {
                        KeyAxisDelegate keyAxisEvent = (KeyAxisDelegate)Delegate.CreateDelegate(typeof(KeyAxisDelegate), a_object, eventMethod, false);
                        if (keyAxisEvent != null)
                            BindFunctionToAxis(eventName.Key, keyAxisEvent);
                    }
                }

                eventMethod = a_object.GetType().GetMethod(eventName.Key, new Type[] { typeof(Key), typeof(bool) });
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

        public string GetDiagnostics()
        {
            string output = "";
            int actions = 0, axes = 0, buttonEvents = 0;
            int delegateCount = 0;
            foreach (Dictionary<Type, InputEvent> eventType in m_events.Values)
            {
                if (eventType.ContainsKey(typeof(InputAction)))
                {
                    delegateCount += eventType[typeof(InputAction)].m_delegate == null ? 0 : eventType[typeof(InputAction)].m_delegate.GetInvocationList().Length;
                    actions++;
                }
                if (eventType.ContainsKey(typeof(InputAxis)))
                {
                    delegateCount += eventType[typeof(InputAxis)].m_delegate == null ? 0 : eventType[typeof(InputAxis)].m_delegate.GetInvocationList().Length;
                    axes++;
                }
                if (eventType.ContainsKey(typeof(InputButton)))
                {
                    delegateCount += eventType[typeof(InputButton)].m_delegate == null ? 0 : eventType[typeof(InputButton)].m_delegate.GetInvocationList().Length;
                    buttonEvents++;
                }
            }

            output += "Number of key events: " + m_events.Count + '\n';
            output += "   Of which " + actions + " are mapped to actions\n";
            output += "   And " + axes + " are mapped to axes\n";
            output += "   And " + buttonEvents + " are mapped to button events\n";

            //delegateCount += (actionEvent.Value == null ? 0 : actionEvent.Value.GetInvocationList().Length);

            output += "Number of key input delegates: " + delegateCount + '\n';
            return output;
        }

    }
}

using System;
using System.Reflection;
using System.Collections.Generic;
using GLXEngine.OpenGL;
using GLXEngine.Core;


namespace GLXEngine
{
    // Type aliasing for keeping track which integer is to represent an event ID and which is to represent a key.
    using EventID = System.Int32;

    // Type aliasing instead of creating a custom delegate in order to gain easier compatibility with any function for the scan.
    using KeyActionDelegate = System.Action<bool>;
    using KeyAxisDelegate = System.Action<float>;

    public class KeyInputHandler
    {
        // Storing each delegate for each action event.
        private Dictionary<EventID, KeyActionDelegate> m_actionEvents;

        // Storing each delegate for each axis event.
        private Dictionary<EventID, KeyAxisDelegate> m_axisEvents;

        // Mapping of action events to keys.
        private Dictionary<int, List<EventID>> m_actionsMap;
        // Mapping of axis events to keys.
        private Dictionary<int, Dictionary<EventID, float>> m_axisMap;
        // Mapping of event names to their ID's.
        private Dictionary<String, EventID> m_eventNames;

        // Keeping track of each axis bound to their event ID and the current value of the axis.
        private Dictionary<EventID, float> m_axes;

        // List of currently pressed keys.
        private List<int> m_pressedKeys = new List<EventID>();
        private List<int> m_releasedKeys = new List<EventID>();

        // Handler constructor.
        public KeyInputHandler()
        {
            m_actionEvents = new Dictionary<EventID, KeyActionDelegate>();
            m_axisEvents = new Dictionary<EventID, KeyAxisDelegate>();
            m_actionsMap = new Dictionary<EventID, List<EventID>>();
            m_axisMap = new Dictionary<EventID, Dictionary<EventID, float>>();
            m_eventNames = new Dictionary<string, EventID>();
            m_axes = new Dictionary<EventID, float>();
        }
        public KeyInputHandler(KeyInputHandler a_source)
        {
            m_actionEvents = new Dictionary<EventID, KeyActionDelegate>(a_source.m_actionEvents);
            List<EventID> events = new List<EventID>(m_actionEvents.Keys);
            foreach (EventID eventID in events)
                m_actionEvents[eventID] = null;
            m_axisEvents = new Dictionary<EventID, KeyAxisDelegate>(a_source.m_axisEvents);
            events = new List<EventID>(m_axisEvents.Keys);
            foreach (EventID eventID in events)
                m_axisEvents[eventID] = null;
            m_actionsMap = new Dictionary<EventID, List<EventID>>(a_source.m_actionsMap);
            m_axisMap = new Dictionary<EventID, Dictionary<EventID, float>>(a_source.m_axisMap);
            m_eventNames = new Dictionary<string, EventID>(a_source.m_eventNames);
            m_axes = new Dictionary<EventID, float>(a_source.m_axes);
        }

        public void RetrieveInput()
        {
            m_releasedKeys.Clear();
            m_releasedKeys.AddRange(GLContext.keyup.Keys);
            foreach (int key in GLContext.keydown.Keys)
                if (!m_pressedKeys.Contains(key)) m_pressedKeys.Add(key);
            foreach (int key in GLContext.keyup.Keys)
                if (m_pressedKeys.Contains(key)) m_pressedKeys.Remove(key);
        }

        // Handle all the updates on the key inputs between frames, this should
        // be run before the update loop to get the latest updated inputs.
        public void Step()
        {
            RetrieveInput();
            List<EventID> axisIDs = new List<EventID>(m_axes.Keys);
            foreach (EventID axisID in axisIDs)
            {
                m_axes[axisID] = 0f;
            }

            foreach (int keyType in m_pressedKeys)
            {
                if (m_actionsMap.ContainsKey(keyType))
                    InvokeInputActions(keyType);

                if (m_axisMap.ContainsKey(keyType))
                    UpdateAxesForKey(keyType);
            }

            foreach (int keyType in m_releasedKeys)
            {
                if (m_actionsMap.ContainsKey(keyType))
                    InvokeInputActions(keyType);
            }

            foreach (KeyValuePair<EventID, float> axis in m_axes)
            {
                m_axisEvents[axis.Key]?.Invoke(axis.Value);
            }
        }

        // Invoke all the function calls for given key.
        private void InvokeInputActions(int a_key)
        {
            if (m_actionsMap.ContainsKey(a_key))
            {
                List<EventID> inputActions = m_actionsMap[a_key];
                foreach (EventID action in inputActions)
                {
                    m_actionEvents[action]?.Invoke(m_pressedKeys.Contains(a_key));
                }
            }
        }

        // Update all axes bound to the given key.
        private void UpdateAxesForKey(int a_key)
        {
            if (m_axisMap.ContainsKey(a_key))
            {
                Dictionary<EventID, float> axesInputs = m_axisMap[a_key];
                foreach (KeyValuePair<EventID, float> axisInput in axesInputs)
                {
                    m_axes[axisInput.Key] += axisInput.Value;
                }
            }
        }

        // Public function for creating a new event.
        public void CreateEvent(String a_name)
        {
            m_eventNames.Add(a_name, m_eventNames.Count);
        }

        // Public function for mapping a premade event to an input for an input action.
        public void MapEventToKeyAction(String a_name, int a_key)
        {
            if (m_eventNames.ContainsKey(a_name))
            {
                // Check if the key has been mapped to any action events before or whether it needs to be added first.
                if (m_actionsMap.ContainsKey(a_key))
                    m_actionsMap[a_key].Add(m_eventNames[a_name]);
                else
                    m_actionsMap.Add(a_key, new List<EventID> { m_eventNames[a_name] });

                // Mark that the event has been mapped to an action event even without any actual delegates to invoke.
                m_actionEvents.Add(m_eventNames[a_name], null);
            }
            else
            {
                throw new Exception("Trying to bind non existing event.");
            }
        }

        // Public function for mapping a premade event to an input for an input axis.
        public void MapEventToKeyAxis(String a_name, int a_key, float a_value)
        {
            if (m_eventNames.ContainsKey(a_name))
            {
                // Check if the key has been mapped to any axis events before or whether it needs to be added first.
                if (m_axisMap.ContainsKey(a_key))
                    if (m_axisMap[a_key].ContainsKey(m_eventNames[a_name]))
                        m_axisMap[a_key][m_eventNames[a_name]] = a_value;
                    else
                        m_axisMap[a_key].Add(m_eventNames[a_name], a_value);
                else
                    m_axisMap.Add(a_key, new Dictionary<EventID, float> { { m_eventNames[a_name], a_value } });

                // Mark that the event has been mapped to an axis event even without any actual delegates to invoke if the axis hasn't been mapped before.
                if (!m_axisEvents.ContainsKey(m_eventNames[a_name]))
                    m_axisEvents.Add(m_eventNames[a_name], null);
                // Create new axis if there wasn't one already.
                if (!m_axes.ContainsKey(m_eventNames[a_name]))
                    m_axes.Add(m_eventNames[a_name], 0f);
            }
            else
            {
                throw new Exception("Trying to bind non existing event.");
            }
        }

        // Public function for binding a function to an event that was mapped to an action.
        public void BindFunctionToAction(String a_name, KeyActionDelegate a_function)
        {
            if (m_eventNames.ContainsKey(a_name))
            {
                if (m_actionEvents.ContainsKey(m_eventNames[a_name]))
                    if (m_actionEvents[m_eventNames[a_name]] == null)
                        m_actionEvents[m_eventNames[a_name]] = a_function;
                    else
                        m_actionEvents[m_eventNames[a_name]] += a_function;
                else
                    throw new Exception("Trying to bind function to an event that wasn't mapped to an action.");
            }
            else
            {
                throw new Exception("Trying to bind function to non existing event.");
            }
        }

        // Public function for binding a function to an event that was mapped to an axis.
        public void BindFunctionToAxis(String a_name, KeyAxisDelegate a_function)
        {
            if (m_eventNames.ContainsKey(a_name))
            {
                if (m_axisEvents.ContainsKey(m_eventNames[a_name]))
                    if (m_axisEvents[m_eventNames[a_name]] == null)
                        m_axisEvents[m_eventNames[a_name]] = a_function;
                    else
                        m_axisEvents[m_eventNames[a_name]] += a_function;
                else
                    throw new Exception("Trying to bind function to an event that wasn't mapped to an axis.");
            }
            else
            {
                throw new Exception("Trying to bind non existing event.");
            }
        }

        // Public function to automatically scan and decide what kind of event it needs to be bound to.
        public void BindFunction(String a_name, Delegate a_function)
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
                    {
                        BindFunctionToAction(a_name, (KeyActionDelegate)(a_function));
                    }
                    else if (parameters[0].ParameterType.Equals(typeof(float)))
                    {
                        BindFunctionToAxis(a_name, (KeyAxisDelegate)(a_function));
                    }
                }
            }
            throw new Exception("Function incompatible with input handler.");
        }

        public void ScanObject(GameObject a_object)
        {
            foreach (KeyValuePair<String, EventID> eventName in m_eventNames)
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
            }
        }

        public string GetDiagnostics()
        {
            string output = "";
            output += "Number of key events: " + m_eventNames.Count + '\n';
            output += "   Of which " + m_actionEvents.Count + " are mapped to actions\n";
            output += "   And axes " + m_axes.Count + " are mapped to axes\n";

            int delegateCount = 0;
            foreach (KeyValuePair<EventID, KeyActionDelegate> actionEvent in m_actionEvents)
                delegateCount += (actionEvent.Value == null ? 0 : actionEvent.Value.GetInvocationList().Length);
            foreach (KeyValuePair<EventID, KeyAxisDelegate> axisEvent in m_axisEvents)
                delegateCount += (axisEvent.Value == null ? 0 : axisEvent.Value.GetInvocationList().Length);
            output += "Number of key input delegates: " + delegateCount + '\n';
            return output;
        }

    }
}

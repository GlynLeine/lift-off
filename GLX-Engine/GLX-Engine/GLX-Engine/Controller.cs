using System.Collections.Generic;
using System;

namespace GLXEngine
{
    public class GameController : ArduinoInterface
    {
        private static int m_controllerCount = 0;
        private int m_controllerID = -1;

        private Dictionary<Key, int> m_analogMap = new Dictionary<Key, int>();
        private Dictionary<Key, int> m_digitalMap = new Dictionary<Key, int>();

        Timer m_cooldownTimer = new Timer(5);

        private bool m_cooldown;

        public GameController(bool a_search = false, bool a_persistent = false) : base(a_search, a_persistent)
        {
            m_controllerID = m_controllerCount++;
            if (found)
            {
                AutoMapInputs();
            }

            m_cooldownTimer.triggerFunc += () => { m_cooldown = false; };
        }

        public int ID { get { return m_controllerID; } }

        public bool connected { get { return found && !closed; } }

        public bool Search(bool a_persistent = false, bool a_cooldown = true)
        {
            if (connected)
                return true;

            if (!m_cooldown || !a_cooldown)
            {
                m_cooldownTimer.m_timeBuffer = 0;
                m_cooldown = true;

                if (base.Search(a_persistent))
                {
                    AutoMapInputs();
                    return true;
                }
            }
            return false;
        }

        public float GetAnalog(Key a_analogInputID)
        {
            if (GetData())
                return m_analogs[m_analogMap[a_analogInputID]];
            else return 0;
        }

        public int analogCount { get { return m_analogs.Count; } }
        public List<Key> analogs { get { return new List<Key>(m_analogMap.Keys); } }

        public bool GetDigital(Key a_digitalInputID)
        {
            if (GetData())
                return m_digitals[m_digitalMap[a_digitalInputID]];
            else return false;
        }

        public int digitalCount { get { return m_digitals.Count; } }
        public List<Key> digitals { get { return new List<Key>(m_digitalMap.Keys); } }

        private void AutoMapInputs()
        {
            for (int analogID = 0; analogID < m_analogs.Count; analogID++)
            {
                if (m_analogMap.ContainsValue(analogID))
                    continue;

                Key analogInputID;
                if (Enum.TryParse("ANALOG" + analogID, out analogInputID))
                    m_analogMap.Add(analogInputID, analogID);
            }

            for (int digitalID = 0; digitalID < m_digitals.Count; digitalID++)
            {
                if (m_digitalMap.ContainsValue(digitalID))
                    continue;

                Key digitalInputID;
                if (Enum.TryParse("DIGITAL" + digitalID, out digitalInputID))
                    m_digitalMap.Add(digitalInputID, digitalID);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}

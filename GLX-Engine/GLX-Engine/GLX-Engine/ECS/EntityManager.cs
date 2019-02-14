using System;
using System.Collections.Generic;
using System.Reflection;

namespace GLXEngine.ECS
{
    class EntityManager
    {
        Dictionary<Type, Dictionary<Type, String>> m_componentNames = new Dictionary<Type, Dictionary<Type, string>>();
        Dictionary<Type, List<object>> m_entities = new Dictionary<Type, List<object>>();
        Dictionary<Type, Dictionary<object, ECSComponentHandle>> m_components = new Dictionary<Type, Dictionary<object, ECSComponentHandle>>();

        public void AddEntity(object a_entity)
        {
            Type entityType = a_entity.GetType();

            FieldInfo[] fields = entityType.GetFields();



            if (!m_entities[entityType].Contains(a_entity))
            {
                if (!m_componentNames.ContainsKey(entityType))
                {
                    if (fields != null)
                    {
                        Dictionary<Type, string> componentNames = new Dictionary<Type, string>();

                        foreach (FieldInfo field in fields)
                        {
                            if (field.FieldType.IsAssignableFrom(typeof(ECSComponent)))
                            {
                                componentNames.Add(field.FieldType, field.Name);

                                if(!m_components.ContainsKey(field.FieldType))
                                    m_components.Add(field.FieldType, new Dictionary<object, ECSComponentHandle>());

                                m_components[field.FieldType].Add(a_entity, new ECSComponentHandle(a_entity, field));
                            }
                        }

                        if (componentNames.Count > 0)
                        {
                            m_componentNames.Add(entityType, componentNames);
                            m_entities[entityType].Add(a_entity);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<Type, string> componentName in m_componentNames[entityType])
                    {
                        m_components[componentName.Key].Add(entityType, new ECSComponentHandle(a_entity, entityType.GetField(componentName.Value)));
                    }
                }
            }
        }

        public void RemoveEntity(object a_entity)
        {
            m_entities[a_entity.GetType()].Remove(a_entity);
            foreach(KeyValuePair<Type, string> components in m_componentNames[a_entity.GetType()])
            {
                m_components[components.Key].Remove(a_entity);
            }
        }

        public Dictionary<object, ECSComponentHandle> GetComponents<T>() where T : ECSComponent
        {
            return m_components[typeof(T)];
        }


    }
}

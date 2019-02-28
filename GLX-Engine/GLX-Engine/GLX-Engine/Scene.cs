using GLXEngine.Core;
using GLXEngine.Managers;
using System.Collections.Generic;

namespace GLXEngine
{
    public abstract class Scene : GameObject
    {
        protected UpdateManager m_updateManager;
        protected CollisionManager m_collisionManager;
        public KeyInputHandler m_keyInputHandler;
        protected List<GameObject> m_gameObjectsContained;

        private List<Scene> m_subScenes;
        private List<Scene> m_garbageScenes;
        private List<Scene> m_newScenes;

        public Vector2i m_dimensions;
        protected Rectangle _renderRange;

        public Scene m_masterScene;

        public bool m_active;
        public float m_timeActive;

        /// <summary>
        /// Step delegate defines the signature of a method used for step callbacks, see OnBeforeStep, OnAfterStep.
        /// </summary>
        public delegate void StepDelegate();

        /// <summary>
        /// Occurs before the engine starts the new update loop. This allows you to define general manager classes that can update itself on/after each frame.
        /// </summary>
        public virtual event StepDelegate OnBeforeStep;
        /// <summary>
        /// Occurs after the engine has finished its last update loop. This allows you to define general manager classes that can update itself on/after each frame.
        /// </summary>
        public virtual event StepDelegate OnAfterStep;

        public delegate void RenderDelegate(GLContext glContext);
        public virtual event RenderDelegate OnAfterRender;

        public Scene()
        {
            Setup(Game.main);
        }

        public Scene(Scene a_masterScene)
        {
            Setup(a_masterScene);
        }

        public virtual void Setup(Scene a_masterScene)
        {
            m_active = false;
            m_timeActive = 0f;

            m_masterScene = a_masterScene;
            m_updateManager = new UpdateManager();
            m_subScenes = new List<Scene>();
            m_garbageScenes = new List<Scene>();
            m_newScenes = new List<Scene>();

            if (a_masterScene != null)
            {
                m_collisionManager = new CollisionManager(a_masterScene.m_collisionManager);
                m_keyInputHandler = new KeyInputHandler(a_masterScene.m_keyInputHandler);
                a_masterScene.AddSubScene(this);
                RenderRange = a_masterScene.RenderRange;
            }
            else
            {
                m_collisionManager = new CollisionManager();
                m_keyInputHandler = new KeyInputHandler();
            }

            m_gameObjectsContained = new List<GameObject>();
            m_dimensions = new Vector2i();
        }

        public virtual void Start()
        {
            m_active = true;
        }

        public virtual void Restart()
        {
            m_children.Clear();
            OnDestroy();
            Setup(m_masterScene);
            Start();
        }


        protected override void OnDestroy()
        {
            m_keyInputHandler.Destroy();
            base.OnDestroy();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Step()
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void Step()
        {
            if (m_active)
            {

                Sound.Step();

                if (OnBeforeStep != null)
                    OnBeforeStep();

                m_keyInputHandler.Step();

                m_subScenes.ForEach(scene => scene.Step());

                m_subScenes.RemoveAll(scene => { return m_garbageScenes.Contains(scene); });
                m_garbageScenes.Clear();

                m_subScenes.AddRange(m_newScenes);
                m_newScenes.Clear();

                m_updateManager.Step();
                m_collisionManager.Step();

                if (OnAfterStep != null)
                    OnAfterStep();

            }
        }

        /// <summary>
        /// Sprites will be rendered if and only if they overlap with this rectangle. 
        /// Default value: (0,0,game.width,game.height). 
        /// You only need to change this when rendering to subwindows (e.g. split screen).
        /// </summary>
        /// <value>The render range.</value>
        public Rectangle RenderRange
        {
            get
            {
                Vector2 topLeft = InverseTransformPoint(_renderRange.left, _renderRange.top);
                Vector2 bottomRight = InverseTransformPoint(_renderRange.right, _renderRange.bottom);
                return new Rectangle(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
            }
            set
            {
                _renderRange = value;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Render(GLContext glContext)
        //------------------------------------------------------------------------------------------------------------------------
        public override void Render(GLContext glContext)
        {
            if (m_active)
                base.Render(glContext);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Add()
        //------------------------------------------------------------------------------------------------------------------------
        internal void Add(GameObject gameObject)
        {
            if (!m_gameObjectsContained.Contains(gameObject))
            {
                m_updateManager.Add(gameObject);
                m_collisionManager.Add(ref gameObject);
                m_gameObjectsContained.Add(gameObject);
                m_keyInputHandler.ScanObject(gameObject);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Remove()
        //------------------------------------------------------------------------------------------------------------------------
        internal void Remove(GameObject gameObject)
        {
            if (m_gameObjectsContained.Contains(gameObject))
            {
                m_updateManager.Remove(gameObject);
                m_collisionManager.Remove(gameObject);
                m_gameObjectsContained.Remove(gameObject);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Contains()
        //------------------------------------------------------------------------------------------------------------------------
        public bool Contains(GameObject gameObject)
        {
            if (m_subScenes.Count > 0)
                foreach (Scene scene in m_subScenes)
                    if (scene.Contains(gameObject)) return true;

            return m_gameObjectsContained.Contains(gameObject);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetGameObjectCollisions()
        //------------------------------------------------------------------------------------------------------------------------
        internal GameObject[] GetGameObjectCollisions(GameObject gameObject)
        {
            return m_collisionManager.GetCurrentCollisions(gameObject);
        }

        protected void AddSubScene(Scene a_scene)
        {
            if (!m_subScenes.Contains(a_scene) && !m_newScenes.Contains(a_scene))
                m_newScenes.Add(a_scene);
        }

        protected void RemoveSubScene(Scene a_scene)
        {
            if (m_subScenes.Contains(a_scene) && !m_garbageScenes.Contains(a_scene))
                m_garbageScenes.Add(a_scene);
        }

        public bool InView(float[] a_points)
        {
            if (a_points.Length % 2 == 0)
                for (int i = 0; i < a_points.Length - 1; i++)
                {
                    Vector2 point = new Vector2(a_points[i], a_points[i + 1]);
                    if (((point.x > RenderRange.left) && (point.y > RenderRange.top) && (point.x <= RenderRange.right) && (point.y <= RenderRange.bottom)))
                        return true;
                }
            return false;
        }

        public bool InView(Vector2[] a_points)
        {
            for (int i = 0; i < a_points.Length; i++)
            {
                Vector2 point = a_points[i];
                if (((point.x > RenderRange.left) && (point.y > RenderRange.top) && (point.x <= RenderRange.right) && (point.y <= RenderRange.bottom)))
                    return true;
            }
            return false;
        }

        public virtual int width
        {
            get { return m_dimensions.x; }
        }

        public virtual int height
        {
            get { return m_dimensions.y; }
        }

        public virtual List<GameObject> objectList
        {
            get { return m_gameObjectsContained; }
        }

        protected int CountSubtreeSize(GameObject subtreeRoot)
        {
            int counter = 1; // for the root
            foreach (GameObject child in subtreeRoot.GetChildren())
            {
                counter += CountSubtreeSize(child);
            }
            return counter;
        }

        public virtual string GetDiagnostics()
        {
            string output = "";
            output += "Number of game objects contained: " + m_gameObjectsContained.Count + '\n';
            output += "Number of objects in hierarchy: " + CountSubtreeSize(this) + '\n';
            output += "Number of subscenes in scene: " + m_subScenes.Count + '\n';
            output += "OnBeforeStep delegates: " + (OnBeforeStep == null ? 0 : OnBeforeStep.GetInvocationList().Length) + '\n';
            output += "OnAfterStep delegates: " + (OnAfterStep == null ? 0 : OnAfterStep.GetInvocationList().Length) + '\n';
            output += "OnAfterRender delegates: " + (OnAfterRender == null ? 0 : OnAfterRender.GetInvocationList().Length) + '\n';
            output += Texture2D.GetDiagnostics();
            output += m_collisionManager.GetDiagnostics();
            output += m_updateManager.GetDiagnostics();
            output += m_keyInputHandler.GetDiagnostics();
            return output;
        }
    }
}

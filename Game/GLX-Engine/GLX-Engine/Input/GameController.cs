using System.Collections.Generic;

namespace GLXEngine
{
    enum ControllerType
    {
        XBOX, DS4, STEAM
    }

    class GameController
    {
        ControllerType m_type;
        List<int> m_axes;

    }
}

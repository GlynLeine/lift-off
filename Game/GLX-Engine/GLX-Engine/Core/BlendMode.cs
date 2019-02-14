using System;
using GLXEngine.OpenGL;

namespace GLXEngine
{
	/// <summary>
	/// Defines different BlendModes, only two present now, but you can add your own.
	/// </summary>
	public class BlendMode
	{

		public static readonly BlendMode NORMAL = new BlendMode (
			"Normal", () => {	GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);	}
		);

		public static readonly BlendMode MULTIPLY = new BlendMode (
			"Multiply", () => {	GL.glBlendFunc(GL.GL_DST_COLOR, GL.GL_ZERO);	}
		);

		public delegate void Action();

		/// <summary>
		/// This should point to an anonymous function updating the blendfunc
		/// </summary>
		public readonly Action enable;

		/// <summary>
		/// A label for this blendmode
		/// </summary>
		public readonly string label;

		public BlendMode (string pLabel, Action pEnable)
		{
			if (pEnable == null) {
				throw new Exception ("Enabled action cannot be null");
			} else {
				enable = pEnable;
			}

			label = pLabel;
		}

		public override string ToString ()
		{
			return label;
		}

	}
}


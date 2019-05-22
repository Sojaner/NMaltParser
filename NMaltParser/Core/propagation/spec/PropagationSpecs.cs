using System.Collections.Generic;
using System.Text;

namespace NMaltParser.Core.Propagation.Spec
{


	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class PropagationSpecs : List<PropagationSpec>
	{
		public const long serialVersionUID = 1L;

		public PropagationSpecs() : base()
		{
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (PropagationSpec spec in this)
			{
				sb.Append(spec.ToString() + "\n");
			}

			return sb.ToString();
		}
	}

}
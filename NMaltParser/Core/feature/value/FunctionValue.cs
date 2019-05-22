using System.Text;

namespace NMaltParser.Core.Feature.Value
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class FunctionValue
	{
		protected internal Function.Function function;

		public FunctionValue(Function.Function function)
		{
			Function = function;
		}

		public virtual Function.Function Function
		{
			get
			{
				return function;
			}
			set
			{
				function = value;
			}
		}


		public abstract void reset();

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			return function.Equals(((FunctionValue)obj).function);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(function.ToString());
			sb.Append(':');
			return sb.ToString();
		}
	}

}
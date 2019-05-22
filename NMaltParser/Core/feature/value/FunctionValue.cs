using System.Text;

namespace org.maltparser.core.feature.value
{
	using  org.maltparser.core.feature.function;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class FunctionValue
	{
		protected internal Function function;

		public FunctionValue(Function function)
		{
			Function = function;
		}

		public virtual Function Function
		{
			get
			{
				return function;
			}
			set
			{
				this.function = value;
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
			if (this.GetType() != obj.GetType())
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
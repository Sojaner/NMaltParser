using System;

namespace NMaltParser.Parser.Transition
{

	/// <summary>
	/// Transition contains one individual transition. For example, Nivre arc-eager algorithms have the unlabeled 
	/// transition <code>SH</code>, <code>RE</code> and the labeled transition<code>RA</code>, <code>LA</code>. These
	/// transition will be four individual transition.
	/// 
	/// @author Joakim Nivre
	/// @author Johan Hall
	/// </summary>
	public class Transition : IComparable<Transition>
	{
		/// <summary>
		/// Transition code
		/// </summary>
		private readonly int code;
		/// <summary>
		/// Transition symbol
		/// </summary>
		private readonly string symbol;
		/// <summary>
		/// <code>true</code> if the transition is labeled, otherwise <code>false</code>
		/// </summary>
		private readonly bool labeled;
		private readonly int cachedHash;
		/// <summary>
		/// Creates a transition 
		/// </summary>
		/// <param name="code">	Transition code </param>
		/// <param name="symbol">	Transition name </param>
		/// <param name="labeled">	<code>true</code> if the transition is labeled, otherwise <code>false</code> </param>
		public Transition(int code, string symbol, bool labeled)
		{
			this.code = code;
			this.symbol = symbol;
			this.labeled = labeled;
			const int prime = 31;
			int result = prime + code;
			result = prime * result + (labeled ? 1231 : 1237);
			cachedHash = prime * result + ((ReferenceEquals(symbol, null)) ? 0 : symbol.GetHashCode());
		}

		/// <summary>
		/// Returns the transition code
		/// </summary>
		/// <returns> the transition code </returns>
		public virtual int Code
		{
			get
			{
				return code;
			}
		}

		/// <summary>
		/// Returns the transition symbol
		/// 
		/// @return	the transition symbol
		/// </summary>
		public virtual string Symbol
		{
			get
			{
				return symbol;
			}
		}

		/// <summary>
		/// Returns true if the transition is labeled, otherwise false
		/// </summary>
		/// <returns> <code>true</code> if the transition is labeled, otherwise <code>false</code> </returns>
		public virtual bool Labeled
		{
			get
			{
				return labeled;
			}
		}


		public virtual int CompareTo(Transition that)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;
			if (code < that.code)
			{
				return BEFORE;
			}
			if (code > that.code)
			{
				return AFTER;
			}
			return EQUAL;
		}

		public override int GetHashCode()
		{
			return cachedHash;
		}

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
			Transition other = (Transition) obj;
			if (code != other.code)
			{
				return false;
			}
			if (labeled != other.labeled)
			{
				return false;
			}
			if (ReferenceEquals(symbol, null))
			{
				if (!ReferenceEquals(other.symbol, null))
				{
					return false;
				}
			}
			else if (!symbol.Equals(other.symbol))
			{
				return false;
			}
			return true;
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			return symbol + " [" + code + "] " + labeled;
		}
	}

}
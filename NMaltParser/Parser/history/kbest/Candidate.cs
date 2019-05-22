using System;

namespace org.maltparser.parser.history.kbest
{

	/// <summary>
	/// A candidate in the k-best list. 
	/// 
	/// @author Johan Hall
	/// </summary>
	public class Candidate
	{
		/// <summary>
		/// The integer representation of the predicted action
		/// </summary>
		protected internal int actionCode;

		/// <summary>
		/// Constructs a candidate object
		/// </summary>
		public Candidate()
		{
			reset();
		}

		/// <summary>
		/// Returns an integer representation of the predicted action
		/// </summary>
		/// <returns> an integer representation of the predicted action </returns>
		public virtual int ActionCode
		{
			get
			{
				return actionCode;
			}
			set
			{
				this.actionCode = value;
			}
		}


		/// <summary>
		/// Resets the candidate object
		/// </summary>
		public virtual void reset()
		{
			this.actionCode = -1;
		}

		public override int GetHashCode()
		{
			return 31 + actionCode;
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
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			return actionCode == ((Candidate)obj).actionCode;
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			return Convert.ToString(actionCode);
		}
	}


}
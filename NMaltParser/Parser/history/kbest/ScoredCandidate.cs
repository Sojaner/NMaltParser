using System.Text;

namespace org.maltparser.parser.history.kbest
{
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ScoredCandidate : Candidate
	{
		/// <summary>
		/// The candidate score
		/// </summary>
		protected internal float score;

		/// <summary>
		/// Constructs a candidate object
		/// </summary>
		public ScoredCandidate() : base()
		{
		}

		/// <summary>
		/// Returns the score for this candidate if it is available, otherwise Double.NaN
		/// </summary>
		/// <returns> the score for this candidate if it is available, otherwise Double.NaN </returns>
		public virtual float getScore()
		{
			return score;
		}

		/// <summary>
		/// Sets the score for this candidate.
		/// </summary>
		/// <param name="score"> a score </param>
		public virtual void setScore(float? score)
		{
			this.score = score.Value;
		}

		/// <summary>
		/// Resets the candidate object
		/// </summary>
		public override void reset()
		{
			base.reset();
			this.score = Float.NaN;
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#equals(java.lang.Object)
		 */
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ScoredCandidate item = (ScoredCandidate)obj;
			ScoredCandidate item = (ScoredCandidate)obj;
			return actionCode == item.actionCode && score == item.score;
		}

		public override int GetHashCode()
		{
			return (31 * 7 + actionCode) * 31 + Float.floatToIntBits(score);
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append('\t');
			sb.Append(score);
			return sb.ToString();
		}
	}

}
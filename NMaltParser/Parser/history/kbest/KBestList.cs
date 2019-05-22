using System.Collections.Generic;
using System.Text;

namespace org.maltparser.parser.history.kbest
{

	using  org.maltparser.core.exception;
	using  org.maltparser.parser.history.action;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class KBestList
	{
		protected internal readonly List<Candidate> kBestList;
		protected internal int k = -1;
		protected internal int topCandidateIndex;
		protected internal int addCandidateIndex;
		protected internal readonly SingleDecision decision;

		/// <summary>
		/// Creates a unrestricted k-best list
		/// </summary>
		/// <param name="decision"> a reference to the single decision that uses the k-best list </param>
		public KBestList(SingleDecision decision) : this(-1, decision)
		{
		}

		/// <summary>
		/// Creates a k-best list
		/// </summary>
		/// <param name="k">	the k-best list size </param>
		/// <param name="decision">	a reference to the single decision that uses the k-best list. </param>
		public KBestList(int? k, SingleDecision decision)
		{
			K = k.Value;
			this.decision = decision;
			if (this.k > 0)
			{
				kBestList = new List<Candidate>(this.k);
				initKBestList();
			}
			else
			{
				kBestList = new List<Candidate>();
			}

		}

		protected internal virtual void initKBestList()
		{
			for (int i = 0; i < this.k; i++)
			{
				kBestList.Add(new Candidate());
			}
		}

		/// <summary>
		/// Resets the k-best list
		/// </summary>
		public virtual void reset()
		{
			this.topCandidateIndex = 0;
			this.addCandidateIndex = 0;
		}

		/// <summary>
		/// Adds a candidate to the k-best list
		/// </summary>
		/// <param name="actionCode"> the integer representation  of candidate action </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(int actionCode) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(int actionCode)
		{
			if (k != -1 && addCandidateIndex >= k)
			{
				return;
			}
			if (addCandidateIndex >= kBestList.Count)
			{
				kBestList.Add(new Candidate());
			}
			kBestList[addCandidateIndex].ActionCode = actionCode;
			if (addCandidateIndex == 0)
			{
	//			if (decision instanceof SingleDecision) {
	//				((SingleDecision)decision).addDecision(actionCode);
	//			}
				decision.addDecision(actionCode);
				topCandidateIndex++;
			}
			addCandidateIndex++;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addList(int[] predictionList) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addList(int[] predictionList)
		{
			int n = (k != -1 && k <= predictionList.Length - 1)?k:predictionList.Length - 1;
			for (int i = 0; i < n; i++)
			{
				add(predictionList[i]);
			}
		}

		/// <summary>
		/// Adds a candidate to the k-best list
		/// </summary>
		/// <param name="symbol"> the string representation of candidate action </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(string symbol)
		{
	//		if (decision instanceof SingleDecision) {
	//			this.add(((SingleDecision)decision).getDecisionCode(symbol));
	//		}
			this.add(decision.getDecisionCode(symbol));
		}


		/// <summary>
		/// Updates the corresponding single decision with the next value in the k-best list.
		/// </summary>
		/// <returns> true if decision has been updated, otherwise false </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean updateActionWithNextKBest() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool updateActionWithNextKBest()
		{
			if (addCandidateIndex != 0 && topCandidateIndex < addCandidateIndex && topCandidateIndex < kBestList.Count)
			{
				int actionCode = kBestList[topCandidateIndex].ActionCode;
				if (decision is SingleDecision)
				{
					((SingleDecision)decision).addDecision(actionCode);
				}
				topCandidateIndex++;
				return true;
			}
			return false;
		}

		public virtual int peekNextKBest()
		{
			if (addCandidateIndex != 0 && topCandidateIndex < addCandidateIndex && topCandidateIndex < kBestList.Count)
			{
				return kBestList[topCandidateIndex].ActionCode;
			}
			return -1;
		}

		/// <summary>
		/// Returns the current size of the k-best list
		/// </summary>
		/// <returns> the current size of the k-best list </returns>
		public virtual int CurrentSize
		{
			get
			{
				return addCandidateIndex;
				//return kBestList.size();
			}
		}

		/// <summary>
		/// Returns the maximum number of candidates in the k-best list.
		/// </summary>
		/// <returns> the maximum number of candidates in the k-best list </returns>
		public virtual int K
		{
			get
			{
				return k;
			}
			set
			{
				if (value == 0)
				{
					this.k = 1; // the value-best list must contain at least one candidate
				}
				if (value < 0)
				{
					this.k = -1; // this means that the value-best list is unrestricted.
				}
				else
				{
					this.k = value;
				}
			}
		}

		protected internal virtual int TopCandidateIndex
		{
			get
			{
				return topCandidateIndex;
			}
		}

		protected internal virtual int AddCandidateIndex
		{
			get
			{
				return addCandidateIndex;
			}
		}

		/// <summary>
		/// Returns a single decision object
		/// </summary>
		/// <returns> a single decision object </returns>
		public virtual SingleDecision Decision
		{
			get
			{
				return decision;
			}
		}

		public virtual int KBestListSize
		{
			get
			{
				return kBestList.Count;
			}
		}

		public virtual ScoredCandidate getCandidate(int i)
		{
			if (i >= kBestList.Count)
			{
				return null;
			}
			return (ScoredCandidate)kBestList[i];
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("[ ");
			for (int i = 0; i < addCandidateIndex; i++)
			{
				sb.Append(kBestList[i]);
				sb.Append(' ');
			}
			sb.Append("] ");
			return sb.ToString();
		}
	}

}
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser.History.KBest
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ScoredKBestList : KBestList
	{

		public ScoredKBestList(SingleDecision action) : this(-1, action)
		{
		}

		public ScoredKBestList(int? k, SingleDecision action) : base(k, action)
		{
		}

		protected internal override void initKBestList()
		{
			for (int i = 0; i < k; i++)
			{
				kBestList.Add(new ScoredCandidate());
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(int actionCode, float score) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(int actionCode, float score)
		{
			if (k != -1 && addCandidateIndex >= k)
			{
				return;
			}
			if (addCandidateIndex >= kBestList.Count)
			{
				kBestList.Add(new ScoredCandidate());
			}
			if (!(kBestList[addCandidateIndex] is ScoredCandidate))
			{
				base.add(actionCode);
				return;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ScoredCandidate scand = (ScoredCandidate)kBestList.get(addCandidateIndex);
			ScoredCandidate scand = (ScoredCandidate)kBestList[addCandidateIndex];
			scand.ActionCode = actionCode;
			scand.setScore(score);
			if (addCandidateIndex == 0)
			{
				if (decision is SingleDecision)
				{
					((SingleDecision)decision).addDecision(actionCode);
				}
				topCandidateIndex++;
			}
			addCandidateIndex++;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(String symbol, float score) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(string symbol, float score)
		{
			if (decision is SingleDecision)
			{
				add(((SingleDecision)decision).getDecisionCode(symbol), score);
			}
		}

		public virtual float peekNextKBestScore()
		{
			if (!(kBestList[addCandidateIndex] is ScoredCandidate))
			{
				return Float.NaN;
			}
			if (addCandidateIndex != 0 && topCandidateIndex < addCandidateIndex && topCandidateIndex < kBestList.Count)
			{
				return ((ScoredCandidate)kBestList[topCandidateIndex]).getScore();
			}
			return Float.NaN;
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			return base.ToString();
		}
	}

}
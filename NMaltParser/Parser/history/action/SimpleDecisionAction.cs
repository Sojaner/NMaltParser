using System.Text;

namespace org.maltparser.parser.history.action
{
	using  org.maltparser.core.exception;
	using  org.maltparser.parser.history.container;
	using  org.maltparser.parser.history.container.TableContainer;
	using  org.maltparser.parser.history.kbest;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class SimpleDecisionAction : SingleDecision
	{
		private readonly TableContainer tableContainer;
		private int decision;
		private readonly KBestList kBestList;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SimpleDecisionAction(int kBestSize, org.maltparser.parser.history.container.TableContainer _tableContainer) throws org.maltparser.core.exception.MaltChainedException
		public SimpleDecisionAction(int kBestSize, TableContainer _tableContainer)
		{
			this.tableContainer = _tableContainer;
			this.kBestList = new KBestList(kBestSize, this);
			clear();
		}

		/* Action interface */
		public virtual void clear()
		{
			decision = -1;
			kBestList.reset();
		}

		public virtual int numberOfDecisions()
		{
			return 1;
		}

		/* SingleDecision interface */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addDecision(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addDecision(int code)
		{
			if (code == -1 || !tableContainer.containCode(code))
			{
				decision = -1;
			}
			decision = code;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addDecision(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addDecision(string symbol)
		{
			decision = tableContainer.getCode(symbol);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDecisionCode() throws org.maltparser.core.exception.MaltChainedException
		public virtual int DecisionCode
		{
			get
			{
				return decision;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDecisionCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getDecisionCode(string symbol)
		{
			return tableContainer.getCode(symbol);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getDecisionSymbol() throws org.maltparser.core.exception.MaltChainedException
		public virtual string DecisionSymbol
		{
			get
			{
				return tableContainer.getSymbol(decision);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean updateFromKBestList() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool updateFromKBestList()
		{
			return kBestList.updateActionWithNextKBest();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean continueWithNextDecision() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool continueWithNextDecision()
		{
			return tableContainer.continueWithNextDecision(decision);
		}

		public virtual TableContainer TableContainer
		{
			get
			{
				return tableContainer;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.kbest.KBestList getKBestList() throws org.maltparser.core.exception.MaltChainedException
		public virtual KBestList KBestList
		{
			get
			{
				return kBestList;
			}
		}

		public virtual TableContainer.RelationToNextDecision RelationToNextDecision
		{
			get
			{
				return tableContainer.getRelationToNextDecision();
			}
		}

	//	private void createKBestList() throws MaltChainedException {
	//		final Class<?> kBestListClass = history.getKBestListClass();
	//		if (kBestListClass == null) {
	//			return;
	//		}
	//		final Class<?>[] argTypes = { java.lang.Integer.class, org.maltparser.parser.history.action.SingleDecision.class };
	//	
	//		final Object[] arguments = new Object[2];
	//		arguments[0] = history.getKBestSize();
	//		arguments[1] = this;
	//		try {
	//			final Constructor<?> constructor = kBestListClass.getConstructor(argTypes);
	//			kBestList = (KBestList)constructor.newInstance(arguments);
	//		} catch (NoSuchMethodException e) {
	//			throw new HistoryException("The kBestlist '"+kBestListClass.getName()+"' cannot be initialized. ", e);
	//		} catch (InstantiationException e) {
	//			throw new HistoryException("The kBestlist '"+kBestListClass.getName()+"' cannot be initialized. ", e);
	//		} catch (IllegalAccessException e) {
	//			throw new HistoryException("The kBestlist '"+kBestListClass.getName()+"' cannot be initialized. ", e);
	//		} catch (InvocationTargetException e) {
	//			throw new HistoryException("The kBestlist '"+kBestListClass.getName()+"' cannot be initialized. ", e);
	//		}
	//	}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(decision);
			return sb.ToString();
		}
	}

}
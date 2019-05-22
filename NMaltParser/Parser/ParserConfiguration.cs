namespace org.maltparser.parser
{
	using  core.exception;
	using  core.syntaxgraph;
	using  history;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class ParserConfiguration
	{
		protected internal HistoryNode historyNode;


		/// <summary>
		/// Creates a parser configuration
		/// </summary>
		public ParserConfiguration()
		{
			HistoryNode = null;
		}

		public virtual HistoryNode HistoryNode
		{
			get
			{
				return historyNode;
			}
			set
			{
				historyNode = value;
			}
		}


		/// <summary>
		/// Sets the dependency structure
		/// </summary>
		/// <param name="dependencyStructure"> a dependency structure </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setDependencyGraph(org.maltparser.core.syntaxgraph.DependencyStructure dependencyStructure) throws org.maltparser.core.exception.MaltChainedException;
		public abstract DependencyStructure DependencyGraph {set;get;}
		/// <summary>
		/// Returns true if the parser configuration is in a terminal state, otherwise false.
		/// </summary>
		/// <returns> true if the parser configuration is in a terminal state, otherwise false. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean isTerminalState() throws org.maltparser.core.exception.MaltChainedException;
		public abstract bool TerminalState {get;}
		/// <summary>
		/// Clears the parser configuration
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void clear() throws org.maltparser.core.exception.MaltChainedException;
		public abstract void clear();

	//	/**
	//	 * Initialize the parser configuration with the same state as the parameter config
	//	 * 
	//	 * @param config a parser configuration
	//	 * @throws MaltChainedException
	//	 */
	//	public abstract void initialize(ParserConfiguration config) throws MaltChainedException;
		/// <summary>
		/// Initialize the parser configuration
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void initialize() throws org.maltparser.core.exception.MaltChainedException;
		public abstract void initialize();
	}

}
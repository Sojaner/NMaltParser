namespace org.maltparser.core.flow.item
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.flow.spec;
	using  org.maltparser.core.flow.system.elem;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public abstract class ChartItem
	{
		protected internal FlowChartInstance flowChartinstance;
		protected internal ChartItemSpecification chartItemSpecification;

		// Signals
		public const int CONTINUE = 1;
		public const int TERMINATE = 2;
		public const int NEWITERATION = 3;

		public ChartItem()
		{
		}

		/// <summary>
		/// Initialize the chart item
		/// </summary>
		/// <param name="flowChartinstance"> the flow chart instance that the chart item belongs to </param>
		/// <param name="chartItemSpecification"> a specification of the chart item </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(org.maltparser.core.flow.FlowChartInstance flowChartinstance, org.maltparser.core.flow.spec.ChartItemSpecification chartItemSpecification) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initialize(FlowChartInstance flowChartinstance, ChartItemSpecification chartItemSpecification)
		{
			FlowChartInstance = flowChartinstance;
			ChartItemSpecification = chartItemSpecification;
		}

		/// <summary>
		/// Cause the chart item to perform the preprocess tasks
		/// </summary>
		/// <param name="signal"> returned by the previous chart item </param>
		/// <returns> true if every thing is ok, otherwise false </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int preprocess(int signal) throws org.maltparser.core.exception.MaltChainedException;
		public abstract int preprocess(int signal);

		/// <summary>
		/// Cause the chart item to perform the process task (for every sentence)
		/// </summary>
		/// <param name="signal"> returned by the previous chart item </param>
		/// <returns> true if it is ready to perform the next sentence, otherwise false </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int process(int signal) throws org.maltparser.core.exception.MaltChainedException;
		public abstract int process(int signal);

		/// <summary>
		/// Cause the chart item to perform the postprocess tasks
		/// </summary>
		/// <param name="signal"> returned by the previous chart item </param>
		/// <returns> true if every thing is ok, otherwise false </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int postprocess(int signal) throws org.maltparser.core.exception.MaltChainedException;
		public abstract int postprocess(int signal);

		/// <summary>
		/// Terminates and cleans up the chart item
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void terminate() throws org.maltparser.core.exception.MaltChainedException;
		public abstract void terminate();

		/// <summary>
		/// Returns the flow chart instance that the chart item belongs to
		/// </summary>
		/// <returns> the flow chart instance that the chart item belongs to </returns>
		public virtual FlowChartInstance FlowChartInstance
		{
			get
			{
				return flowChartinstance;
			}
			set
			{
				this.flowChartinstance = value;
			}
		}


		/// <summary>
		/// Returns the option container index
		/// </summary>
		/// <returns> the option container index </returns>
		public virtual int OptionContainerIndex
		{
			get
			{
				return flowChartinstance.OptionContainerIndex;
			}
		}

		/// <summary>
		/// Returns the chart element in the flow chart system description
		/// </summary>
		/// <param name="key"> a chart element key </param>
		/// <returns> the chart element in the flow chart system description </returns>
		public virtual ChartElement getChartElement(string key)
		{
			return flowChartinstance.FlowChartManager.FlowChartSystem.getChartElement(key);
		}

		/// <summary>
		/// Returns a chart item specification
		/// </summary>
		/// <returns> a chart item specification </returns>
		public virtual ChartItemSpecification ChartItemSpecification
		{
			get
			{
				return chartItemSpecification;
			}
			set
			{
				this.chartItemSpecification = value;
			}
		}

	}

}
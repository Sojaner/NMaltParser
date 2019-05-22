namespace org.maltparser.parser.guide.decision
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureModel = org.maltparser.core.feature.FeatureModel;
	using FeatureVector = org.maltparser.core.feature.FeatureVector;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using AtomicModel = org.maltparser.parser.guide.instance.AtomicModel;
	using FeatureDivideModel = org.maltparser.parser.guide.instance.FeatureDivideModel;
	using InstanceModel = org.maltparser.parser.guide.instance.InstanceModel;
	using GuideDecision = org.maltparser.parser.history.action.GuideDecision;
	using MultipleDecision = org.maltparser.parser.history.action.MultipleDecision;
	using SingleDecision = org.maltparser.parser.history.action.SingleDecision;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class OneDecisionModel : DecisionModel
	{
		private readonly ClassifierGuide guide;
		private readonly string modelName;
	//	private final FeatureModel featureModel;
		private readonly int decisionIndex;
		private readonly DecisionModel prevDecisionModel;
		private readonly string branchedDecisionSymbols;
		private InstanceModel instanceModel;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OneDecisionModel(org.maltparser.parser.guide.ClassifierGuide _guide) throws org.maltparser.core.exception.MaltChainedException
		public OneDecisionModel(ClassifierGuide _guide)
		{
			this.branchedDecisionSymbols = "";
			this.guide = _guide;
	//		this.featureModel = _featureModel;
			this.decisionIndex = 0;
			if (string.ReferenceEquals(guide.GuideName, null) || guide.GuideName.Equals(""))
			{
				this.modelName = "odm" + decisionIndex;
			}
			else
			{
				this.modelName = guide.GuideName + ".odm" + decisionIndex;
			}
			this.prevDecisionModel = null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OneDecisionModel(org.maltparser.parser.guide.ClassifierGuide _guide, DecisionModel _prevDecisionModel, String _branchedDecisionSymbol) throws org.maltparser.core.exception.MaltChainedException
		public OneDecisionModel(ClassifierGuide _guide, DecisionModel _prevDecisionModel, string _branchedDecisionSymbol)
		{
			this.prevDecisionModel = _prevDecisionModel;
			this.decisionIndex = prevDecisionModel.DecisionIndex + 1;
			if (!string.ReferenceEquals(_branchedDecisionSymbol, null) && _branchedDecisionSymbol.Length > 0)
			{
				this.branchedDecisionSymbols = _branchedDecisionSymbol;
				this.modelName = "odm" + decisionIndex + branchedDecisionSymbols;
			}
			else
			{
				this.branchedDecisionSymbols = "";
				this.modelName = "odm" + decisionIndex;
			}
			this.guide = _guide;
	//		this.featureModel = prevDecisionModel.getFeatureModel();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private final void initInstanceModel(org.maltparser.core.feature.FeatureModel featureModel, String subModelName) throws org.maltparser.core.exception.MaltChainedException
		private void initInstanceModel(FeatureModel featureModel, string subModelName)
		{
			if (featureModel.hasDivideFeatureFunction())
			{
				instanceModel = new FeatureDivideModel(this);
			}
			else
			{
				instanceModel = new AtomicModel(-1, this);
			}
		}

	//	public void updateFeatureModel() throws MaltChainedException {
	//		featureModel.update();
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void finalizeSentence(DependencyStructure dependencyGraph)
		{
			if (instanceModel != null)
			{
				instanceModel.finalizeSentence(dependencyGraph);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException
		public virtual void noMoreInstances(FeatureModel featureModel)
		{
			if (guide.GuideMode == org.maltparser.parser.guide.ClassifierGuide_GuideMode.CLASSIFY)
			{
				throw new GuideException("The decision model could not create it's model. ");
			}

			if (instanceModel != null)
			{
				instanceModel.noMoreInstances(featureModel);
				instanceModel.train();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			if (instanceModel != null)
			{
				instanceModel.terminate();
				instanceModel = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(FeatureModel featureModel, GuideDecision decision)
		{
			featureModel.update();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = (decision instanceof org.maltparser.parser.history.action.SingleDecision)?(org.maltparser.parser.history.action.SingleDecision)decision:((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = (decision is SingleDecision)?(SingleDecision)decision:((MultipleDecision)decision).getSingleDecision(decisionIndex);

			if (instanceModel == null)
			{
				initInstanceModel(featureModel, singleDecision.TableContainer.TableContainerName);
			}
			instanceModel.addInstance(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.TableContainer.TableContainerName), singleDecision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureModel featureModel, GuideDecision decision)
		{
			featureModel.update();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = (decision instanceof org.maltparser.parser.history.action.SingleDecision)?(org.maltparser.parser.history.action.SingleDecision)decision:((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = (decision is SingleDecision)?(SingleDecision)decision:((MultipleDecision)decision).getSingleDecision(decisionIndex);

			if (instanceModel == null)
			{
				initInstanceModel(featureModel, singleDecision.TableContainer.TableContainerName);
			}
			return instanceModel.predict(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.TableContainer.TableContainerName), singleDecision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector predictExtract(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector predictExtract(FeatureModel featureModel, GuideDecision decision)
		{
			featureModel.update();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = (decision instanceof org.maltparser.parser.history.action.SingleDecision)?(org.maltparser.parser.history.action.SingleDecision)decision:((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = (decision is SingleDecision)?(SingleDecision)decision:((MultipleDecision)decision).getSingleDecision(decisionIndex);

			if (instanceModel == null)
			{
				initInstanceModel(featureModel, singleDecision.TableContainer.TableContainerName);
			}
			return instanceModel.predictExtract(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.TableContainer.TableContainerName), singleDecision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector extract(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector extract(FeatureModel featureModel)
		{
			featureModel.update();
			return null; //instanceModel.extract(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.getTableContainer().getTableContainerName()));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predictFromKBestList(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predictFromKBestList(FeatureModel featureModel, GuideDecision decision)
		{
			if (decision is SingleDecision)
			{
				return ((SingleDecision)decision).updateFromKBestList();
			}
			else
			{
				return ((MultipleDecision)decision).getSingleDecision(decisionIndex).updateFromKBestList();
			}
		}

		public virtual ClassifierGuide Guide
		{
			get
			{
				return guide;
			}
		}

		public virtual string ModelName
		{
			get
			{
				return modelName;
			}
		}

		public virtual int DecisionIndex
		{
			get
			{
				return decisionIndex;
			}
		}

		public virtual DecisionModel PrevDecisionModel
		{
			get
			{
				return prevDecisionModel;
			}
		}

		public override string ToString()
		{
			return modelName;
		}
	}

}
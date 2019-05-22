using System.Text;

namespace org.maltparser.parser.guide.decision
{
    using  core.feature;
    using  core.syntaxgraph;
	using  instance;
    using  history.action;

    /// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.1
	/// 
	/// </summary>
	public class SeqDecisionModel : DecisionModel
	{
		private readonly ClassifierGuide guide;
		private readonly string modelName;
	//	private final FeatureModel featureModel;
		private InstanceModel instanceModel;
		private readonly int decisionIndex;
		private readonly DecisionModel prevDecisionModel;
		private DecisionModel nextDecisionModel;
		private readonly string branchedDecisionSymbols;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SeqDecisionModel(org.maltparser.parser.guide.ClassifierGuide _guide) throws org.maltparser.core.exception.MaltChainedException
		public SeqDecisionModel(ClassifierGuide _guide)
		{
			guide = _guide;
			branchedDecisionSymbols = "";
	//		this.featureModel = _featureModel;
			decisionIndex = 0;
			modelName = "sdm" + decisionIndex;
			prevDecisionModel = null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SeqDecisionModel(org.maltparser.parser.guide.ClassifierGuide _guide, DecisionModel _prevDecisionModel, String _branchedDecisionSymbol) throws org.maltparser.core.exception.MaltChainedException
		public SeqDecisionModel(ClassifierGuide _guide, DecisionModel _prevDecisionModel, string _branchedDecisionSymbol)
		{
			guide = _guide;
			decisionIndex = _prevDecisionModel.DecisionIndex + 1;
			if (!ReferenceEquals(_branchedDecisionSymbol, null) && _branchedDecisionSymbol.Length > 0)
			{
				branchedDecisionSymbols = _branchedDecisionSymbol;
				modelName = "sdm" + decisionIndex + branchedDecisionSymbols;
			}
			else
			{
				branchedDecisionSymbols = "";
				modelName = "sdm" + decisionIndex;
			}
	//		this.featureModel = _prevDecisionModel.getFeatureModel();
			prevDecisionModel = _prevDecisionModel;
		}

	//	public void updateFeatureModel() throws MaltChainedException {
	//		featureModel.update();
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initInstanceModel(org.maltparser.core.feature.FeatureModel featureModel, String subModelName) throws org.maltparser.core.exception.MaltChainedException
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void finalizeSentence(DependencyStructure dependencyGraph)
		{
			if (instanceModel != null)
			{
				instanceModel.finalizeSentence(dependencyGraph);
			}
			if (nextDecisionModel != null)
			{
				nextDecisionModel.finalizeSentence(dependencyGraph);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException
		public virtual void noMoreInstances(FeatureModel featureModel)
		{
			if (guide.GuideMode == ClassifierGuide_GuideMode.CLASSIFY)
			{
				throw new GuideException("The decision model could not create it's model. ");
			}
			if (instanceModel != null)
			{
				instanceModel.noMoreInstances(featureModel);
				instanceModel.train();
			}
			if (nextDecisionModel != null)
			{
				nextDecisionModel.noMoreInstances(featureModel);
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
			if (nextDecisionModel != null)
			{
				nextDecisionModel.terminate();
				nextDecisionModel = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(FeatureModel featureModel, GuideDecision decision)
		{
			if (decision is SingleDecision)
			{
				throw new GuideException("A sequantial decision model expect a sequence of decisions, not a single decision. ");
			}
			featureModel.update();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = ((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = ((MultipleDecision)decision).getSingleDecision(decisionIndex);
			if (instanceModel == null)
			{
				initInstanceModel(featureModel, singleDecision.TableContainer.TableContainerName);
			}
			instanceModel.addInstance(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.TableContainer.TableContainerName), singleDecision);
			if (singleDecision.continueWithNextDecision() && decisionIndex + 1 < decision.numberOfDecisions())
			{
				if (nextDecisionModel == null)
				{
					initNextDecisionModel(((MultipleDecision)decision).getSingleDecision(decisionIndex + 1), branchedDecisionSymbols);
				}
				nextDecisionModel.addInstance(featureModel, decision);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureModel featureModel, GuideDecision decision)
		{
			if (decision is SingleDecision)
			{
				throw new GuideException("A sequantial decision model expect a sequence of decisions, not a single decision. ");
			}
			featureModel.update();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = ((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = ((MultipleDecision)decision).getSingleDecision(decisionIndex);
			if (instanceModel == null)
			{
				initInstanceModel(featureModel, singleDecision.TableContainer.TableContainerName);
			}

			bool success = instanceModel.predict(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.TableContainer.TableContainerName), singleDecision);
			if (singleDecision.continueWithNextDecision() && decisionIndex + 1 < decision.numberOfDecisions())
			{
				if (nextDecisionModel == null)
				{
					initNextDecisionModel(((MultipleDecision)decision).getSingleDecision(decisionIndex + 1), branchedDecisionSymbols);
				}
				success = nextDecisionModel.predict(featureModel, decision) && success;
			}
			return success;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector predictExtract(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector predictExtract(FeatureModel featureModel, GuideDecision decision)
		{
			if (decision is SingleDecision)
			{
				throw new GuideException("A sequantial decision model expect a sequence of decisions, not a single decision. ");
			}
			featureModel.update();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = ((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = ((MultipleDecision)decision).getSingleDecision(decisionIndex);
			if (instanceModel == null)
			{
				initInstanceModel(featureModel, singleDecision.TableContainer.TableContainerName);
			}

			FeatureVector fv = instanceModel.predictExtract(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.TableContainer.TableContainerName), singleDecision);
			if (singleDecision.continueWithNextDecision() && decisionIndex + 1 < decision.numberOfDecisions())
			{
				if (nextDecisionModel == null)
				{
					initNextDecisionModel(((MultipleDecision)decision).getSingleDecision(decisionIndex + 1), branchedDecisionSymbols);
				}
				nextDecisionModel.predictExtract(featureModel, decision);
			}
			return fv;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector extract(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector extract(FeatureModel featureModel)
		{
			featureModel.update();
			return null; //instanceModel.extract(); // TODO handle many feature vectors
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predictFromKBestList(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predictFromKBestList(FeatureModel featureModel, GuideDecision decision)
		{
			if (decision is SingleDecision)
			{
				throw new GuideException("A sequantial decision model expect a sequence of decisions, not a single decision. ");
			}

			bool success = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = ((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = ((MultipleDecision)decision).getSingleDecision(decisionIndex);
			// TODO develop different strategies for resolving which kBestlist that should be used
			if (nextDecisionModel != null && singleDecision.continueWithNextDecision())
			{
				success = nextDecisionModel.predictFromKBestList(featureModel, decision);
			}
			if (!success)
			{
				success = singleDecision.updateFromKBestList();
				if (success && singleDecision.continueWithNextDecision() && decisionIndex + 1 < decision.numberOfDecisions())
				{
					if (nextDecisionModel == null)
					{
						initNextDecisionModel(((MultipleDecision)decision).getSingleDecision(decisionIndex + 1), branchedDecisionSymbols);
					}
					nextDecisionModel.predict(featureModel, decision);
				}
			}
			return success;
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

	//	public FeatureModel getFeatureModel() {
	//		return featureModel;
	//	}

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

		public virtual DecisionModel NextDecisionModel
		{
			get
			{
				return nextDecisionModel;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initNextDecisionModel(org.maltparser.parser.history.action.SingleDecision decision, String branchedDecisionSymbol) throws org.maltparser.core.exception.MaltChainedException
		private void initNextDecisionModel(SingleDecision decision, string branchedDecisionSymbol)
		{
			if (decision.RelationToNextDecision == RelationToNextDecision.SEQUANTIAL)
			{
				nextDecisionModel = new SeqDecisionModel(guide, this, branchedDecisionSymbol);
			}
			else if (decision.RelationToNextDecision == RelationToNextDecision.BRANCHED)
			{
				nextDecisionModel = new BranchedDecisionModel(guide, this, branchedDecisionSymbol);
			}
			else if (decision.RelationToNextDecision == RelationToNextDecision.NONE)
			{
				nextDecisionModel = new OneDecisionModel(guide, this, branchedDecisionSymbol);
			}
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(modelName + ", ");
			sb.Append(nextDecisionModel.ToString());
			return sb.ToString();
		}
	}

}
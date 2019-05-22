using System.Text;

namespace org.maltparser.parser.guide.decision
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.syntaxgraph;
	using  org.maltparser.parser.guide.instance;
	using  org.maltparser.parser.guide.instance;
	using  org.maltparser.parser.guide.instance;
	using  org.maltparser.parser.history.action;
	using  org.maltparser.parser.history.action;
	using  org.maltparser.parser.history.action;
	using  org.maltparser.parser.history.container.TableContainer;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class BranchedDecisionModel : DecisionModel
	{
		private readonly ClassifierGuide guide;
		private readonly string modelName;
	//	private final FeatureModel featureModel;
		private InstanceModel instanceModel;
		private readonly int decisionIndex;
		private readonly DecisionModel parentDecisionModel;
		private readonly HashMap<int, DecisionModel> children;
		private readonly string branchedDecisionSymbols;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BranchedDecisionModel(org.maltparser.parser.guide.ClassifierGuide _guide) throws org.maltparser.core.exception.MaltChainedException
		public BranchedDecisionModel(ClassifierGuide _guide)
		{
			this.guide = _guide;
			this.branchedDecisionSymbols = "";
	//		this.featureModel = _featureModel;
			this.decisionIndex = 0;
			this.modelName = "bdm0";
			this.parentDecisionModel = null;
			this.children = new HashMap<int, DecisionModel>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BranchedDecisionModel(org.maltparser.parser.guide.ClassifierGuide _guide, DecisionModel _parentDecisionModel, String _branchedDecisionSymbol) throws org.maltparser.core.exception.MaltChainedException
		public BranchedDecisionModel(ClassifierGuide _guide, DecisionModel _parentDecisionModel, string _branchedDecisionSymbol)
		{
			this.guide = _guide;
			this.parentDecisionModel = _parentDecisionModel;
			this.decisionIndex = parentDecisionModel.DecisionIndex + 1;
			if (!string.ReferenceEquals(_branchedDecisionSymbol, null) && _branchedDecisionSymbol.Length > 0)
			{
				this.branchedDecisionSymbols = _branchedDecisionSymbol;
				this.modelName = "bdm" + decisionIndex + branchedDecisionSymbols;
			}
			else
			{
				this.branchedDecisionSymbols = "";
				this.modelName = "bdm" + decisionIndex;
			}
	//		this.featureModel = parentDecisionModel.getFeatureModel();
			this.children = new HashMap<int, DecisionModel>();
		}

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
			if (children != null)
			{
				foreach (DecisionModel child in children.Values)
				{
					child.finalizeSentence(dependencyGraph);
				}
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
			if (children != null)
			{
				foreach (DecisionModel child in children.Values)
				{
					child.noMoreInstances(featureModel);
				}
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
			if (children != null)
			{
				foreach (DecisionModel child in children.Values)
				{
					child.terminate();
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(FeatureModel featureModel, GuideDecision decision)
		{
			if (decision is SingleDecision)
			{
				throw new GuideException("A branched decision model expect more than one decisions. ");
			}
			featureModel.update();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = ((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = ((MultipleDecision)decision).getSingleDecision(decisionIndex);
			if (instanceModel == null)
			{
				initInstanceModel(featureModel,singleDecision.TableContainer.TableContainerName);
			}

			instanceModel.addInstance(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.TableContainer.TableContainerName), singleDecision);
			if (decisionIndex + 1 < decision.numberOfDecisions())
			{
				if (singleDecision.continueWithNextDecision())
				{
					DecisionModel child = children[singleDecision.DecisionCode];
					if (child == null)
					{
						child = initChildDecisionModel(((MultipleDecision)decision).getSingleDecision(decisionIndex + 1), branchedDecisionSymbols + (branchedDecisionSymbols.Length == 0?"":"_") + singleDecision.DecisionSymbol);
						children[singleDecision.DecisionCode] = child;
					}
					child.addInstance(featureModel,decision);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureModel featureModel, GuideDecision decision)
		{
			featureModel.update();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = ((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = ((MultipleDecision)decision).getSingleDecision(decisionIndex);
			if (instanceModel == null)
			{
				initInstanceModel(featureModel, singleDecision.TableContainer.TableContainerName);
			}
			instanceModel.predict(featureModel.getFeatureVector(branchedDecisionSymbols, singleDecision.TableContainer.TableContainerName), singleDecision);
			if (decisionIndex + 1 < decision.numberOfDecisions())
			{
				if (singleDecision.continueWithNextDecision())
				{
					DecisionModel child = children[singleDecision.DecisionCode];
					if (child == null)
					{
						child = initChildDecisionModel(((MultipleDecision)decision).getSingleDecision(decisionIndex + 1), branchedDecisionSymbols + (branchedDecisionSymbols.Length == 0?"":"_") + singleDecision.DecisionSymbol);
						children[singleDecision.DecisionCode] = child;
					}
					child.predict(featureModel, decision);
				}
			}

			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector predictExtract(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector predictExtract(FeatureModel featureModel, GuideDecision decision)
		{
			if (decision is SingleDecision)
			{
				throw new GuideException("A branched decision model expect more than one decisions. ");
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
			if (decisionIndex + 1 < decision.numberOfDecisions())
			{
				if (singleDecision.continueWithNextDecision())
				{
					DecisionModel child = children[singleDecision.DecisionCode];
					if (child == null)
					{
						child = initChildDecisionModel(((MultipleDecision)decision).getSingleDecision(decisionIndex + 1), branchedDecisionSymbols + (branchedDecisionSymbols.Length == 0?"":"_") + singleDecision.DecisionSymbol);
						children[singleDecision.DecisionCode] = child;
					}
					child.predictExtract(featureModel, decision);
				}
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
				throw new GuideException("A branched decision model expect more than one decisions. ");
			}

			bool success = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = ((org.maltparser.parser.history.action.MultipleDecision)decision).getSingleDecision(decisionIndex);
			SingleDecision singleDecision = ((MultipleDecision)decision).getSingleDecision(decisionIndex);
			if (decisionIndex + 1 < decision.numberOfDecisions())
			{
				if (singleDecision.continueWithNextDecision())
				{
					DecisionModel child = children[singleDecision.DecisionCode];
					if (child != null)
					{
						success = child.predictFromKBestList(featureModel, decision);
					}

				}
			}
			if (!success)
			{
				success = singleDecision.updateFromKBestList();
				if (decisionIndex + 1 < decision.numberOfDecisions())
				{
					if (singleDecision.continueWithNextDecision())
					{
						DecisionModel child = children[singleDecision.DecisionCode];
						if (child == null)
						{
							child = initChildDecisionModel(((MultipleDecision)decision).getSingleDecision(decisionIndex + 1), branchedDecisionSymbols + (branchedDecisionSymbols.Length == 0?"":"_") + singleDecision.DecisionSymbol);
							children[singleDecision.DecisionCode] = child;
						}
						child.predict(featureModel, decision);
					}
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

		public virtual DecisionModel ParentDecisionModel
		{
			get
			{
				return parentDecisionModel;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private DecisionModel initChildDecisionModel(org.maltparser.parser.history.action.SingleDecision decision, String branchedDecisionSymbol) throws org.maltparser.core.exception.MaltChainedException
		private DecisionModel initChildDecisionModel(SingleDecision decision, string branchedDecisionSymbol)
		{
			if (decision.RelationToNextDecision == RelationToNextDecision.SEQUANTIAL)
			{
				return new SeqDecisionModel(guide, this, branchedDecisionSymbol);
			}
			else if (decision.RelationToNextDecision == RelationToNextDecision.BRANCHED)
			{
				return new BranchedDecisionModel(guide, this, branchedDecisionSymbol);
			}
			else if (decision.RelationToNextDecision == RelationToNextDecision.NONE)
			{
				return new OneDecisionModel(guide, this, branchedDecisionSymbol);
			}
			throw new GuideException("Could not find an appropriate decision model for the relation to the next decision");
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(modelName + ", ");
			foreach (DecisionModel model in children.Values)
			{
				sb.Append(model.ToString() + ", ");
			}
			return sb.ToString();
		}
	}

}
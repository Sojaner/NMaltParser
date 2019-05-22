using System.Text;

namespace org.maltparser.parser.guide
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureModel = org.maltparser.core.feature.FeatureModel;
	using FeatureVector = org.maltparser.core.feature.FeatureVector;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using BranchedDecisionModel = org.maltparser.parser.guide.decision.BranchedDecisionModel;
	using DecisionModel = org.maltparser.parser.guide.decision.DecisionModel;
	using OneDecisionModel = org.maltparser.parser.guide.decision.OneDecisionModel;
	using SeqDecisionModel = org.maltparser.parser.guide.decision.SeqDecisionModel;
	using GuideDecision = org.maltparser.parser.history.action.GuideDecision;
	using MultipleDecision = org.maltparser.parser.history.action.MultipleDecision;
	using SingleDecision = org.maltparser.parser.history.action.SingleDecision;
	using RelationToNextDecision = org.maltparser.parser.history.container.TableContainer.RelationToNextDecision;


	/// <summary>
	/// The guide is used by a parsing algorithm to predict the next parser action during parsing and to
	/// add a instance to the training instance set during learning.
	/// 
	/// @author Johan Hall
	/// </summary>
	public class SingleGuide : ClassifierGuide
	{
		private readonly DependencyParserConfig configuration;
		private readonly ClassifierGuide_GuideMode guideMode;
		private readonly FeatureModel featureModel2;
		private DecisionModel decisionModel = null;
		private string guideName;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SingleGuide(org.maltparser.parser.AlgoritmInterface algorithm, ClassifierGuide_GuideMode guideMode) throws org.maltparser.core.exception.MaltChainedException
		public SingleGuide(AlgoritmInterface algorithm, ClassifierGuide_GuideMode guideMode)
		{
			this.configuration = algorithm.Manager;
			this.guideMode = guideMode;

			string featureModelFileName = Configuration.getOptionValue("guide", "features").ToString().Trim();
	//		if (getConfiguration().isLoggerInfoEnabled()) {
	//			
	//			getConfiguration().logDebugMessage("  Feature model        : " + featureModelFileName+"\n");
	//			if (getGuideMode() == ClassifierGuide.GuideMode.BATCH) {
	//				getConfiguration().logDebugMessage("  Learner              : " + getConfiguration().getOptionValueString("guide", "learner").toString()+"\n");
	//			} else {
	//				getConfiguration().logDebugMessage("  Classifier           : " + getConfiguration().getOptionValueString("guide", "learner")+"\n");	
	//			}
	//		}
			string dataSplitColumn = Configuration.getOptionValue("guide", "data_split_column").ToString().Trim();
			string dataSplitStructure = Configuration.getOptionValue("guide", "data_split_structure").ToString().Trim();
			featureModel2 = Configuration.FeatureModelManager.getFeatureModel(findURL(featureModelFileName, Configuration), 0, algorithm.ParserRegistry, dataSplitColumn, dataSplitStructure);
	//		if (getGuideMode() == ClassifierGuide.GuideMode.BATCH) {
	//				getConfiguration().writeInfoToConfigFile("\nFEATURE MODEL\n");
	//				getConfiguration().writeInfoToConfigFile(featureModel.toString());
	//		}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.core.feature.FeatureModel featureModel,org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(FeatureModel featureModel, GuideDecision decision)
		{
			if (decisionModel == null)
			{
				if (decision is SingleDecision)
				{
					initDecisionModel((SingleDecision)decision);
				}
				else if (decision is MultipleDecision && decision.numberOfDecisions() > 0)
				{
					initDecisionModel(((MultipleDecision)decision).getSingleDecision(0));
				}
			}
			decisionModel.addInstance(featureModel,decision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void finalizeSentence(DependencyStructure dependencyGraph)
		{
			if (decisionModel != null)
			{
				decisionModel.finalizeSentence(dependencyGraph);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances() throws org.maltparser.core.exception.MaltChainedException
		public virtual void noMoreInstances()
		{
			if (decisionModel != null)
			{
				decisionModel.noMoreInstances(featureModel2);
			}
			else
			{
				configuration.logDebugMessage("The guide cannot create any models because there is no decision model. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			if (decisionModel != null)
			{
				decisionModel.terminate();
				decisionModel = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void predict(org.maltparser.core.feature.FeatureModel featureModel,org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void predict(FeatureModel featureModel, GuideDecision decision)
		{
			if (decisionModel == null)
			{
				if (decision is SingleDecision)
				{
					initDecisionModel((SingleDecision)decision);
				}
				else if (decision is MultipleDecision && decision.numberOfDecisions() > 0)
				{
					initDecisionModel(((MultipleDecision)decision).getSingleDecision(0));
				}
			}
			decisionModel.predict(featureModel,decision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector predictExtract(org.maltparser.core.feature.FeatureModel featureModel,org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector predictExtract(FeatureModel featureModel, GuideDecision decision)
		{
			if (decisionModel == null)
			{
				if (decision is SingleDecision)
				{
					initDecisionModel((SingleDecision)decision);
				}
				else if (decision is MultipleDecision && decision.numberOfDecisions() > 0)
				{
					initDecisionModel(((MultipleDecision)decision).getSingleDecision(0));
				}
			}
			return decisionModel.predictExtract(featureModel,decision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector extract(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector extract(FeatureModel featureModel)
		{
			return decisionModel.extract(featureModel);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predictFromKBestList(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predictFromKBestList(FeatureModel featureModel, GuideDecision decision)
		{
			if (decisionModel != null)
			{
				return decisionModel.predictFromKBestList(featureModel,decision);
			}
			else
			{
				throw new GuideException("The decision model cannot be found. ");
			}
		}

		public virtual DecisionModel DecisionModel
		{
			get
			{
				return decisionModel;
			}
		}

		public virtual DependencyParserConfig Configuration
		{
			get
			{
				return configuration;
			}
		}

		public virtual ClassifierGuide_GuideMode GuideMode
		{
			get
			{
				return guideMode;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initDecisionModel(org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initDecisionModel(SingleDecision decision)
		{
			if (decision.RelationToNextDecision == RelationToNextDecision.SEQUANTIAL)
			{
				decisionModel = new SeqDecisionModel(this);
			}
			else if (decision.RelationToNextDecision == RelationToNextDecision.BRANCHED)
			{
				decisionModel = new BranchedDecisionModel(this);
			}
			else if (decision.RelationToNextDecision == RelationToNextDecision.NONE)
			{
				decisionModel = new OneDecisionModel(this);
			}
		}

		public virtual string GuideName
		{
			get
			{
				return guideName;
			}
			set
			{
				this.guideName = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.net.URL findURL(String specModelFileName, org.maltparser.parser.DependencyParserConfig config) throws org.maltparser.core.exception.MaltChainedException
		public static URL findURL(string specModelFileName, DependencyParserConfig config)
		{
			URL url = null;
			File specFile = config.getFile(specModelFileName);
			if (specFile != null && specFile.exists())
			{
				try
				{
					url = new URL("file:///" + specFile.AbsolutePath);
				}
				catch (MalformedURLException e)
				{
					throw new MaltChainedException("Malformed URL: " + specFile, e);
				}
			}
			else
			{
				url = config.getConfigFileEntryURL(specModelFileName);
			}
			return url;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			return sb.ToString();
		}
	}

}
using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Feature;
using NMaltParser.Core.Feature.Value;
using NMaltParser.Core.Helper;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Core.LW.Parser
{
    /// <summary>
	/// A lightweight version of the decision models, the guide model and the instance models located in org.maltparser.parser.guide.{decision,instance} and
	/// can only be used in parsing mode. It is also limited to predict at most two decisions.
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class LWDecisionModel
	{
		private readonly string classifierName;
		private readonly HashMap<string, LWClassifier> classifiers;

		public LWDecisionModel(McoModel mcoModel, bool _excludeNullValues, string _classifierName)
		{
			classifierName = _classifierName;
			classifiers = new HashMap<string, LWClassifier>();
			ISet<string> mcoEntryObjectKeys = mcoModel.McoEntryObjectKeys;
			foreach (string key in mcoEntryObjectKeys)
			{
				if (key.EndsWith(".moo", StringComparison.Ordinal))
				{
					string prefixFileName = key.Substring(0,key.Length - 4);
					classifiers[prefixFileName] = new LWClassifier(mcoModel, prefixFileName, _excludeNullValues);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.ComplexDecisionAction decision, boolean one_prediction) throws org.maltparser.core.exception.MaltChainedException
		public bool predict(FeatureModel featureModel, ComplexDecisionAction decision, bool one_prediction)
		{
			if (decision.numberOfDecisions() > 2)
			{
				throw new MaltChainedException("Number of decisions is greater than two,  which is unsupported in the light-weight parser (lw.parser)");
			}
			featureModel.update();
			bool success = true;
			for (int i = 0; i < decision.numberOfDecisions(); i++)
			{
				LWClassifier classifier = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.parser.history.action.SingleDecision singleDecision = decision.getSingleDecision(i);
				SingleDecision singleDecision = decision.getSingleDecision(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder classifierString = new StringBuilder();
				StringBuilder classifierString = new StringBuilder();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder decisionModelString = new StringBuilder();
				StringBuilder decisionModelString = new StringBuilder();
				if (singleDecision.RelationToNextDecision == RelationToNextDecision.BRANCHED)
				{
					decisionModelString.Append("bdm");
				}
				else if (singleDecision.RelationToNextDecision == RelationToNextDecision.SEQUANTIAL)
				{
					decisionModelString.Append("sdm");
				}
				else
				{
					decisionModelString.Append("odm");
				}
				decisionModelString.Append(i);
				string decisionSymbol = "";
				if (i == 1 && singleDecision.RelationToNextDecision == RelationToNextDecision.BRANCHED)
				{
					decisionSymbol = singleDecision.DecisionSymbol;
					decisionModelString.Append(decisionSymbol);
				}
				decisionModelString.Append('.');
				FeatureVector featureVector = featureModel.getFeatureVector(decisionSymbol, singleDecision.TableContainer.TableContainerName);

				if (featureModel.hasDivideFeatureFunction())
				{
					SingleFeatureValue featureValue = (SingleFeatureValue)featureModel.DivideFeatureFunction.FeatureValue;
					classifierString.Append(decisionModelString);
					classifierString.Append(string.Format("{0:D3}", featureValue.IndexCode));
					classifierString.Append('.');
					classifierString.Append(classifierName);
					classifier = classifiers[classifierString.ToString()];
					if (classifier != null)
					{
						FeatureVector dividefeatureVector = featureModel.getFeatureVector("/" + featureVector.SpecSubModel.SubModelName);
						success = classifier.predict(dividefeatureVector, singleDecision, one_prediction) && success;
						continue;
					}
					classifierString.Length = 0;
				}

				classifierString.Append(decisionModelString);
				classifierString.Append(classifierName);
				classifier = classifiers[classifierString.ToString()];
				if (classifier != null)
				{
					success = classifier.predict(featureVector, singleDecision, one_prediction) && success;
				}
				else
				{
					singleDecision.addDecision(1);
				}
				if (!singleDecision.continueWithNextDecision())
				{
					break;
				}
			}
			return success;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predictFromKBestList(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.ComplexDecisionAction decision) throws org.maltparser.core.exception.MaltChainedException
		public bool predictFromKBestList(FeatureModel featureModel, ComplexDecisionAction decision)
		{
			predict(featureModel, decision, false);
			if (decision.numberOfDecisions() == 1)
			{
				return decision.getSingleDecision(0).updateFromKBestList();
			}
			else if (decision.numberOfDecisions() > 2)
			{
				throw new MaltChainedException("Number of decisions is greater than two,  which is unsupported in the light-weight parser (lw.parser)");
			}
			bool success = false;
			if (decision.getSingleDecision(0).continueWithNextDecision())
			{
				success = decision.getSingleDecision(1).updateFromKBestList();
			}
			return success;
		}
	}

}
using System.Collections.Generic;
using System.IO;

namespace org.maltparser.ml
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureVector = org.maltparser.core.feature.FeatureVector;
	using FeatureFunction = org.maltparser.core.feature.function.FeatureFunction;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using SingleDecision = org.maltparser.parser.history.action.SingleDecision;


	public interface LearningMethod
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.parser.history.action.SingleDecision decision, org.maltparser.core.feature.FeatureVector featureVector) throws org.maltparser.core.exception.MaltChainedException;
		void addInstance(SingleDecision decision, FeatureVector featureVector);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException;
		void finalizeSentence(DependencyStructure dependencyGraph);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances() throws org.maltparser.core.exception.MaltChainedException;
		void noMoreInstances();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException;
		void train();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void moveAllInstances(LearningMethod method, org.maltparser.core.feature.function.FeatureFunction divideFeature, java.util.ArrayList<int> divideFeatureIndexVector) throws org.maltparser.core.exception.MaltChainedException;
		void moveAllInstances(LearningMethod method, FeatureFunction divideFeature, List<int> divideFeatureIndexVector);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException;
		void terminate();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureVector features, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException;
		bool predict(FeatureVector features, SingleDecision decision);
		StreamWriter InstanceWriter {get;}
		void increaseNumberOfInstances();
		void decreaseNumberOfInstances();
	}

	public static class LearningMethod_Fields
	{
		public const int BATCH = 0;
		public const int CLASSIFY = 1;
	}

}
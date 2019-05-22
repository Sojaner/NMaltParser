﻿using System;
using System.Collections.Generic;

namespace org.maltparser.concurrent
{

	using ConcurrentDependencyGraph = org.maltparser.concurrent.graph.ConcurrentDependencyGraph;
	using DataFormat = org.maltparser.concurrent.graph.dataformat.DataFormat;
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureModelManager = org.maltparser.core.feature.FeatureModelManager;
	using FeatureEngine = org.maltparser.core.feature.system.FeatureEngine;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using DataFormatManager = org.maltparser.core.io.dataformat.DataFormatManager;
	using LWDeprojectivizer = org.maltparser.core.lw.graph.LWDeprojectivizer;
	using LWDependencyGraph = org.maltparser.core.lw.graph.LWDependencyGraph;
	using LWSingleMalt = org.maltparser.core.lw.parser.LWSingleMalt;
	using McoModel = org.maltparser.core.lw.parser.McoModel;
	using OptionManager = org.maltparser.core.options.OptionManager;
	using PluginLoader = org.maltparser.core.plugin.PluginLoader;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using HashSymbolTableHandler = org.maltparser.core.symbol.hash.HashSymbolTableHandler;
	using ParseSymbolTableHandler = org.maltparser.core.symbol.parse.ParseSymbolTableHandler;

	/// <summary>
	/// A concurrent MaltParser model that can be used to parse sentences in both a single threaded or multi threaded 
	/// environment. To create an object of ConcurrentMaltParserModel use the static methods in ConcurrentMaltParserService.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class ConcurrentMaltParserModel
	{
		private readonly DataFormatInstance dataFormatInstance;
		private readonly DataFormat concurrentDataFormat;
		private readonly SymbolTableHandler parentSymbolTableHandler;
		private readonly LWSingleMalt singleMalt;
		private readonly int optionContainer;
		private readonly McoModel mcoModel;
		private readonly int markingStrategy;
		private readonly bool coveredRoot;
		private readonly string defaultRootLabel;

		/// <summary>
		/// This constructor can only be used by ConcurrentMaltParserService
		/// </summary>
		/// <param name="_optionContainer"> a option container index </param>
		/// <param name="_mcoURL"> a URL to a valid MaltParser model file. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ConcurrentMaltParserModel(int _optionContainer, java.net.URL _mcoURL) throws org.maltparser.core.exception.MaltChainedException
		protected internal ConcurrentMaltParserModel(int _optionContainer, URL _mcoURL)
		{
			this.optionContainer = _optionContainer;
			this.mcoModel = new McoModel(_mcoURL);
			string inputFormatName = OptionManager.instance().getOptionValue(optionContainer, "input", "format").ToString().Trim();
			URL inputFormatURL = null;
			try
			{
				inputFormatURL = mcoModel.getMcoEntryURL(inputFormatName);
			}
			catch (IOException e)
			{
				throw new MaltChainedException("Couldn't read file " + inputFormatName + " from mco-file ", e);
			}
			DataFormatManager dataFormatManager = new DataFormatManager(inputFormatURL, inputFormatURL);
			this.parentSymbolTableHandler = new HashSymbolTableHandler();
			this.dataFormatInstance = dataFormatManager.InputDataFormatSpec.createDataFormatInstance(this.parentSymbolTableHandler, OptionManager.instance().getOptionValueString(optionContainer, "singlemalt", "null_value"));
			try
			{
				this.parentSymbolTableHandler.load(mcoModel.getInputStreamReader("symboltables.sym", "UTF-8"));
			}
			catch (IOException e)
			{
				throw new MaltChainedException("Couldn't read file symboltables.sym from mco-file ", e);
			}
			this.defaultRootLabel = OptionManager.instance().getOptionValue(optionContainer, "graph", "root_label").ToString().Trim();
			this.markingStrategy = LWDeprojectivizer.getMarkingStrategyInt(OptionManager.instance().getOptionValue(optionContainer, "pproj", "marking_strategy").ToString().Trim());
			this.coveredRoot = !OptionManager.instance().getOptionValue(optionContainer, "pproj", "covered_root").ToString().Trim().Equals("none", StringComparison.OrdinalIgnoreCase);
	//		final PropagationManager propagationManager = loadPropagationManager(this.optionContainer, mcoModel);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.FeatureModelManager featureModelManager = loadFeatureModelManager(this.optionContainer, mcoModel);
			FeatureModelManager featureModelManager = loadFeatureModelManager(this.optionContainer, mcoModel);
			this.singleMalt = new LWSingleMalt(this.optionContainer, this.dataFormatInstance, mcoModel, null, featureModelManager);
			this.concurrentDataFormat = DataFormat.parseDataFormatXMLfile(inputFormatURL);
		}

		/// <summary>
		/// Parses an array of tokens and returns a dependency graph.
		/// </summary>
		/// <param name="tokens"> an array of tokens </param>
		/// <returns> a dependency graph </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.concurrent.graph.ConcurrentDependencyGraph parse(String[] tokens) throws org.maltparser.core.exception.MaltChainedException
		public ConcurrentDependencyGraph parse(string[] tokens)
		{
			return new ConcurrentDependencyGraph(concurrentDataFormat, internalParse(tokens), defaultRootLabel);
		}

		/// <summary>
		/// Same as parse(String[] tokens), but instead it returns an array of tokens with a head index and a dependency type at the end of string
		/// </summary>
		/// <param name="tokens"> an array of tokens to parse </param>
		/// <returns> an array of tokens with a head index and a dependency type at the end of string </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String[] parseTokens(String[] tokens) throws org.maltparser.core.exception.MaltChainedException
		public string[] parseTokens(string[] tokens)
		{
			LWDependencyGraph outputGraph = internalParse(tokens);
			string[] outputTokens = new string[tokens.Length];
			for (int i = 0; i < outputTokens.Length; i++)
			{
				outputTokens[i] = outputGraph.getDependencyNode(i + 1).ToString();
			}
			return outputTokens;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.lw.graph.LWDependencyGraph internalParse(String[] tokens) throws org.maltparser.core.exception.MaltChainedException
		private LWDependencyGraph internalParse(string[] tokens)
		{
			if (tokens == null || tokens.Length == 0)
			{
				throw new MaltChainedException("Nothing to parse. ");
			}

			LWDependencyGraph parseGraph = new LWDependencyGraph(concurrentDataFormat, new ParseSymbolTableHandler(parentSymbolTableHandler), tokens, defaultRootLabel, false);

			singleMalt.parse(parseGraph);
			if (markingStrategy != 0 || coveredRoot)
			{
				(new LWDeprojectivizer()).deprojectivize(parseGraph, markingStrategy);
			}

			return parseGraph;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.List<String[]> parseSentences(java.util.List<String[]> inputSentences) throws org.maltparser.core.exception.MaltChainedException
		public IList<string[]> parseSentences(IList<string[]> inputSentences)
		{
			return singleMalt.parseSentences(inputSentences, defaultRootLabel, markingStrategy, coveredRoot, parentSymbolTableHandler, concurrentDataFormat);
	//    	List<String[]> outputSentences = Collections.synchronizedList(new ArrayList<String[]>());
	//		for (int i = 0; i < inputSentences.size(); i++) {
	//			String[] tokens = inputSentences.get(i);
	//			// TODO nothing to parse
	//			LWDependencyGraph parseGraph = new LWDependencyGraph(concurrentDataFormat, new ParseSymbolTableHandler(parentSymbolTableHandler), tokens, defaultRootLabel, false);
	//			singleMalt.parse(parseGraph);
	//			if (markingStrategy != 0 || coveredRoot) { 
	//				new LWDeprojectivizer().deprojectivize(parseGraph, markingStrategy);
	//			}
	//			String[] outputTokens = new String[tokens.length];
	//			for (int j = 0; j < outputTokens.length; j++) {
	//				outputTokens[i] = parseGraph.getDependencyNode(j+1).toString();
	//			}
	//			outputSentences.add(outputTokens);
	//		}
	//		return outputSentences;
		}


	//	private PropagationManager loadPropagationManager(int optionContainer, McoModel mcoModel) throws MaltChainedException {
	//		String propagationSpecFileName = OptionManager.instance().getOptionValue(optionContainer, "singlemalt", "propagation").toString();
	//		PropagationManager propagationManager = null;
	//		if (propagationSpecFileName != null && propagationSpecFileName.length() > 0) {
	//			propagationManager = new PropagationManager();
	//			try {
	//				propagationManager.loadSpecification(mcoModel.getMcoEntryURL(propagationSpecFileName));
	//			} catch(IOException e) {
	//				throw new MaltChainedException("Couldn't read file "+propagationSpecFileName+" from mco-file ", e);
	//			}
	//		}
	//		return propagationManager;
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.feature.FeatureModelManager loadFeatureModelManager(int optionContainer, org.maltparser.core.lw.parser.McoModel mcoModel) throws org.maltparser.core.exception.MaltChainedException
		private FeatureModelManager loadFeatureModelManager(int optionContainer, McoModel mcoModel)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.system.FeatureEngine system = new org.maltparser.core.feature.system.FeatureEngine();
			FeatureEngine system = new FeatureEngine();
			system.load("/appdata/features/ParserFeatureSystem.xml");
			system.load(PluginLoader.instance());
			FeatureModelManager featureModelManager = new FeatureModelManager(system);
			string featureModelFileName = OptionManager.instance().getOptionValue(optionContainer, "guide", "features").ToString().Trim();
			try
			{
				if (featureModelFileName.EndsWith(".par", StringComparison.Ordinal))
				{
					string markingStrategy = OptionManager.instance().getOptionValue(optionContainer, "pproj", "marking_strategy").ToString().Trim();
					string coveredRoot = OptionManager.instance().getOptionValue(optionContainer, "pproj", "covered_root").ToString().Trim();
					featureModelManager.loadParSpecification(mcoModel.getMcoEntryURL(featureModelFileName), markingStrategy, coveredRoot);
				}
				else
				{
					featureModelManager.loadSpecification(mcoModel.getMcoEntryURL(featureModelFileName));
				}
			}
			catch (IOException e)
			{
				throw new MaltChainedException("Couldn't read file " + featureModelFileName + " from mco-file ", e);
			}
			return featureModelManager;
		}
	}

}
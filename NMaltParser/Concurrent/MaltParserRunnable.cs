using System;
using System.Collections.Generic;
using System.Threading;

namespace org.maltparser.concurrent
{

	using  core.exception;

	public class MaltParserRunnable : ThreadStart
	{
		private readonly IList<string[]> inputSentences;
		private IList<string[]> outputSentences;
		private readonly ConcurrentMaltParserModel model;

		public MaltParserRunnable(IList<string[]> sentences, ConcurrentMaltParserModel _model)
		{
			inputSentences = new List<string[]>(sentences);
			outputSentences = null;
			model = _model;
		}

		public virtual void run()
		{
			try
			{
				outputSentences = model.parseSentences(inputSentences);
			}
			catch (MaltChainedException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
	//		for (int i = 0; i < inputSentences.size(); i++) {
	//			try {
	//				outputSentences.add(model.parseTokens(inputSentences.get(i)));
	//			} catch (MaltChainedException e) {
	//				e.printStackTrace();
	//			}
	//		}
		}

		public virtual IList<string[]> OutputSentences
		{
			get
			{
				if (outputSentences == null)
				{
					return Collections.synchronizedList(new List<string[]>());
				}
				return Collections.synchronizedList(new List<string[]>(outputSentences));
			}
		}
	}

}
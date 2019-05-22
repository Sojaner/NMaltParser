﻿using System;
using System.Text;
using System.IO;

namespace org.maltparser.ml.lib
{

	using Util = org.maltparser.core.helper.Util;

	using SolverType = de.bwaldvogel.liblinear.SolverType;

	/// <summary>
	/// <para>This class borrows code from liblinear.Model.java of the Java implementation of the liblinear package.
	/// MaltLiblinearModel stores the model obtained from the training procedure. In addition to the original code the model is more integrated to
	/// MaltParser. Instead of moving features from MaltParser's internal data structures to liblinear's data structure it uses MaltParser's data 
	/// structure directly on the model. </para> 
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	[Serializable]
	public class MaltLiblinearModel : MaltLibModel
	{
		private const long serialVersionUID = 7526471155622776147L;
		private static readonly Charset FILE_CHARSET = Charset.forName("ISO-8859-1");
		private double bias;
		/// <summary>
		/// label of each class </summary>
		private int[] labels;
		private int nr_class;
		private int nr_feature;
		private SolverType solverType;
		/// <summary>
		/// feature weight array </summary>
		private double[][] w;

		public MaltLiblinearModel(int[] labels, int nr_class, int nr_feature, double[][] w, SolverType solverType)
		{
			this.labels = labels;
			this.nr_class = nr_class;
			this.nr_feature = nr_feature;
			this.w = w;
			this.solverType = solverType;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MaltLiblinearModel(java.io.Reader inputReader) throws java.io.IOException
		public MaltLiblinearModel(Reader inputReader)
		{
			loadModel(inputReader);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MaltLiblinearModel(java.io.File modelFile) throws java.io.IOException
		public MaltLiblinearModel(File modelFile)
		{
			StreamReader inputReader = new StreamReader(new FileStream(modelFile, FileMode.Open, FileAccess.Read), FILE_CHARSET);
			loadModel(inputReader);
		}

		/// <returns> number of classes </returns>
		public virtual int NrClass
		{
			get
			{
				return nr_class;
			}
		}

		/// <returns> number of features </returns>
		public virtual int NrFeature
		{
			get
			{
				return nr_feature;
			}
		}

		public virtual int[] Labels
		{
			get
			{
				return Util.copyOf(labels, nr_class);
			}
		}

		/// <summary>
		/// The nr_feature*nr_class array w gives feature weights. We use one
		/// against the rest for multi-class classification, so each feature
		/// index corresponds to nr_class weight values. Weights are
		/// organized in the following way
		/// 
		/// <pre>
		/// +------------------+------------------+------------+
		/// | nr_class weights | nr_class weights | ...
		/// | for 1st feature | for 2nd feature |
		/// +------------------+------------------+------------+
		/// </pre>
		/// 
		/// If bias &gt;= 0, x becomes [x; bias]. The number of features is
		/// increased by one, so w is a (nr_feature+1)*nr_class array. The
		/// value of bias is stored in the variable bias. </summary>
		/// <seealso cref= #getBias() </seealso>
		/// <returns> a <b>copy of</b> the feature weight array as described </returns>
	//    public double[] getFeatureWeights() {
	//        return Util.copyOf(w, w.length);
	//    }

		/// <returns> true for logistic regression solvers </returns>
		public virtual bool ProbabilityModel
		{
			get
			{
				return (solverType == SolverType.L2R_LR || solverType == SolverType.L2R_LR_DUAL || solverType == SolverType.L1R_LR);
			}
		}

		public virtual double Bias
		{
			get
			{
				return bias;
			}
		}

		public virtual int[] predict(MaltFeatureNode[] x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dec_values = new double[nr_class];
			double[] dec_values = new double[nr_class];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = (bias >= 0)?nr_feature + 1:nr_feature;
			int n = (bias >= 0)?nr_feature + 1:nr_feature;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xlen = x.length;
			int xlen = x.Length;

			for (int i = 0; i < xlen; i++)
			{
				if (x[i].index <= n)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int t = (x[i].index - 1);
					int t = (x[i].index - 1);
					if (w[t] != null)
					{
						for (int j = 0; j < w[t].Length; j++)
						{
							dec_values[j] += w[t][j] * x[i].value;
						}
					}
				}
			}


			double tmpDec;
			int tmpObj;
			int iMax;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] predictionList = new int[nr_class];
			int[] predictionList = new int[nr_class];
			Array.Copy(labels, 0, predictionList, 0, nr_class);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nc = nr_class-1;
			int nc = nr_class - 1;
			for (int i = 0; i < nc; i++)
			{
				iMax = i;
				for (int j = i + 1; j < nr_class; j++)
				{
					if (dec_values[j] > dec_values[iMax])
					{
						iMax = j;
					}
				}
				if (iMax != i)
				{
					tmpDec = dec_values[iMax];
					dec_values[iMax] = dec_values[i];
					dec_values[i] = tmpDec;
					tmpObj = predictionList[iMax];
					predictionList[iMax] = predictionList[i];
					predictionList[i] = tmpObj;
				}
			}
			return predictionList;
		}

		public virtual int predict_one(MaltFeatureNode[] x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dec_values = new double[nr_class];
			double[] dec_values = new double[nr_class];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = (bias >= 0)?nr_feature + 1:nr_feature;
			int n = (bias >= 0)?nr_feature + 1:nr_feature;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xlen = x.length;
			int xlen = x.Length;

			for (int i = 0; i < xlen; i++)
			{
				if (x[i].index <= n)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int t = (x[i].index - 1);
					int t = (x[i].index - 1);
					if (w[t] != null)
					{
						for (int j = 0; j < w[t].Length; j++)
						{
							dec_values[j] += w[t][j] * x[i].value;
						}
					}
				}
			}

			double max = dec_values[0];
			int max_index = 0;
			for (int i = 1; i < dec_values.Length; i++)
			{
				if (dec_values[i] > max)
				{
					max = dec_values[i];
					max_index = i;
				}
			}

			return labels[max_index];
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream is) throws ClassNotFoundException, java.io.IOException
		private void readObject(ObjectInputStream @is)
		{
			@is.defaultReadObject();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream os) throws java.io.IOException
		private void writeObject(ObjectOutputStream os)
		{
			os.defaultWriteObject();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void loadModel(java.io.Reader inputReader) throws java.io.IOException
		private void loadModel(Reader inputReader)
		{
			labels = null;
			Pattern whitespace = Pattern.compile("\\s+");
			StreamReader reader = null;
			if (inputReader is StreamReader)
			{
				reader = (StreamReader)inputReader;
			}
			else
			{
				reader = new StreamReader(inputReader);
			}

			try
			{
				string line = null;
				while (!string.ReferenceEquals((line = reader.ReadLine()), null))
				{
					string[] split = whitespace.split(line);
					if (split[0].Equals("solver_type"))
					{
						SolverType solver = SolverType.valueOf(split[1]);
						if (solver == null)
						{
							throw new Exception("unknown solver type");
						}
						solverType = solver;
					}
					else if (split[0].Equals("nr_class"))
					{
						nr_class = Util.atoi(split[1]);
						int.Parse(split[1]);
					}
					else if (split[0].Equals("nr_feature"))
					{
						nr_feature = Util.atoi(split[1]);
					}
					else if (split[0].Equals("bias"))
					{
						bias = Util.atof(split[1]);
					}
					else if (split[0].Equals("w"))
					{
						break;
					}
					else if (split[0].Equals("label"))
					{
						labels = new int[nr_class];
						for (int i = 0; i < nr_class; i++)
						{
							labels[i] = Util.atoi(split[i + 1]);
						}
					}
					else
					{
						throw new Exception("unknown text in model file: [" + line + "]");
					}
				}

				int w_size = nr_feature;
				if (bias >= 0)
				{
					w_size++;
				}

				int nr_w = nr_class;
				if (nr_class == 2 && solverType != SolverType.MCSVM_CS)
				{
					nr_w = 1;
				}
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: w = new double[w_size][nr_w];
				w = RectangularArrays.RectangularDoubleArray(w_size, nr_w);
				int[] buffer = new int[128];

				for (int i = 0; i < w_size; i++)
				{
					for (int j = 0; j < nr_w; j++)
					{
						int b = 0;
						while (true)
						{
							int ch = reader.Read();
							if (ch == -1)
							{
								throw new EOFException("unexpected EOF");
							}
							if (ch == ' ')
							{
								w[i][j] = Util.atof(new string(buffer, 0, b));
								break;
							}
							else
							{
								buffer[b++] = ch;
							}
						}
					}
				}
			}
			finally
			{
				Util.closeQuietly(reader);
			}
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			long temp = System.BitConverter.DoubleToInt64Bits(bias);
			int result = prime * 1 + (int)(temp ^ ((long)((ulong)temp >> 32)));
			result = prime * result + Arrays.GetHashCode(labels);
			result = prime * result + nr_class;
			result = prime * result + nr_feature;
			result = prime * result + ((solverType == null) ? 0 : solverType.GetHashCode());
			for (int i = 0; i < w.Length; i++)
			{
				result = prime * result + Arrays.GetHashCode(w[i]);
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			MaltLiblinearModel other = (MaltLiblinearModel)obj;
			if (System.BitConverter.DoubleToInt64Bits(bias) != Double.doubleToLongBits(other.bias))
			{
				return false;
			}
			if (!Arrays.Equals(labels, other.labels))
			{
				return false;
			}
			if (nr_class != other.nr_class)
			{
				return false;
			}
			if (nr_feature != other.nr_feature)
			{
				return false;
			}
			if (solverType == null)
			{
				if (other.solverType != null)
				{
					return false;
				}
			}
			else if (!solverType.Equals(other.solverType))
			{
				return false;
			}
			for (int i = 0; i < w.Length; i++)
			{
				if (other.w.Length <= i)
				{
					return false;
				}
				if (!Util.Equals(w[i], other.w[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder("Model");
			StringBuilder sb = new StringBuilder("Model");
			sb.Append(" bias=").Append(bias);
			sb.Append(" nr_class=").Append(nr_class);
			sb.Append(" nr_feature=").Append(nr_feature);
			sb.Append(" solverType=").Append(solverType);
			return sb.ToString();
		}
	}

}
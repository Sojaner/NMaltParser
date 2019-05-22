﻿using System;

namespace NMaltParser.ML.Lib
{
    /// <summary>
	/// <para>This class borrows code from libsvm.svm.java of the Java implementation of the libsvm package.
	/// MaltLibsvmModel stores the model obtained from the training procedure. In addition to the original code the model is more integrated to
	/// MaltParser. Instead of moving features from MaltParser's internal data structures to liblinear's data structure it uses MaltParser's data 
	/// structure directly on the model. </para> 
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	[Serializable]
	public class MaltLibsvmModel : MaltLibModel
	{
		private const long serialVersionUID = 7526471155622776147L;
		public svm_parameter param; // parameter
		public int nr_class; // number of classes, = 2 in regression/one class svm
		public int l; // total #SV
		public svm_node[][] SV; // SVs (SV[l])
		public double[][] sv_coef; // coefficients for SVs in decision functions (sv_coef[k-1][l])
		public double[] rho; // constants in decision functions (rho[k*(k-1)/2])

		// for classification only
		public int[] label; // label of each class (label[k])
		public int[] nSV; // number of SVs for each class (nSV[k])
									// nSV[0] + nSV[1] + ... + nSV[k-1] = l   
		public int[] start;

		public MaltLibsvmModel(svm_model model, svm_problem problem)
		{
			param = model.param;
			nr_class = model.nr_class;
			l = model.l;
			SV = model.SV;
			sv_coef = model.sv_coef;
			rho = model.rho;
			label = model.label;
			nSV = model.nSV;
			start = new int[nr_class];
			start[0] = 0;
			for (int i = 1;i < nr_class;i++)
			{
				start[i] = start[i - 1] + nSV[i - 1];
			}
		}

		public virtual int[] predict(MaltFeatureNode[] x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dec_values = new double[nr_class*(nr_class-1)/2];
			double[] dec_values = new double[nr_class * (nr_class - 1) / 2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] kvalue = new double[l];
			double[] kvalue = new double[l];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] vote = new int[nr_class];
			int[] vote = new int[nr_class];
			int i;
			for (i = 0;i < l;i++)
			{
				kvalue[i] = k_function(x,SV[i],param);
			}
			for (i = 0;i < nr_class;i++)
			{
				vote[i] = 0;
			}

			int p = 0;
			for (i = 0;i < nr_class;i++)
			{
				for (int j = i + 1;j < nr_class;j++)
				{
					double sum = 0;
					int si = start[i];
					int sj = start[j];
					int ci = nSV[i];
					int cj = nSV[j];

					int k;
					double[] coef1 = sv_coef[j - 1];
					double[] coef2 = sv_coef[i];
					for (k = 0;k < ci;k++)
					{
						sum += coef1[si + k] * kvalue[si + k];
					}
					for (k = 0;k < cj;k++)
					{
						sum += coef2[sj + k] * kvalue[sj + k];
					}
					sum -= rho[p];
					dec_values[p] = sum;

					if (dec_values[p] > 0)
					{
						++vote[i];
					}
					else
					{
						++vote[j];
					}
					p++;
				}
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] predictionList = new int[nr_class];
			int[] predictionList = new int[nr_class];
			Array.Copy(label, 0, predictionList, 0, nr_class);
			int tmp;
			int iMax;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nc = nr_class-1;
			int nc = nr_class - 1;
			for (i = 0; i < nc; i++)
			{
				iMax = i;
				for (int j = i + 1; j < nr_class; j++)
				{
					if (vote[j] > vote[iMax])
					{
						iMax = j;
					}
				}
				if (iMax != i)
				{
					tmp = vote[iMax];
					vote[iMax] = vote[i];
					vote[i] = tmp;
					tmp = predictionList[iMax];
					predictionList[iMax] = predictionList[i];
					predictionList[i] = tmp;
				}
			}
			return predictionList;
		}


		public virtual int predict_one(MaltFeatureNode[] x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dec_values = new double[nr_class*(nr_class-1)/2];
			double[] dec_values = new double[nr_class * (nr_class - 1) / 2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] kvalue = new double[l];
			double[] kvalue = new double[l];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] vote = new int[nr_class];
			int[] vote = new int[nr_class];
			int i;
			for (i = 0;i < l;i++)
			{
				kvalue[i] = k_function(x,SV[i],param);
			}
			for (i = 0;i < nr_class;i++)
			{
				vote[i] = 0;
			}

			int p = 0;
			for (i = 0;i < nr_class;i++)
			{
				for (int j = i + 1;j < nr_class;j++)
				{
					double sum = 0;
					int si = start[i];
					int sj = start[j];
					int ci = nSV[i];
					int cj = nSV[j];

					int k;
					double[] coef1 = sv_coef[j - 1];
					double[] coef2 = sv_coef[i];
					for (k = 0;k < ci;k++)
					{
						sum += coef1[si + k] * kvalue[si + k];
					}
					for (k = 0;k < cj;k++)
					{
						sum += coef2[sj + k] * kvalue[sj + k];
					}
					sum -= rho[p];
					dec_values[p] = sum;

					if (dec_values[p] > 0)
					{
						++vote[i];
					}
					else
					{
						++vote[j];
					}
					p++;
				}
			}


			int max = vote[0];
			int max_index = 0;
			for (i = 1; i < vote.Length; i++)
			{
				if (vote[i] > max)
				{
					max = vote[i];
					max_index = i;
				}
			}

			return label[max_index];
		}

		internal static double dot(MaltFeatureNode[] x, svm_node[] y)
		{
			double sum = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xlen = x.length;
			int xlen = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ylen = y.length;
			int ylen = y.Length;
			int i = 0;
			int j = 0;
			while (i < xlen && j < ylen)
			{
				if (x[i].index == y[j].index)
				{
					sum += x[i++].value * y[j++].value;
				}
				else
				{
					if (x[i].index > y[j].index)
					{
						++j;
					}
					else
					{
						++i;
					}
				}
			}
			return sum;
		}

		internal static double powi(double @base, int times)
		{
			double tmp = @base, ret = 1.0;

			for (int t = times; t > 0; t /= 2)
			{
				if (t % 2 == 1)
				{
					ret *= tmp;
				}
				tmp = tmp * tmp;
			}
			return ret;
		}

		internal static double k_function(MaltFeatureNode[] x, svm_node[] y, svm_parameter param)
		{
			switch (param.kernel_type)
			{
				case svm_parameter.LINEAR:
					return dot(x,y);
				case svm_parameter.POLY:
					return powi(param.gamma * dot(x,y) + param.coef0,param.degree);
				case svm_parameter.RBF:
				{
					double sum = 0;
					int xlen = x.Length;
					int ylen = y.Length;
					int i = 0;
					int j = 0;
					while (i < xlen && j < ylen)
					{
						if (x[i].index == y[j].index)
						{
							double d = x[i++].value - y[j++].value;
							sum += d * d;
						}
						else if (x[i].index > y[j].index)
						{
							sum += y[j].value * y[j].value;
							++j;
						}
						else
						{
							sum += x[i].value * x[i].value;
							++i;
						}
					}

					while (i < xlen)
					{
						sum += x[i].value * x[i].value;
						++i;
					}

					while (j < ylen)
					{
						sum += y[j].value * y[j].value;
						++j;
					}

					return Math.Exp(-param.gamma * sum);
				}
				case svm_parameter.SIGMOID:
					return Math.Tanh(param.gamma * dot(x,y) + param.coef0);
				case svm_parameter.PRECOMPUTED:
					return x[(int)(y[0].value)].value;
				default:
					return 0; // java
			}
		}

		public virtual int[] Labels
		{
			get
			{
				if (label != null)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int[] labels = new int[nr_class];
					int[] labels = new int[nr_class];
					for (int i = 0;i < nr_class;i++)
					{
						labels[i] = label[i];
					}
					return labels;
				}
				return null;
			}
		}
	}

}
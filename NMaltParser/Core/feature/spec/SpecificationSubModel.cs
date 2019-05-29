using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NMaltParser.Utilities;

namespace NMaltParser.Core.Feature.Spec
{
    /// <inheritdoc />
    public class SpecificationSubModel : IEnumerable<string>
    {
        private readonly Regex blanks = new Regex("\\s+");

        private readonly ISet<string> featureSpecSet;

        private readonly string name;

        public SpecificationSubModel() : this("MAIN")
        {
        }

        public SpecificationSubModel(string name)
        {
            this.name = name;

            featureSpecSet = new SortedSet<string>();
        }

        public virtual void Add(string featureSpec)
        {
            if (featureSpec != null && featureSpec.Trim().Length > 0)
            {
                string strippedFeatureSpec = blanks.Replace(featureSpec, "");

                featureSpecSet.Add(strippedFeatureSpec);
            }
        }

        public virtual string SubModelName => name;

        public virtual int Size()
        {
            return featureSpecSet.Count;
        }

        public virtual IEnumerator<string> GetEnumerator()
        {
            return featureSpecSet.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (string str in featureSpecSet)
            {
                stringBuilder.Append(str);

                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }

        public override int GetHashCode()
        {
            const int prime = 31;

            int result = 1;

            result = prime * result + ((featureSpecSet == null) ? 0 : featureSpecSet.HashCode());

            result = prime * result + (name == null ? 0 : name.HashCode());

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
            if (GetType() != obj.GetType())
            {
                return false;
            }

            SpecificationSubModel other = (SpecificationSubModel)obj;

            if (featureSpecSet == null)
            {
                if (other.featureSpecSet != null)
                {
                    return false;
                }
            }
            else if (!featureSpecSet.SetEquals(other.featureSpecSet))
            {
                return false;
            }

            if (ReferenceEquals(name, null))
            {
                if (!ReferenceEquals(other.name, null))
                {
                    return false;
                }
            }
            else if (!name.Equals(other.name))
            {
                return false;
            }

            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main;

namespace NBC
{
    public class MyPair<TKey, TValue> : IComparable
            where TKey : IComparable
            where TValue : IComparable
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public MyPair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as MyPair<TKey, TValue>);
        }
        public int CompareTo(MyPair<TKey, TValue> other)
        {
            int kc = Key.CompareTo(other.Key);
            int vc = Value.CompareTo(other.Value);

            if (kc == vc)
                return kc;
            if (kc == 1)
                return kc;
            if (kc == 0)
                return vc;
            if (kc == -1)
                return kc;

            return Key.CompareTo(other.Key);
        }
    }
    public class MyAttribute : IItemAttribute
    {
        public string Name { get; set; }
        public IComparable Value { get; set; }
        public AttributeType AttributeType { get; } = AttributeType.Discrete;

        public MyAttribute(string name, IComparable value)
        {
            this.Name = name;
            this.Value = value;
        }

        public MyAttribute(string name, IComparable value, AttributeType attributeType)
        {
            this.Name = name;
            this.Value = value;
            this.AttributeType = attributeType;
        }
    }
    public partial class NBCTree : ITree
    {
        public string Name => "NBC";

        private string _classificationAttributeName;
        public string ClassificationAttributeName
        {
            get
            {
                return _classificationAttributeName;
            }
            set
            {
                _classificationAttributeName = value?.ToLower();
            }
        }

        public DataSet Data { get; set; }

        private Tuple<SortedDictionary<IComparable, double>, SortedDictionary<MyPair<IComparable, IComparable>, double>> classifier;
        public Tuple<SortedDictionary<IComparable, double>, SortedDictionary<MyPair<IComparable, IComparable>, double>> Train(DataSet set)
        {
            //List<Tuple<string, char>> names
            var classes = new SortedDictionary<IComparable, double>();
            var freq = new SortedDictionary<MyPair<IComparable, IComparable>, double>();
            foreach (var item in set.Items)
            {
                IComparable label = item.Attributes[this.ClassificationAttributeName].Value;

                if (!classes.ContainsKey(label))
                    classes[label] = 0;
                classes[label]++;

                foreach (var feature in item.Attributes.Values)
                {
                    if (feature.Name == this.ClassificationAttributeName) continue;

                    var value = feature.Value;

                    var kp = new MyPair<IComparable, IComparable>(label, value);
                    if (!freq.ContainsKey(kp))
                        freq[kp] = 0;
                    freq[kp]++;
                }
            }


            foreach (var pair in freq.Keys.ToList())
            {
                freq[pair] = freq[pair] / classes[pair.Key];
            }
            foreach (var c in classes.Keys.ToList())
            {
                classes[c] = 1.0 * classes[c] / set.Items.Count;
            }
            return new Tuple<SortedDictionary<IComparable, double>, SortedDictionary<MyPair<IComparable, IComparable>, double>>(classes, freq);
        }

        public Classification Classify(Item newItem)
        {
            var classes = classifier.Item1;
            var prob = classifier.Item2;

            double _min = 1e10;
            object _val = null;
            foreach (var c1 in classes.Keys)
            {
                double res = -Math.Log(classes[c1]);
                double res1 = 0;
                foreach (var feat in newItem.Attributes.Values)
                {
                    if (feat.Name == this.ClassificationAttributeName) continue;

                    var value = feat.Value;

                    var pp = new MyPair<IComparable, IComparable>(c1, value);
                    if (prob.ContainsKey(pp))
                        res1 += -Math.Log(prob[pp]);
                    else
                        res1 += -Math.Log(1e-7);
                }

                res += res1;
                if (res < _min)
                {
                    _min = res;
                    _val = c1;
                }
            }

            var ans = new Classification();
            ans.Result.Add(new ClassificationItem()
            {
                Value = _val,
                Count = 1,
                Percent = 100
            });
            return ans;
        }

        public void Build()
        {
            classifier = Train(this.Data);
        }

        public void Prunning(DataSet set, double z) { }

        public double GetRightProbability(DataSet set)
        {
            int right = 0;
            foreach (var item in set.Items)
            {
                var res = this.Classify(item);
                var top = res.Result.FirstOrDefault().Value;
                var expected = item.Attributes[this.ClassificationAttributeName].Value;
                if (expected.CompareTo(top) == 0)
                    right++;
            }
            double percent = 100.0 * right / set.Items.Count;
            return percent;
        }
    }
}

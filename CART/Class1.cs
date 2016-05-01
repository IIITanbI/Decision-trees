using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT_Algorithm
{
    public enum AttributeType
    {
        Discrete,
        Continuous,

        Numerical,
        CategoricalNominal, //0 - home, 1 - car
        CategoricalOrdinal, //rate from 0 to 5
    }
    public interface IItemAttribute : IComparable
    {
        string Name { get; set; }
        AttributeType AttributeType { get; }
        object Value { get; set; }
    }

    public class MyAttribute : IItemAttribute
    {
        public MyAttribute(string name, IComparable value, AttributeType attributeType)
        {
            this.Name = name;
            this.Value = value;

            this.AttributeType = attributeType;
        }

        public string Name { get; set; }
        public object Value { get; set; }

        public AttributeType AttributeType { get; set; } = AttributeType.Discrete;

        public int CompareTo(object obj)
        {
            if (Value.GetType() != obj.GetType())
                throw new ArgumentException("Types are not equal");

            return ((IComparable)this.Value).CompareTo(obj);
        }
    }

    public class Item
    {
        public Dictionary<string, IItemAttribute> Attributes { get; set; } = new Dictionary<string, IItemAttribute>();
    }
    public class DataSet
    {
        public List<Item> Items { get; set; } = new List<Item>();
    }


    public class Classification
    {
        //class - percent
        public List<Tuple<object, double>> Result = new List<Tuple<object, double>>();
    }
    public class Node
    {
        public string ClassificationAttributeName { get; set; }
        //public Dictionary<string, IItemAttribute> AllowedAttributes { get; set; } = new Dictionary<string, IItemAttribute>();

        public Tree Tree { get; }
        public Node(Tree tree, int height)
        {
            this.Tree = tree;
            this.Height = height;
        }

        public Classification Category { get; set; } = new Classification();
        public string SplitAttributeName { get; set; }
        public object SplitValue { get; set; }

        public DataSet Data { get; set; }

        public Node LeftChild { get; private set; }
        public Node RightChild { get; private set; }

        [Obsolete]
        public List<Node> Nodes { get; private set; } = new List<Node>();

        public int Height { get; }


        public void Build()
        {
            double _gini = Utility.Gini(Data, ClassificationAttributeName);

            foreach (var pair in Tree._attributeMap)
            {
                string attrName = pair.Key;
                AttributeType attrType = pair.Value.Item1;
                double max_gini = 0;

                #region Numerical Attribute
                if (attrType == AttributeType.Numerical)
                {
                    DataSet skippedSet = new DataSet();
                    DataSet nonSkippedSet = new DataSet();

                    var set = new SortedSet<object>();

                    foreach (var item in Data.Items)
                    {
                        if (!item.Attributes.ContainsKey(attrName))
                        {
                            skippedSet.Items.Add(item);
                            continue;
                        }
                        else
                        {
                            var val = item.Attributes[attrName].Value;
                            nonSkippedSet.Items.Add(item);
                            set.Add(val);
                        }
                    }


                    foreach (var threshold in set)
                    {
                        var split = Utility.SplitNumerical(nonSkippedSet, attrName, threshold);
                        double gini = split[0].Items.Count * Utility.Gini(split[0], ClassificationAttributeName) + split[1].Items.Count * Utility.Gini(split[1], ClassificationAttributeName);
                        gini /= nonSkippedSet.Items.Count;

                        double giniGain = _gini - gini;
                        if (giniGain > max_gini)
                        {
                            max_gini = giniGain;
                        }
                    }
                }
                #endregion

                #region  Categorical Attribute
                if (attrType == AttributeType.CategoricalNominal)
                {
                    DataSet skippedSet = new DataSet();
                    DataSet nonSkippedSet = new DataSet();

                    var set = new SortedSet<object>();
                    foreach (var item in Data.Items)
                    {
                        if (!item.Attributes.ContainsKey(attrName))
                        {
                            skippedSet.Items.Add(item);
                            continue;
                        }
                        else
                        {
                            nonSkippedSet.Items.Add(item);

                            var val = item.Attributes[attrName].Value;
                            set.Add(val);
                        }
                    }
                    var list = set.ToList();

                    int count = set.Count;
                    long mask = (1 << count);

                    for (long i = 1; i < mask - 1; i++)
                    {
                        DataSet oneSet = new DataSet();
                        DataSet twoSet = new DataSet();

                        long t = i;
                        for (int ind = 0; ind < count; ind++)
                        {
                            long bit = t & (1 << ind);
                            if (bit == 1)
                            {
                                oneSet.Items.AddRange(Data.Items.Where(x => ((IComparable)x.Attributes[attrName].Value).CompareTo(list[ind]) == 0));
                            }
                            else
                            {
                                twoSet.Items.AddRange(Data.Items.Where(x => ((IComparable)x.Attributes[attrName].Value).CompareTo(list[ind]) == 0));
                            }
                        }

                        double gini = oneSet.Items.Count * Utility.Gini(oneSet, ClassificationAttributeName) + twoSet.Items.Count * Utility.Gini(twoSet, ClassificationAttributeName);
                        gini /= nonSkippedSet.Items.Count;

                        double giniGain = _gini - gini;
                        if (giniGain > max_gini)
                        {
                            max_gini = giniGain;
                            SplitAttributeName = attrName;
                            SplitValue = oneSet;

                        }
                    }
                }
                #endregion
            }

        }


        

        public void RecalcClassification()
        {
            var mapping = Utility.ClassFrequency(Data, ClassificationAttributeName);

            this.Category = new Classification();
            double totalCount = Data.Items.Count;
            foreach (var pair in mapping)
            {
                object value = pair.Key;
                double count = pair.Value;

                double p = count / totalCount;
                Category.Result.Add(new Tuple<object, double>(value, p));
            }
        }
    }

    public class Tree
    {
        //<string, Tuple<Attr type, Value type>>
        internal Dictionary<string, Tuple<AttributeType, Type>> _attributeMap = new Dictionary<string, Tuple<AttributeType, Type>>();
        public string ClassificationAttributeName { get; set; }

        public DataSet Data { get; set; }
        public Node Root { get; private set; }

        private void BuildAttributeMap()
        {
            foreach (var item in Data.Items)
            {
                foreach (var attribute in item.Attributes.Values)
                {
                    if (_attributeMap.ContainsKey(attribute.Name))
                    {
                        var tuple = _attributeMap[attribute.Name];
                        AttributeType attributeType = tuple.Item1;
                        Type valueType = tuple.Item2;


                        if (attributeType != attribute.AttributeType)
                        {
                            throw new ArgumentException($"Attribute {attribute.Name} of {item}  have different attribute types : {attributeType} != {attribute.AttributeType}");
                        }

                        if (attribute.Value != null && valueType != attribute.Value.GetType())
                        {
                            throw new ArgumentException($"Attribute {attribute.Name} of {item}  have different type of values : {valueType} != {attribute.Value.GetType()}");
                        }
                    }
                    else
                    {
                        _attributeMap.Add(attribute.Name, new Tuple<AttributeType, Type>(attribute.AttributeType, attribute.Value.GetType()));
                    }
                }
            }
        }

        public void Build()
        {
            BuildAttributeMap();
            Root = new Node(this, 0);
            Root.ClassificationAttributeName = this.ClassificationAttributeName;
            Root.Data = this.Data;
            Root.Build();
        }
    }


    public static class Utility
    {
        //split set with continuous attrbibute into to 2 parts;
        //List[0] - match
        //List[1] - no match
        public static List<DataSet> SplitNumerical(DataSet set, string attributeName, object threshold)
        {
            List<DataSet> res = new List<DataSet>();
            res.Add(new DataSet());
            res.Add(new DataSet());

            foreach (var item in set.Items)
            {
                var attr = item.Attributes[attributeName];
                var value = attr.Value;

                //<= threshold
                if (((IComparable)value).CompareTo(threshold) <= 0)
                {
                    res[0].Items.Add(item);
                }
                else
                {
                    res[1].Items.Add(item);
                }
            }

            return res;
        }

        public static List<DataSet> SplitICategorical(DataSet set, string attributeName)
        {
            List<DataSet> res = new List<DataSet>();

            var mapping = new Dictionary<object, List<Item>>();

            foreach (var item in set.Items)
            {
                var attr = item.Attributes[attributeName];
                var value = attr.Value;

                if (!mapping.ContainsKey(value))
                    mapping[value] = new List<Item>();
                mapping[value].Add(item);
            }

            foreach (var it in mapping)
            {
                DataSet ds = new DataSet();
                ds.Items = it.Value;
                res.Add(ds);
            }

            return res;
        }

        public static double Gini(DataSet set, string attributeName)
        {
            var mapping = ClassFrequency(set, attributeName);

            double summ = 0;
            double totalCount = set.Items.Count;
            foreach (var count in mapping.Values)
            {
                summ += Math.Pow(1.0 * count / totalCount, 2);
            }

            return 1.0 - summ;
        }

        public static Dictionary<object, int> ClassFrequency(DataSet set, string attributeName)
        {
            var mapping = new Dictionary<object, int>();
            foreach (var item in set.Items)
            {
                if (!item.Attributes.ContainsKey(attributeName))
                    continue;

                if (!mapping.ContainsKey(item.Attributes[attributeName].Value))
                    mapping[item.Attributes[attributeName].Value] = 0;

                mapping[item.Attributes[attributeName].Value]++;
            }

            return mapping;
        }
    }
}

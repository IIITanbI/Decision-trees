using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main;
using System.Diagnostics;

namespace KNN
{
    public class KnnAttribute : IItemAttribute
    {
        public string Name { get; set; }
        public IComparable Value { get; set; }
        public AttributeType AttributeType { get; set; } = AttributeType.Discrete;

        public KnnAttribute(string name, IComparable value)
        {
            this.Name = name;
            this.Value = value;
        }

        public KnnAttribute(string name, IComparable value, AttributeType attributeType)
        {
            this.Name = name;
            this.Value = value;
            this.AttributeType = attributeType;
        }
    }
    public partial class KNNTree : ITree
    {
        public int K { get; set; } = 40;

        public string Name => "KNN";

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
        List<string> fullList = new List<string>();

        //min, max
        Dictionary<string, Tuple<double, double>> dct = new Dictionary<string, Tuple<double, double>>();
        Dictionary<Item, double> CurrentDistance = new Dictionary<Item, double>();
        public Classification Classify(Item newItem)
        {
            SortedDictionary<double, List<Item>> Distances = new SortedDictionary<double, List<Item>>();

            foreach (var item in Data.Items)
            {
                var dist = DefaultDistance(item, newItem);

                if (!Distances.ContainsKey(dist))
                    Distances[dist] = new List<Item>();

                Distances[dist].Add(item);
                CurrentDistance[item] = dist;
            }
            var hash = new HashSet<double>();

            int count = 0;
            var knn = new List<Item>();
            foreach (var list in Distances.Values)
            {
                foreach (var item in list)
                {
                    if (count >= this.K) break;
                    knn.Add(item);
                    count++;
                }
            }

            Classification res = Votes(knn, newItem);
            return res;
        }

        private Classification Votes(List<Item> items, Item newItem)
        {

            if (items.FirstOrDefault().Attributes[this.ClassificationAttributeName].AttributeType == AttributeType.Continuous)
            {
                double value = 0;

                double kdst = 0;
                double dstsumm = 0;
                foreach (var item in items)
                {
                    double dstS = Math.Pow(CurrentDistance[item], 2);
                    kdst += dstS * (double)item.Attributes[this.ClassificationAttributeName].Value;
                    dstsumm += dstS;
                }
                value = kdst / dstsumm;
                return new Classification()
                {
                    Result = new SortedSet<ClassificationItem>()
                    {
                        new ClassificationItem()
                        {
                            Value = value,
                            Percent = 100,
                            Count = 1
                        }
                    }
                };
            }

            var map = new Dictionary<object, double>();

            var classes = new HashSet<object>();
            foreach (var item in items)
            {
                var _class = item.Attributes[this.ClassificationAttributeName].Value;
                map[_class] = 0;
                classes.Add(_class);
            }

            var cnt = new Dictionary<object, int>();
            foreach (var _class in classes)
            {
                cnt[_class] = 0;
                double vote = 0;
                foreach (var item in items)
                {
                    if (item.Attributes[this.ClassificationAttributeName].Value.Equals(_class))
                    {
                        double dist = CurrentDistance[item];
                        double sqr_dist = dist * dist;
                        vote += 1.0 / sqr_dist;
                        cnt[_class]++;

                    }
                }
                map[_class] = vote;
            }

            double max = 0;
            object _classify = null;

            double total = items.Count;

            Classification cls = new Classification();

            foreach (var pair in map)
            {
                if (pair.Value > max)
                {
                    max = pair.Value;
                    _classify = pair.Key;

                    cls.Result.Clear();
                    cls.Result.Add(new ClassificationItem()
                    {
                        Value = pair.Key,
                        Count = pair.Value,
                        Percent = pair.Value / total
                    });
                }
            }

            return cls;
        }

        public double DefaultDistance(Item a, Item b)
        {
            double result = 0;
            foreach (var name in fullList)
            {
                IItemAttribute a_attr = null, b_attr = null;

                a.Attributes.TryGetValue(name, out a_attr);
                b.Attributes.TryGetValue(name, out b_attr);

                IComparable a_value = a_attr?.Value;
                IComparable b_value = b_attr?.Value;

                if (a_value != null && b_value != null)
                {
                    try
                    {
                        if (dct.ContainsKey(name))
                        {
                            var cur = dct[name];
                            var min = cur.Item1;
                            var max = cur.Item2;

                            a_value = ((double)a_value - min) / (max - min);
                            b_value = ((double)b_value - min) / (max - min);

                            result += Math.Pow((double)a_value - (double)b_value, 2);
                            continue;
                        }
                     }
                    catch (Exception)
                    {
                        
                    }
                    result += a_value.CompareTo(b_value) == 0 ? 0 : 1;
                }
            }

            result = Math.Sqrt(result);
            return result;
        }

        public void Build()
        {
            IEnumerable<string> temp = new List<string>();
            foreach (var item in Data.Items)
            {
                temp = temp.Union(item.Attributes.Keys);
            }
            fullList = temp.ToList();


            foreach (var item in Data.Items)
            {
                foreach(var attribute in item.Attributes.Values)
                {
                    var value = attribute.Value;
                    var name = attribute.Name;
                    try
                    {
                        var d = (double)value;
                        
                        if (!dct.ContainsKey(name))
                        {
                            dct[name] = new Tuple<double, double>(d, d);
                        }
                        else
                        {
                            var cur = dct[name];
                            dct[name] = new Tuple<double, double>(Math.Min(cur.Item1, d), Math.Max(cur.Item2, d));
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public void Prunning(DataSet set, double z) { }

        public double GetRightProbability(DataSet set)
        {
            var sw1 = Stopwatch.StartNew();

            double range = 5.0;

            int right = 0;
            foreach (var item in set.Items)
            {

                var res = this.Classify(item);

                if (set.Items.First().Attributes[this.ClassificationAttributeName].AttributeType == AttributeType.Continuous)
                {
                    double rr = 0.0;
                    foreach (var r in res.Result)
                    {
                        rr += (double)r.Value * r.Percent / 100.0;
                    }
                    var expected = item.Attributes[this.ClassificationAttributeName].Value;
                    if (expected.CompareTo(rr + range) <= 0 && expected.CompareTo(rr - range) >= 0)
                        right++;
                }
                else
                {
                    var top = res.Result.FirstOrDefault().Value;
                    var expected = item.Attributes[this.ClassificationAttributeName].Value;
                    if (expected.CompareTo(top) == 0)
                        right++;
                }
            }
            double percent = 100.0 * right / set.Items.Count;

            double ftime = sw1.Elapsed.Milliseconds;
            return percent;
        }

        public double Test(DataSet set, DataSet test)
        {
            this.Data = set;
            this.Build();
            var sw1 = Stopwatch.StartNew();
            double best = 0;
            for (int k = 1; k <= test.Items.Count; k++)
            {

                double summ = 0;
                this.K = k;
                foreach (var attr in fullList)
                {
                    this.ClassificationAttributeName = attr;
                    int right = 0;
                    foreach (var item in test.Items)
                    {
                        var res = this.Classify(item);
                        var top = res.Result.FirstOrDefault().Value;
                        var expected = item.Attributes[this.ClassificationAttributeName].Value;
                        if (expected.CompareTo(top) == 0)
                            right++;
                    }
                    double percent = 100.0 * right / test.Items.Count;
                    summ += percent;
                }


                best = Math.Max(best, summ);
                Console.WriteLine("k = " + k + " best = " + best + " current = " + summ);
            }


            double ftime = sw1.Elapsed.Milliseconds;
            return best;
        }
    }
}

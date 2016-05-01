using ID3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeUI
{
    public class S_Node
    {
        public Node Node { get; set; }
        public int LeftWidth { get; set; } = 0;
        public int RigthWidth { get; set; } = 0;
        public int OwnWidth { get; set; } = 0;
        public int TotalWidth
        {
            get
            {
                return LeftWidth + OwnWidth + RigthWidth;
            }
        }

        public int Height { get; } = 0;
        public int StartPosition { get; set; } = -1;

        public List<S_Node> Nodes { get; private set; } = new List<S_Node>();

        public S_Node(Node sourceNode)
        {
            this.Node = sourceNode;
            this.Height = sourceNode.Height;

            foreach (Node node in sourceNode.Nodes)
            {
                S_Node s_node = new S_Node(node);
                Nodes.Add(s_node);
            }

        }
    }
    class TreeStats
    {
        public int MaxHeight { get; private set; } = -1;
        public int MaxWidth { get; private set; } = -1;
        public S_Node Root { get; private set; }

        public List<int> mapLevels { get; } = new List<int>();
        public TreeStats(Tree tree)
        {
            Parse(tree);
            CopyTree(tree);
        }

        private void Parse(Tree tree)
        {
            var queue = new Queue<Node>();
            queue.Enqueue(tree.Root);

            int maxWidth = -1;
            int curWidth = 0;
            int maxHeight = -1;

            while (queue.Count > 0)
            {
                Node cur = queue.Dequeue();
                int height = cur.Height;

                if (height > maxHeight)
                {
                    maxWidth = Math.Max(maxWidth, curWidth);
                    if (height > 0)
                        mapLevels.Add(curWidth);
                    curWidth = 0;
                    maxHeight = height;
                }

                foreach (Node node in cur.Nodes)
                {
                    queue.Enqueue(node);
                }
                curWidth++;
            }
            maxWidth = Math.Max(maxWidth, curWidth);
            mapLevels.Add(curWidth);

            this.MaxHeight = maxHeight;
            this.MaxWidth = MaxWidth;
        }

        private void CopyTree(Tree tree)
        {
            S_Node root = new S_Node(tree.Root);
            this.Root = root;
        }
    }
    public partial class TreeVisualizer : UserControl
    {
        TreeStats treeStats;
        S_Node[,] matrix;
        public TreeVisualizer()
        {
            InitializeComponent();
        }


        public int GetWidth(S_Node node)
        {
            int ownWidth = 1;
            int leftWidth = 0;
            int rigthWidth = 0;

            int cnt = node.Nodes.Count;
            if (cnt % 2 == 0)
            {
                int to = cnt / 2;
                for (int i = 0; i < to; i++)
                {
                    leftWidth += GetWidth(node.Nodes[i]);

                    //space
                    //if (i < to - 1)
                        leftWidth += 1;
                }

                int from = cnt / 2;
                for (int i = from; i < cnt; i++)
                {
                    rigthWidth += GetWidth(node.Nodes[i]);

                    //space
                    //if (i < cnt - 1)
                        rigthWidth += 1;
                }
            }
            else
            {
                int to = cnt / 2;
                for (int i = 0; i < to; i++)
                {
                    leftWidth += GetWidth(node.Nodes[i]);

                    //space
                    leftWidth += 1;
                }

                GetWidth(node.Nodes[cnt / 2]);
                leftWidth += node.Nodes[cnt / 2].LeftWidth;
                rigthWidth += node.Nodes[cnt / 2].RigthWidth;

                int from = cnt / 2 + 1;
                for (int i = from; i < cnt; i++)
                {
                    rigthWidth += GetWidth(node.Nodes[i]);

                    //space
                    rigthWidth += 1;
                }
            }

            node.LeftWidth = leftWidth;
            node.RigthWidth = rigthWidth;
            node.OwnWidth = ownWidth;

            return node.TotalWidth;
        }
        public void Draw(Tree tree)
        {
            treeStats = new TreeStats(tree);
            int total = GetWidth(treeStats.Root);

            int n = (treeStats.MaxHeight + 1) * 2 - 1;
            int m = total;
            matrix = new S_Node[n, m];

            var root = treeStats.Root;
            root.StartPosition = root.LeftWidth;
            RegisterNode(root);

            Dfs(root);

            for (int i = 0; i < n; i++)
            {
                RowDefinition row = new RowDefinition();
                //row.Height = GridLength.Auto;
                //row.Height = new GridLength(50);
                row.MinHeight = 50;
                MainGrid.RowDefinitions.Add(row);
            }

            for (int i = 0; i < m; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                //column.Width = GridLength.Auto;
                //column.Width = new GridLength(100);
                column.MinWidth = 50;
                MainGrid.ColumnDefinitions.Add(column);
            }

            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < m; j++)
                {
                    if (matrix[i, j] != null)
                    {
                        S_Node node = matrix[i, j];
                        Label label = new Label();
                        string res = 
                            $"split value: {node.Node.SplitValue} \n" +
                            $"split threshold: {node.Node.SplitValueThreshold} \n" +
                            $"split sign: {node.Node.SplitValueThresholdSign} \n" +
                            $"split attr: {node.Node.SplitAttributeName} \n" +
                            $"res: {node.Node.Classification.Result.FirstOrDefault()?.Value}  {node.Node.Classification.Result.FirstOrDefault()?.Percent}";
                        label.Content = res;

                        if (node.Nodes.Count > 0)
                        {
                            foreach(S_Node children in node.Nodes)
                            {
                                int ii = children.Height;
                                int jj = children.StartPosition;

                                int di = Math.Abs(ii - i) - 1;
                                

                                if (jj > j)
                                {
                                    var path = GetPath(new Point(0, 0), new Point(1, 1));
                                    int dj = jj - j - 1;
                                    int column = j + 1;
                                    if (dj > 0)
                                    {
                                        Grid.SetColumnSpan(path, dj);
                                        Grid.SetColumn(path, column);
                                    }
                                    else
                                    {
                                        Grid.SetColumn(path, column);
                                    }
                                    Grid.SetRow(path, i * 2 + 1);
                                    MainGrid.Children.Add(path);

                                }
                                else if (j > jj)
                                {
                                    //left
                                    var path = GetPath(new Point(0, 1), new Point(1, 0));
                                    int dj = j - jj - 1;
                                    int column = j - 1;
                                    if (dj > 0)
                                    {
                                        Grid.SetColumnSpan(path, dj);
                                        Grid.SetColumn(path, column - dj  + 1);
                                    } else
                                    {
                                        Grid.SetColumn(path, column);
                                    }
                                    Grid.SetRow(path, i * 2 + 1);
                                    MainGrid.Children.Add(path);
                                } 
                                else
                                {
                                    //mid
                                    var path = GetPath(new Point(0, 0), new Point(0, 1));
                                    Grid.SetRow(path, i * 2 + 1);
                                    Grid.SetColumn(path, j);
                                    MainGrid.Children.Add(path);
                                }


                            }
                        }

                        Grid.SetRow(label, i*2);
                        Grid.SetColumn(label, j);
                        MainGrid.Children.Add(label);
                    }
                }
            }
            MainGrid.ShowGridLines = true;
        }

        private Path GetPath(Point start, Point end)
        {
            Path path = new Path();
            path.Data = new LineGeometry(start, end);
            path.HorizontalAlignment = HorizontalAlignment.Stretch;
            path.VerticalAlignment = VerticalAlignment.Stretch;
            path.Stretch = Stretch.Fill;
            path.StrokeThickness = 4;
            path.Stroke = Brushes.Black;
            path.Fill = Brushes.MediumSlateBlue;
            return path;

        }
        private void RegisterNode(S_Node node)
        {
            matrix[node.Height, node.StartPosition] = node;
        }
        public void Dfs(S_Node node)
        {
            int cnt = node.Nodes.Count;
            if (cnt % 2 == 0)
            {
                int sPos = 1;
                int to = cnt / 2;
                for (int i = to - 1; i >= 0; i--)
                {
                    S_Node curNode = node.Nodes[i];
                    curNode.StartPosition = node.StartPosition - sPos - curNode.RigthWidth - curNode.OwnWidth;
                    sPos += node.Nodes[i].TotalWidth + 1;
                    RegisterNode(curNode);
                }

                sPos = 1;
                int from = cnt / 2;
                for (int i = from; i < cnt; i++)
                {
                    S_Node curNode = node.Nodes[i];
                    curNode.StartPosition = node.StartPosition + node.OwnWidth + sPos + curNode.LeftWidth;
                    sPos += node.Nodes[i].TotalWidth + 1;
                    RegisterNode(curNode);
                }
            }
            else
            {
                int sPos = 0;

                S_Node midNode = node.Nodes[cnt / 2];
                {
                    midNode.StartPosition = node.StartPosition;
                    RegisterNode(midNode);
                }

                sPos = midNode.LeftWidth + 1;
                int to = cnt / 2;
                for (int i = to - 1; i >= 0; i--)
                {
                    S_Node curNode = node.Nodes[i];
                    curNode.StartPosition = node.StartPosition - sPos - curNode.RigthWidth - curNode.OwnWidth;
                    sPos += node.Nodes[i].TotalWidth + 1;
                    RegisterNode(curNode);
                }

                sPos = midNode.RigthWidth + 1;
                int from = cnt / 2 + 1;
                for (int i = from; i < cnt; i++)
                {
                    S_Node curNode = node.Nodes[i];
                    curNode.StartPosition = node.StartPosition + node.OwnWidth + sPos + curNode.LeftWidth;
                    sPos += node.Nodes[i].TotalWidth + 1;
                    RegisterNode(curNode);
                }
            }

            foreach(S_Node children in node.Nodes)
            {
                Dfs(children);
            }
        }
    }
}

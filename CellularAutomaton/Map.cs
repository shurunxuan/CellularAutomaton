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

namespace CellularAutomaton
{
    public class Map
    {
        public enum EdgeType
        {
            Periodic = 0,
            Reflective = 1,
            Constant = 2
        }

        private int[,] _content;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public readonly EdgeType Type;

        public int this[int index0, int index1]
        {
            get
            {
                int x = index0;
                int y = index1;

                switch (Type)
                {
                    case EdgeType.Periodic:
                        while (y < 0)
                            y += Height;
                        while (y >= Height)
                            y -= Height;
                        while (x < 0)
                            x += Width;
                        while (x >= Width)
                            x -= Width;
                        break;
                    case EdgeType.Reflective:
                        while (y < 0 || y >= Height)
                        {
                            if (Math.Abs(y) < Math.Abs(y - Height))
                            {
                                y = -y - 1;
                            }
                            else
                            {
                                y = 2 * Height - y - 1;
                            }
                        }

                        while (x < 0 || x >= Width)
                        {
                            if (Math.Abs(x) < Math.Abs(x - Width))
                            {
                                x = -x - 1;
                            }
                            else
                            {
                                x = 2 * Width - x - 1;
                            }
                        }
                        break;
                    case EdgeType.Constant:
                        {
                            return 0;
                        }
                }

                return _content[x, y];
            }
            set
            {
                int y = index0;
                int x = index1;

                switch (Type)
                {
                    case EdgeType.Periodic:
                        while (y < 0)
                            y += Height;
                        while (y >= Height)
                            y -= Height;
                        while (x < 0)
                            x += Width;
                        while (x >= Width)
                            x -= Width;
                        break;
                    case EdgeType.Reflective:
                        while (y < 0 || y >= Height)
                        {
                            if (Math.Abs(y) < Math.Abs(y - Height))
                            {
                                y = -y - 1;
                            }
                            else
                            {
                                y = 2 * Height - y - 1;
                            }
                        }

                        while (x < 0 || x >= Width)
                        {
                            if (Math.Abs(x) < Math.Abs(x - Width))
                            {
                                x = -x - 1;
                            }
                            else
                            {
                                x = 2 * Width - x - 1;
                            }
                        }
                        break;
                    case EdgeType.Constant:
                        {
                            return;
                        }
                }
                _content[x, y] = value;
            }
        }

        public Map(int width, int height, EdgeType edgeType = EdgeType.Periodic)
        {
            Width = width;
            Height = height;
            Type = edgeType;
            _content = new int[Width, Height];
        }

        public void Resize(int width, int height)
        {
            int[,] newContent = new int[width, height];

            for (uint i = 0; i < Math.Min(width, Width); ++i)
            {
                for (uint j = 0; j < Math.Min(height, Height); ++j)
                {
                    newContent[i, j] = _content[i, j];
                }
            }

            Height = height;
            Width = width;
            _content = newContent;
        }

        public void Evolve(List<Rule> rules)
        {
            int[,] tempBuffer = new int[Width, Height];

            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                {
                    bool result = false;
                    foreach (var r in rules)
                    {
                        result = r.Match(this, i, j);
                        if (result)
                        {
                            tempBuffer[i, j] = r.Result == -1 ? this[i, j] : r.Result;
                            break;
                        }
                    }

                    if (!result) tempBuffer[i, j] = this[i, j];
                }

            _content = tempBuffer;
        }


    }
}
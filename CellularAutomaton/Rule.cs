using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CellularAutomaton
{
    public class Rule
    {
        public int Top
        {
            get => _top;
            set
            {
                if (_top == value) return;
                int[,] newRule = new int[Left + 1 + Right, value + 1 + Bottom];
                // TODO: Copy the old rule
                Content = newRule;
                _top = value;

                for (int i = -Left; i <= Right; ++i)
                for (int j = -_top; j <= Bottom; ++j)
                {
                    this[i, j] = ((i == 0 && j == 0) ? 0 : -1);
                }
            }
        }
        public int Bottom
        {
            get => _bottom;
            set
            {
                if (_bottom == value) return;
                int[,] newRule = new int[Left + 1 + Right, Top + 1 + value];
                // TODO: Copy the old rule
                Content = newRule;
                _bottom = value;

                for (int i = -Left; i <= Right; ++i)
                for (int j = -Top; j <= _bottom; ++j)
                {
                    this[i, j] = ((i == 0 && j == 0) ? 0 : -1);
                }
            }
        }
        public int Left
        {
            get => _left;
            set
            {
                if (_left == value) return;
                int[,] newRule = new int[value + 1 + Right, Top + 1 + Bottom];
                // TODO: Copy the old rule
                Content = newRule;
                _left = value;

                for (int i = -_left; i <= Right; ++i)
                for (int j = -Top; j <= Bottom; ++j)
                {
                    this[i, j] = ((i == 0 && j == 0) ? 0 : -1);
                }
            }
        }
        public int Right
        {
            get => _right;
            set
            {
                if (_right == value) return;
                int[,] newRule = new int[Left + 1 + value, Top + 1 + Bottom];
                // TODO: Copy the old rule
                Content = newRule;
                _right = value;

                for (int i = -Left; i <= _right; ++i)
                for (int j = -Top; j <= Bottom; ++j)
                {
                    this[i, j] = ((i == 0 && j == 0) ? 0 : -1);
                }
            }
        }

        private int _top;
        private int _bottom;
        private int _left;
        private int _right;

        public short RotateMode;
        public string DescriptionString { get; set; }

        public int Result;

        private static bool GetBit(short num, int bit)
        {
            return ((num >> bit) & 1) == 1;
        }

        private static void SetBit(ref short num, int bit, bool value)
        {
            if (value)
            {
                num |= (short)(1 << bit);
            }
            else
            {
                num &= (short)~(1 << bit);
            }
        }

        public bool AllowRotateType0
        {
            get => true;
            set => SetBit(ref RotateMode, 0, true);
        }

        public bool AllowRotateType1
        {
            get => GetBit(RotateMode, 1);
            set => SetBit(ref RotateMode, 1, value);
        }

        public bool AllowRotateType2
        {
            get => GetBit(RotateMode, 2);
            set => SetBit(ref RotateMode, 2, value);
        }

        public bool AllowRotateType3
        {
            get => GetBit(RotateMode, 3);
            set => SetBit(ref RotateMode, 3, value);
        }

        public bool AllowRotateType4
        {
            get => GetBit(RotateMode, 4);
            set => SetBit(ref RotateMode, 4, value);
        }

        public bool AllowRotateType5
        {
            get => GetBit(RotateMode, 5);
            set => SetBit(ref RotateMode, 5, value);
        }

        public bool AllowRotateType6
        {
            get => GetBit(RotateMode, 6);
            set => SetBit(ref RotateMode, 6, value);
        }

        public bool AllowRotateType7
        {
            get => GetBit(RotateMode, 7);
            set => SetBit(ref RotateMode, 7, value);
        }

        public int this[int index0, int index1]
        {
            get => Content[index0 + Left, index1 + Top];
            set => Content[index0 + Left, index1 + Top] = value;
        }

        public int[,] Content { get; private set; }

        public Rule(string description, int top, int bottom, int left, int right, short rotateMode = 0b00000001)
        {
            DescriptionString = description;
            _top = top;
            _bottom = bottom;
            _left = left;
            _right = right;
            RotateMode = rotateMode;

            Content = new int[Left + 1 + Right, Top + 1 + Bottom];
            for (int i = -Left; i <= Right; ++i)
                for (int j = -Top; j <= Bottom; ++j)
                {
                    this[i, j] = ((i == 0 && j == 0) ? 0 : -1);
                }
        }

        private bool Match0(Map map, int x, int y)
        {
            for (int i = -Left; i <= Right; ++i)
                for (int j = -Top; j <= Bottom; ++j)
                {
                    if (this[i, j] != -1 && this[i, j] != map[x + i, y + j])
                        return false;
                }

            return true;
        }

        private bool Match1(Map map, int x, int y)
        {
            for (int i = -Bottom; i <= Top; ++i)
            for (int j = -Left; j <= Right; ++j)
            {
                if (this[j, -i] != -1 && this[j, -i] != map[x + i, y + j])
                    return false;
            }

            return true;
        }

        private bool Match2(Map map, int x, int y)
        {
            for (int i = -Right; i <= Left; ++i)
                for (int j = -Bottom; j <= Top; ++j)
                {
                    if (this[-i, -j] != -1 && this[-i, -j] != map[x + i, y + j])
                        return false;
                }

            return true;
        }

        private bool Match3(Map map, int x, int y)
        {
            for (int i = -Top; i <= Bottom; ++i)
            for (int j = -Right; j <= Left; ++j)
            {
                if (this[-j, i] != -1 && this[-j, i] != map[x + i, y + j])
                    return false;
            }

            return true;
        }

        private bool Match4(Map map, int x, int y)
        {
            for (int i = -Right; i <= Left; ++i)
                for (int j = -Top; j <= Bottom; ++j)
                {
                    if (this[-i, j] != -1 && this[-i, j] != map[x + i, y + j])
                        return false;
                }

            return true;
        }

        private bool Match5(Map map, int x, int y)
        {
            for (int i = -Bottom; i <= Top; ++i)
                for (int j = -Right; j <= Left; ++j)
                {
                    if (this[-j, -i] != -1 && this[-j, -i] != map[x + i, y + j])
                        return false;
                }

            return true;
        }

        private bool Match6(Map map, int x, int y)
        {
            for (int i = -Left; i <= Right; ++i)
                for (int j = -Bottom; j <= Top; ++j)
                {
                    if (this[i, -j] != -1 && this[i, -j] != map[x + i, y + j])
                        return false;
                }

            return true;
        }

        private bool Match7(Map map, int x, int y)
        {
            for (int i = -Top; i <= Bottom; ++i)
                for (int j = -Left; j <= Right; ++j)
                {
                    if (this[j, i] != -1 && this[j, i] != map[x + i, y + j])
                        return false;
                }

            return true;
        }

        public bool Match(Map map, int x, int y)
        {
            // Match cell
            if (this[0, 0] != -1 && this[0, 0] != map[x, y])
                return false;

            // Match original rule
            if (Match0(map, x, y)) return true;
            if (AllowRotateType1) if (Match1(map, x, y)) return true;
            if (AllowRotateType2) if (Match2(map, x, y)) return true;
            if (AllowRotateType3) if (Match3(map, x, y)) return true;
            if (AllowRotateType4) if (Match4(map, x, y)) return true;
            if (AllowRotateType5) if (Match5(map, x, y)) return true;
            if (AllowRotateType6) if (Match6(map, x, y)) return true;
            if (AllowRotateType7) if (Match7(map, x, y)) return true;

            return false;
        }


    }
}

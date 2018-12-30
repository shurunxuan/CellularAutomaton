using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CellularAutomaton
{
    public class Status
    {
        public string Description;
        public Color DisplayColor;

        public Status(string description, Color displayColor)
        {
            Description = description;
            DisplayColor = displayColor;
        }
    }

    public class StatusList
    {
        private readonly Status _ignore;
        private List<Status> _list;

        public Status this[int id] => id == -1 ? _ignore : _list[id];

        public Status this[string description]
        {
            get
            {
                if (description == "Any State / Keep") return _ignore;
                foreach (var status in _list)
                {
                    if (status.Description == description)
                        return status;
                }

                return new Status("ErrorStatus", Colors.Transparent);
            }
        }

        public Status this[Color displayColor]
        {
            get
            {
                if (displayColor == Colors.DimGray) return _ignore;
                foreach (var status in _list)
                {
                    if (status.DisplayColor == displayColor)
                        return status;
                }

                return new Status("ErrorStatus", Colors.Transparent);

            }
        }

        public int Length => _list.Count;

        public StatusList(bool withDefault = true)
        {
            _list = new List<Status>();
            _ignore = new Status("Any State / Keep", Colors.DimGray);
            if (withDefault)
            {
                Status defaultStatus = new Status("Default", Colors.Black);
                _list.Add(defaultStatus);
            }
        }

        public bool Add(string description, Color displayColor)
        {
            if (this[description].Description != "ErrorStatus") return false;
            if (this[displayColor].Description != "ErrorStatus") return false;
            _list.Add(new Status(description, displayColor));
            return true;
        }

        public bool Add(Status newStatus)
        {
            if (this[newStatus.Description].Description != "ErrorStatus") return false;
            if (this[newStatus.DisplayColor].Description != "ErrorStatus") return false;
            _list.Add(newStatus);
            return true;
        }

        public int Find(string description)
        {
            if (description == "Any State / Keep") return -1;
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i].Description == description) return i;
            }

            return -2;
        }

        public int Find(Color displayColor)
        {
            if (displayColor == Colors.DimGray) return -1;
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i].DisplayColor == displayColor) return i;
            }

            return -2;
        }
    }
}

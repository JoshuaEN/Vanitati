﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Properties
{
    public sealed class PropData
    {
        public Type Type
        {
            get { return Info.PropertyType; }
        }
        public PropertyInfo Info { get; }
        public bool Readable { get; }
        public bool Writeable { get; }

        public PropData(PropertyInfo info, bool readable, bool writeable)
        {
            Info = info;
            Readable = readable;
            Writeable = writeable;
        }
    }
}

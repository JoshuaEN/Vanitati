using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UnnamedStrategyGame.UI.Settings
{
    public class KeyInput : Input
    {
        public Key Key { get; }

        public KeyInput(Key key)
        {
            Key = key;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
                return false;

            if (!(obj is KeyInput))
                return false;

            var keyInput = (obj as KeyInput);

            return this == keyInput;
        }

        public bool Equals(KeyInput keyInput)
        {
            return this == keyInput;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public static bool operator ==(KeyInput a, KeyInput b)
        {
            if (object.ReferenceEquals(a, b))
                return true;

            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;

            return a.Key == b.Key;
        }

        public static bool operator !=(KeyInput a, KeyInput b)
        {
            return !(a == b);
        }
    }
}

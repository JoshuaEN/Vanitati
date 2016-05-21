using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UnnamedStrategyGame.UI.Settings
{
    public class MouseInput : Input
    {
        public MouseButton Button { get; }

        public MouseInput(MouseButton button)
        {
            Button = button;
        }

        public override bool IsActive()
        {
            switch(Button)
            {
                case MouseButton.Left:
                    return Mouse.LeftButton == MouseButtonState.Pressed;
                case MouseButton.Middle:
                    return Mouse.MiddleButton == MouseButtonState.Pressed;
                case MouseButton.Right:
                    return Mouse.RightButton == MouseButtonState.Pressed;
                case MouseButton.XButton1:
                    return Mouse.XButton1 == MouseButtonState.Pressed;
                case MouseButton.XButton2:
                    return Mouse.XButton2 == MouseButtonState.Pressed;
                default:
                    throw new ArgumentException($"Unknown Mouse Button of {Button}");
            }
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
                return false;

            if (!(obj is MouseInput))
                return false;

            var mouseInput = (obj as MouseInput);

            return this == mouseInput;
        }

        public bool Equals(MouseInput mouseInput)
        {
            return this == mouseInput;
        }

        public override int GetHashCode()
        {
            return Button.GetHashCode();
        }

        public static bool operator== (MouseInput a, MouseInput b)
        {
            if (object.ReferenceEquals(a, b))
                return true;

            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;

            return a.Button == b.Button;
        }

        public static bool operator!= (MouseInput a, MouseInput b)
        {
            return !(a == b);
        }
    }
}

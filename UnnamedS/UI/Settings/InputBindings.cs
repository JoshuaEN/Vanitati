using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UnnamedStrategyGame.UI.Settings
{
    public class InputBindings
    {
        public InputBindSet MoveUp { get; set; } = new InputBindSet(new InputBind(new KeyInput(Key.W)));
        public InputBindSet MoveDown { get; set; } = new InputBindSet(new InputBind(new KeyInput(Key.S)));
        public InputBindSet MoveLeft { get; set; } = new InputBindSet(new InputBind(new KeyInput(Key.A)));
        public InputBindSet MoveRight { get; set; } = new InputBindSet(new InputBind(new KeyInput(Key.D)));

        public InputBindSet ZoomIn { get; set; } = new InputBindSet(new InputBind(new KeyInput(Key.PageUp)));
        public InputBindSet ZoomOut { get; set; } = new InputBindSet(new InputBind(new KeyInput(Key.PageDown)));

        public InputBindSet CancelSelection { get; set; } = new InputBindSet(new InputBind(new MouseInput(MouseButton.Right)));

        public class InputBindSet
        {
            public IList<InputBind> BindSet { get; }

            public InputBindSet(IList<InputBind> set)
            {
                Contract.Requires<ArgumentNullException>(null != set);
                BindSet = set;
            }

            public InputBindSet(InputBind bind) : this(new List<InputBind>() { bind }) { }

            public bool Active(IList<Input> activeInput)
            {
                Contract.Requires<ArgumentNullException>(null != activeInput);

                foreach (var bind in BindSet)
                {
                    if (bind.Active(activeInput) == true)
                        return true;
                }
                return false;
            }
        }

        public class InputBind
        {
            public IList<Input> Input { get; }

            public InputBind(IList<Input> input)
            {
                Contract.Requires<ArgumentNullException>(null != input);

                Input = input;
            }

            public InputBind(params Input[] input) : this(input.ToList())
            {
                Contract.Requires<ArgumentNullException>(null != input);
            }

            public bool Active(IList<Input> activeInput)
            {
                Contract.Requires<ArgumentNullException>(null != activeInput);

                if (activeInput.Count < 1)
                    return false;

                foreach (var i in Input)
                {
                    if (activeInput.Contains(i) == false)
                        return false;
                }

                return true;
            }
        }
    }
}

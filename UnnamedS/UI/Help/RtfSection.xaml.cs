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

namespace UnnamedStrategyGame.UI.Help
{
    /// <summary>
    /// Interaction logic for RtfSection.xaml
    /// </summary>
    public partial class RtfSection : UserControl
    {
        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (value.StartsWith(System.IO.Path.PathSeparator.ToString()))
                    _path = value;
                else
                    _path = System.IO.Path.Combine("UI", "Help", "RtfSections", value);

                _path = System.IO.Path.GetFullPath(_path);

                LoadRtf();
            }
        }
        public RtfSection()
        {
            InitializeComponent();
        }

        private void LoadRtf()
        {
            var doc = rtfBox.Document = new FlowDocument();
            if (System.IO.File.Exists(Path))
            {
                var range = new TextRange(doc.ContentStart, doc.ContentEnd);

                try
                {
                    using (var stream = System.IO.File.OpenRead(Path))
                    {
                        range.Load(stream, DataFormats.Rtf);
                    }
                }
                catch(Exception ex)
                {
                    doc.Blocks.Clear();
                    doc.Blocks.Add(new Paragraph(new Run($"Error: {ex.Message}")));
                    doc.Blocks.Add(new Paragraph(new Run(ex.StackTrace)));
                }
            }
            else
            {
                doc.Blocks.Clear();
                doc.Blocks.Add(new Paragraph(new Run($"Error: File {Path} does not exist.")));
            }
        }
    }
}

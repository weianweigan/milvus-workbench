using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Reflection;
using System.Windows.Controls;

namespace IO.Milvus.Workbench.DocumentViews
{
    /// <summary>
    /// CollectionPage.xaml 的交互逻辑
    /// </summary>
    public partial class CollectionPage : Page
    {
        public CollectionPage()
        {
            LoadHighlighting();
            InitializeComponent();
        }

        public void LoadHighlighting()
        {
            var url = Assembly.GetExecutingAssembly().GetName().Name + ".Assets.Editor.Query.xshd";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(url))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    var sqlDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    HighlightingManager.Instance.RegisterHighlighting("milvusquery", new string[] { ".mq" }, sqlDefinition);
                }
            }
        }
    }
}

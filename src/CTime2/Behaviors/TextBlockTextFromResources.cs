using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using CTime2.Strings;
using Microsoft.Xaml.Interactivity;

namespace CTime2.Behaviors
{
    public class TextBlockTextFromResources : Behavior<TextBlock>
    {
        public static readonly DependencyProperty ResourceProperty = DependencyProperty.Register(
            nameof(Resource), typeof(string), typeof(TextBlockTextFromResources), new PropertyMetadata(default(string)));

        public string Resource
        {
            get { return (string)this.GetValue(ResourceProperty); }
            set { this.SetValue(ResourceProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            var xaml = this.GetXaml();
            var content = (Span)XamlReader.Load(xaml);

            this.WireEvents(content);

            this.AssociatedObject.Inlines.Clear();
            this.AssociatedObject.Inlines.Add(content);
        }

        private string GetXaml()
        {
            var xaml = CTime2Resources.Get(this.Resource);
            return $"<Span xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">{xaml}</Span>";
        }

        protected virtual void WireEvents(Span span)
        {
            
        }
    }
}

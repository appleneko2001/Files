using Avalonia.Controls;
using Files.ViewModels.Breadcrumb;

namespace Files.Views.Browser
{
    public class Breadcrumbs : ResourceDictionary
    {
        private void RecyclingElementFactory_OnSelectTemplateKey(object sender, SelectTemplateEventArgs e)
        {
            //BreadcrumbElementTemplate
            switch (e.DataContext)
            {
                case BreadcrumbNodeEditViewModel:
                case BreadcrumbNodeSchemeViewModel:
                    e.TemplateKey = "NodeWithIcon";
                    break;
                
                case BreadcrumbNodeHostViewModel:
                case BreadcrumbNodeViewModel:
                    e.TemplateKey = "Node";
                    break;

                default:
                    // Use dummy template to avoid exception
                    e.TemplateKey = "DummyNode";
                    break;
                    //throw new NotSupportedException();
            }
        }
    }
}
using System;
using Avalonia.Controls;
using Files.Views.Models.Breadcrumb;

namespace Files.Views.Resources
{
    public class Breadcrumbs : ResourceDictionary
    {
        private void RecyclingElementFactory_OnSelectTemplateKey(object sender, SelectTemplateEventArgs e)
        {
            //BreadcrumbElementTemplate
            switch (e.DataContext)
            {
                case BreadcrumbNodeEditViewModel:
                    e.TemplateKey = "EditNode";
                    break;
                
                case BreadcrumbNodeSchemeViewModel:
                    e.TemplateKey = "SchemeNode";
                    break;
                
                case BreadcrumbNodeHostViewModel:
                case BreadcrumbNodeViewModel:
                    e.TemplateKey = "Node";
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
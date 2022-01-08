using System;

namespace Files.Views.Models.Breadcrumb
{
    public class BreadcrumbNodeHostViewModel : BreadcrumbNodeViewModel
    {
        public BreadcrumbNodeHostViewModel(BreadcrumbPathViewModel parent, int index, string path) : base(parent, index)
        {
            Path = path;
            
            if (path == string.Empty || path == "/")
            {
                Header = "Local";
                Path = "/";
            }
        }
    }
}
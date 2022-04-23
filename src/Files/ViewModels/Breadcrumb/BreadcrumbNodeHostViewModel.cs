using System;
using System.Linq;

namespace Files.ViewModels.Breadcrumb
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
            else
            {
                if (!path.Contains('@'))
                {
                    Header = path;
                    return;
                }

                var part = path.Split('@');
                if (!part.Any())
                    throw new ArgumentNullException();

                var host = part[1];
                var ui = part[0];

                if (ui.Contains(':'))
                    ui = ui[..ui.IndexOf(':')];

                Header = $"{host} {ui}";
            }
        }
    }
}
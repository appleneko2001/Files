﻿using Material.Icons;

namespace Files.Views.Models.Breadcrumb
{
    public class BreadcrumbNodeWithIconViewModel : BreadcrumbNodeViewModel
    {
        private MaterialIconKind? _iconKind;
        public MaterialIconKind? IconKind
        {
            get => _iconKind;
            protected set
            {
                _iconKind = value;
                RaiseOnPropertyChanged();
            }
        }

        protected BreadcrumbNodeWithIconViewModel(BreadcrumbPathViewModel parent, int index) : base(parent, index)
        {
        }

        public BreadcrumbNodeWithIconViewModel(BreadcrumbPathViewModel parent, int index, string path, string header) : base(parent, index, path, header)
        {
        }
    }
}
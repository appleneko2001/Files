﻿using System;
using System.Threading;

namespace Files.Views.Models.Browser
{
    public class HomeBrowserContentViewModel : BrowserContentViewModelBase
    {
        protected HomeBrowserContentViewModel(BrowserWindowTabViewModel parent) : base(parent)
        {
        }

        public override void LoadContent(Uri uri, CancellationToken _cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override void RequestPreviews(CancellationToken _cancellationToken = default)
        {
            // TODO: ¯\_(ツ)_/¯
        }
    }
}
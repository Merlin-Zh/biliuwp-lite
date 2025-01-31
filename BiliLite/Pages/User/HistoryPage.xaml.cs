﻿using BiliLite.Helpers;
using BiliLite.Modules.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliLite.Pages.User
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HistoryPage : Page
    {
        HistoryVM  historyVM;
        public HistoryPage()
        {
            this.InitializeComponent();
            historyVM = new HistoryVM();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New && historyVM.Videos == null)
            {
                historyVM.Page = 1;
                await historyVM.LoadHistory();
            }
        }



        private void Video_ItemClick(object sender, ItemClickEventArgs e)
        {
            var data = e.ClickedItem as HistoryItemModel;
            MessageCenter.OpenNewWindow(this, new NavigationInfo()
            {
                icon = Symbol.Play,
                page = typeof(VideoDetailPage),
                title = data.title,
                parameters = data.aid
            });
        }

    }
}

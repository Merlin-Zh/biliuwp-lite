﻿using BiliLite.Controls;
using BiliLite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliLite
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NoTabMainPage : Page
    {
        public NoTabMainPage()
        {
            this.InitializeComponent();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            mode = SettingHelper.GetValue<int>(SettingHelper.UI.DISPLAY_MODE, 0);
            Window.Current.SetTitleBar(TitleBar);
            frame.Navigated += Frame_Navigated;
            MessageCenter.OpenNewWindowEvent += NavigationHelper_OpenNewWindowEvent;
            MessageCenter.ChangeTitleEvent += MessageCenter_ChangeTitleEvent;
            MessageCenter.ViewImageEvent += MessageCenter_ViewImageEvent;
            MessageCenter.MiniWindowEvent += MessageCenter_MiniWindowEvent;
            Window.Current.Content.PointerPressed += Content_PointerPressed;
        }
        private void MessageCenter_MiniWindowEvent(object sender, bool e)
        {
            if (e)
            {
                MiniWindowsTitleBar.Visibility = Visibility.Visible;
                Window.Current.SetTitleBar(MiniWindowsTitleBar);
            }
            else
            {
                MiniWindowsTitleBar.Visibility = Visibility.Collapsed;
                Window.Current.SetTitleBar(TitleBar);
            }
        }
        private void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var par = e.GetCurrentPoint(sender as Frame).Properties.PointerUpdateKind;
            if (par == Windows.UI.Input.PointerUpdateKind.XButton1Pressed || par == Windows.UI.Input.PointerUpdateKind.MiddleButtonPressed)
            { 
                //如果打开了图片浏览，则关闭图片浏览
                if (gridViewer.Visibility == Visibility.Visible)
                {
                    imgViewer_CloseEvent(this, null);
                    e.Handled = true;
                    return;
                }
                //处理多标签
                if (this.frame.CanGoBack)
                {
                    this.frame.GoBack();
                    e.Handled = true;
                }

            }
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if(e.Content is Pages.HomePage)
            {
                txtTitle.Text = "哔哩哔哩 UWP";
            }
            if (frame.CanGoBack)
            {
                btnBack.Visibility = Visibility.Visible;
            }
            else
            {
                btnBack.Visibility = Visibility.Collapsed;
            }
        }
        private int mode = 1;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
           
            frame.Navigate(typeof(Pages.HomePage));
            if (e.NavigationMode == NavigationMode.New && e.Parameter != null && !string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                var result = await MessageCenter.HandelUrl(e.Parameter.ToString());
                if (!result)
                {
                    Utils.ShowMessageToast("无法打开链接:" + e.Parameter.ToString());
                }
            }
#if !DEBUG
            await Utils.CheckVersion();
#endif
        }

        private void MessageCenter_ChangeTitleEvent(object sender, string e)
        {
            if (mode == 1)
            {
                txtTitle.Text = e;
            }
        }

        private void NavigationHelper_OpenNewWindowEvent(object sender, NavigationInfo e)
        {
            if (mode==1)
            {
                txtTitle.Text = e.title;
                frame.Navigate(e.page, e.parameters);
            }
            else
            {
               OpenNewWindow(e);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
        }

        private async void OpenNewWindow(NavigationInfo e)
        {

            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
           await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var res=App.Current.Resources;
                Frame frame = new Frame();
                frame.Navigate(e.page, e.parameters);
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
                ApplicationView.GetForCurrentView().Consolidated += (sender, args) =>
                {
                    frame.Navigate(typeof(Pages.BlankPage));
                    CoreWindow.GetForCurrentThread().Close();
                };
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }
        private void MessageCenter_ViewImageEvent(object sender, ImageViewerParameter e)
        {
            gridViewer.Visibility = Visibility.Visible;
            imgViewer.InitImage(e);
        }
        private void imgViewer_CloseEvent(object sender, EventArgs e)
        {
            if (gridViewer.Visibility == Visibility.Visible)
            {
                imgViewer.ClearImage();
                gridViewer.Visibility = Visibility.Collapsed;
            }
        }
    }
}

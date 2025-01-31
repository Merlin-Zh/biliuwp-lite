﻿using BiliLite.Helpers;
using BiliLite.Modules;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliLite.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        SettingVM settingVM;
        public SettingPage()
        {
            this.InitializeComponent();
            settingVM = new SettingVM();
            LoadUI();
            LoadPlayer();
            LoadDanmu();
            LoadLiveDanmu();
            LoadDownlaod();
        }
        private void LoadUI()
        {
            //主题
            cbTheme.SelectedIndex = SettingHelper.GetValue<int>(SettingHelper.UI.THEME, 0);
            cbTheme.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbTheme.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.THEME, cbTheme.SelectedIndex);
                    Frame rootFrame = Window.Current.Content as Frame;
                    switch (cbTheme.SelectedIndex)
                    {
                        case 1:
                            rootFrame.RequestedTheme = ElementTheme.Light;
                            break;
                        case 2:
                            rootFrame.RequestedTheme = ElementTheme.Dark;
                            break;
                        default:
                            rootFrame.RequestedTheme = ElementTheme.Default;
                            break;
                    }
                    App.ExtendAcrylicIntoTitleBar();
                });
            });

            cbColor.SelectedIndex = SettingHelper.GetValue<int>(SettingHelper.UI.THEME_COLOR, 0);
            cbColor.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbColor.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.THEME_COLOR, cbColor.SelectedIndex);
                    Color color = new Color();
                    if (cbColor.SelectedIndex == 0)
                    {
                        var uiSettings = new Windows.UI.ViewManagement.UISettings();
                        color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                    }
                    else
                    {
                        color = Utils.ToColor((cbColor.SelectedItem as AppThemeColor).color);

                    }
                    (Application.Current.Resources["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
                    (Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
                    //(App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemAccentColor"] = Utils.ToColor(item.color);

                });
            });


            //显示模式
            cbDisplayMode.SelectedIndex = SettingHelper.GetValue<int>(SettingHelper.UI.DISPLAY_MODE, 0);
            cbDisplayMode.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbDisplayMode.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.DISPLAY_MODE, cbDisplayMode.SelectedIndex);
                    if (cbDisplayMode.SelectedIndex==2)
                    {
                        Utils.ShowMessageToast("多窗口模式正在开发测试阶段，可能会有一堆问题");
                    }
                    else
                    {
                        Utils.ShowMessageToast("重启生效");
                    }
                    
                });
            });
            //加载原图
            swPictureQuality.IsOn = SettingHelper.GetValue<bool>(SettingHelper.UI.ORTGINAL_IMAGE, false);
            swPictureQuality.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swPictureQuality.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.ORTGINAL_IMAGE, swPictureQuality.IsOn);
                    SettingHelper.UI._loadOriginalImage = null;
                });
            });
            //缓存页面
            swHomeCache.IsOn = SettingHelper.GetValue<bool>(SettingHelper.UI.CACHE_HOME, true);
            swHomeCache.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swHomeCache.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.CACHE_HOME, swHomeCache.IsOn);

                });
            });

            //右侧详情宽度
            numRightWidth.Value = SettingHelper.GetValue<double>(SettingHelper.UI.RIGHT_DETAIL_WIDTH, 320);
            numRightWidth.Loaded += new RoutedEventHandler((sender, e) =>
            {

                numRightWidth.ValueChanged += new TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs>((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.RIGHT_DETAIL_WIDTH, args.NewValue);
                });
            });
            //图片圆角半径
            numImageCornerRadius.Value = SettingHelper.GetValue<double>(SettingHelper.UI.IMAGE_CORNER_RADIUS, 0);
            ImageCornerRadiusExample.CornerRadius = new CornerRadius(numImageCornerRadius.Value);
            numImageCornerRadius.Loaded += new RoutedEventHandler((sender, e) =>
            {
                numImageCornerRadius.ValueChanged += new TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs>((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.IMAGE_CORNER_RADIUS, args.NewValue);
                    ImageCornerRadiusExample.CornerRadius = new CornerRadius(args.NewValue);
                    App.Current.Resources["ImageCornerRadius"] = new CornerRadius(args.NewValue);
                });
            });

            //显示视频封面
            swVideoDetailShowCover.IsOn = SettingHelper.GetValue<bool>(SettingHelper.UI.SHOW_DETAIL_COVER, true);
            swVideoDetailShowCover.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swVideoDetailShowCover.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.SHOW_DETAIL_COVER, swVideoDetailShowCover.IsOn);
                });
            });

            //新窗口浏览图片
            swPreviewImageOpenNewWindow.IsOn = SettingHelper.GetValue<bool>(SettingHelper.UI.NEW_WINDOW_PREVIEW_IMAGE, false);
            swPreviewImageOpenNewWindow.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swPreviewImageOpenNewWindow.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.NEW_WINDOW_PREVIEW_IMAGE, swPreviewImageOpenNewWindow.IsOn);
                });
            });
            //鼠标侧键返回
            swMouseClosePage.IsOn = SettingHelper.GetValue<bool>(SettingHelper.UI.MOUSE_BACK, true);
            swMouseClosePage.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swMouseClosePage.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.MOUSE_BACK, swMouseClosePage.IsOn);
                });
            });
            //隐藏赞助图标
            swHideSponsor.IsOn = SettingHelper.GetValue<bool>(SettingHelper.UI.HIDE_SPONSOR, false);
            swHideSponsor.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swHideSponsor.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.HIDE_SPONSOR, swHideSponsor.IsOn);
                });
            });
            //动态显示
            cbDetailDisplay.SelectedIndex = SettingHelper.GetValue<int>(SettingHelper.UI.DETAIL_DISPLAY, 0);
            cbDetailDisplay.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbDetailDisplay.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.DETAIL_DISPLAY, cbDetailDisplay.SelectedIndex);
                });
            });

            //动态显示
            cbDynamicDisplayMode.SelectedIndex = SettingHelper.GetValue<int>(SettingHelper.UI.DYNAMIC_DISPLAY_MODE, 0);
            cbDynamicDisplayMode.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbDynamicDisplayMode.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.DYNAMIC_DISPLAY_MODE, cbDynamicDisplayMode.SelectedIndex);
                });
            });

            //推荐显示
            cbRecommendDisplayMode.SelectedIndex = SettingHelper.GetValue<int>(SettingHelper.UI.RECMEND_DISPLAY_MODE, 0);
            cbRecommendDisplayMode.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbRecommendDisplayMode.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.UI.RECMEND_DISPLAY_MODE, cbRecommendDisplayMode.SelectedIndex);
                });
            });

            gridHomeCustom.ItemsSource = SettingHelper.GetValue<ObservableCollection<HomeNavItem>>(SettingHelper.UI.HOEM_ORDER, HomeVM.GetAllNavItems());
            ExceptHomeNavItems();



        }
        private void LoadPlayer()
        {
            //播放类型
            cbVideoType.SelectedIndex = SettingHelper.GetValue<int>(SettingHelper.Player.DEFAULT_VIDEO_TYPE, 1);
            cbVideoType.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbVideoType.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.DEFAULT_VIDEO_TYPE, cbVideoType.SelectedIndex);
                });
            });
            //视频倍速
            cbVideoSpeed.SelectedIndex = SettingHelper.Player.VideoSpeed.IndexOf(SettingHelper.GetValue<double>(SettingHelper.Player.DEFAULT_VIDEO_SPEED, 1.0d));
            cbVideoSpeed.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbVideoSpeed.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.DEFAULT_VIDEO_SPEED, SettingHelper.Player.VideoSpeed[cbVideoSpeed.SelectedIndex]);
                });
            });

            //硬解视频
            swHardwareDecode.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Player.HARDWARE_DECODING, true);
            swHardwareDecode.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swHardwareDecode.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.HARDWARE_DECODING, swHardwareDecode.IsOn);
                });
            });
            //自动播放
            swAutoPlay.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Player.AUTO_PLAY, false);
            swAutoPlay.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swAutoPlay.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.AUTO_PLAY, swAutoPlay.IsOn);
                });
            });

            //使用其他网站
            swPlayerSettingUseOtherSite.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Player.USE_OTHER_SITEVIDEO, false);
            swPlayerSettingUseOtherSite.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swPlayerSettingUseOtherSite.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.USE_OTHER_SITEVIDEO, swPlayerSettingUseOtherSite.IsOn);
                });
            });

            //自动跳转进度
            swPlayerSettingAutoToPosition.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Player.AUTO_TO_POSITION, true);
            swPlayerSettingAutoToPosition.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swPlayerSettingAutoToPosition.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.AUTO_TO_POSITION, swPlayerSettingAutoToPosition.IsOn);
                });
            });
            //自动铺满屏幕
            swPlayerSettingAutoFullWindows.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Player.AUTO_FULL_WINDOW, false);
            swPlayerSettingAutoFullWindows.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swPlayerSettingAutoFullWindows.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.AUTO_FULL_WINDOW, swPlayerSettingAutoFullWindows.IsOn);
                });
            });
            //自动全屏
            swPlayerSettingAutoFullScreen.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Player.AUTO_FULL_SCREEN, false);
            swPlayerSettingAutoFullScreen.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swPlayerSettingAutoFullScreen.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.AUTO_FULL_SCREEN, swPlayerSettingAutoFullScreen.IsOn);
                });
            });
           

            //双击全屏
            swPlayerSettingDoubleClickFullScreen.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Player.DOUBLE_CLICK_FULL_SCREEN, false);
            swPlayerSettingDoubleClickFullScreen.Loaded += new RoutedEventHandler((sender, e) =>
            {
                swPlayerSettingDoubleClickFullScreen.Toggled += new RoutedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Player.DOUBLE_CLICK_FULL_SCREEN, swPlayerSettingDoubleClickFullScreen.IsOn);
                });
            });
        }

        private void LoadDanmu()
        {
            //弹幕开关
            var state = SettingHelper.GetValue<Visibility>(SettingHelper.VideoDanmaku.SHOW, Visibility.Visible) == Visibility.Visible;
            DanmuSettingState.IsOn = state;
            DanmuSettingState.Toggled += new RoutedEventHandler((e, args) =>
            {
                SettingHelper.SetValue(SettingHelper.VideoDanmaku.SHOW, DanmuSettingState.IsOn ? Visibility.Visible : Visibility.Collapsed);
            });
            //弹幕关键词
            DanmuSettingListWords.ItemsSource = settingVM.ShieldWords;

            //正则关键词
            DanmuSettingListRegulars.ItemsSource = settingVM.ShieldRegulars;

            //用户
            DanmuSettingListUsers.ItemsSource = settingVM.ShieldUsers;

            //弹幕顶部距离
            numDanmakuTopMargin.Value = SettingHelper.GetValue<double>(SettingHelper.VideoDanmaku.TOP_MARGIN, 0);
            numDanmakuTopMargin.Loaded += new RoutedEventHandler((sender, e) =>
            {
                numDanmakuTopMargin.ValueChanged += new TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs>((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.VideoDanmaku.TOP_MARGIN, args.NewValue);
                });
            });
        }
        private void LoadLiveDanmu()
        {
            //弹幕开关
            var state = SettingHelper.GetValue<Visibility>(SettingHelper.Live.SHOW, Visibility.Visible) == Visibility.Visible;
            LiveDanmuSettingState.IsOn = state;
            LiveDanmuSettingState.Toggled += new RoutedEventHandler((e, args) =>
            {
                SettingHelper.SetValue(SettingHelper.Live.SHOW, LiveDanmuSettingState.IsOn ? Visibility.Visible : Visibility.Collapsed);
            });
            //弹幕关键词
            LiveDanmuSettingListWords.ItemsSource = settingVM.LiveWords;
        }
        private void LoadDownlaod()
        {
            //下载路径
            txtDownloadPath.Text = SettingHelper.GetValue(SettingHelper.Download.DOWNLOAD_PATH, SettingHelper.Download.DEFAULT_PATH);
            DownloadOpenPath.Click += new RoutedEventHandler(async (e, args) =>
            {
                if (txtDownloadPath.Text == SettingHelper.Download.DEFAULT_PATH)
                {
                    var videosLibrary = Windows.Storage.KnownFolders.VideosLibrary;
                    videosLibrary = await videosLibrary.CreateFolderAsync("哔哩哔哩下载", CreationCollisionOption.OpenIfExists);

                    await Windows.System.Launcher.LaunchFolderAsync(videosLibrary);
                }
                else
                {
                    await Windows.System.Launcher.LaunchFolderPathAsync(txtDownloadPath.Text);
                }
            });
            DownloadChangePath.Click += new RoutedEventHandler(async (e, args) =>
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.FileTypeFilter.Add("*");
                folderPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                var folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    SettingHelper.SetValue(SettingHelper.Download.DOWNLOAD_PATH, folder.Path);
                    txtDownloadPath.Text = folder.Path;
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(folder);
                    DownloadVM.Instance.RefreshDownloaded();
                }
            });
            //旧版下载目录
            txtDownloadOldPath.Text = SettingHelper.GetValue(SettingHelper.Download.OLD_DOWNLOAD_PATH, SettingHelper.Download.DEFAULT_OLD_PATH);
            DownloadOpenOldPath.Click += new RoutedEventHandler(async (e, args) =>
            {
                if (txtDownloadOldPath.Text == SettingHelper.Download.DEFAULT_OLD_PATH)
                {
                    var videosLibrary = Windows.Storage.KnownFolders.VideosLibrary;
                    videosLibrary = await videosLibrary.CreateFolderAsync("BiliBiliDownload", CreationCollisionOption.OpenIfExists);
                    await Windows.System.Launcher.LaunchFolderAsync(videosLibrary);
                }
                else
                {
                    await Windows.System.Launcher.LaunchFolderPathAsync(txtDownloadOldPath.Text);
                }
            });
            DownloadChangeOldPath.Click += new RoutedEventHandler(async (e, args) =>
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.FileTypeFilter.Add("*");
                folderPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                var folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    SettingHelper.SetValue(SettingHelper.Download.OLD_DOWNLOAD_PATH, folder.Path);
                    txtDownloadOldPath.Text = folder.Path;
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(folder);
                }
            });

            //并行下载
            swDownloadParallelDownload.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Download.PARALLEL_DOWNLOAD, true);
            swDownloadParallelDownload.Toggled += new RoutedEventHandler((e, args) =>
            {
                SettingHelper.SetValue(SettingHelper.Download.PARALLEL_DOWNLOAD, swDownloadParallelDownload.IsOn);
                DownloadVM.Instance.UpdateSetting();
            });
            //付费网络下载
            swDownloadAllowCostNetwork.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Download.ALLOW_COST_NETWORK, false);
            swDownloadAllowCostNetwork.Toggled += new RoutedEventHandler((e, args) =>
            {
                SettingHelper.SetValue(SettingHelper.Download.ALLOW_COST_NETWORK, swDownloadAllowCostNetwork.IsOn);
                DownloadVM.Instance.UpdateSetting();
            });
            //下载完成发送通知
            swDownloadSendToast.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Download.SEND_TOAST, false);
            swDownloadSendToast.Toggled += new RoutedEventHandler((e, args) =>
            {
                SettingHelper.SetValue(SettingHelper.Download.SEND_TOAST, swDownloadSendToast.IsOn);
            });
            //下载类型
            cbDownloadVideoType.SelectedIndex = SettingHelper.GetValue<int>(SettingHelper.Download.DEFAULT_VIDEO_TYPE, 1);
            cbDownloadVideoType.Loaded += new RoutedEventHandler((sender, e) =>
            {
                cbDownloadVideoType.SelectionChanged += new SelectionChangedEventHandler((obj, args) =>
                {
                    SettingHelper.SetValue(SettingHelper.Download.DEFAULT_VIDEO_TYPE, cbDownloadVideoType.SelectedIndex);
                });
            });
            //加载旧版本下载的视频
            swDownloadLoadOld.IsOn = SettingHelper.GetValue<bool>(SettingHelper.Download.LOAD_OLD_DOWNLOAD, false);
            swDownloadLoadOld.Toggled += new RoutedEventHandler((e, args) =>
            {
                SettingHelper.SetValue(SettingHelper.Download.LOAD_OLD_DOWNLOAD, swDownloadLoadOld.IsOn);
            });
        }

        private void ExceptHomeNavItems()
        {
            List<HomeNavItem> list = new List<HomeNavItem>();
            var all = HomeVM.GetAllNavItems();
            foreach (var item in all)
            {
                if ((gridHomeCustom.ItemsSource as ObservableCollection<HomeNavItem>).FirstOrDefault(x => x.Title == item.Title) == null)
                {
                    list.Add(item);
                }
            }
            gridHomeNavItem.ItemsSource = list;
        }
        private void gridHomeCustom_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            SettingHelper.SetValue(SettingHelper.UI.HOEM_ORDER, gridHomeCustom.ItemsSource as ObservableCollection<HomeNavItem>);
            Utils.ShowMessageToast("更改成功,重启生效");
        }

        private void gridHomeNavItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as HomeNavItem;
            (gridHomeCustom.ItemsSource as ObservableCollection<HomeNavItem>).Add(item);
            SettingHelper.SetValue(SettingHelper.UI.HOEM_ORDER, gridHomeCustom.ItemsSource as ObservableCollection<HomeNavItem>);
            ExceptHomeNavItems();
            Utils.ShowMessageToast("更改成功,重启生效");
        }

        private void menuRemoveHomeItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem).DataContext as HomeNavItem;
            if (gridHomeCustom.Items.Count == 1)
            {
                Utils.ShowMessageToast("至少要留一个页面");
                return;
            }
           (gridHomeCustom.ItemsSource as ObservableCollection<HomeNavItem>).Remove(item);
            SettingHelper.SetValue(SettingHelper.UI.HOEM_ORDER, gridHomeCustom.ItemsSource as ObservableCollection<HomeNavItem>);
            ExceptHomeNavItems();
            Utils.ShowMessageToast("更改成功,重启生效");
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                version.Text = $"版本 {SystemInformation.ApplicationVersion.Major}.{SystemInformation.ApplicationVersion.Minor}.{SystemInformation.ApplicationVersion.Build}.{SystemInformation.ApplicationVersion.Revision}";
                txtHelp.Text = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Text/help.md")));
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async void DanmuSettingAddWord_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DanmuSettingTxtWord.Text))
            {
                Utils.ShowMessageToast("关键词不能为空");
                return;
            }
            settingVM.ShieldWords.Add(DanmuSettingTxtWord.Text);
            SettingHelper.SetValue(SettingHelper.VideoDanmaku.SHIELD_WORD, settingVM.ShieldWords);
            var result = await settingVM.AddDanmuFilterItem(DanmuSettingTxtWord.Text, 0);
            DanmuSettingTxtWord.Text = "";
            if (!result)
            {
                Utils.ShowMessageToast("已经添加到本地，但远程同步失败");
            }
        }

        private async void DanmuSettingSyncWords_Click(object sender, RoutedEventArgs e)
        {
            await settingVM.SyncDanmuFilter();
        }

        private void RemoveDanmuWord_Click(object sender, RoutedEventArgs e)
        {
            var word = (sender as AppBarButton).DataContext as string;
            settingVM.ShieldWords.Remove(word);
            SettingHelper.SetValue(SettingHelper.VideoDanmaku.SHIELD_WORD, settingVM.ShieldWords);
        }

        private void RemoveDanmuRegular_Click(object sender, RoutedEventArgs e)
        {
            var word = (sender as AppBarButton).DataContext as string;
            settingVM.ShieldRegulars.Remove(word);
            SettingHelper.SetValue(SettingHelper.VideoDanmaku.SHIELD_REGULAR, settingVM.ShieldRegulars);
        }

        private void RemoveDanmuUser_Click(object sender, RoutedEventArgs e)
        {
            var word = (sender as AppBarButton).DataContext as string;
            settingVM.ShieldUsers.Remove(word);
            SettingHelper.SetValue(SettingHelper.VideoDanmaku.SHIELD_USER, settingVM.ShieldUsers);
        }

        private async void DanmuSettingAddRegex_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DanmuSettingTxtRegex.Text))
            {
                Utils.ShowMessageToast("正则表达式不能为空");
                return;
            }
            var txt = DanmuSettingTxtRegex.Text.Trim('/');
            settingVM.ShieldRegulars.Add(txt);
            SettingHelper.SetValue(SettingHelper.VideoDanmaku.SHIELD_REGULAR, settingVM.ShieldRegulars);
            var result = await settingVM.AddDanmuFilterItem(txt, 1);
            DanmuSettingTxtRegex.Text = "";
            if (!result)
            {
                Utils.ShowMessageToast("已经添加到本地，但远程同步失败");
            }
        }

        private async void DanmuSettingAddUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DanmuSettingTxtUser.Text))
            {
                Utils.ShowMessageToast("用户ID不能为空");
                return;
            }
            settingVM.ShieldUsers.Add(DanmuSettingTxtUser.Text);
            SettingHelper.SetValue(SettingHelper.VideoDanmaku.SHIELD_WORD, settingVM.ShieldUsers);
            var result = await settingVM.AddDanmuFilterItem(DanmuSettingTxtUser.Text, 2);
            DanmuSettingTxtUser.Text = "";
            if (!result)
            {
                Utils.ShowMessageToast("已经添加到本地，但远程同步失败");
            }
        }


        private async void txtHelp_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            if (e.Link == "OpenLog")
            {
                var path = Windows.Storage.ApplicationData.Current.LocalFolder.Path + @"\log\";
                await Windows.System.Launcher.LaunchFolderPathAsync(path);
            }
            else
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
            }

        }

        private void LiveDanmuSettingAddWord_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LiveDanmuSettingTxtWord.Text))
            {
                Utils.ShowMessageToast("关键字不能为空");
                return;
            }
            if (!settingVM.LiveWords.Contains(LiveDanmuSettingTxtWord.Text))
            {
                settingVM.LiveWords.Add(LiveDanmuSettingTxtWord.Text);
                SettingHelper.SetValue(SettingHelper.Live.SHIELD_WORD, settingVM.LiveWords);
            }

            DanmuSettingTxtWord.Text = "";
            SettingHelper.SetValue(SettingHelper.Live.SHIELD_WORD, settingVM.LiveWords);
        }

        private void RemoveLiveDanmuWord_Click(object sender, RoutedEventArgs e)
        {
            var word = (sender as AppBarButton).DataContext as string;
            settingVM.LiveWords.Remove(word);
            SettingHelper.SetValue(SettingHelper.Live.SHIELD_WORD, settingVM.LiveWords);
        }

        private async void btnCleanImageCache_Click(object sender, RoutedEventArgs e)
        {
           await ImageCache.Instance.ClearAsync();
            Utils.ShowMessageToast("已清除图片缓存");
        }
    }
}

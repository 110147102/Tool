using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using WinRT.Interop;
using WinUI.Pages;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinUI.Method;
using ImageMagick;

namespace WinUI.Method
{
    class Format
    {
        public async void Select_Window(HomePage homePage)
        {
            try
            {
                var picker = new FileOpenPicker();
                nint hwnd = WindowNative.GetWindowHandle(App.MainWindow);
                InitializeWithWindow.Initialize(picker, hwnd);
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                foreach (var format in homePage.formats)
                {
                    picker.FileTypeFilter.Add(format);
                }

                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    homePage._selectedFile = file;
                    homePage.GetFileText().Text = file.Name;
                    homePage.GetFilePathText().Text = file.Path;

                    using (var stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream);
                        homePage.GetPreviewImage().Source = bitmap;
                        homePage.GetPlaceholderText().Visibility = Visibility.Collapsed;
                        homePage.GetPreviewBorder().Visibility = Visibility.Visible;
                    }

                    homePage.GetConvertButton().IsEnabled = true;
                    homePage.GetStatusText().Text = "已选择文件，点击转换";
                }
                else
                {
                    homePage.GetFileText().Text = "未选择文件";
                    homePage.GetFilePathText().Text = "";
                    homePage.GetPreviewBorder().Visibility = Visibility.Collapsed;
                    homePage.GetConvertButton().IsEnabled = false;
                    homePage.GetStatusText().Text = "就绪";
                }
            }
            catch (Exception ex)
            {
                homePage.GetFileText().Text = $"错误: {ex.Message}";
                homePage.GetStatusText().Text = $"错误: {ex.Message}";
            }
        }

        public static void Format_Conversion(HomePage homePage, string sourcePath, string destinationPath, string targetFormat, int quality = 85)
        {
            using (var image = new MagickImage(sourcePath))
            {
                if (targetFormat.ToLower() == "jpg" || targetFormat.ToLower() == "jpeg")
                {
                    image.ColorAlpha(MagickColors.White);
                }

                image.Format = targetFormat.ToLower() switch
                {
                    "jpg" or "jpeg" => MagickFormat.Jpeg,
                    "png" => MagickFormat.Png,
                    "webp" => MagickFormat.WebP,
                    "bmp" => MagickFormat.Bmp,
                    "gif" => MagickFormat.Gif,
                    _ => throw new NotSupportedException($"格式 {targetFormat} 不支持")
                };

                if (image.Format == MagickFormat.Jpeg)
                {
                    image.Quality = (uint)quality;
                }

                image.Write(destinationPath);
            }
        }

        public async Task<StorageFolder> SelectSaveFolder()
        {
            try
            {
                var picker = new FolderPicker();
                nint hwnd = WindowNative.GetWindowHandle(App.MainWindow);
                InitializeWithWindow.Initialize(picker, hwnd);
                picker.ViewMode = PickerViewMode.List;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add("*");

                var folder = await picker.PickSingleFolderAsync();
                return folder;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"选择目录失败: {ex.Message}");
                return null;
            }
        }
    }
}

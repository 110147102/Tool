using ImageMagick;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using WinUI.Method;

namespace WinUI.Pages
{
    public sealed partial class HomePage : Page
    {
        public StorageFile _selectedFile;
        private string SelectedFormat = "jpg";
        public Dictionary<string, object> Controls { get; } = new Dictionary<string, object>();
        public HomePage()
        {
            this.InitializeComponent();
            ResourceLimits.LimitMemory(new Percentage(10));
            Controls["PlaceholderText"] = PlaceholderText;
            Controls["PreviewBorder"] = PreviewBorder;
            Controls["PreviewImage"] = PreviewImage;
            Controls["SelectButton"] = SelectButton;
            Controls["FileText"] = FileText;
            Controls["FilePathText"] = FilePathText;
            Controls["ConvertButton"] = ConvertButton;
            Controls["ProgressBar"] = ProgressBar;
            Controls["StatusText"] = StatusText;
        }

        public TextBlock GetPlaceholderText() => PlaceholderText;
        public Border GetPreviewBorder() => PreviewBorder;
        public Image GetPreviewImage() => PreviewImage;
        public Button GetSelectButton() => SelectButton;
        public TextBlock GetFileText() => FileText;
        public TextBlock GetFilePathText() => FilePathText;
        public Button GetConvertButton() => ConvertButton;
        public ProgressBar GetProgressBar() => ProgressBar;
        public TextBlock GetStatusText() => StatusText;
        public string GetSelectedFormat() => SelectedFormat;

        public List<string> formats = new List<string> {".png",
                                                 ".jpg",
                                                 ".jpeg",
                                                 ".bmp",
                                                 ".webp",
                                                 ".gif" };
        Format format = new Format();
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            format.Select_Window(this);
        }
        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile == null)
            {
                StatusText.Text = "选择一张图片";
                return;
            }
            FormatJPG.IsChecked = true;
            ResourceLimits.LimitMemory(new Percentage(10));
            try
            {
                Format formatHelper = new Format();
                StorageFolder saveFolder = await formatHelper.SelectSaveFolder();

                if (saveFolder == null)
                {
                    StatusText.Text = "已取消保存";
                    return;
                }

                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(_selectedFile.Name);
                string destFileName = $"{fileNameWithoutExt}_converted.{SelectedFormat}";

                int counter = 1;
                while (await saveFolder.GetFileAsync(destFileName).AsTask().ContinueWith(t => t.IsCompletedSuccessfully))
                {
                    destFileName = $"{fileNameWithoutExt}_converted_{counter}.{SelectedFormat}";
                    counter++;
                }

                string sourcePath = _selectedFile.Path;
                string destPath = Path.Combine(saveFolder.Path, destFileName);

                ConvertButton.IsEnabled = false;
                SelectButton.IsEnabled = false;
                ProgressBar.Visibility = Visibility.Visible;
                ProgressBar.Value = 0;
                StatusText.Text = $"正在转换为 {SelectedFormat.ToUpper()}...";
                await Task.Run(() => Format.Format_Conversion(this, sourcePath, destPath, GetSelectedFormat(), 85));
                ProgressBar.Value = 100;
                StatusText.Text = $"转换成功！";
                FilePathText.Text = destPath;
                FilePathText.Opacity = 0.6;
                ConvertButton.IsEnabled = true;
                SelectButton.IsEnabled = true;
                ProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"转换失败: {ex.Message}";
                ConvertButton.IsEnabled = true;
                SelectButton.IsEnabled = true;
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void FormatRadio_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio == null) return;

            string displayName = "";

            if (radio == FormatJPG)
            {
                SelectedFormat = "jpg";
                displayName = "JPG / JPEG";
            }
            else if (radio == FormatPNG)
            {
                SelectedFormat = "png";
                displayName = "PNG";
            }
            else if (radio == FormatWebP)
            {
                SelectedFormat = "webp";
                displayName = "WebP";
            }
            else if (radio == FormatBMP)
            {
                SelectedFormat = "bmp";
                displayName = "BMP";
            }
            else if (radio == FormatGIF)
            {
                SelectedFormat = "gif";
                displayName = "GIF";
            }
            else if (radio == FormatTIFF)
            {
                SelectedFormat = "tiff";
                displayName = "TIFF";
            }

            if (StatusText != null)
            {
                StatusText.Text = $"当前格式: {SelectedFormat.ToUpper()}";
            }
            FormatExpander.Header = $"选择输出格式: {displayName}";
        }
    }
}

namespace MonthlyMileageCropper
{
    using System.IO;
    using System.Windows;

    using SkiaSharp;

    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);

            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        protected override void OnPreviewDrop(DragEventArgs e)
        {
            base.OnPreviewDrop(e);

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filePaths == null)
            {
                return;
            }

            foreach (var filePath in filePaths)
            {
                try
                {
                    Crop(filePath);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"{exception}");
                }
            }
        }

        private static void Crop(string readFilePath)
        {
            if (string.IsNullOrWhiteSpace(readFilePath) || Path.GetExtension(readFilePath) != ".png")
            {
                return;
            }

            var writeFilePath = Path.Combine(
                Path.GetDirectoryName(readFilePath) ?? string.Empty,
                string.Format(
                    "{0}_Cropped{1}",
                    Path.GetFileNameWithoutExtension(readFilePath),
                    Path.GetExtension(readFilePath)));

            using (var source = SKBitmap.Decode(readFilePath))
            {
                var cropRect = new SKRectI(0, 180, source.Width, source.Height - 1000);

                using (var destination = new SKBitmap(cropRect.Width, cropRect.Height))
                {
                    source.ExtractSubset(destination, cropRect);

                    using (var stream = new SKFileWStream(writeFilePath))
                    {
                        destination.Encode(stream, SKEncodedImageFormat.Png, 100);
                    }
                }
            }
        }
    }
}

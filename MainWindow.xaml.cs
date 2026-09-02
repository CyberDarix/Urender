using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls.Primitives;
using ImageMagick;

namespace URender;

public partial class MainWindow : System.Windows.Window
{
    private readonly Dictionary<Border, string> _files = new();
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ChooseFiles_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Choisir les fichiers à convertir",
            Filter = "Images et vidéos|*.png;*.jpg;*.jpeg;*.webp;*.gif;*.bmp;*.tif;*.tiff;*.ico;*.mp4;*.webm;*.mov;*.avi|Tous les fichiers|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            foreach (var fileName in dialog.FileNames)
            {
                AddFileRow(fileName);
            }
            EmptyLogo.Visibility = Visibility.Collapsed;
        }
    }

    private void RemoveFile_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Border row)
        {
            FileRows.Children.Remove(row);
            _files.Remove(row);
            EmptyLogo.Visibility = FileRows.Children.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void Convert_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (FileRows.Children.Count == 0)
        {
            ChooseFiles_Click(sender, e);
            return;
        }

        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var errors = new List<string>();
        var converted = 0;
        foreach (var entry in _files)
        {
            try
            {
                FrameworkElement? picker = null;
                foreach (var child in ((Grid)entry.Key.Child).Children)
                {
                    if (child is UIElement element && Grid.GetColumn(element) == 3)
                    {
                        picker = element as FrameworkElement;
                        break;
                    }
                }
                var targetFormat = picker?.Tag as string ?? "PNG";
                ConvertFile(entry.Value, targetFormat, desktop);
                converted++;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException or InvalidOperationException)
            {
                errors.Add($"{Path.GetFileName(entry.Value)} : {ex.Message}");
            }
        }

        var result = $"{converted} fichier(s) enregistré(s) dans le Bureau.";
        if (errors.Count > 0)
        {
            result += Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, errors);
        }
        System.Windows.MessageBox.Show(result, "urender");
    }

    private void AddFileRow(string fileName)
    {
        var row = new Border
        {
            Height = 72,
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromRgb(236, 237, 239)),
            BorderThickness = new Thickness(0, 0, 0, 1)
        };
        var layout = new Grid { Margin = new Thickness(22, 0, 22, 0) };
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(48) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(54) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(72) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });

        var preview = new Border
        {
            Width = 32, Height = 32, CornerRadius = new CornerRadius(4),
            Background = new SolidColorBrush(Color.FromRgb(240, 233, 255)),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Child = CreatePreview(fileName)
        };
        Grid.SetColumn(preview, 0);
        layout.Children.Add(preview);
        var name = new TextBlock { Text = Path.GetFileName(fileName), Foreground = (Brush)FindResource("TextBrush"), FontSize = 13, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        Grid.SetColumn(name, 1);
        layout.Children.Add(name);
        var source = new TextBlock { Text = "en", Foreground = (Brush)FindResource("MutedBrush"), FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(source, 2);
        layout.Children.Add(source);
        var format = CreateFormatPicker("SVG");
        Grid.SetColumn(format, 3);
        layout.Children.Add(format);
        var ready = new Border { Width = 32, Height = 19, BorderBrush = new SolidColorBrush(Color.FromRgb(68, 199, 123)), BorderThickness = new Thickness(1), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Child = new TextBlock { Text = "PRÊT", Foreground = new SolidColorBrush(Color.FromRgb(54, 184, 107)), FontSize = 8, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } };
        Grid.SetColumn(ready, 4);
        layout.Children.Add(ready);
        var size = new TextBlock { Text = FormatFileSize(new FileInfo(fileName).Length), Foreground = (Brush)FindResource("MutedBrush"), FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(size, 5);
        layout.Children.Add(size);
        var remove = new Button { Content = "×", Tag = row, Foreground = new SolidColorBrush(Color.FromRgb(165, 167, 172)), FontSize = 27, Style = (System.Windows.Style)FindResource("FlatButton") };
        remove.Click += RemoveFile_Click;
        Grid.SetColumn(remove, 6);
        layout.Children.Add(remove);
        row.Child = layout;
        remove.Tag = row;
        FileRows.Children.Add(row);
        _files[row] = fileName;
    }

    private FrameworkElement CreateFormatPicker(string initialFormat)
    {
        var root = new Grid { Width = 78, Height = 34 };
        root.Tag = initialFormat;
        var selected = new TextBlock { Text = initialFormat, Foreground = new SolidColorBrush(Color.FromRgb(52, 54, 59)), FontSize = 12, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0) };
        var button = new Button { Background = Brushes.White, BorderBrush = new SolidColorBrush(Color.FromRgb(201, 203, 208)), BorderThickness = new Thickness(1), Content = selected, HorizontalContentAlignment = HorizontalAlignment.Left, Padding = new Thickness(0), Cursor = System.Windows.Input.Cursors.Hand };
        var arrow = new TextBlock { Text = "⌄", Foreground = new SolidColorBrush(Color.FromRgb(90, 92, 96)), FontSize = 12, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 8, 0), IsHitTestVisible = false };
        root.Children.Add(button);
        root.Children.Add(arrow);

        var popup = new Popup { PlacementTarget = root, Placement = PlacementMode.Bottom, StaysOpen = false, AllowsTransparency = true };
        var panel = new Border { Width = 220, Height = 274, Background = new SolidColorBrush(Color.FromRgb(29, 29, 30)), Padding = new Thickness(12), CornerRadius = new CornerRadius(2) };
        var formats = new WrapPanel { Width = 196 };
        var formatNames = new[]
        {
            "SVG", "ICO", "JPG", "WEBP", "JPEG", "CUR", "DDS", "GIF", "BMP",
            "TIFF", "PSD", "HDR", "TGA", "AVIF", "HEIC", "RGB", "JFIF", "EXR",
            "PGM", "RGBA", "PPM", "HEIF", "XPM", "PCX", "WBMP", "PICON", "JP2",
            "XBM", "MAP", "RAS", "JBG", "PDB", "SIXEL", "PBM", "PNM", "JPE",
            "JIF", "G3", "YUV", "PCT", "PGX", "RGF", "PICT", "PAL", "JPS",
            "PCD", "SUN", "SGI", "SIX", "MNG", "XV", "VIFF", "FAX", "OTB",
            "MTV", "FTS", "JBIG", "G4", "JFI", "UYVY", "IPL", "RGBO", "HRZ",
            "XWD", "VIPS", "PALM", "PAM", "PFM", "MP4", "WEBM", "MOV", "AVI"
        };
        foreach (var formatName in formatNames)
        {
            var formatButton = new Button { Content = formatName, Width = 45, Height = 30, Margin = new Thickness(3), Background = new SolidColorBrush(Color.FromRgb(57, 57, 59)), Foreground = Brushes.White, BorderThickness = new Thickness(0), FontSize = 11, Cursor = System.Windows.Input.Cursors.Hand };
            formatButton.Click += (_, _) => { selected.Text = formatName; root.Tag = formatName; popup.IsOpen = false; };
            formats.Children.Add(formatButton);
        }
        var formatScroll = new ScrollViewer
        {
            Width = 196,
            Height = 250,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = formats
        };
        panel.Child = formatScroll;
        popup.Child = panel;
        button.Click += (_, _) => popup.IsOpen = !popup.IsOpen;
        root.Children.Add(popup);
        return root;
    }

    private static void ConvertFile(string sourcePath, string targetFormat, string desktop)
    {
        var normalized = targetFormat.ToUpperInvariant();
        var sourceName = Path.GetFileNameWithoutExtension(sourcePath);
        var extension = normalized.ToLowerInvariant();
        var outputPath = Path.Combine(desktop, $"{sourceName}_converted.{extension}");

        using var image = new MagickImage(sourcePath);
        image.AutoOrient();
        image.Write(outputPath);
    }

    private static System.Windows.UIElement CreatePreview(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension is ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp")
        {
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(fileName);
                image.EndInit();
                return new Image { Source = image, Stretch = System.Windows.Media.Stretch.UniformToFill };
            }
            catch (IOException)
            {
            }
            catch (NotSupportedException)
            {
            }
        }

        return new TextBlock
        {
            Text = "◆",
            Foreground = new SolidColorBrush(Color.FromRgb(156, 107, 238)),
            FontSize = 12,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private static string FormatFileSize(long bytes)
    {
        return bytes < 1024
            ? $"{bytes} B"
            : bytes < 1024 * 1024
                ? $"{bytes / 1024.0:0.##} KB"
                : $"{bytes / (1024.0 * 1024.0):0.##} MB";
    }
}

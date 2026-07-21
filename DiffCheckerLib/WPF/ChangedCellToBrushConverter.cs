using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DiffCheckerLib.WPF
{
    /// <summary>
    /// 重要度セルがプロファイル適用時の基準値から変更されている場合に着色するコンバーター
    /// [0]=現在の重要度(日本語表記)、[1]=StbName(XMLパス) のMultiBindingで使用する
    /// </summary>
    public class ChangedCellToBrushConverter : IMultiValueConverter
    {
        /// <summary>
        /// StbName(XMLパス)→プロファイル適用時の重要度(日本語表記)
        /// </summary>
        public IReadOnlyDictionary<string, string> Baseline { get; set; } = new Dictionary<string, string>();

        public Brush HighlightBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xFF, 0xE0, 0x8C));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2
                && values[0] is string importance
                && values[1] is string stbName
                && Baseline.TryGetValue(stbName, out string baseValue)
                && baseValue != importance)
            {
                return HighlightBrush;
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

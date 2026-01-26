using System.Globalization;

namespace HRChatbot.Converters;

public class RoleToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string role)
        {
            return role == "user" ? Color.FromRgb(219, 234, 254) : Color.FromRgb(229, 231, 235);
        }
        return Color.FromRgb(229, 231, 235);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

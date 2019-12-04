using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace Sample
{
    public static class ResourcesHelper
    {
        public static Color GetResourceColor(string key)
        {
            if (Application.Current.Resources.TryGetValue(key, out var value))
            {
                return (Color)value;
            }

            throw new InvalidOperationException($"key {key} not found in the resource dictionary");
        }

        public static void SetDynamicResource(string targetResourceName, string sourceResourceName)
        {
            if (!Application.Current.Resources.TryGetValue(sourceResourceName, out var value))
            {
                throw new InvalidOperationException($"key {sourceResourceName} not found in the resource dictionary");
            }

            Application.Current.Resources[targetResourceName] = value;
        }

        public static void SetDarkGameMode()
        {
            ResourcesHelper.SetDynamicResource("GameListBackground", "DarkSurface");
            ResourcesHelper.SetDynamicResource("CellBackgroundColor", "DarkElevation4dp");
            ResourcesHelper.SetDynamicResource("CellTextColor", "TextWhitePrimaryColor");
            ResourcesHelper.SetDynamicResource("CellSecondaryTextColor", "TextSecondaryColor");
        }

        public static void SetWhiteCellMode()
        {
            ResourcesHelper.SetDynamicResource("GameListBackground", "DarkSurface");
            ResourcesHelper.SetDynamicResource("CellBackgroundColor", "TosWindows");
            ResourcesHelper.SetDynamicResource("CellTextColor", "TextPrimaryColor");
            ResourcesHelper.SetDynamicResource("CellSecondaryTextColor", "TextTernaryColor");
        }

        public static void SetTosCellMode()
        {
            ResourcesHelper.SetDynamicResource("GameListBackground", "TosBackground");
            ResourcesHelper.SetDynamicResource("CellBackgroundColor", "TosWindows");
            ResourcesHelper.SetDynamicResource("CellTextColor", "TextPrimaryColor");
            ResourcesHelper.SetDynamicResource("CellSecondaryTextColor", "TextTernaryColor");
        }
    }
}

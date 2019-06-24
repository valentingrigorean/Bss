using System.Collections.Generic;
using UIKit;
using System.Linq;
using Foundation;

// Analysis disable once CheckNamespace
namespace CoreAnimation
{
    public static class CaGradientLayerExtension
    {
        public static CAGradientLayer SetGradient(this CAGradientLayer layer, UIColor one, UIColor two)
        {
            SetGradient(layer, new [] { one, two }, new [] { 0.0f, 1.0f });
            return layer;
        }

        public static CAGradientLayer SetGradient(this CAGradientLayer layer, IDictionary<UIColor,float> settings)
        {
            SetGradient(layer, settings.Keys.ToArray(), settings.Values.ToArray());
            return layer;
        }

        public static CAGradientLayer SetGradient(this CAGradientLayer layer, UIColor[] colors, float[] locations)
        {
            var _colors = colors.Select(_ => _.CGColor).ToArray();
            var _locations = locations.Select(NSNumber.FromFloat).ToArray();
            layer.Colors = _colors;
            layer.Locations = _locations;
            return layer;
        }
    }
}


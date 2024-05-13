using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;
using System.Xml;
using System.Windows.Media;

namespace MouseTracking
{
    internal class ConfigLoader
    {
        public static ConfigSettings LoadConfigSettings(string xmlFilePath)
        {
            // Load XML document
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            // Read animation settings from XML
            XmlNode animationNode = doc.SelectSingleNode("/ConfigSettings");
            double moveX = Convert.ToDouble(animationNode.SelectSingleNode("MoveX").InnerText);
            double moveY = Convert.ToDouble(animationNode.SelectSingleNode("MoveY").InnerText);
            double from = Convert.ToDouble(animationNode.SelectSingleNode("FromSize").InnerText);
            double to = Convert.ToDouble(animationNode.SelectSingleNode("ToSize").InnerText);
            double durationSeconds = Convert.ToDouble(animationNode.SelectSingleNode("DurationSeconds").InnerText);
            string defaultColor = animationNode.SelectSingleNode("DefaultColor").InnerText;
            string leftMBClickColor = animationNode.SelectSingleNode("LMBClickColor").InnerText;
            string rightMBClickColor = animationNode.SelectSingleNode("RMBClickColor").InnerText;

            var configSettings = new ConfigSettings()
            {
                MoveX = moveX,
                MoveY = moveY,
                FromSize = from,
                ToSize = to,
                Duration = durationSeconds,
                DefaultColor = ConvertColor(defaultColor),
                LMBClickColor = ConvertColor(leftMBClickColor),
                RMBClickColor = ConvertColor(rightMBClickColor),
            };

            return configSettings;
        }

        public static Brush ConvertColor(string colorName)
        {
            Brush brush = Brushes.Black; // Default brush (in case of invalid color name)

            try
            {
                // Attempt to convert color name to SolidColorBrush using reflection
                brush = (SolidColorBrush)new BrushConverter().ConvertFromString(colorName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting color: {ex.Message}");
            }

            return brush;
        }
    }

    public class ConfigSettings
    {
        public double MoveX { get; set; }
        public double MoveY { get; set; }
        public double FromSize { get; set; }
        public double ToSize { get; set; }
        public double Duration { get; set; }
        public Brush DefaultColor { get;set; }
        public Brush LMBClickColor { get;set; }
        public Brush RMBClickColor { get;set; }
    }
}

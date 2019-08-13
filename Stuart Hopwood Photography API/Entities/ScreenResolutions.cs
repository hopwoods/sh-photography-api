using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Entities
{
    public static class ScreenResolutions
    {
        public static Dictionary<string, ImageDimensions> Resolutions { get; private set; }

        static ScreenResolutions()
        {
            Resolutions = new Dictionary<string, ImageDimensions>
            {
                {"1920x1080", new ImageDimensions {Width = 1920, Height = 1080}},
                {"1680x1050", new ImageDimensions {Width = 1680, Height = 1050}},
                {"1600x900", new ImageDimensions {Width = 1600, Height = 900}},
                {"1440x900", new ImageDimensions {Width = 1400, Height = 900}},
                {"1366x768", new ImageDimensions {Width = 1366, Height = 768}},
                {"1360x768", new ImageDimensions {Width = 1360, Height = 768}},
                {"1280x800", new ImageDimensions {Width = 1280, Height = 800}},
                {"1024x768", new ImageDimensions {Width = 1024, Height = 768}},
                {"768x1024", new ImageDimensions {Width = 768, Height = 1024}},
                {"720x1280", new ImageDimensions {Width = 720, Height = 1280}},
                {"480x800", new ImageDimensions {Width = 480, Height = 800}},
                {"360x640", new ImageDimensions {Width = 360, Height = 640}},
                {"320x568", new ImageDimensions {Width = 320, Height = 568}}
            };
        }
    }
}
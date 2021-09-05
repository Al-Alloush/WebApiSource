using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers.Images
{
    public class ImageTransformation
    {
        private int _height;
        public int Height
        {
            get => _height;
            set => _height = value;

        }

        private int _width;
        public int Width
        {
            get => _width;
            set => _width = value;

        }

        private string _crop = null;
        public string Crop
        {
            get => _crop;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _crop = value.ToLower().Trim();
                };
            }
        }


        private string _radius = null;
        public string Radius
        {
            get => _radius;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _radius = value.ToLower().Trim();
                };
            }
        }
    }
}

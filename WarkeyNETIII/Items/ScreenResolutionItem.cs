﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarkeyNETIII.ViewModels;

namespace WarkeyNETIII.Items
{
    public class ScreenResolutionItem : BaseViewModel
    {
        private int width;
        private int height;

        public int Width
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
                OnPropertyChanged();
            }
        }
    }
}

﻿using Xamarin.Forms;

namespace MovieArchive.Models
{
    public class AdBanner : View
    {
        public enum Sizes { Standardbanner, LargeBanner, MediumRectangle, FullBanner, Leaderboard, SmartBannerPortrait }
        public Sizes Size { get; set; }
        public AdBanner()
        {
            this.BackgroundColor = Color.Accent;
        }
    }
}

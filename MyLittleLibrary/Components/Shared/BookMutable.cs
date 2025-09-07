
using System.Text.RegularExpressions;

namespace MyLittleLibrary.Components.Shared
{
    public abstract class BookMutable
    {
        private string title = string.Empty;
        private string titleSlug = string.Empty;

        public string Title
        {
            get => title;
            set
            {
                title = value;
                // Only auto-generate slug if it hasn't been set manually
                if (string.IsNullOrEmpty(titleSlug))
                {
                    titleSlug = GenerateSlug(value);
                }
            }
        }

        public string TitleSlug
        {
            get => GenerateSlug(title);
            private set => titleSlug = value;
        }
        
        public string Author { get; set; } = string.Empty;
        public string Illustrator { get; set; } = string.Empty;
        public abstract object VolumeValue { get; }
        public string? ImagePath { get; set; }

        // For setting title without triggering slug generation (used for existing records)
        public void SetTitle(string title)
        {
            this.title = title;
        }

        // For setting slug directly (used for existing records)
        public void SetTitleSlug(string slug)
        {
            titleSlug = slug;
        }

        // Helper method to generate slug from title
        private string GenerateSlug(string title)
        {
            // Remove special characters and replace spaces with empty string
            return Regex.Replace(title, @"[^a-zA-Z0-9]", "");
        }
    }
}
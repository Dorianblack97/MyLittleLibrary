
using System.Text.RegularExpressions;

namespace MyLittleLibrary.Components.Shared
{
    public abstract class BookMutable
    {
        private string _title = string.Empty;
        private string _titleSlug = string.Empty;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                // Only auto-generate slug if it hasn't been set manually
                if (string.IsNullOrEmpty(_titleSlug))
                {
                    _titleSlug = GenerateSlug(value);
                }
            }
        }

        public string TitleSlug
        {
            get => GenerateSlug(_title);
            private set => _titleSlug = value;
        }
        
        public string Author { get; set; } = string.Empty;
        public string Illustrator { get; set; } = string.Empty;
        public abstract object VolumeValue { get; }
        public string? ImagePath { get; set; }

        // For setting title without triggering slug generation (used for existing records)
        public void SetTitle(string title)
        {
            _title = title;
        }

        // For setting slug directly (used for existing records)
        public void SetTitleSlug(string slug)
        {
            _titleSlug = slug;
        }

        // Helper method to generate slug from title
        private string GenerateSlug(string title)
        {
            // Remove special characters and replace spaces with empty string
            return Regex.Replace(title, @"[^a-zA-Z0-9]", "");
        }
    }
}
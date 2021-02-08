using System;
using System.Collections.Generic;

namespace Sample.Domain
{
    public class Company
    {
        public Company(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }

        public string Name { get; }
    }

    public class Genre
    {
        public Genre(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }

        public string Name { get; }
    }

    public class Game
    {
        public Game(
            long id,
            string coverUrl,
            string screenshotUrl,
            DateTime firstReleaseDate,
            List<Genre> genres,
            List<Company> companies,
            string name,
            double? rating,
            string summary,
            string storyLine)
        {
            Id = id;
            CoverUrl = coverUrl;
            ScreenshotUrl = screenshotUrl;
            FirstReleaseDate = firstReleaseDate;
            Genres = genres;
            Companies = companies;
            Name = name;
            Rating = rating;
            Storyline = storyLine;
            Summary = summary;
        }

        public string Storyline { get; set; }

        public string Summary { get; set; }

        public long Id { get; }

        public string CoverUrl { get; }

        public string ScreenshotUrl { get; }

        public DateTime FirstReleaseDate { get; }

        public string DisplayableFirstReleaseDate => $"{FirstReleaseDate:Y}";

        public int FirstReleaseYear => FirstReleaseDate.Year;

        public List<Genre> Genres { get; }

        public string MajorGenre => Genres.Count == 0 ? "Unknown Genre" : Genres[0].Name;

        public List<Company> Companies { get; }

        public string MajorCompany => Companies.Count == 0 ? "Unknown Company" : Companies[0].Name;

        public double? Rating { get; }

        public bool HasRating => Rating.HasValue;

        public string DisplayableRating => !HasRating ? null : $"{(int)Rating.Value}%";

        public string Name { get; }
    }
}

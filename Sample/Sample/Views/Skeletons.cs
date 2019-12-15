using System;
using System.Collections.Generic;

using Sample.Domain;

namespace Sample.Views
{
    public static class Skeletons
    {
        public static Game[] Games = new[]
            {
                new Game(
                    0,
                    null,
                    null,
                    DateTime.Now,
                    new List<Genre> { new Genre(1, "Genre genre") },
                    new List<Company> { new Company(1, "The Company") },
                    "Name name name",
                    null),
                new Game(
                    0,
                    null,
                    null,
                    DateTime.Now,
                    new List<Genre> { new Genre(1, "Genre genre") },
                    new List<Company> { new Company(1, "The Company") },
                    "Name name name",
                    null),
                new Game(
                    0,
                    null,
                    null,
                    DateTime.Now,
                    new List<Genre> { new Genre(1, "Genre genre") },
                    new List<Company> { new Company(1, "The Company") },
                    "Name name name",
                    null),
                new Game(
                    0,
                    null,
                    null,
                    DateTime.Now,
                    new List<Genre> { new Genre(1, "Genre genre") },
                    new List<Company> { new Company(1, "The Company") },
                    "Name name name",
                    null),
                new Game(
                    0,
                    null,
                    null,
                    DateTime.Now,
                    new List<Genre> { new Genre(1, "Genre genre") },
                    new List<Company> { new Company(1, "The Company") },
                    "Name name name",
                    null),
            };
    }
}

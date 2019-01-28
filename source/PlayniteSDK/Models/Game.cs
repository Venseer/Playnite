﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Playnite.SDK.Models
{
    /// <summary>
    /// Represents Playnite game object.
    /// </summary>
    public class Game : DatabaseObject
    {
        private string backgroundImage;
        /// <summary>
        /// Gets or sets background image. Local file path, HTTP URL or database file ids are supported.
        /// </summary>
        public string BackgroundImage
        {
            get
            {
                return backgroundImage;
            }

            set
            {
                backgroundImage = value;
                OnPropertyChanged();
            }
        }

        private string description;
        /// <summary>
        /// Gets or sets HTML game description.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        private List<Guid> genreIds;
        /// <summary>
        /// Gets or sets list of genres.
        /// </summary>
        public List<Guid> GenreIds
        {
            get
            {
                return genreIds;
            }

            set
            {
                genreIds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Genres));
            }
        }

        private bool hidden;
        /// <summary>
        /// Gets or sets value indicating if the game is hidden in library.
        /// </summary>
        public bool Hidden
        {
            get
            {
                return hidden;
            }

            set
            {
                hidden = value;
                OnPropertyChanged();
            }
        }

        private bool favorite;
        /// <summary>
        /// Gets or sets avlue indicating if the game is marked as favorite in library.
        /// </summary>
        public bool Favorite
        {
            get
            {
                return favorite;
            }

            set
            {
                favorite = value;
                OnPropertyChanged();
            }
        }


        private string icon;
        /// <summary>
        /// Gets or sets game icon. Local file path, HTTP URL or database file ids are supported.
        /// </summary>
        public string Icon
        {
            get
            {
                return icon;
            }

            set
            {
                icon = value;
                OnPropertyChanged();
            }
        }

        private string coverImage;
        /// <summary>
        /// Gets or sets game cover image. Local file path, HTTP URL or database file ids are supported.
        /// </summary>
        public string CoverImage
        {
            get
            {
                return coverImage;
            }

            set
            {
                coverImage = value;
                OnPropertyChanged();
            }
        }

        private string installDirectory;
        /// <summary>
        /// Gets or sets game installation directory path.
        /// </summary>
        public string InstallDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(installDirectory))
                {
                    if (PlayAction != null)
                    {
                        return PlayAction.WorkingDir;
                    }
                }

                return installDirectory;
            }

            set
            {
                installDirectory = value;
                OnPropertyChanged();
            }
        }

        private string gameImagePath;
        /// <summary>
        /// Gets or sets game's ISO, ROM or other type of executable image path.
        /// </summary>
        public string GameImagePath
        {
            get
            {
                return gameImagePath;
            }

            set
            {
                gameImagePath = value;
                OnPropertyChanged();
            }
        }

        private DateTime? lastActivity;
        /// <summary>
        /// Gets or sets last played date.
        /// </summary>
        public DateTime? LastActivity
        {
            get
            {
                return lastActivity;
            }

            set
            {
                lastActivity = value;
                OnPropertyChanged();
            }
        }

        private string sortingName;
        /// <summary>
        /// Gets or sets optional name used for sorting the game by name.
        /// </summary>
        public string SortingName
        {
            get
            {
                return sortingName;
            }

            set
            {
                sortingName = value;
                OnPropertyChanged();
            }
        }

        private string gameId;
        /// <summary>
        /// Gets or sets provider id. For example game's Steam ID.
        /// </summary>
        public string GameId
        {
            get
            {
                return gameId;
            }

            set
            {
                gameId = value;
                OnPropertyChanged();
            }
        }

        private Guid pluginId = Guid.Empty;
        /// <summary>
        /// Gets or sets id of plugin responsible for handling this game.
        /// </summary>
        public Guid PluginId
        {
            get
            {
                return pluginId;
            }

            set
            {
                pluginId = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<GameAction> otherActions;
        /// <summary>
        /// Gets or sets list of additional game actions.
        /// </summary>
        public ObservableCollection<GameAction> OtherActions
        {
            get
            {
                return otherActions;
            }

            set
            {
                otherActions = value;
                OnPropertyChanged();
            }
        }

        private GameAction playAction;
        /// <summary>
        /// Gets or sets game action used to starting the game.
        /// </summary>
        public GameAction PlayAction
        {
            get
            {
                return playAction;
            }

            set
            {
                playAction = value;
                OnPropertyChanged();
            }
        }

        private Guid platformId;
        /// <summary>
        /// Gets or sets platform id.
        /// </summary>
        public Guid PlatformId
        {
            get
            {
                return platformId;
            }

            set
            {
                platformId = value;
                OnPropertyChanged();
            }
        }

        private List<Guid> publisherIds;
        /// <summary>
        /// Gets or sets list of publishers.
        /// </summary>
        public List<Guid> PublisherIds
        {
            get
            {
                return publisherIds;
            }

            set
            {
                publisherIds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Publishers));
            }
        }

        // TODO change to standard list
        private List<Guid> developerIds;
        /// <summary>
        /// Gets or sets list of developers.
        /// </summary>
        public List<Guid> DeveloperIds
        {
            get
            {
                return developerIds;
            }

            set
            {
                developerIds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Developers));
            }
        }

        private DateTime? releaseDate;
        /// <summary>
        /// Gets or set game's release date.
        /// </summary>
        public DateTime? ReleaseDate
        {
            get
            {
                return releaseDate;
            }

            set
            {
                releaseDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReleaseYear));
            }
        }

        private List<Guid> categoryIds;
        /// <summary>
        /// Gets or sets game categories.
        /// </summary>
        public List<Guid> CategoryIds
        {
            get
            {
                return categoryIds;
            }

            set
            {
                categoryIds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Categories));
            }
        }

        private List<Guid> tagIds;
        /// <summary>
        /// Gets or sets list of tags.
        /// </summary>
        public List<Guid> TagIds
        {
            get
            {
                return tagIds;
            }

            set
            {
                tagIds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Tags));
            }
        }

        private ObservableCollection<Link> links;
        /// <summary>
        /// Gets or sets list of game related web links.
        /// </summary>
        public ObservableCollection<Link> Links
        {
            get
            {
                return links;
            }

            set
            {
                links = value;
                OnPropertyChanged();
            }
        }

        private bool isInstalling;
        /// <summary>
        /// Gets or sets value indicating wheter a game is being installed..
        /// </summary>
        public bool IsInstalling
        {
            get => isInstalling;
            set
            {
                isInstalling = value;
                OnPropertyChanged();
            }
        }

        private bool isUninstalling;
        /// <summary>
        /// Gets or sets value indicating wheter a game is being uninstalled.
        /// </summary>
        public bool IsUninstalling
        {
            get => isUninstalling;
            set
            {
                isUninstalling = value;
                OnPropertyChanged();
            }
        }

        private bool isLaunching;
        /// <summary>
        /// Gets or sets value indicating wheter a game is being launched.
        /// </summary>
        public bool IsLaunching
        {
            get => isLaunching;
            set
            {
                isLaunching = value;
                OnPropertyChanged();
            }
        }

        private bool isRunning;
        /// <summary>
        /// Gets or sets value indicating wheter a game is currently running.
        /// </summary>
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                OnPropertyChanged();
            }
        }

        private bool isInstalled;
        /// <summary>
        /// Gets or sets value indicating wheter a game is installed.
        /// </summary>
        public bool IsInstalled
        {
            get => isInstalled;
            set
            {
                isInstalled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets value indicating wheter the game is custom game.
        /// </summary>
        [JsonIgnore]
        public bool IsCustomGame
        {
            get => PluginId == Guid.Empty;
        }

        private long playtime = 0;
        /// <summary>
        /// Gets or sets played time in seconds.
        /// </summary>
        public long Playtime
        {
            get
            {
                return playtime;
            }

            set
            {
                playtime = value;
                OnPropertyChanged();
            }
        }

        private DateTime? added;
        /// <summary>
        /// Gets or sets date when game was added into library.
        /// </summary>
        public DateTime? Added
        {
            get
            {
                return added;
            }

            set
            {
                added = value;
                OnPropertyChanged();
            }
        }

        private DateTime? modified;
        /// <summary>
        /// Gets or sets date of last modification made to a game.
        /// </summary>
        public DateTime? Modified
        {
            get
            {
                return modified;
            }

            set
            {
                modified = value;
                OnPropertyChanged();
            }
        }

        private long playCount = 0;
        /// <summary>
        /// Gets or sets a number indicating how many times the game has been played.
        /// </summary>
        public long PlayCount
        {
            get
            {
                return playCount;
            }

            set
            {
                playCount = value;
                OnPropertyChanged();
            }
        }

        private Guid seriesId;
        /// <summary>
        /// Gets or sets game series.
        /// </summary>
        public Guid SeriesId
        {
            get
            {
                return seriesId;
            }

            set
            {
                seriesId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Series));
            }
        }

        private string version;
        /// <summary>
        /// Gets or sets game version.
        /// </summary>
        public string Version
        {
            get
            {
                return version;
            }

            set
            {
                version = value;
                OnPropertyChanged();
            }
        }

        private Guid ageRatingId;
        /// <summary>
        /// Gets or sets age rating for a game.
        /// </summary>
        public Guid AgeRatingId
        {
            get
            {
                return ageRatingId;
            }

            set
            {
                ageRatingId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AgeRating));
            }
        }

        private Guid regionId;
        /// <summary>
        /// Gets or sets game region.
        /// </summary>
        public Guid RegionId
        {
            get
            {
                return regionId;
            }

            set
            {
                regionId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Region));
            }
        }

        private Guid sourceId;
        /// <summary>
        /// Gets or sets source of the game.
        /// </summary>
        public Guid SourceId
        {
            get
            {
                return sourceId;
            }

            set
            {
                sourceId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Source));
            }
        }

        private CompletionStatus completionStatus = CompletionStatus.NotPlayed;
        /// <summary>
        /// Gets or sets game completion status.
        /// </summary>
        public CompletionStatus CompletionStatus
        {
            get
            {
                return completionStatus;
            }

            set
            {
                completionStatus = value;
                OnPropertyChanged();
            }
        }

        private int? userScore = null;
        /// <summary>
        /// Gets or sets user's rating score.
        /// </summary>
        public int? UserScore
        {
            get
            {
                return userScore;
            }

            set
            {
                userScore = value;
                OnPropertyChanged();
            }
        }

        private int? criticScore = null;
        /// <summary>
        /// Gets or sets critic based rating score.
        /// </summary>
        public int? CriticScore
        {
            get
            {
                return criticScore;
            }

            set
            {
                criticScore = value;
                OnPropertyChanged();
            }
        }

        private int? communityScore = null;
        /// <summary>
        /// Gets or sets community rating score.
        /// </summary>
        public int? CommunityScore
        {
            get
            {
                return communityScore;
            }

            set
            {
                communityScore = value;
                OnPropertyChanged();
            }
        }

        #region Expanded        

        [JsonIgnore]
        public List<Genre> Genres
        {
            get
            {
                if (genreIds?.Any() == true)
                {
                    return new List<Genre>(DatabaseReference?.Genres.Where(a => genreIds.Contains(a.Id)));
                }

                return null;
            }
        }

        [JsonIgnore]
        public List<Company> Developers
        {
            get
            {
                if (developerIds?.Any() == true)
                {
                    return new List<Company>(DatabaseReference?.Companies.Where(a => developerIds.Contains(a.Id)));
                }

                return null;
            }
        }

        [JsonIgnore]
        public List<Company> Publishers
        {
            get
            {
                if (publisherIds?.Any() == true)
                {
                    return new List<Company>(DatabaseReference?.Companies.Where(a => publisherIds.Contains(a.Id)));
                }

                return null;
            }
        }

        [JsonIgnore]
        public List<Tag> Tags
        {
            get
            {
                if (tagIds?.Any() == true)
                {
                    return new List<Tag>(DatabaseReference?.Tags.Where(a => tagIds.Contains(a.Id)));
                }

                return null;
            }
        }

        [JsonIgnore]
        public List<Category> Categories
        {
            get
            {
                if (categoryIds?.Any() == true)
                {
                    return new List<Category>(DatabaseReference?.Categories.Where(a => categoryIds.Contains(a.Id)));                    
                }

                return null;
            }
        }

        // TODO add caching
        [JsonIgnore]
        public Platform Platform
        {
            get => DatabaseReference?.Platforms[platformId];
        }

        [JsonIgnore]
        public Series Series
        {
            get => DatabaseReference?.Series[seriesId];
        }

        [JsonIgnore]
        public AgeRating AgeRating
        {
            get => DatabaseReference?.AgeRatings[ageRatingId];
        }

        [JsonIgnore]
        public Region Region
        {
            get => DatabaseReference?.Regions[regionId];
        }

        [JsonIgnore]
        public GameSource Source
        {
            get => DatabaseReference?.Sources[sourceId];
        }

        [JsonIgnore]
        public int? ReleaseYear
        {
            get => ReleaseDate?.Year;
        }

        #endregion Expanded

        // TODO internal
        public static IGameDatabase DatabaseReference
        {
            get; set;
        }

        /// <summary>
        /// Creates new instance of a Game object.
        /// </summary>
        public Game() : base()
        {
            GameId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates new instance of a Game object with specific name.
        /// </summary>
        /// <param name="name">Game name.</param>
        public Game(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}

﻿using CefSharp;
using NLog;
using Playnite;
using Playnite.Database;
using Playnite.Providers.Steam;
using Playnite.SDK;
using PlayniteUI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlayniteUI.ViewModels
{
    public class SettingsViewModel : ObservableObject, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IWindowFactory window;
        private IDialogsFactory dialogs;
        private IResourceProvider resources;
        private GameDatabase database;
        private Settings origSettings;

        private Playnite.Providers.GOG.WebApiClient gogApiClient = new Playnite.Providers.GOG.WebApiClient();

        public string GogLoginStatus
        {
            get
            {
                try
                {
                    if (gogApiClient.GetLoginRequired())
                    {
                        return resources.FindString("LOCLoginRequired");
                    }
                    else
                    {
                        return resources.FindString("LOCOKLabel");
                    }
                }
                catch (Exception e) when (!PlayniteEnvironment.ThrowAllErrors)
                {
                    logger.Error(e, "Failed to test GOG login status.");
                    return resources.FindString("LOCLoginFailed");
                }
            }
        }

        private Playnite.Providers.Origin.WebApiClient originApiClient = new Playnite.Providers.Origin.WebApiClient();

        public string OriginLoginStatus
        {
            get
            {
                try
                {
                    if (originApiClient.GetLoginRequired())
                    {
                        return resources.FindString("LOCLoginRequired");
                    }
                    else
                    {
                        return resources.FindString("LOCOKLabel");
                    }
                }
                catch (Exception e) when (!PlayniteEnvironment.ThrowAllErrors)
                {
                    logger.Error(e, "Failed to test Origin login status.");
                    return resources.FindString("LOCLoginFailed");
                }
            }
        }

        private Playnite.Providers.BattleNet.WebApiClient battleNetApiClient = new Playnite.Providers.BattleNet.WebApiClient();

        public string BattleNetLoginStatus
        {
            get
            {
                try
                {
                    if (battleNetApiClient.GetLoginRequired())
                    {
                        return resources.FindString("LOCLoginRequired");
                    }
                    else
                    {
                        return resources.FindString("LOCOKLabel");
                    }
                }
                catch (Exception e) when (!PlayniteEnvironment.ThrowAllErrors)
                {
                    logger.Error(e, "Failed to test BattleNet login status.");
                    return resources.FindString("LOCLoginFailed");
                }
            }
        }

        public List<LocalSteamUser> SteamUsers
        {
            get
            {
                return new SteamLibrary().GetSteamUsers();
            }
        }

        private Settings settings;
        public Settings Settings
        {
            get
            {
                return settings;
            }

            set
            {
                settings = value;
                OnPropertyChanged("Settings");
            }
        }

        public List<Theme> AvailableSkins
        {
            get => Themes.AvailableThemes;
        }

        public List<PlayniteLanguage> AvailableLanguages
        {
            get => Localization.AvailableLanguages;
        }

        public List<Theme> AvailableFullscreenSkins
        {
            get => Themes.AvailableFullscreenThemes;
        }

        public bool ProviderIntegrationChanged
        {
            get;
            private set;
        } = false;

        public bool DatabaseLocationChanged
        {
            get;
            private set;
        } = false;

        public RelayCommand<object> CancelCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                CloseView();
            });
        }

        public RelayCommand<object> ConfirmCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                ConfirmDialog();
            });
        }

        public RelayCommand<object> DisposeCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                Dispose();
            });
        }

        public RelayCommand<object> SelectDbFileCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                SelectDbFile();
            });
        }

        public RelayCommand<object> AuthGOGCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                AuthenticateGOG();
            }, (a) => Settings.GOGSettings.LibraryDownloadEnabled);
        }

        public RelayCommand<object> AuthOriginCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                AuthenticateOrigin();
            }, (a) => Settings.OriginSettings.LibraryDownloadEnabled);
        }

        public RelayCommand<object> AuthBattleNetCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                AuthenticateBattleNet();
            }, (a) => Settings.BattleNetSettings.LibraryDownloadEnabled);
        }

        public RelayCommand<Uri> NavigateUrlCommand
        {
            get => new RelayCommand<Uri>((url) =>
            {
                NavigateUrl(url.AbsoluteUri);
            });
        }

        public RelayCommand<object> ImportSteamCategoriesCommand
        {
            get => new RelayCommand<object>((url) =>
            {
                ImportSteamCategories();
            });
        }

        public RelayCommand<object> ClearWebCacheCommand
        {
            get => new RelayCommand<object>((url) =>
            {
                ClearWebcache();
            });
        }

        public SettingsViewModel(GameDatabase database, Settings settings, IWindowFactory window, IDialogsFactory dialogs, IResourceProvider resources)
        {
            origSettings = settings.CloneJson();
            Settings = settings;
            Settings.BeginEdit();
            this.database = database;
            this.window = window;
            this.dialogs = dialogs;
            this.resources = resources;
        }

        public bool? OpenView()
        {
            return window.CreateAndOpenDialog(this);
        }

        public void CloseView()
        {
            Settings.CancelEdit();
            window.Close(false);
            Dispose();
        }

        public void Dispose()
        {
            battleNetApiClient?.Dispose();
            originApiClient?.Dispose();
            gogApiClient?.Dispose();
            battleNetApiClient = null;
            originApiClient = null;
            gogApiClient = null;
        }

        public void ConfirmDialog()
        {
            if ((Settings.SteamSettings.IntegrationEnabled && Settings.SteamSettings.LibraryDownloadEnabled && Settings.SteamSettings.IdSource == SteamIdSource.Name)
                && string.IsNullOrEmpty(Settings.SteamSettings.AccountName))
            {
                dialogs.ShowMessage(resources.FindString("LOCSettingsInvalidSteamAccountName"),
                    resources.FindString("LOCInvalidDataTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if ((Settings.SteamSettings.IntegrationEnabled && Settings.SteamSettings.LibraryDownloadEnabled && Settings.SteamSettings.IdSource == SteamIdSource.LocalUser)
                && Settings.SteamSettings.AccountId == 0)
            {
                dialogs.ShowMessage(resources.FindString("LOCSettingsInvalidSteamAccountLibImport"),
                    resources.FindString("LOCInvalidDataTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Paths.GetValidFilePath(Settings.DatabasePath))
            {
                dialogs.ShowMessage(resources.FindString("LOCSettingsInvalidDBLocation"),
                    resources.FindString("LOCInvalidDataTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Settings.EndEdit();
            Settings.SaveSettings();

            if (Settings.EditedFields?.Any() == true)
            {
                if (Settings.EditedFields.IntersectsExactlyWith(
                    new List<string>() { "Skin", "AsyncImageLoading", "DisableHwAcceleration", "DatabasePath" }))
                {
                    if (dialogs.ShowMessage(
                        resources.FindString("LOCSettingsRestartAskMessage"),
                        resources.FindString("LOCSettingsRestartTitle"),
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        App.CurrentApp.Restart();
                    }
                }
            }

            if (origSettings.DatabasePath != Settings.DatabasePath)
            {
                DatabaseLocationChanged = true;
            }

            if (!origSettings.SteamSettings.IsEqualJson(Settings.SteamSettings))
            {
                ProviderIntegrationChanged = true;
            }

            if (!origSettings.OriginSettings.IsEqualJson(Settings.OriginSettings))
            {
                ProviderIntegrationChanged = true;
            }

            if (!origSettings.GOGSettings.IsEqualJson(Settings.GOGSettings))
            {
                ProviderIntegrationChanged = true;
            }

            if (!origSettings.BattleNetSettings.IsEqualJson(Settings.BattleNetSettings))
            {
                ProviderIntegrationChanged = true;
            }

            if (!origSettings.UplaySettings.IsEqualJson(Settings.UplaySettings))
            {
                ProviderIntegrationChanged = true;
            }

            window.Close(true);
            Dispose();
        }

        public void SelectDbFile()
        {
            var path = dialogs.SelectFile("Database file (*.db)|*.db");
            if (!string.IsNullOrEmpty(path))
            {
                dialogs.ShowMessage(resources.FindString("LOCSettingsDBPathNotification"));
                Settings.DatabasePath = path;
                Settings.OnPropertyChanged("DatabasePath", true);
            }
        }

        public void AuthenticateGOG()
        {
            try
            {
                gogApiClient.Login();
                OnPropertyChanged("GogLoginStatus");
            }
            catch (Exception e) when (!PlayniteEnvironment.ThrowAllErrors)
            {
                logger.Error(e, "GOG auth failed.");
                dialogs.ShowMessage(resources.FindString("LOCLoginFailed"), "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AuthenticateOrigin()
        {
            try
            {
                originApiClient.Login();
                OnPropertyChanged("OriginLoginStatus");
            }
            catch (Exception e) when (!PlayniteEnvironment.ThrowAllErrors)
            {
                logger.Error(e, "Origin auth failed.");
                dialogs.ShowMessage(resources.FindString("LOCLoginFailed"), "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AuthenticateBattleNet()
        {
            try
            {
                battleNetApiClient.Login();
                OnPropertyChanged("BattleNetLoginStatus");
            }
            catch (Exception e) when (!PlayniteEnvironment.ThrowAllErrors)
            {
                logger.Error(e, "BattleNet auth failed.");
                dialogs.ShowMessage(resources.FindString("LOCLoginFailed"), "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NavigateUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        public void ImportSteamCategories()
        {
            if (dialogs.ShowMessage(
                resources.FindString("LOCSettingsSteamCatImportWarn"),
                resources.FindString("LOCSettingsSteamCatImportWarnTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            if (Settings.SteamSettings.AccountId == 0)
            {
                dialogs.ShowMessage(
                    resources.FindString("LOCSettingsSteamCatImportErrorAccount"),
                    resources.FindString("LOCImportError"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (database.GamesCollection == null)
            {
                dialogs.ShowMessage(
                    resources.FindString("LOCSettingsSteamCatImportErrorDb"),
                    resources.FindString("LOCImportError"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var steamLib = new SteamLibrary();
                var games = steamLib.GetCategorizedGames(Settings.SteamSettings.AccountId);

                database.ImportCategories(games);
                dialogs.ShowMessage(resources.FindString("LOCImportCompleted"));
            }
            catch (Exception exc) when (!PlayniteEnvironment.ThrowAllErrors)
            {
                logger.Error(exc, "Failed to import Steam categories.");
                dialogs.ShowMessage(
                    resources.FindString("LOCSettingsSteamCatImportError"),
                    resources.FindString("LOCImportError"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ClearWebcache()
        {
            if (dialogs.ShowMessage(
                    resources.FindString("LOCSettingsClearCacheWarn"),
                    resources.FindString("LOCSettingsClearCacheTitle"),
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Cef.Shutdown();
                System.IO.Directory.Delete(Paths.BrowserCachePath, true);
                App.CurrentApp.Restart();
            }            
        }
    }
}

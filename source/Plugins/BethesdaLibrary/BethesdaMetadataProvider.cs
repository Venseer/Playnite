﻿using Playnite;
using Playnite.Common.System;
using Playnite.SDK;
using Playnite.SDK.Metadata;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaLibrary
{
    public class BethesdaMetadataProvider : ILibraryMetadataProvider
    {
        public GameMetadata GetMetadata(Game game)
        {
            var program = Bethesda.GetBethesdaInstallEntried().FirstOrDefault(a => a.UninstallString?.Contains($"uninstall/{game.GameId}") == true);
            if (program == null)
            {
                return null;
            }

            var gameInfo = new GameInfo
            {
                Name = StringExtensions.NormalizeGameName(program.DisplayName)
            };

            var metadata = new GameMetadata()
            {
                GameInfo = gameInfo
            };

            if (!string.IsNullOrEmpty(program.DisplayIcon) && File.Exists(program.DisplayIcon))
            {
                metadata.Icon = new MetadataFile(
                    Path.GetFileName(program.DisplayIcon),
                    File.ReadAllBytes(program.DisplayIcon));
            }

            return metadata;
        }
    }
}

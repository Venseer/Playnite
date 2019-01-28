﻿using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playnite.Database
{
    public class GamesCollection : ItemCollection<Game>
    {
        private readonly GameDatabase db;

        public GamesCollection(GameDatabase database) : base((Game game) =>
        {
            game.IsInstalling = false;
            game.IsUninstalling = false;
            game.IsLaunching = false;
            game.IsRunning = false;
        })
        {
            db = database;
        }

        public override Game Add(string itemName)
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<Game> Add(List<string> items)
        {
            throw new NotSupportedException();
        }

        public override void Add(Game item)
        {
            item.Added = DateTime.Today;
            base.Add(item);
        }

        public override void Add(IEnumerable<Game> items)
        {
            foreach (var item in items)
            {
                item.Added = DateTime.Today;
            }

            base.Add(items);
        }

        public override bool Remove(Guid id)
        {
            var item = Get(id);
            var result = base.Remove(id);
            db.RemoveFile(item.Icon);
            db.RemoveFile(item.CoverImage);
            db.RemoveFile(item.BackgroundImage);
            return result;
        }

        public override bool Remove(Game item)
        {
            return Remove(item.Id);
        }

        public override bool Remove(IEnumerable<Game> items)
        {
            foreach (var item in items)
            {
                // Get item from in case that passed platform doesn't have actual metadata.
                var dbItem = Get(item.Id);
                db.RemoveFile(dbItem.Icon);
                db.RemoveFile(dbItem.CoverImage);
                db.RemoveFile(dbItem.BackgroundImage);
            }

            var result = base.Remove(items);
            return result;
        }

        // TODO overload Update methods and remove metadata when updating to new data
    }
}

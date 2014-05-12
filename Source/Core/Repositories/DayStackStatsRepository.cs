﻿#region Copyright 2014 Exceptionless

// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
//     http://www.gnu.org/licenses/agpl-3.0.html

#endregion

using System;
using Exceptionless.Core.Caching;
using Exceptionless.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Exceptionless.Core.Repositories {
    public class DayStackStatsRepository : MongoRepositoryOwnedByProjectAndStack<DayStackStats> {
        public DayStackStatsRepository(MongoDatabase database, ICacheClient cacheClient = null) : base(database, cacheClient) {
            _getIdValue = s => s;
        }

        #region Collection Setup

        public const string CollectionName = "stack.stats.day";

        protected override string GetCollectionName() {
            return CollectionName;
        }

        public static class FieldNames {
            public const string Id = CommonFieldNames.Id;
            public const string ProjectId = CommonFieldNames.ProjectId;
            public const string StackId = CommonFieldNames.StackId;
            public const string Total = "tot";
            public const string MinuteStats = "mn";
            public const string MinuteStats_Format = "mn.{0}";
        }

        protected override void InitializeCollection(MongoDatabase database) {
            base.InitializeCollection(database);

            _collection.CreateIndex(IndexKeys.Ascending(FieldNames.ProjectId), IndexOptions.SetBackground(true));
            _collection.CreateIndex(IndexKeys.Ascending(FieldNames.StackId), IndexOptions.SetBackground(true));
        }

        protected override void ConfigureClassMap(BsonClassMap<DayStackStats> cm) {
            base.ConfigureClassMap(cm);
            cm.SetIdMember(cm.GetMemberMap(c => c.Id));
            cm.GetMemberMap(c => c.ProjectId).SetElementName(FieldNames.ProjectId).SetRepresentation(BsonType.ObjectId);
            cm.GetMemberMap(c => c.StackId).SetElementName(FieldNames.StackId).SetRepresentation(BsonType.ObjectId);
            cm.GetMemberMap(c => c.Total).SetElementName(FieldNames.Total);
            cm.GetMemberMap(c => c.MinuteStats).SetElementName(FieldNames.MinuteStats).SetSerializationOptions(DictionarySerializationOptions.Document);
        }

        #endregion
    }
}
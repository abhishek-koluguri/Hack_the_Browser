using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hack_the_Browser.Models;
using MongoDB.Driver;

namespace Hack_the_Browser.MetaDataRepositories
{
    public class MongoDbDataRepository : IImageDataRepository
    {
        private const string DatabaseName = "ImageService";
        private const string VersionedImagesCollection = "Images";
        private const string HashedPdfDataCollection = "PdfData";
        private readonly IMongoDatabase _database;
        private const int MaxNumberOfAttempts = 100;

        public MongoDbDataRepository(string databaseString)
        {
            var client = new MongoClient(databaseString);
            _database = client.GetDatabase(DatabaseName);
        }

        public async Task<bool> ReferenceIdsExistAsync(IList<Guid> referenceIds)
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Where(i => referenceIds.Contains(i.ReferenceId));
            var result = await collection.CountAsync(filter).ConfigureAwait(false);
            return referenceIds.LongCount() == result;
        }

        public async Task SaveImageAsync(Image image)
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Eq(i => i.ReferenceId, image.ReferenceId);
            var update = Builders<Image>.Update
                .Set(i => i.ReferenceId, image.ReferenceId)
                .Set(i => i.DateAccessed, DateTime.UtcNow)
                .Set(i => i.FrameCount, image.FrameCount)
                .Set(i => i.AnnotationFileSize, image.AnnotationFileSize)
                .Set(i => i.Extension, image.Extension)
                .Set(i => i.ExternalData, image.ExternalData)
                .Set(i => i.ExternalId, image.ExternalId)
                .Set(i => i.Height, image.Height)
                .Set(i => i.ImageFileSize, image.ImageFileSize)
                .Set(i => i.ReferenceVersion, image.ReferenceVersion)
                .Set(i => i.RotationAngle, image.RotationAngle)
                .Set(i => i.Width, image.Width)
                .Set(i => i.XResolution, image.XResolution)
                .Set(i => i.YResolution, image.YResolution)
                .Set(i => i.ETag, image.ETag);

            await collection.UpdateOneAsync(filter, update, new UpdateOptions
            {
                IsUpsert = true
            }).ConfigureAwait(false);
        }

        public async Task<Guid> GetReferenceIdAsync(string externalId)
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Eq(i => i.ExternalId, externalId);
            var sort = Builders<Image>.Sort.Descending(i => i.ReferenceVersion);
            var projection = Builders<Image>.Projection.Include(i => i.ReferenceId);
            var cursor = await collection.FindAsync(filter, new FindOptions<Image>
            {
                Limit = 1,
                Sort = sort,
                Projection = projection
            }).ConfigureAwait(false);

            var found = await cursor.MoveNextAsync().ConfigureAwait(false);

            if (!found || !cursor.Current.Any() || !cursor.Current.Any())
            {
                throw new Exception($"Reference ID not found for external ID {externalId}");
            }

            return cursor.Current.First().ReferenceId;
        }

        public async Task<Image> GetImageAsync(Guid referenceId, bool includeFrames)
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Eq(i => i.ReferenceId, referenceId);
            var findOptions = new FindOptions<Image>
            {
                Limit = 1
            };
            if (!includeFrames)
            {
                
            }

            var cursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);
            var found = await cursor.MoveNextAsync().ConfigureAwait(false);

            if (!found || !cursor.Current.Any() || !cursor.Current.Any())
            {
                //throw new DatabaseException(string.Format("Image not found for reference ID {0}", referenceId));
                return null;
            }

            return cursor.Current.First();
        }

        public async Task DeleteImagesAsync(IList<Guid> referenceIds)
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Where(i => referenceIds.Contains(i.ReferenceId));
            await collection.DeleteManyAsync(filter).ConfigureAwait(false);
        }

        public async Task UpdateImageAccessTimeAsync(Guid referenceId)
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Eq(i => i.ReferenceId, referenceId);
            var update = Builders<Image>.Update
                .Set(i => i.DateAccessed, DateTime.UtcNow);

            await collection.UpdateOneAsync(filter, update).ConfigureAwait(false);
        }

        public async Task<IList<Guid>> GetLeastRecentlyUsedReferenceIdsAsync(int count)
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Empty;
            var sort = Builders<Image>.Sort.Ascending(i => i.DateAccessed);
            var projection = Builders<Image>.Projection.Include(i => i.ReferenceId);

            var cursor = await collection.FindAsync(filter, new FindOptions<Image>
            {
                Limit = count,
                Sort = sort,
                Projection = projection
            }).ConfigureAwait(false);

            var result = new List<Guid>();

            await cursor.ForEachAsync(i =>
            {
                result.Add(i.ReferenceId);
            }).ConfigureAwait(false);

            return result;
        }



        //public async Task<IList<Guid>> GetLeastRecentlyUsedPdfReferenceIdsAsync(int count)
        //{
        //    var collection = _database.GetCollection<Image>(HashedPdfDataCollection);
        //    var filter = Builders<Image>.Filter.Empty;
        //    var sort = Builders<Image>.Sort.Ascending(pdf => pdf.DateAccessed);
        //    var projection = Builders<HashedPdfData>.Projection.Include(i => i.ReferenceId);

        //    var cursor = await collection.FindAsync(filter, new FindOptions<HashedPdfData>
        //    {
        //        Limit = count,
        //        Sort = sort,
        //        Projection = projection
        //    }).ConfigureAwait(false);

        //    var result = new List<Guid>();

        //    await cursor.ForEachAsync(pdf =>
        //    {
        //        result.Add(pdf.ReferenceId);
        //    }).ConfigureAwait(false);

        //    return result;
        //}

        public async Task<IList<Guid>> GetAllImageIdsAsync()
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Empty;
            var projection = Builders<Image>.Projection.Include(i => i.ReferenceId);

            var cursor = await collection.FindAsync(filter, new FindOptions<Image>
            {
                Projection = projection
            }).ConfigureAwait(false);

            var result = new List<Guid>();

            await cursor.ForEachAsync(i =>
            {
                result.Add(i.ReferenceId);
            }).ConfigureAwait(false);

            return result;
        }
        
        public async Task<IList<int>> LockFramesAsync(Guid referenceId, int startFrame, int endFrame)
        {
            var collection = _database.GetCollection<Image>(VersionedImagesCollection);
            var filter = Builders<Image>.Filter.Eq(i => i.ReferenceId, referenceId);

            var projection = Builders<Image>.Projection
                    .Exclude(i => i.ReferenceId);

            var attemptNumber = 0;

            while (attemptNumber < MaxNumberOfAttempts)
            {
                var cursor = await collection.FindAsync(filter, new FindOptions<Image>
                {
                    Limit = 1,
                    Projection = projection
                }).ConfigureAwait(false);

                var found = await cursor.MoveNextAsync().ConfigureAwait(false);

                if (!found || !cursor.Current.Any())
                {
                    throw new Exception($"Image not found for reference ID {referenceId}");
                }

                var image = cursor.Current.First();
                var result = new List<int>();
                

                

                var framesFilter = Builders<Image>.Filter.Eq(i => i.ReferenceId, referenceId);
                var update = Builders<Image>.Update
                    .Set(i => i.DateAccessed, DateTime.UtcNow);

                var updateResult = await collection.UpdateOneAsync(framesFilter, update).ConfigureAwait(false);

                if (updateResult.MatchedCount > 0)
                {
                    return result;
                }

                attemptNumber++;
            }

            throw new Exception(
                $"Could not lock frames for reference ID {referenceId} with start frame {startFrame} and end frame {endFrame}");
        }
    }
}

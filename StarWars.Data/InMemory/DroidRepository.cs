using StarWars.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarWars.Core.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace StarWars.Data.InMemory
{
    public class DroidRepository : IDroidRepository
    {
        private readonly ILogger _logger;

        public DroidRepository() { }

        public DroidRepository(ILogger<DroidRepository> logger)
        {
            _logger = logger;
        }

        private List<Droid> _droids = new List<Droid> {
            new Droid { Id = 1, Name = "R2-D2" }
        };

        public Droid Add(Droid entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Droid> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Droid> Get(int id)
        {
            _logger.LogInformation("Get droid with id = {id}", id);
            return Task.FromResult(_droids.FirstOrDefault(droid => droid.Id == id));
        }

        public Task<List<Droid>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(Droid entity)
        {
            throw new NotImplementedException();
        }
    }
}

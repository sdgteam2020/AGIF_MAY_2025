using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.IO.Util.IntHashtable;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace DataAccessLayer.Repositories
{
    public abstract class GenericRepositoryDL<T> : IGenericRepositoryDL<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        protected GenericRepositoryDL(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<T> Get(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);

            if (entity == null)
            {
                // Handle the case where no entity was found, e.g., throw an exception or return a default value
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }

            return entity;


        }
        public async Task<T> GetByGen<T2>(T2 val1)
        {
            var entity= await _context.Set<T>().FindAsync(val1);
            if (entity == null)
            {
                // Handle the case where no entity was found, e.g., throw an exception or return a default value
                throw new KeyNotFoundException($"Entity with id {val1} not found.");
            }

            return entity;
        }
        public async Task<T> GetByByte(byte id)
        {
           
            var ret= await _context.Set<T>().FindAsync(id);
            if (ret == null)
            {
                // Handle the case where no entity was found, e.g., throw an exception or return a default value
                throw new KeyNotFoundException($"Entity with id {ret} not found.");
            }

            return ret;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var ret = await _context.Set<T>().ToListAsync();
            if (ret == null)
            {
                // Handle the case where no entity was found, e.g., throw an exception or return a default value
                throw new KeyNotFoundException($"Entity not found.");
            }
            return ret;
        }

        public async Task Add(T entity)
        {
            if(entity==null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }
            await _context.Set<T>().AddAsync(entity);
            await SaveAsync();
        }
        public async Task<T> AddWithReturn(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }
            _context.Set<T>().Remove(entity);
            await SaveAsync();
        }

        public async Task Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }
            _context.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
        }
        public async Task<T> UpdateWithReturn(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }



    }

}

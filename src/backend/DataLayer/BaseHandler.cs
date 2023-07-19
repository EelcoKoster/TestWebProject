using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace DataLayer
{
    public abstract class BaseHandler<T> where T : BaseEntity
    {
        internal DataContext db;
        internal DbSet<T> entities;

        public BaseHandler(DataContext dataContext)
        {
            db = dataContext;
            entities = db.Set<T>();
        }

        public async Task Add(T item)
        {
            if (string.IsNullOrEmpty(item.Id)) item.Id = Guid.NewGuid().ToString();
            entities.Add(item);
            await db.SaveChangesAsync();
        }

        public virtual IEnumerable<T> Get(string orderByPropertyName, bool? ascending)
        {
            return entities.OrderBy(GetOrderStatement(orderByPropertyName, ascending));
        }

        public async Task<T?> GetById(string id)
        {
            return await entities.FindAsync(id);
        }

        public async Task Update(T item)
        {
            var itemToUpdate = entities.Find(item.Id);
            if (itemToUpdate == null) throw new KeyNotFoundException($"Item with id '{item.Id}' not found");

            Type typeSrc = itemToUpdate.GetType();
            PropertyInfo[] props = typeSrc.GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == "Id") continue;
                PropertyInfo targetProperty = typeSrc?.GetProperty(prop.Name);
                targetProperty.SetValue(itemToUpdate, prop.GetValue(item, null), null);
            }

            await db.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var item = entities.Find(id);
            if (item != null)
            {
                entities.Remove(item);
                await db.SaveChangesAsync();
            }
        }

        internal static string GetOrderStatement(string orderByPropertyName, bool? ascending)
        {
            orderByPropertyName = (string.IsNullOrEmpty(orderByPropertyName)) ? "Id" : orderByPropertyName;
            string ascDsc = (ascending.HasValue && !ascending.Value) ? "DESC" : "ASC";
            return $"{orderByPropertyName} {ascDsc}";
        }
    }
}

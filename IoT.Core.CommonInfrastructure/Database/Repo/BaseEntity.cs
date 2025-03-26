using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Core.CommonInfrastructure.Database.Repo
{
    public abstract class BaseEntity<TId>
    { 
        public TId Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual bool IsDirty(BaseEntity<TId>? baseEntity)
        {
            if (baseEntity == null) return true;

            // Get the actual derived type
            var entityType = this.GetType();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name != nameof(Id) && p.Name != nameof(CreatedAt) && p.Name != nameof(UpdatedAt));

            foreach (var property in properties)
            {
                var oldValue = property.GetValue(baseEntity);
                var newValue = property.GetValue(this);

                if (!Equals(oldValue, newValue)) return true;
            }
            return false;
        }
        public virtual void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        public virtual void MarkAsUndeleted()
        {
            IsDeleted = false;
        }
    }
}

using EFCore.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public int IsDeleted { get; set; } = 0;
        public List<Post> Posts { get; set; } = new List<Post>();
    }

    class CategoryEntityConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(Tables.CategoryTable);//与哪一个表对应
            builder.Property(c => c.CategoryName).IsRequired();
            builder.HasKey(t => t.Id);
        }
    }
}

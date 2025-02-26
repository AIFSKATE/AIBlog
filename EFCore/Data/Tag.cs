using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.Shared;

namespace EFCore.Data
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public List<Post> Posts { get; set; }
    }

    class TagEntityConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable(Tables.TagTable);//与哪一个表对应
            builder.HasKey(t => t.Id);
            builder.Property(b => b.TagName).HasMaxLength(50).IsRequired();
            builder.HasMany<Post>(t=>t.Posts).WithMany(p=>p.Tags).UsingEntity(j=>j.ToTable(Tables.PostTagTable));
        }
    }
}

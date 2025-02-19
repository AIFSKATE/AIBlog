using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using EFCore.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.Data
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Tag> Tags { get; set; }
        public DateTime CreationTime { get; set; }
    }

    class PostEntityConfig : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable(Consts.PostTable);//与哪一个表对应
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).HasMaxLength(50).IsRequired();
            builder.Property(t => t.CreationTime).IsRequired();
        }
    }
}

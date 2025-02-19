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
        public Post post { get; set; }
    }

    class TagEntityConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable(Consts.PostTable);//与哪一个表对应
            builder.HasKey(t => t.Id);
            builder.Property(b => b.TagName).HasMaxLength(50).IsRequired();
        }
    }
}

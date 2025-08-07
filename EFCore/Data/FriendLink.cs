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
    public class FriendLink
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public int IsDeleted { get; set; } = 0; // 默认值为0，表示未删除

        public DateTime CreationTime { get; set; } = DateTime.Now;
    }

    class FriendLinkEntityConfig : IEntityTypeConfiguration<FriendLink>
    {
        public void Configure(EntityTypeBuilder<FriendLink> builder)
        {
            builder.ToTable(Tables.FriendLinkTable);//与哪一个表对应
            builder.HasKey(t => t.Id);
            builder.Property(x => x.LinkUrl).IsRequired();
        }
    }
}

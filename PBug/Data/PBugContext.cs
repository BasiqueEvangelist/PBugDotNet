using Microsoft.EntityFrameworkCore;

namespace PBug.Data
{
    public partial class PBugContext : DbContext
    {
        public PBugContext()
        {
        }

        public PBugContext(DbContextOptions<PBugContext> options)
            : base(options)
        {
        }

        public virtual DbSet<InfopageComment> InfopageComments { get; set; }
        public virtual DbSet<Infopage> Infopages { get; set; }
        public virtual DbSet<Invite> Invites { get; set; }
        public virtual DbSet<IssueActivity> IssueActivities { get; set; }
        public virtual DbSet<IssueFile> IssueFiles { get; set; }
        public virtual DbSet<IssuePost> IssuePosts { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<IssueWatcher> IssueWatchers { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region InfopageComment
            modelBuilder.Entity<InfopageComment>(entity =>
            {
                entity.ToTable("infopagecomments");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("infopagecomments_authorid_foreign");

                entity.HasIndex(e => e.InfopageId)
                    .HasName("infopagecomments_infopageid_foreign");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AuthorId)
                    .HasColumnName("authorid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ContainedText)
                    .HasColumnName("containedtext")
                    .HasColumnType("text");

                entity.Property(e => e.DateOfCreation)
                    .HasColumnName("dateofcreation")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateOfEdit)
                    .HasColumnName("dateofedit")
                    .HasColumnType("datetime");

                entity.Property(e => e.InfopageId)
                    .HasColumnName("infopageid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.InfopageComments)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("infopagecomments_authorid_foreign");

                entity.HasOne(d => d.Infopage)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.InfopageId)
                    .HasConstraintName("infopagecomments_infopageid_foreign");
            });
            #endregion

            #region Infopage
            modelBuilder.Entity<Infopage>(entity =>
            {
                entity.ToTable("infopages");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("infopages_authorid_foreign");

                entity.HasIndex(e => e.EditorId)
                    .HasName("infopages_editorid_foreign");

                entity.HasIndex(e => e.Path)
                    .HasName("infopages_path_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AuthorId)
                    .HasColumnName("authorid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ContainedText)
                    .IsRequired()
                    .HasColumnName("containedtext")
                    .HasColumnType("text");

                entity.Property(e => e.DateOfCreation)
                    .HasColumnName("dateofcreation")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateOfEdit)
                    .HasColumnName("dateofedit")
                    .HasColumnType("datetime");

                entity.Property(e => e.EditorId)
                    .HasColumnName("editorid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("pagename")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasColumnName("pagetags")
                    .HasColumnType("text");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnName("path")
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.Secrecy)
                    .HasColumnName("secrecy")
                    .HasColumnType("int(4)");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.InfopagesAuthored)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("infopages_authorid_foreign");

                entity.HasOne(d => d.Editor)
                    .WithMany(p => p.InfopagesEdited)
                    .HasForeignKey(d => d.EditorId)
                    .HasConstraintName("infopages_editorid_foreign");
            });
            #endregion

            #region Invite
            modelBuilder.Entity<Invite>(entity =>
            {
                entity.ToTable("invites");

                entity.HasIndex(e => e.RoleId)
                    .HasName("invites_roleid_foreign");

                entity.HasIndex(e => e.Uid)
                    .HasName("invites_uid_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.RoleId)
                    .HasColumnName("roleid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Uid)
                    .IsRequired()
                    .HasColumnName("uid")
                    .HasColumnType("varchar(128)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Invites)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("invites_roleid_foreign");
            });
            #endregion

            #region IssueActivity
            modelBuilder.Entity<IssueActivity>(entity =>
            {
                entity.ToTable("issueactivities");

                entity.HasDiscriminator(x => x.Type)
                    .HasValue<CreateIssueActivity>("createissue")
                    .HasValue<EditIssueActivity>("editissue")
                    .HasValue<PostActivity>("post")
                    .HasValue<EditPostActivity>("editpost");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("issueactivities_authorid_foreign");

                entity.HasIndex(e => e.IssueId)
                    .HasName("issueactivities_issueid_foreign");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AuthorId)
                    .HasColumnName("authorid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.DateOfOccurance)
                    .HasColumnName("dateofoccurance")
                    .HasColumnType("datetime");

                entity.Property(e => e.IssueId)
                    .HasColumnName("issueid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Type)
                   .HasColumnName("type")
                   .HasColumnType("varchar(30)");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.IssueActivities)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("issueactivities_authorid_foreign");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issueactivities_issueid_foreign");
            });
            #endregion

            #region CreateIssueActivity
            modelBuilder.Entity<CreateIssueActivity>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasColumnName("tags")
                    .HasColumnType("text");

                entity.Property(e => e.ProjectId)
                    .IsRequired()
                    .HasColumnName("projectid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssigneeId)
                    .IsRequired()
                    .HasColumnName("assigneeid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Assignee)
                    .WithMany()
                    .HasForeignKey(d => d.AssigneeId)
                    .HasConstraintName("issueactivities_assigneeid_foreign");

                entity.HasOne(d => d.Project)
                    .WithMany()
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issueactivities_projectid_foreign");
            });
            #endregion

            #region EditIssueActivity
            modelBuilder.Entity<EditIssueActivity>(entity =>
            {
                entity.Property(e => e.OldName)
                    .IsRequired()
                    .HasColumnName("oldname")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.OldDescription)
                    .IsRequired()
                    .HasColumnName("olddescription")
                    .HasColumnType("text");

                entity.Property(e => e.OldTags)
                    .IsRequired()
                    .HasColumnName("oldtags")
                    .HasColumnType("text");

                entity.Property(e => e.OldProjectId)
                    .IsRequired()
                    .HasColumnName("oldprojectid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.OldAssigneeId)
                    .IsRequired()
                    .HasColumnName("oldassigneeid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.OldStatus)
                    .IsRequired()
                    .HasColumnName("oldstatus")
                    .HasColumnType("smallint(6)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.NewName)
                    .IsRequired()
                    .HasColumnName("newname")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.NewDescription)
                    .IsRequired()
                    .HasColumnName("newdescription")
                    .HasColumnType("text");

                entity.Property(e => e.NewTags)
                    .IsRequired()
                    .HasColumnName("newtags")
                    .HasColumnType("text");

                entity.Property(e => e.NewProjectId)
                    .IsRequired()
                    .HasColumnName("newprojectid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.NewAssigneeId)
                    .IsRequired()
                    .HasColumnName("newassigneeid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.NewStatus)
                    .IsRequired()
                    .HasColumnName("newstatus")
                    .HasColumnType("smallint(6)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.OldAssignee)
                    .WithMany()
                    .HasForeignKey(d => d.OldAssigneeId)
                    .HasConstraintName("issueactivities_oldassigneeid_foreign");

                entity.HasOne(d => d.NewAssignee)
                    .WithMany()
                    .HasForeignKey(d => d.NewAssigneeId)
                    .HasConstraintName("issueactivities_newassigneeid_foreign");

                entity.HasOne(d => d.OldProject)
                    .WithMany()
                    .HasForeignKey(d => d.OldProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issueactivities_oldprojectid_foreign");

                entity.HasOne(d => d.NewProject)
                    .WithMany()
                    .HasForeignKey(d => d.NewProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issueactivities_newprojectid_foreign");
            });
            #endregion

            #region PostActivity
            modelBuilder.Entity<PostActivity>(entity =>
            {
                entity.Property(e => e.ContainedText)
                    .IsRequired()
                    .HasColumnName("containedtext")
                    .HasColumnType("text");

                entity.Property(e => e.PostId)
                    .IsRequired()
                    .HasColumnName("postid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Post)
                    .WithMany()
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("issueactivities_postid_foreign");
            });
            #endregion

            #region EditPostActivity
            modelBuilder.Entity<EditPostActivity>(entity =>
            {
                entity.Property(e => e.OldContainedText)
                    .IsRequired()
                    .HasColumnName("oldcontainedtext")
                    .HasColumnType("text");

                entity.Property(e => e.NewContainedText)
                    .IsRequired()
                    .HasColumnName("newcontainedtext")
                    .HasColumnType("text");

                entity.Property(e => e.PostId)
                    .IsRequired()
                    .HasColumnName("postid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Post)
                    .WithMany()
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("issueactivities_postid_foreign");
            });
            #endregion

            #region IssueFile
            modelBuilder.Entity<IssueFile>(entity =>
            {
                entity.ToTable("issuefiles");

                entity.HasIndex(e => e.IssueId)
                    .HasName("issuefiles_issueid_foreign");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.FileId)
                    .HasColumnName("fileid")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnName("filename")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.IssueId)
                    .HasColumnName("issueid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.IssueId)
                    .HasConstraintName("issuefiles_issueid_foreign");
            });
            #endregion

            #region IssuePost
            modelBuilder.Entity<IssuePost>(entity =>
            {
                entity.ToTable("issueposts");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("issueposts_authorid_foreign");

                entity.HasIndex(e => e.IssueId)
                    .HasName("issueposts_issueid_foreign");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AuthorId)
                    .HasColumnName("authorid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ContainedText)
                    .IsRequired()
                    .HasColumnName("containedtext")
                    .HasColumnType("text");

                entity.Property(e => e.DateOfCreation)
                    .HasColumnName("dateofcreation")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateOfEdit)
                    .HasColumnName("dateofedit")
                    .HasColumnType("datetime");

                entity.Property(e => e.IssueId)
                    .HasColumnName("issueid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.IssuePosts)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("issueposts_authorid_foreign");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issueposts_issueid_foreign");
            });
            #endregion

            #region Issue
            modelBuilder.Entity<Issue>(entity =>
            {
                entity.ToTable("issues");

                entity.HasIndex(e => e.AssigneeId)
                    .HasName("issues_assigneeid_foreign");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("issues_authorid_foreign");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("issues_projectid_foreign");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssigneeId)
                    .HasColumnName("assigneeid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AuthorId)
                    .HasColumnName("authorid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.DateOfCreation)
                    .HasColumnName("dateofcreation")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("issuename")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasColumnName("issuetags")
                    .HasColumnType("text");

                entity.Property(e => e.ProjectId)
                    .HasColumnName("projectid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("smallint(6)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Assignee)
                    .WithMany(p => p.IssuesAssignedTo)
                    .HasForeignKey(d => d.AssigneeId)
                    .HasConstraintName("issues_assigneeid_foreign");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.IssuesAuthored)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("issues_authorid_foreign");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issues_projectid_foreign");
            });
            #endregion

            #region IssueWatcher
            modelBuilder.Entity<IssueWatcher>(entity =>
            {
                entity.HasKey(e => new { e.WatcherId, e.IssueId })
                    .HasName("PRIMARY");

                entity.ToTable("issuewatchers");

                entity.HasIndex(e => e.IssueId)
                    .HasName("issuewatchers_issueid_foreign");

                entity.Property(e => e.WatcherId)
                    .HasColumnName("watcherid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.IssueId)
                    .HasColumnName("issueid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.Watchers)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issuewatchers_issueid_foreign");

                entity.HasOne(d => d.Watcher)
                    .WithMany(p => p.IssuesWatched)
                    .HasForeignKey(d => d.WatcherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issuewatchers_watcherid_foreign");
            });
            #endregion

            #region Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("projects");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("projects_authorid_foreign");

                entity.HasIndex(e => e.ShortProjectId)
                    .HasName("projects_shortprojectid_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AuthorId)
                    .HasColumnName("authorid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("projectname")
                    .HasColumnType("varchar(100)");
                entity.Property(e => e.ShortProjectId)
                    .IsRequired()
                    .HasColumnName("shortprojectid")
                    .HasColumnType("varchar(3)");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("projects_authorid_foreign");
            });
            #endregion

            #region Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasIndex(e => e.Name)
                    .HasName("roles_name_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Permissions)
                    .HasColumnName("permissions")
                    .HasColumnType("text");
            });
            #endregion

            #region User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.RoleId)
                    .HasName("users_roleid_foreign");

                entity.HasIndex(e => e.Username)
                    .HasName("users_username_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("fullname")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnName("passwordhash")
                    .HasColumnType("varbinary(64)");

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasColumnName("passwordsalt")
                    .HasColumnType("varbinary(64)");

                entity.Property(e => e.RoleId)
                    .HasColumnName("roleid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.PasswordFunc)
                    .IsRequired()
                    .HasColumnName("passwordfunc")
                    .HasColumnType("tinyint(4)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("users_roleid_foreign");
            });
            #endregion

            #region Session
            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("sessions");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("binary(64)");

                entity.Property(e => e.SessionData)
                    .IsRequired()
                    .HasColumnName("data")
                    .HasColumnType("blob");

                entity.Property(e => e.Expires)
                    .HasColumnName("expires")
                    .HasColumnType("timestamp");
            });
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

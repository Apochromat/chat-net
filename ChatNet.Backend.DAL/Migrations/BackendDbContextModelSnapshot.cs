﻿// <auto-generated />
using System;
using System.Collections.Generic;
using ChatNet.Backend.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatNet.Backend.DAL.Migrations
{
    [DbContext(typeof(BackendDbContext))]
    partial class BackendDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ChatAvatarId")
                        .HasColumnType("uuid");

                    b.Property<string>("ChatName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<List<Guid>>("FileIds")
                        .IsRequired()
                        .HasColumnType("uuid[]");

                    b.HasKey("Id");

                    b.ToTable("Chats");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Chat");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("EditedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<List<Guid>>("Files")
                        .IsRequired()
                        .HasColumnType("uuid[]");

                    b.Property<string>("TextMessage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<List<Guid>>("ViewedBy")
                        .IsRequired()
                        .HasColumnType("uuid[]");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.NotificationPreferences", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("PreferenceType")
                        .HasColumnType("integer");

                    b.Property<Guid>("PreferencedChatId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PreferencedChatId");

                    b.HasIndex("UserId");

                    b.ToTable("NotificationPreferences");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.Reaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ReactedMessageId")
                        .HasColumnType("uuid");

                    b.Property<int>("ReactionType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReactedMessageId");

                    b.ToTable("Reactions");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.UserBackend", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("GroupChatId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("GroupChatId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ChatUserBackend", b =>
                {
                    b.Property<Guid>("ChatsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uuid");

                    b.HasKey("ChatsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("ChatUserBackend");
                });

            modelBuilder.Entity("ReactionUserBackend", b =>
                {
                    b.Property<Guid>("ReactionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uuid");

                    b.HasKey("ReactionsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("ReactionUserBackend");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.GroupChat", b =>
                {
                    b.HasBaseType("ChatNet.Backend.DAL.Entities.Chat");

                    b.HasDiscriminator().HasValue("GroupChat");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.PrivateChat", b =>
                {
                    b.HasBaseType("ChatNet.Backend.DAL.Entities.Chat");

                    b.HasDiscriminator().HasValue("PrivateChat");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.Message", b =>
                {
                    b.HasOne("ChatNet.Backend.DAL.Entities.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChatNet.Backend.DAL.Entities.UserBackend", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.NotificationPreferences", b =>
                {
                    b.HasOne("ChatNet.Backend.DAL.Entities.Chat", "PreferencedChat")
                        .WithMany()
                        .HasForeignKey("PreferencedChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChatNet.Backend.DAL.Entities.UserBackend", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PreferencedChat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.Reaction", b =>
                {
                    b.HasOne("ChatNet.Backend.DAL.Entities.Message", "ReactedMessage")
                        .WithMany("Reactions")
                        .HasForeignKey("ReactedMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReactedMessage");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.UserBackend", b =>
                {
                    b.HasOne("ChatNet.Backend.DAL.Entities.GroupChat", null)
                        .WithMany("Administrators")
                        .HasForeignKey("GroupChatId");
                });

            modelBuilder.Entity("ChatUserBackend", b =>
                {
                    b.HasOne("ChatNet.Backend.DAL.Entities.Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChatNet.Backend.DAL.Entities.UserBackend", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReactionUserBackend", b =>
                {
                    b.HasOne("ChatNet.Backend.DAL.Entities.Reaction", null)
                        .WithMany()
                        .HasForeignKey("ReactionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChatNet.Backend.DAL.Entities.UserBackend", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.Chat", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.Message", b =>
                {
                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("ChatNet.Backend.DAL.Entities.GroupChat", b =>
                {
                    b.Navigation("Administrators");
                });
#pragma warning restore 612, 618
        }
    }
}

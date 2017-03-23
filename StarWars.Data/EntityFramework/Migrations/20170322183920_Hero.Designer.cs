using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using StarWars.Data.EntityFramework;

namespace StarWars.Data.EntityFramework.Migrations
{
    [DbContext(typeof(StarWarsContext))]
    [Migration("20170322183920_Hero")]
    partial class Hero
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StarWars.Core.Models.Character", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Characters");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Character");
                });

            modelBuilder.Entity("StarWars.Core.Models.CharacterEpisode", b =>
                {
                    b.Property<int>("CharacterId");

                    b.Property<int>("EpisodeId");

                    b.HasKey("CharacterId", "EpisodeId");

                    b.HasIndex("EpisodeId");

                    b.ToTable("CharacterEpisodes");
                });

            modelBuilder.Entity("StarWars.Core.Models.CharacterFriend", b =>
                {
                    b.Property<int>("CharacterId");

                    b.Property<int>("FriendId");

                    b.HasKey("CharacterId", "FriendId");

                    b.HasIndex("FriendId");

                    b.ToTable("CharacterFriends");
                });

            modelBuilder.Entity("StarWars.Core.Models.Episode", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int?>("HeroId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("HeroId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("StarWars.Core.Models.Planet", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Planets");
                });

            modelBuilder.Entity("StarWars.Core.Models.Droid", b =>
                {
                    b.HasBaseType("StarWars.Core.Models.Character");

                    b.Property<string>("PrimaryFunction");

                    b.ToTable("Droid");

                    b.HasDiscriminator().HasValue("Droid");
                });

            modelBuilder.Entity("StarWars.Core.Models.Human", b =>
                {
                    b.HasBaseType("StarWars.Core.Models.Character");

                    b.Property<int?>("HomePlanetId");

                    b.HasIndex("HomePlanetId");

                    b.ToTable("Human");

                    b.HasDiscriminator().HasValue("Human");
                });

            modelBuilder.Entity("StarWars.Core.Models.CharacterEpisode", b =>
                {
                    b.HasOne("StarWars.Core.Models.Character", "Character")
                        .WithMany("CharacterEpisodes")
                        .HasForeignKey("CharacterId");

                    b.HasOne("StarWars.Core.Models.Episode", "Episode")
                        .WithMany("CharacterEpisodes")
                        .HasForeignKey("EpisodeId");
                });

            modelBuilder.Entity("StarWars.Core.Models.CharacterFriend", b =>
                {
                    b.HasOne("StarWars.Core.Models.Character", "Character")
                        .WithMany("CharacterFriends")
                        .HasForeignKey("CharacterId");

                    b.HasOne("StarWars.Core.Models.Character", "Friend")
                        .WithMany("FriendCharacters")
                        .HasForeignKey("FriendId");
                });

            modelBuilder.Entity("StarWars.Core.Models.Episode", b =>
                {
                    b.HasOne("StarWars.Core.Models.Character", "Hero")
                        .WithMany()
                        .HasForeignKey("HeroId");
                });

            modelBuilder.Entity("StarWars.Core.Models.Human", b =>
                {
                    b.HasOne("StarWars.Core.Models.Planet", "HomePlanet")
                        .WithMany("Humans")
                        .HasForeignKey("HomePlanetId");
                });
        }
    }
}

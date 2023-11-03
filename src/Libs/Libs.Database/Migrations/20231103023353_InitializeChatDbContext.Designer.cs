﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RichasyAssistant.Libs.Database;

#nullable disable

namespace RichasyAssistant.Libs.Database.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20231103023353_InitializeChatDbContext")]
    partial class InitializeChatDbContext
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.ChatMessage", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Extension")
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SessionPayloadId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SessionPayloadId");

                    b.ToTable("ChatMessage");
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.SessionOptions", b =>
                {
                    b.Property<string>("SessionId")
                        .HasColumnType("TEXT");

                    b.Property<double>("FrequencyPenalty")
                        .HasColumnType("REAL");

                    b.Property<int>("MaxResponseTokens")
                        .HasColumnType("INTEGER");

                    b.Property<double>("PresencePenalty")
                        .HasColumnType("REAL");

                    b.Property<double>("Temperature")
                        .HasColumnType("REAL");

                    b.Property<double>("TopP")
                        .HasColumnType("REAL");

                    b.HasKey("SessionId");

                    b.ToTable("SessionOptions");
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.SessionPayload", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.ChatMessage", b =>
                {
                    b.HasOne("RichasyAssistant.Models.App.Kernel.SessionPayload", null)
                        .WithMany("Messages")
                        .HasForeignKey("SessionPayloadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.SessionOptions", b =>
                {
                    b.HasOne("RichasyAssistant.Models.App.Kernel.SessionPayload", null)
                        .WithOne("Options")
                        .HasForeignKey("RichasyAssistant.Models.App.Kernel.SessionOptions", "SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.SessionPayload", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Options")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

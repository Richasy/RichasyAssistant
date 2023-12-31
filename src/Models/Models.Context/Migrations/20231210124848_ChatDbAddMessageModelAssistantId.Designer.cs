﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RichasyAssistant.Models.Context;

#nullable disable

namespace RichasyAssistant.Models.Context.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20231210124848_ChatDbAddMessageModelAssistantId")]
    partial class ChatDbAddMessageModelAssistantId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.Assistant", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Instruction")
                        .HasColumnType("TEXT");

                    b.Property<int>("Kernel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Model")
                        .HasColumnType("TEXT");

                    b.Property<string>("ModelDeploymentName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Remark")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Assistants");
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.ChatMessage", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<string>("AssistantId")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "assistant_id");

                    b.Property<string>("ChatSessionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "content");

                    b.Property<string>("Extension")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "extension");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "role");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "time");

                    b.HasKey("Id");

                    b.HasIndex("ChatSessionId");

                    b.ToTable("ChatMessage");
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.ChatSession", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Assistants")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.SessionOptions", b =>
                {
                    b.Property<string>("SessionId")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "session_id");

                    b.Property<double>("FrequencyPenalty")
                        .HasColumnType("REAL")
                        .HasAnnotation("Relational:JsonPropertyName", "frequency_penalty");

                    b.Property<int>("MaxResponseTokens")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "max_response_tokens");

                    b.Property<double>("PresencePenalty")
                        .HasColumnType("REAL")
                        .HasAnnotation("Relational:JsonPropertyName", "presence_penalty");

                    b.Property<double>("Temperature")
                        .HasColumnType("REAL")
                        .HasAnnotation("Relational:JsonPropertyName", "temperature");

                    b.Property<double>("TopP")
                        .HasColumnType("REAL")
                        .HasAnnotation("Relational:JsonPropertyName", "top_p");

                    b.HasKey("SessionId");

                    b.ToTable("SessionOptions");
                });

            modelBuilder.Entity("System.Collections.Generic.List<string>", b =>
                {
                    b.Property<int>("Capacity")
                        .HasColumnType("INTEGER");

                    b.ToTable("List<string>");
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.ChatMessage", b =>
                {
                    b.HasOne("RichasyAssistant.Models.App.Kernel.ChatSession", null)
                        .WithMany("Messages")
                        .HasForeignKey("ChatSessionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.SessionOptions", b =>
                {
                    b.HasOne("RichasyAssistant.Models.App.Kernel.ChatSession", null)
                        .WithOne("Options")
                        .HasForeignKey("RichasyAssistant.Models.App.Kernel.SessionOptions", "SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RichasyAssistant.Models.App.Kernel.ChatSession", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Options")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

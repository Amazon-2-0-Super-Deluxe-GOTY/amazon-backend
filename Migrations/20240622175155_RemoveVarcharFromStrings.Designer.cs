﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using amazon_backend.Data;

#nullable disable

namespace amazon_backend.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240622175155_RemoveVarcharFromStrings")]
    partial class RemoveVarcharFromStrings
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ProductProductImage", b =>
                {
                    b.Property<Guid>("ProductImagesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ProductsId")
                        .HasColumnType("char(36)");

                    b.HasKey("ProductImagesId", "ProductsId");

                    b.HasIndex("ProductsId");

                    b.ToTable("ProductProductImage");
                });

            modelBuilder.Entity("ProductProductProperty", b =>
                {
                    b.Property<Guid>("ProductPropertiesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ProductsId")
                        .HasColumnType("char(36)");

                    b.HasKey("ProductPropertiesId", "ProductsId");

                    b.HasIndex("ProductsId");

                    b.ToTable("ProductProductProperty");
                });

            modelBuilder.Entity("ProductUser", b =>
                {
                    b.Property<Guid>("WishListersId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("WishedProductsId")
                        .HasColumnType("char(36)");

                    b.HasKey("WishListersId", "WishedProductsId");

                    b.HasIndex("WishedProductsId");

                    b.ToTable("ProductUser");
                });

            modelBuilder.Entity("Review", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Mark")
                        .HasColumnType("int");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Text")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews", t =>
                        {
                            t.HasCheckConstraint("ValidMark", "Mark > 0 AND Mark < 6");
                        });
                });

            modelBuilder.Entity("ReviewReviewImage", b =>
                {
                    b.Property<Guid>("ReviewId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ReviewImagesId")
                        .HasColumnType("char(36)");

                    b.HasKey("ReviewId", "ReviewImagesId");

                    b.HasIndex("ReviewImagesId");

                    b.ToTable("ReviewReviewImage");
                });

            modelBuilder.Entity("ReviewReviewTag", b =>
                {
                    b.Property<Guid>("ReviewTagsId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ReviewsId")
                        .HasColumnType("char(36)");

                    b.HasKey("ReviewTagsId", "ReviewsId");

                    b.HasIndex("ReviewsId");

                    b.ToTable("ReviewReviewTag");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.AboutProductItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("AboutProductItems");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Cart", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("CartId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Category", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    b.Property<Guid?>("CategoryPropertyKeyId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<Guid>("ImageId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<string>("Logo")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<uint?>("ParentCategoryId")
                        .HasColumnType("int unsigned");

                    b.HasKey("Id");

                    b.HasIndex("CategoryPropertyKeyId");

                    b.HasIndex("ImageId");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.CategoryImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("CategoryImages");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.CategoryPropertyKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<uint>("CategoryId")
                        .HasColumnType("int unsigned");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsFilter")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("NameCategory")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("CategoryPropertyKeys");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.DeliveryAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("City")
                        .HasColumnType("longtext");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<string>("PostIndex")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("DeliveryAddresses");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.JwtToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("JwtTokens");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<double>("Price")
                        .HasColumnType("double");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<double>("TotalPrice")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<uint>("CategoryId")
                        .HasColumnType("int unsigned");

                    b.Property<string>("Code")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("DiscountPercent")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("Price")
                        .HasColumnType("double");

                    b.Property<Guid?>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Slug")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.ProductImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.ProductProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsOption")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("ProductProperties");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.ReviewImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeleteDt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ReviewImages");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.ReviewLike", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ReviewId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ReviewId");

                    b.HasIndex("UserId");

                    b.ToTable("ReviewLikes");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.ReviewTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("ReviewTags");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.TokenJournal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ActivatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeactivatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("TokenId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("TokenId");

                    b.HasIndex("UserId");

                    b.ToTable("TokenJournals");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmailCode")
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TempEmail")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProductProductImage", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.ProductImage", null)
                        .WithMany()
                        .HasForeignKey("ProductImagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductProductProperty", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.ProductProperty", null)
                        .WithMany()
                        .HasForeignKey("ProductPropertiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductUser", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.User", null)
                        .WithMany()
                        .HasForeignKey("WishListersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.Product", null)
                        .WithMany()
                        .HasForeignKey("WishedProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Review", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.Product", "Product")
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ReviewReviewImage", b =>
                {
                    b.HasOne("Review", null)
                        .WithMany()
                        .HasForeignKey("ReviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.ReviewImage", null)
                        .WithMany()
                        .HasForeignKey("ReviewImagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReviewReviewTag", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.ReviewTag", null)
                        .WithMany()
                        .HasForeignKey("ReviewTagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Review", null)
                        .WithMany()
                        .HasForeignKey("ReviewsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.AboutProductItem", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.Product", "Product")
                        .WithMany("AboutProductItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Cart", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.User", "User")
                        .WithMany("Carts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.CartItem", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.Cart", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.Product", "Product")
                        .WithMany("CartItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Category", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.CategoryPropertyKey", null)
                        .WithMany("Categories")
                        .HasForeignKey("CategoryPropertyKeyId");

                    b.HasOne("amazon_backend.Data.Entity.CategoryImage", "Image")
                        .WithMany("Categories")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.Category", "ParentCategory")
                        .WithMany()
                        .HasForeignKey("ParentCategoryId");

                    b.Navigation("Image");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.CategoryPropertyKey", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.Category", "Category")
                        .WithMany("CategoryPropertyKeys")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.DeliveryAddress", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.Order", "Order")
                        .WithMany("DeliveryAddresses")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Order", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.OrderItem", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Product", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.Product", null)
                        .WithMany("Products")
                        .HasForeignKey("ProductId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.ReviewImage", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.User", "User")
                        .WithMany("ReviewImages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.ReviewLike", b =>
                {
                    b.HasOne("Review", "Review")
                        .WithMany("ReviewLikes")
                        .HasForeignKey("ReviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.User", "User")
                        .WithMany("ReviewLikes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Review");

                    b.Navigation("User");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.TokenJournal", b =>
                {
                    b.HasOne("amazon_backend.Data.Entity.JwtToken", "Token")
                        .WithMany("TokenJournals")
                        .HasForeignKey("TokenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("amazon_backend.Data.Entity.User", "User")
                        .WithMany("TokenJournals")
                        .HasForeignKey("UserId");

                    b.Navigation("Token");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Review", b =>
                {
                    b.Navigation("ReviewLikes");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Cart", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Category", b =>
                {
                    b.Navigation("CategoryPropertyKeys");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.CategoryImage", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.CategoryPropertyKey", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.JwtToken", b =>
                {
                    b.Navigation("TokenJournals");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Order", b =>
                {
                    b.Navigation("DeliveryAddresses");

                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.Product", b =>
                {
                    b.Navigation("AboutProductItems");

                    b.Navigation("CartItems");

                    b.Navigation("Products");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("amazon_backend.Data.Entity.User", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Orders");

                    b.Navigation("ReviewImages");

                    b.Navigation("ReviewLikes");

                    b.Navigation("Reviews");

                    b.Navigation("TokenJournals");
                });
#pragma warning restore 612, 618
        }
    }
}

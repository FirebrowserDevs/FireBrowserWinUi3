﻿// <auto-generated />
using FireBrowserDataCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FireBrowserDataCore.Migrations.Settings
{
    [DbContext(typeof(SettingsContext))]
    partial class SettingsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("FireBrowserMultiCore.Settings", b =>
                {
                    b.Property<string>("PackageName")
                        .HasColumnType("TEXT");

                    b.Property<string>("AdblockBtn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Auto")
                        .HasColumnType("TEXT");

                    b.Property<string>("Background")
                        .HasColumnType("TEXT");

                    b.Property<string>("BrowserKeys")
                        .HasColumnType("TEXT");

                    b.Property<string>("BrowserScripts")
                        .HasColumnType("TEXT");

                    b.Property<string>("ColorBackground")
                        .HasColumnType("TEXT");

                    b.Property<string>("ColorTV")
                        .HasColumnType("TEXT");

                    b.Property<string>("ColorTool")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConfirmCloseDlg")
                        .HasColumnType("TEXT");

                    b.Property<string>("DarkIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisableGenAutoFill")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisableJavaScript")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisablePassSave")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisableWebMess")
                        .HasColumnType("TEXT");

                    b.Property<string>("Downloads")
                        .HasColumnType("TEXT");

                    b.Property<string>("EngineFriendlyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Eq2fa")
                        .HasColumnType("TEXT");

                    b.Property<string>("EqHis")
                        .HasColumnType("TEXT");

                    b.Property<string>("Eqfav")
                        .HasColumnType("TEXT");

                    b.Property<string>("Eqsets")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExceptionLog")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExitDialog")
                        .HasColumnType("TEXT");

                    b.Property<string>("Favorites")
                        .HasColumnType("TEXT");

                    b.Property<string>("FavoritesL")
                        .HasColumnType("TEXT");

                    b.Property<string>("Historybtn")
                        .HasColumnType("TEXT");

                    b.Property<string>("IsFavoritesToggled")
                        .HasColumnType("TEXT");

                    b.Property<string>("IsHistoryToggled")
                        .HasColumnType("TEXT");

                    b.Property<string>("Lang")
                        .HasColumnType("TEXT");

                    b.Property<string>("LightMode")
                        .HasColumnType("TEXT");

                    b.Property<string>("NtpDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("NtpTextColor")
                        .HasColumnType("TEXT");

                    b.Property<string>("OpSw")
                        .HasColumnType("TEXT");

                    b.Property<string>("OpenTabHandel")
                        .HasColumnType("TEXT");

                    b.Property<string>("QrCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReadButton")
                        .HasColumnType("TEXT");

                    b.Property<string>("ResourceSave")
                        .HasColumnType("TEXT");

                    b.Property<string>("SearchUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("SelfPackageName")
                        .HasColumnType("TEXT");

                    b.Property<string>("StatusBar")
                        .HasColumnType("TEXT");

                    b.Property<string>("ToolIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("TrackPrevention")
                        .HasColumnType("TEXT");

                    b.Property<string>("Translate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Useragent")
                        .HasColumnType("TEXT");

                    b.Property<string>("isFavoritesVisible")
                        .HasColumnType("TEXT");

                    b.Property<string>("isHistoryVisible")
                        .HasColumnType("TEXT");

                    b.Property<string>("isSearchVisible")
                        .HasColumnType("TEXT");

                    b.HasKey("PackageName");

                    b.HasIndex("SelfPackageName");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("FireBrowserMultiCore.Settings", b =>
                {
                    b.HasOne("FireBrowserMultiCore.Settings", "Self")
                        .WithMany()
                        .HasForeignKey("SelfPackageName");

                    b.Navigation("Self");
                });
#pragma warning restore 612, 618
        }
    }
}

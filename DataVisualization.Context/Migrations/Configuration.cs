namespace DataVisualization.Context.Migrations
{
    using DataVisualization.Common;
    using DataVisualization.Common.Helper;
    using DataVisualization.Models.Identity;
    using DataVisualization.Models.Workspace;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using static DataVisualization.Common.Enum.Enumerators;

    internal sealed class Configuration : DbMigrationsConfiguration<DataVisualization.Context.VisContext>
    {
        private readonly List<Category> categories = new List<Category>
        {
            new Category("Everyone's","Shared categories and charts between everyone")
            {
                IsRoot = true,
                IsEveryones = true
            },
            new Category("My categories")
            {
                IsRoot = true,
                OwnerId = CommonConstants.SUPER_USER_ID,
            },
            new Category("My categories")
            {
                IsRoot = true,
                OwnerId = CommonConstants.TEST_USER_ID,
                ChildCategories = new List<Category>
                {
                     new Category("Category 1","Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
            {
                OwnerId = CommonConstants.TEST_USER_ID,
                ChartObjects = new List<ChartObject>
                {
                    new ChartObject("Chart 1","Maecenas fringilla dignissim dui, lacinia scelerisque tortor pretium eget.")
            {
                OwnerId = CommonConstants.TEST_USER_ID
            },
            new ChartObject("Chart 2")
            {
                OwnerId = CommonConstants.TEST_USER_ID
            },
            new ChartObject("Chart 3","Vestibulum risus nulla, molestie et rutrum faucibus, suscipit nec neque.")
            {
                OwnerId = CommonConstants.TEST_USER_ID
            },
                },
                ChildCategories = new List<Category>
                {
                    new Category("Category 1.1","Aenean ut orci magna. Aenean efficitur vehicula diam sit amet placerat.")
                    {
                        OwnerId = CommonConstants.TEST_USER_ID,
                        ChildCategories = new List<Category>
                        {
                            new Category("Category 1.1.1")
                            {
                                OwnerId = CommonConstants.TEST_USER_ID,
                                ChartObjects = new List<ChartObject>
                                {
                                    new ChartObject("Chart 7","Donec neque justo, venenatis sed feugiat eu, ornare et ligula.")
            {
                CategoryId = 2,
                OwnerId = CommonConstants.TEST_USER_ID
            },
            new ChartObject("Chart 8")
            {
                CategoryId = 4,
                OwnerId = CommonConstants.TEST_USER_ID
            }
                                }
                            },
                            new Category("Category 1.1.2","Pellentesque consectetur tellus dolor, a pulvinar nisl dignissim in.")
                            {
                                OwnerId = CommonConstants.TEST_USER_ID
                            }
                        }
                    },
                    new Category("Category 1.2","Donec malesuada libero ut lectus suscipit, sed ultricies nisl gravida.")
                    {
                        OwnerId = CommonConstants.TEST_USER_ID
                    },
                    new Category("Category 1.3")
                    {
                        OwnerId = CommonConstants.TEST_USER_ID
                    }
                }
            },
            new Category("Category 2","Nulla vitae enim venenatis, aliquet nulla quis, porta tellus. Curabitur arcu augue, dignissim sed magna eget, volutpat dignissim tortor.")
            {
                OwnerId = CommonConstants.TEST_USER_ID,
                ChartObjects = new List<ChartObject>
                {
                     new ChartObject("Chart 4","Donec tincidunt tristique est sed auctor. Nullam id risus dolor.")
            {
                CategoryId = 7,
                OwnerId = CommonConstants.TEST_USER_ID
            },
            new ChartObject("Chart 5")
            {
                CategoryId = 10,
                OwnerId = CommonConstants.TEST_USER_ID
            }
                },
                ChildCategories = new List<Category>
                {
                    new Category("Category 2.1","Ut ac eleifend ipsum, in iaculis elit.")
                    {
                        OwnerId = CommonConstants.TEST_USER_ID
                    },
                    new Category("Category 2.2")
                    {
                        OwnerId = CommonConstants.TEST_USER_ID,
                        ChartObjects = new List<ChartObject>
                        {
                            new ChartObject("Chart 6","Nulla maximus magna et ante luctus sagittis.")
            {
                CategoryId = 11,
                OwnerId = CommonConstants.TEST_USER_ID
            }
                        },
                        ChildCategories = new List<Category>
                        {
                            new Category("Category 2.2.1")
                            {
                                OwnerId = CommonConstants.TEST_USER_ID
                            },
                            new Category("Category 2.2.2","Proin accumsan orci massa, id pulvinar sapien vehicula in.")
                            {
                                OwnerId = CommonConstants.TEST_USER_ID
                            },
                            new Category("Category 2.2.3")
                            {
                                OwnerId = CommonConstants.TEST_USER_ID
                            }
                        }
                    },
                    new Category("Category 2.3","Aenean sit amet ante et ex aliquam placerat ut sit amet ex.")
                    {
                        OwnerId = CommonConstants.TEST_USER_ID
                    }
                }
            }
                }
            },

        };


        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DataVisualization.Context.KGTContext";
        }

        protected override void Seed(VisContext context)
        {
            CreateUsers(context);

            categories.ForEach(c =>
            {
                context.Set<Category>().AddOrUpdate(c);
            });

            context.SaveChanges();

            AddChartImage(context);
        }

        private void AddChartImage(VisContext context)
        {
            var chart1 = context.ChartObjects.ToList().ElementAt(0);
            chart1.Thumbnail = new Thumbnail();
            chart1.Thumbnail.SetThumbnailImage(@"C:\Users\Xhulio\source\Diploma\mlguiplayground\DataVisualization\Content\icons\line-chart-96.png");
            context.ChartObjects.AddOrUpdate(chart1);

            var chart2 = context.ChartObjects.ToList().ElementAt(1);
            chart2.Thumbnail = new Thumbnail();
            chart2.Thumbnail.SetThumbnailImage(@"C:\Users\Xhulio\source\Diploma\mlguiplayground\DataVisualization\Content\icons\line-chart-96.png");
            context.ChartObjects.AddOrUpdate(chart2);
            context.SaveChanges();
        }

        private void CreateUsers(VisContext context)
        {

            var userRoles = CreateUserRoles(context);

            var adminUser = new ApplicationUser
            {
                Id = CommonConstants.SUPER_USER_ID,
                UserName = "Admin",
                PasswordHash = "AGL8FliPdwFXPUPbEBPurFNBsh2SsW0tvhE9FhHTLK9M7Bcku/ayuzHdyB5hkssiWA==", //890iop
                SecurityStamp = "ed47f8e7-e231-4489-bd2a-cd6ee0e129ab"
            };

            var testUser = new ApplicationUser
            {
                Id = CommonConstants.TEST_USER_ID,
                UserName = "TestUser",
                PasswordHash = "AKaPTRelTIOTBqeWUx/PyRnTkKaO5IQwZQZcdoISabFzShJ7kmmMWODmaJXp2BWTDg==", //890iop
                SecurityStamp = "3024946e-c785-4fb0-9c8d-b0dc14d7c7b7"
            };

            context.Set<ApplicationUser>().AddOrUpdate(adminUser, testUser);

            context.Set<IdentityUserRole>().AddOrUpdate(
                new IdentityUserRole
                {
                    RoleId = userRoles.Single(r => r.Name == UserRoles.Admin.ToString()).Id,
                    UserId = adminUser.Id
                },
            new IdentityUserRole
            {
                RoleId = userRoles.Single(r => r.Name == UserRoles.User.ToString()).Id,
                UserId = testUser.Id
            });

            context.SaveChanges();
        }

        private List<IdentityRole> CreateUserRoles(VisContext context)
        {
            var userRoles = new List<IdentityRole>();
            foreach (var role in System.Enum.GetNames(typeof(UserRoles)))
            {
                userRoles.Add(new IdentityRole
                {
                    Id = System.Guid.NewGuid().ToString(),
                    Name = role
                });
            }

            context.Set<IdentityRole>().AddRange(userRoles);

            context.SaveChanges();

            return userRoles;
        }
    }
}

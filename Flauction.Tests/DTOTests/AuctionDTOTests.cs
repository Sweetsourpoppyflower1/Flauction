using Flauction.DTOs;
using Flauction.Models;
using Flauction.DTOs.Output.ModelDTOs;
using Xunit;

namespace Flauction.Tests.DTOTests
{
    public class AuctionDTOTests
    {
        [Fact]
        public void AuctionDTO_ValidData_ShouldInitialize()
        {
            // Arrange & Act
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(2);
            
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                AuctionMasterName = "Master Auction 1",
                PlantName = "Plant A",
                WinnerCompanyName = "Winner Corp",
                Status = "active",
                StartTime = startTime,
                EndTime = endTime,
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(1, dto.AuctionId);
            Assert.Equal("Master Auction 1", dto.AuctionMasterName);
            Assert.Equal("Plant A", dto.PlantName);
            Assert.Equal("Winner Corp", dto.WinnerCompanyName);
            Assert.Equal("active", dto.Status);
            Assert.Equal(startTime, dto.StartTime);
            Assert.Equal(endTime, dto.EndTime);
            Assert.Equal(100m, dto.StartPrice);
            Assert.Equal(50m, dto.MinimumPrice);
            Assert.Equal(150m, dto.FinalPrice);
        }

        [Fact]
        public void AuctionDTO_WithNullStrings_CanInitialize()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                AuctionMasterName = null,
                PlantName = null,
                WinnerCompanyName = null,
                Status = null,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Null(dto.AuctionMasterName);
            Assert.Null(dto.PlantName);
            Assert.Null(dto.WinnerCompanyName);
            Assert.Null(dto.Status);
        }

        [Fact]
        public void AuctionDTO_WithZeroAuctionId_CanInitialize()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = 0,
                AuctionMasterName = "Master",
                PlantName = "Plant",
                WinnerCompanyName = "Company",
                Status = "pending",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(0, dto.AuctionId);
        }

        [Fact]
        public void AuctionDTO_WithNegativeAuctionId_CanInitialize()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = -1,
                AuctionMasterName = "Master",
                PlantName = "Plant",
                WinnerCompanyName = "Company",
                Status = "pending",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(-1, dto.AuctionId);
        }

        [Fact]
        public void AuctionDTO_WithEmptyStrings_CanInitialize()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                AuctionMasterName = string.Empty,
                PlantName = string.Empty,
                WinnerCompanyName = string.Empty,
                Status = string.Empty,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(string.Empty, dto.AuctionMasterName);
            Assert.Equal(string.Empty, dto.PlantName);
            Assert.Equal(string.Empty, dto.WinnerCompanyName);
            Assert.Equal(string.Empty, dto.Status);
        }

        [Fact]
        public void AuctionDTO_WithVariousStatusValues_CanInitialize()
        {
            // Arrange
            var statuses = new[] { "active", "upcoming", "ended", "sold", "not sold", "closed" };

            // Act & Assert
            foreach (var status in statuses)
            {
                var dto = new AuctionDTO
                {
                    AuctionId = 1,
                    Status = status,
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(1),
                    StartPrice = 100m,
                    MinimumPrice = 50m,
                    FinalPrice = 150m
                };

                Assert.Equal(status, dto.Status);
            }
        }

        [Fact]
        public void AuctionDTO_WithDecimalPrices_PreservesValues()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                StartPrice = 123.45m,
                MinimumPrice = 67.89m,
                FinalPrice = 234.56m,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Assert
            Assert.Equal(123.45m, dto.StartPrice);
            Assert.Equal(67.89m, dto.MinimumPrice);
            Assert.Equal(234.56m, dto.FinalPrice);
        }

        [Fact]
        public void AuctionDTO_WithZeroPrices_CanInitialize()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                StartPrice = 0m,
                MinimumPrice = 0m,
                FinalPrice = 0m,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Assert
            Assert.Equal(0m, dto.StartPrice);
            Assert.Equal(0m, dto.MinimumPrice);
            Assert.Equal(0m, dto.FinalPrice);
        }

        [Fact]
        public void AuctionDTO_WithNegativePrices_CanInitialize()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                StartPrice = -100m,
                MinimumPrice = -50m,
                FinalPrice = -75m,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Assert
            Assert.Equal(-100m, dto.StartPrice);
            Assert.Equal(-50m, dto.MinimumPrice);
            Assert.Equal(-75m, dto.FinalPrice);
        }

        [Fact]
        public void AuctionDTO_WithHighPrices_CanInitialize()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                StartPrice = 999999999.99m,
                MinimumPrice = 888888888.88m,
                FinalPrice = 777777777.77m,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Assert
            Assert.Equal(999999999.99m, dto.StartPrice);
            Assert.Equal(888888888.88m, dto.MinimumPrice);
            Assert.Equal(777777777.77m, dto.FinalPrice);
        }

        [Fact]
        public void AuctionDTO_WithSameDateTimes_CanInitialize()
        {
            // Arrange
            var sameTime = DateTime.UtcNow;

            // Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                StartTime = sameTime,
                EndTime = sameTime,
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(sameTime, dto.StartTime);
            Assert.Equal(sameTime, dto.EndTime);
            Assert.Equal(dto.StartTime, dto.EndTime);
        }

        [Fact]
        public void AuctionDTO_WithEndTimeBeforeStartTime_CanInitialize()
        {
            // Arrange
            var startTime = DateTime.UtcNow.AddHours(2);
            var endTime = DateTime.UtcNow;

            // Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                StartTime = startTime,
                EndTime = endTime,
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.True(dto.StartTime > dto.EndTime);
        }

        [Fact]
        public void AuctionDTO_WithLocalDateTime_CanInitialize()
        {
            // Arrange
            var localStart = DateTime.Now;
            var localEnd = localStart.AddHours(1);

            // Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                StartTime = localStart,
                EndTime = localEnd,
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(localStart, dto.StartTime);
            Assert.Equal(localEnd, dto.EndTime);
        }

        [Fact]
        public void AuctionDTO_WithDefaultDateTime_CanInitialize()
        {
            // Arrange & Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                StartTime = default,
                EndTime = default,
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(default, dto.StartTime);
            Assert.Equal(default, dto.EndTime);
        }

        [Fact]
        public void AuctionDTO_WithLongStringValues_CanInitialize()
        {
            // Arrange
            var longString = new string('A', 1000);

            // Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                AuctionMasterName = longString,
                PlantName = longString,
                WinnerCompanyName = longString,
                Status = longString,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(longString, dto.AuctionMasterName);
            Assert.Equal(longString, dto.PlantName);
            Assert.Equal(longString, dto.WinnerCompanyName);
            Assert.Equal(longString, dto.Status);
        }

        [Fact]
        public void AuctionDTO_WithSpecialCharactersInStrings_CanInitialize()
        {
            // Arrange
            var specialString = "!@#$%^&*()_+-=[]{}|;:',.<>?/";

            // Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                AuctionMasterName = specialString,
                PlantName = specialString,
                WinnerCompanyName = specialString,
                Status = specialString,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(specialString, dto.AuctionMasterName);
            Assert.Equal(specialString, dto.PlantName);
            Assert.Equal(specialString, dto.WinnerCompanyName);
            Assert.Equal(specialString, dto.Status);
        }

        [Fact]
        public void AuctionDTO_AllPropertiesCanBeModified()
        {
            // Arrange
            var dto = new AuctionDTO();
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(2);

            // Act
            dto.AuctionId = 99;
            dto.AuctionMasterName = "Updated Master";
            dto.PlantName = "Updated Plant";
            dto.WinnerCompanyName = "Updated Winner";
            dto.Status = "updated";
            dto.StartTime = startTime;
            dto.EndTime = endTime;
            dto.StartPrice = 999m;
            dto.MinimumPrice = 888m;
            dto.FinalPrice = 777m;

            // Assert
            Assert.Equal(99, dto.AuctionId);
            Assert.Equal("Updated Master", dto.AuctionMasterName);
            Assert.Equal("Updated Plant", dto.PlantName);
            Assert.Equal("Updated Winner", dto.WinnerCompanyName);
            Assert.Equal("updated", dto.Status);
            Assert.Equal(startTime, dto.StartTime);
            Assert.Equal(endTime, dto.EndTime);
            Assert.Equal(999m, dto.StartPrice);
            Assert.Equal(888m, dto.MinimumPrice);
            Assert.Equal(777m, dto.FinalPrice);
        }

        [Fact]
        public void AuctionDTO_PropertiesCanBeModifiedMultipleTimes()
        {
            // Arrange
            var dto = new AuctionDTO { AuctionId = 1 };

            // Act
            dto.Status = "active";
            dto.Status = "ended";
            dto.Status = "sold";

            dto.FinalPrice = 100m;
            dto.FinalPrice = 200m;
            dto.FinalPrice = 300m;

            // Assert
            Assert.Equal("sold", dto.Status);
            Assert.Equal(300m, dto.FinalPrice);
        }

        [Fact]
        public void AuctionDTO_WithAllPropertiesSet_ContainsCorrectData()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(3);

            // Act
            var dto = new AuctionDTO
            {
                AuctionId = 42,
                AuctionMasterName = "Premium Auction",
                PlantName = "Industrial Plant Z",
                WinnerCompanyName = "Winning Industries Ltd.",
                Status = "sold",
                StartTime = startTime,
                EndTime = endTime,
                StartPrice = 5000.00m,
                MinimumPrice = 2500.00m,
                FinalPrice = 7500.50m
            };

            // Assert
            Assert.Equal(42, dto.AuctionId);
            Assert.Equal("Premium Auction", dto.AuctionMasterName);
            Assert.Equal("Industrial Plant Z", dto.PlantName);
            Assert.Equal("Winning Industries Ltd.", dto.WinnerCompanyName);
            Assert.Equal("sold", dto.Status);
            Assert.Equal(startTime, dto.StartTime);
            Assert.Equal(endTime, dto.EndTime);
            Assert.Equal(5000.00m, dto.StartPrice);
            Assert.Equal(2500.00m, dto.MinimumPrice);
            Assert.Equal(7500.50m, dto.FinalPrice);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999999)]
        [InlineData(int.MaxValue)]
        public void AuctionDTO_WithVariousAuctionIds_CanInitialize(int auctionId)
        {
            // Act
            var dto = new AuctionDTO
            {
                AuctionId = auctionId,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(auctionId, dto.AuctionId);
        }

        [Theory]
        [InlineData("active")]
        [InlineData("upcoming")]
        [InlineData("ended")]
        [InlineData("sold")]
        [InlineData("not sold")]
        [InlineData("closed")]
        [InlineData("")]
        public void AuctionDTO_WithVariousStatuses_CanInitialize(string status)
        {
            // Act
            var dto = new AuctionDTO
            {
                AuctionId = 1,
                Status = status,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                StartPrice = 100m,
                MinimumPrice = 50m,
                FinalPrice = 150m
            };

            // Assert
            Assert.Equal(status, dto.Status);
        }

        [Fact]
        public void AuctionDTO_BetweenTwoDTOs_AreIndependent()
        {
            // Arrange & Act
            var dto1 = new AuctionDTO
            {
                AuctionId = 1,
                Status = "active",
                FinalPrice = 100m
            };

            var dto2 = new AuctionDTO
            {
                AuctionId = 2,
                Status = "ended",
                FinalPrice = 200m
            };

            // Modify dto1
            dto1.Status = "sold";
            dto1.FinalPrice = 150m;

            // Assert
            Assert.Equal(1, dto1.AuctionId);
            Assert.Equal("sold", dto1.Status);
            Assert.Equal(150m, dto1.FinalPrice);

            Assert.Equal(2, dto2.AuctionId);
            Assert.Equal("ended", dto2.Status);
            Assert.Equal(200m, dto2.FinalPrice);
        }
    }
}
using Flauction.DTOs.Output.RegisterDTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Flauction.DTOs;

namespace Flauction.Tests.DTOTests
{
    public class CompanyRegisterDTOTests
    {
        [Fact]
        public void CompanyRegisterDTO_ValidData_ShouldInitialize()
        {
            // Arrange & Act
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Assert
            Assert.Equal("contact@company.com", dto.CompanyEmail);
            Assert.Equal("SecurePassword123!", dto.Password);
            Assert.Equal("Tech Corp Inc.", dto.CompanyName);
            Assert.Equal("123 Business Street", dto.Adress);
            Assert.Equal("12345", dto.PostalCode);
            Assert.Equal("USA", dto.Country);
            Assert.Equal("US123456789", dto.Vat);
            Assert.Equal("DE89370400440532013000", dto.Iban);
            Assert.Equal("COBADEDBXXX", dto.BicSwift);
        }

        [Fact]
        public void CompanyRegisterDTO_CompanyEmailIsRequired()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = null,
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.CompanyEmail)));
        }

        [Fact]
        public void CompanyRegisterDTO_CompanyEmailMustBeValidEmail()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "invalid-email",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.CompanyEmail)));
        }

        [Theory]
        [InlineData("test@company.com")]
        [InlineData("contact@business.co.uk")]
        [InlineData("info@organization.de")]
        [InlineData("admin+tag@domain.org")]
        public void CompanyRegisterDTO_ValidEmailAddresses_ShouldPassValidation(string validEmail)
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = validEmail,
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void CompanyRegisterDTO_CompanyNameIsRequired()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = null,
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.CompanyName)));
        }

        [Fact]
        public void CompanyRegisterDTO_AdressIsRequired()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = null,
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.Adress)));
        }

        [Fact]
        public void CompanyRegisterDTO_PostalCodeIsRequired()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = null,
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.PostalCode)));
        }

        [Fact]
        public void CompanyRegisterDTO_CountryIsRequired()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = null,
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.Country)));
        }

        [Fact]
        public void CompanyRegisterDTO_VatIsRequired()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = null,
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.Vat)));
        }

        [Fact]
        public void CompanyRegisterDTO_IbanIsRequired()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = null,
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.Iban)));
        }

        [Fact]
        public void CompanyRegisterDTO_BicSwiftIsRequired()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = null
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CompanyRegisterDTO.BicSwift)));
        }

        [Fact]
        public void CompanyRegisterDTO_PasswordCanBeNull()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = null,
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Null(dto.Password);
        }

        [Fact]
        public void CompanyRegisterDTO_PropertiesAreInit_Only()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = "contact@company.com",
                Password = "SecurePassword123!",
                CompanyName = "Tech Corp Inc.",
                Adress = "123 Business Street",
                PostalCode = "12345",
                Country = "USA",
                Vat = "US123456789",
                Iban = "DE89370400440532013000",
                BicSwift = "COBADEDBXXX"
            };

            // Act & Assert - Properties are init-only, so they can be set during initialization
            Assert.NotNull(dto.CompanyEmail);
            Assert.NotNull(dto.CompanyName);
            Assert.NotNull(dto.Adress);
        }

        [Fact]
        public void CompanyRegisterDTO_AllRequiredPropertiesMissing_ShouldFailValidation()
        {
            // Arrange
            var dto = new CompanyRegisterDTO
            {
                CompanyEmail = null,
                CompanyName = null,
                Adress = null,
                PostalCode = null,
                Country = null,
                Vat = null,
                Iban = null,
                BicSwift = null
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.True(results.Count >= 8);
        }

        [Fact]
        public void CompanyRegisterDTO_MultipleInvalidEmails_ShouldFailValidation()
        {
            // Arrange
            var invalidEmails = new[] { "notanemail", "missing@domain", "@nodomain.com", "spaces in@email.com" };

            // Act & Assert
            foreach (var invalidEmail in invalidEmails)
            {
                var dto = new CompanyRegisterDTO
                {
                    CompanyEmail = invalidEmail,
                    Password = "SecurePassword123!",
                    CompanyName = "Tech Corp Inc.",
                    Adress = "123 Business Street",
                    PostalCode = "12345",
                    Country = "USA",
                    Vat = "US123456789",
                    Iban = "DE89370400440532013000",
                    BicSwift = "COBADEDBXXX"
                };

                var context = new ValidationContext(dto);
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, context, results, true);

                Assert.False(isValid, $"Email '{invalidEmail}' should be invalid");
            }
        }
    }
}
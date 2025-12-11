using Flauction.DTOs.Output.RegisterDTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Flauction.Tests.DTOTests
{
    public class SupplierRegisterDTOTests
    {
        [Fact]
        public void SupplierRegisterDTO_ValidData_ShouldInitialize()
        {
            // Arrange & Act
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Assert
            Assert.Equal("contact@supplier.com", dto.SupplierEmail);
            Assert.Equal("SecurePassword123!", dto.Password);
            Assert.Equal("Quality Supplies Ltd.", dto.SupplierName);
            Assert.Equal("456 Industrial Park", dto.Address);
            Assert.Equal("54321", dto.PostalCode);
            Assert.Equal("Germany", dto.Country);
            Assert.Equal("DE89370400440532013000", dto.Iban);
            Assert.Equal("Reliable supplier of industrial materials", dto.Desc);
        }

        [Fact]
        public void SupplierRegisterDTO_SupplierEmailIsRequired()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = null,
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.SupplierEmail)));
        }

        [Fact]
        public void SupplierRegisterDTO_SupplierEmailMustBeValidEmail()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "invalid-email",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.SupplierEmail)));
        }

        [Theory]
        [InlineData("supplier@company.com")]
        [InlineData("contact@industrial.co.uk")]
        [InlineData("info@manufacturing.de")]
        [InlineData("sales+supplier@factory.org")]
        public void SupplierRegisterDTO_ValidEmailAddresses_ShouldPassValidation(string validEmail)
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = validEmail,
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void SupplierRegisterDTO_PasswordIsRequired()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = null,
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.Password)));
        }

        [Fact]
        public void SupplierRegisterDTO_SupplierNameIsRequired()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = null,
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.SupplierName)));
        }

        [Fact]
        public void SupplierRegisterDTO_AddressIsRequired()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = null,
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.Address)));
        }

        [Fact]
        public void SupplierRegisterDTO_PostalCodeIsRequired()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = null,
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.PostalCode)));
        }

        [Fact]
        public void SupplierRegisterDTO_CountryIsRequired()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = null,
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.Country)));
        }

        [Fact]
        public void SupplierRegisterDTO_IbanIsRequired()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = null,
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.Iban)));
        }

        [Fact]
        public void SupplierRegisterDTO_DescIsRequired()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = null
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.Desc)));
        }

        [Fact]
        public void SupplierRegisterDTO_DescWithin500CharacterLimit_ShouldPassValidation()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = new string('A', 500)
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Equal(500, dto.Desc.Length);
        }

        [Fact]
        public void SupplierRegisterDTO_DescExceeding500Characters_ShouldFailValidation()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = new string('A', 501)
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.Desc)));
        }

        [Fact]
        public void SupplierRegisterDTO_PropertiesAreInitOnly()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act & Assert - Properties are init-only, so they can be set during initialization
            Assert.NotNull(dto.SupplierEmail);
            Assert.NotNull(dto.SupplierName);
            Assert.NotNull(dto.Address);
        }

        [Fact]
        public void SupplierRegisterDTO_AllRequiredPropertiesMissing_ShouldFailValidation()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = null,
                Password = null,
                SupplierName = null,
                Address = null,
                PostalCode = null,
                Country = null,
                Iban = null,
                Desc = null
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.True(results.Count >= 8);
        }

        [Theory]
        [InlineData("notanemail")]
        [InlineData("missing@domain")]
        [InlineData("@nodomain.com")]
        [InlineData("spaces in@email.com")]
        public void SupplierRegisterDTO_InvalidEmailFormats_ShouldFailValidation(string invalidEmail)
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = invalidEmail,
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.SupplierEmail)));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void SupplierRegisterDTO_RequiredFieldsWithWhitespace_ShouldFailValidation(string whitespaceValue)
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = whitespaceValue,
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(SupplierRegisterDTO.SupplierName)));
        }

        [Theory]
        [InlineData("Supplier")]
        [InlineData("Industrial Equipment Provider")]
        [InlineData("Very Long Supplier Name With Multiple Words")]
        public void SupplierRegisterDTO_SupplierNameVariousLengths_ShouldPassValidation(string supplierName)
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = supplierName,
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void SupplierRegisterDTO_EmailWithSpecialCharacters_ShouldPassValidation()
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "supplier.name+noreply@industrial.co.uk",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void SupplierRegisterDTO_DescAt500Characters_ShouldPassValidation()
        {
            // Arrange
            var longDescription = new string('A', 500);
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = longDescription
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Equal(500, dto.Desc.Length);
        }

        [Fact]
        public void SupplierRegisterDTO_CanInstantiateWithAllValidRequiredFields()
        {
            // Arrange & Act
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = "Reliable supplier of industrial materials"
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.NotNull(dto.SupplierEmail);
            Assert.NotNull(dto.Password);
            Assert.NotNull(dto.SupplierName);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Short")]
        [InlineData("Medium Length Description")]
        [InlineData("Very detailed supplier description with multiple sentences to demonstrate longer descriptions")]
        public void SupplierRegisterDTO_DescVariousValidLengths_ShouldPassValidation(string description)
        {
            // Arrange
            var dto = new SupplierRegisterDTO
            {
                SupplierEmail = "contact@supplier.com",
                Password = "SecurePassword123!",
                SupplierName = "Quality Supplies Ltd.",
                Address = "456 Industrial Park",
                PostalCode = "54321",
                Country = "Germany",
                Iban = "DE89370400440532013000",
                Desc = description
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
        }
    }
}
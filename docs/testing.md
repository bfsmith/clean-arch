Various types of testing will be used in this project.

# Unit Testing Standards

## Overview
This project uses NUnit as the test framework with Moq, AutoFixture, AutoMoq, and FluentAssertions for comprehensive unit testing. These rules guide AI assistance to write maintainable, readable, and effective unit tests that follow Clean Architecture principles.

## Testing Libraries

### Core Libraries
- **NUnit**: Test framework and test runner
- **Moq**: Mocking framework for creating test doubles
- **Moq.AutoMocker**: An automocking container for Moq
- **AutoFixture**: Automatic test data generation
- **FluentAssertions**: Fluent API for assertions 

Often there is boilerplate code to set up Fixture, AutoMoq, and to create the System Under Test (SUT). This should be automated as a base class for all unit tests. The below is an example, but it is an approxmiation to show the intent, not a complete implementation and it may use invalid methods or properties.

```csharp
public abstract class UnitTestBase<T>
{
    protected T SystemUnderTest;
    protected Fixture Fixture;
    protected Moq.AutoMocker AutoMock;
}

public class UserServiceTests : UnitTestBase<UserService>
{
    [Test]
    public void GetUserById_ShouldReturnUser()
    {
        // Arrange
        var user = Fixture.Create<User>();
        AutoMock.GetMock<IRepository<User>>().Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var userFromDb = await SystemUnderTest.GetUserByIdAsync(user.Id);

        // Assert
        userFromDb.Should().Be(user);
    }
}
```

# Integration Testing Standards

TBD

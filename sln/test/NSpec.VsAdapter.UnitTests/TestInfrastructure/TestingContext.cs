using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class TestingContext
{
    private readonly Fixture fixture = new Fixture();
    private readonly Dictionary<Type, object> injectedMocks = new Dictionary<Type, object>();
    private readonly Dictionary<Type, object> injectedConcreteClasses = new Dictionary<Type, object>();
 
	[SetUp]
    public void fixture_setup()
    {
        fixture.Customize(new AutoNSubstituteCustomization());
    }

	public abstract void before_each();

    /// <summary>
    /// Generates a mock for a class and injects it into the final fixture
    /// </summary>
    /// <typeparam name="TMockType"></typeparam>
    /// <returns></returns>
    public TMockType GetSubstituteFor<TMockType>() where TMockType : class
    {
        var existingMock = injectedMocks.FirstOrDefault(x => x.Key == typeof(TMockType));
        if (existingMock.Key == null)
        {
            var newMock = Substitute.For<TMockType>();
            existingMock = new KeyValuePair<Type, object>(typeof(TMockType), newMock);
            injectedMocks.Add(existingMock.Key, existingMock.Value);
            fixture.Inject(newMock);
        }
        return existingMock.Value as TMockType;
    }
 
    /// <summary>
    /// Injects a concrete class to be used when generating the fixture. 
    /// </summary>
    /// <typeparam name="ClassType"></typeparam>
    /// <returns></returns>
    public void InjectClassFor<ClassType>(ClassType injectedClass) where ClassType : class
    {
        var existingClass = injectedConcreteClasses.FirstOrDefault(x => x.Key == typeof(ClassType));
        if (existingClass.Key != null)
        {
            throw new Exception($"{injectedClass.GetType().Name} has been injected more than once");
        }
        injectedConcreteClasses.Add(typeof(ClassType), injectedClass);
        fixture.Inject(injectedClass);
    }

	public T GetFixtureFor<T>() where T : class => fixture.Create<T>();
}
using Machine.Specifications;
using Moq;
using Snooze.Mspecc.AutoMock.Castle;

namespace Nice.Ukps.Spec.contexts
{
    public class with_auto_mocking<TUnderTest> where TUnderTest : class
    {
        protected static AutoMockContainer<TUnderTest> autoMocker;

        Establish container = () => autoMocker = new AutoMockContainer<TUnderTest>();

        public static Mock<TInterface> Stub<TInterface>() where TInterface : class
        {
            var mocked = autoMocker.GetService<TInterface>();
            return Mock.Get(mocked);
        }

        protected static TUnderTest class_under_test { get { return autoMocker.ClassUnderTest; } }

    }
}
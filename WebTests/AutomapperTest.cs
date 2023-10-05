using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebTests
{
    [TestClass]
    public class AutomapperTest
    {
        [TestMethod]
        public void ShouldHaveAllExplicitMappings()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddMaps(typeof(Profile));
            });

            mappingConfig.AssertConfigurationIsValid();
        }
    }
}

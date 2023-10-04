using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace RazorModule.Tests
{
    public class TestControllerTests
    {
        [Fact]
        public async Task HelloWorldTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            using (var driver = new ChromeDriver(".", options))
            {
                driver.Navigate().GoToUrl("http://localhost:5000/TestController");

                driver.FindElement(By.CssSelector("#run-test")).Click();
                IWebElement result = null;
                var maxAttempts = 5;
                var attempts = 0;
                while (result == null && attempts < maxAttempts)
                {
                    result = driver.FindElements(By.CssSelector("#results")).FirstOrDefault();
                    if (result == null)
                    {
                        await Task.Delay(1000);
                    }

                    attempts++;
                }

                Assert.NotNull(result);
                Assert.Equal("Hello World", result.Text);
            }
        }
    }
}
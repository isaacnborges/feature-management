using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace feature_flags.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IFeatureManager _featureManager;

        public CustomersController(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var countryEnabled = await _featureManager.IsEnabledAsync("CountryEnabled");
            var customers = GetCustomers(countryEnabled);

            return Ok(customers);
        }

        [HttpGet("simple")]
        [FeatureGate("SimpleEnabled")]
        public IActionResult GetSimpleAsync()
        {
            var customers = GetSimplesCustomers();

            return Ok(customers);
        }

        [HttpGet("custom-simple")]
        public async Task<IActionResult> GetCustomSimpleAsync()
        {
            var customSimpleEnabled = await _featureManager.IsEnabledAsync("CustomSimpleEnabled");

            var customers = customSimpleEnabled ? GetSimplesCustomers() : GetCustomers(true);

            return Ok(customers);
        }

        [HttpGet("browser-filter")]
        public async Task<IActionResult> GetCustomersByBrowserAsync()
        {
            var customSimpleEnabled = await _featureManager.IsEnabledAsync("BrowserEnabled");

            return customSimpleEnabled ? Ok("This browser is supported") : NotFound("This browser is not supported");
        }

        private static List<Customer> GetSimplesCustomers()
        {
            var fake = new Faker<Customer>()
                .CustomInstantiator(x => new Customer
                {
                    Name = x.Person.FullName
                });

            var customers = fake.Generate(5);
            return customers;
        }

        private static IEnumerable<Customer> GetCustomers(bool countryEnabled)
        {
            var fake = new Faker<Customer>()
                .CustomInstantiator(x => new Customer
                {
                    Name = x.Person.FirstName,
                    Age = x.Random.Int(10, 70),
                    Country = countryEnabled ? x.Address.Country() : null
                });

            return fake.Generate(5);
        }
    }
}

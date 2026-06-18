using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProductManagement.Api.IntegrationTests.Infrastructure;
using ProductManagement.Application.Contracts.Products;
using ProductManagement.Domain.Enums;

namespace ProductManagement.Api.IntegrationTests;

public sealed class ProductsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/products?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated()
    {
        var request = new CreateProductRequest("Produto API", "SKU-API-001", "Descricao", 100m, 12, "Categoria", ProductStatus.Active);
        var response = await _client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}

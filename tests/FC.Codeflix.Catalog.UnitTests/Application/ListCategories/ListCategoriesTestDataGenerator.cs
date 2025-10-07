using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;

public class ListCategoriesTestDataGenerator
{
    public static IEnumerable<object[]> GetInputsWithoutAllParameters()
    {
        var fixture = new ListCategoriesTestFixture();
        var validInput = fixture.GetValidInput();

        // Input apenas com página e itens por página (search vazio)
        yield return new object[] { new ListCategoriesInput(
            page: validInput.Page,
            perPage: validInput.PerPage,
            sort: validInput.Sort,
            sortDir: validInput.SortDir
        ) };

        // Input apenas com página e itens por página (sort vazio)
        yield return new object[] { new ListCategoriesInput(
            page: validInput.Page,
            perPage: validInput.PerPage
        ) };

        // Input com valores mínimos (página 1, 15 itens por página)
        yield return new object[] { new ListCategoriesInput(
            page: 1,
            perPage: 15,
            sortDir: ESearchOrder.Asc
        ) };

        // Input com search e sort vazios
        yield return new object[] { new ListCategoriesInput(
            perPage: validInput.PerPage,
            sortDir: validInput.SortDir
        ) };

        // Input com sort específico "name" e ordem descendente
        yield return new object[] { new ListCategoriesInput(
            sort: "name",
            sortDir: ESearchOrder.Desc
        ) };

        // Input com sort específico "createdAt" e ordem ascendente
        yield return new object[] { new ListCategoriesInput(
            page: 2,
            perPage: 30,
            sort: "createdAt",
            sortDir: ESearchOrder.Asc
        ) };

        // Input com valores altos para página e perPage
        yield return new object[] { new ListCategoriesInput(
            page: 10,
            perPage: 100,
            search: "categoria",
            sort: "name",
            sortDir: ESearchOrder.Asc
        ) };

        // Input com busca específica e sem ordenação
        yield return new object[] { new ListCategoriesInput(
            page: 1,
            perPage: 25,
            search: "documentário",
            sortDir: ESearchOrder.Desc
        ) };

        // Input sem parâmetros (todos default)
        yield return new object[] { new ListCategoriesInput() };
    }
}
